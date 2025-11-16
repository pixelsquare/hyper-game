using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatClient : Client
{
    public static string Ip;
    public static int Port;

    [NonSerialized] public bool entered = false;
    [NonSerialized] public bool recvCachedGuidChats = false;
    [NonSerialized] public ChatConnectionUsage usage = ChatConnectionUsage.Channel;

    void Awake()
    {
        chatClient = this;
        targetIP = Ip;
        targetPort = Port;
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnDestroy()
    {
        if (chatClient == this)
            chatClient = null;

        base.OnDestroy();
    }

    protected override void _RegisterHandler()
    {
        base._RegisterHandler();
        handler.Add(MsgId.EnterChatReqAck, OnEnterAck);
        handler.Add(MsgId.BroadcastChatNot, OnBroadcastChatNot);
        handler.Add(MsgId.GuildChatEnterNot, OnGuildChatEnterNot);
        handler.Add(MsgId.ChatBawlingNot, OnChatBawlingNot);
        handler.Add(MsgId.ChatGuildAttendRewardNot, OnChatGuildAttendRewardNot);
        handler.Add(MsgId.OccWarComChangeNot, OnOccWarComChangeNot);
    }

    protected override void OnConnected()
    {        
        Debug.Log(Str.White("Chat Client Connected"));
        
        var mChat = FindObjectOfType<Chat>();
        if(mChat != null)
            StartCoroutine(mChat.Restart());
    }

    public override void Connect()
    {
        entered = false;
        base.Connect();
    }

    protected override void OnError(Msg msg)
    {
        ErrorCode err = (ErrorCode)msg.ReadShort();
        string message = msg.ReadString();
        Debug.LogError("Error " + err.ToString());

        switch (err)
        {
            case ErrorCode.ChatBlockUser:
                {
                    PageMsgBox.Open(false, false);
                    PageMsgBox.title = Str.Get("Label_Notice");
                    PageMsgBox.desc = Str.Get("Msg_353");
                    PageMsgBox.confirm = Str.Get("Btn_Confirm");
                }
                break;

            default:
                {
                    if (PageMsgBox.exists)
                        return;

                    PageMsgBox.Open(false, false);
                    PageMsgBox.title = Str.Get("Label_Notice");
                    PageMsgBox.desc = Str.Get($"Error_{err}");
                    PageMsgBox.confirm = Str.Get("Btn_Confirm");
                }
                break;
        }

        //if (PageMsgBox.exists)
        //    return;

        //PageMsgBox.Open(false, true);
        //PageMsgBox.title = Str.Get("Label_Notice");
        //PageMsgBox.desc = Str.Get($"Error_Default", msg.msgId.ToString());
        //PageMsgBox.confirm = Str.Get("Btn_Confirm");
    }

    public void EnterReq()
    {
        Msg writer = BeginWrite(MsgId.EnterChatReqAck);
        writer.Write((byte)usage);
        writer.Write(accInfo.accUid);
        writer.FinishWrite();
        Send(writer);
    }

    public void SendChat(ChatMsg msg)
    {
        Msg writer = BeginWrite(MsgId.ChatNot);
        writer.Write(msg);
        writer.FinishWrite();
        Send(writer);
    }

    void OnError(int netUid, Msg msg) 
    {
        ErrorCode err = (ErrorCode)msg.ReadShort();

        Debug.LogError("Error " + err.ToString());

        switch (err)
        {
            case ErrorCode.WrongAccUid:
            case ErrorCode.WrongBattleServerUid:
                break;
            case ErrorCode.EmoticonExistsNot:
                SystemMsg.AddBan(Str.Get("Msg_229"), 1.5f);
                break;
            default:
                {
                    PageMsgBox.Open(false, false);
                    PageMsgBox.title = Str.Get("Label_Notice");
                    PageMsgBox.desc = Str.Get($"Error_{err}");
                    PageMsgBox.confirm = Str.Get("Btn_Confirm");
                }
                break;
        }
    }

    void OnEnterAck (Msg msg)
    {
        Debug.Log(Str.Yellow("Enter Chat Server" + usage));
        entered = true;

        if (usage == ChatConnectionUsage.OccWar)
            _ReadChat(msg);

        usage = ChatConnectionUsage.Channel;
    }

    void OnBroadcastChatNot (Msg msg)
    {
        _ReadChat(msg);
    }

    void OnGuildChatEnterNot(Msg msg)
    {
        _ReadChat(msg, true);
    }

    void OnChatBawlingNot(Msg msg)
    {
        ResultItemInfo resultItemInfo = msg.Read<ResultItemInfo>();
        
        Account.SetResult(resultItemInfo);
    }

    void OnChatGuildAttendRewardNot(Msg msg)
    {
        ResultItemInfo resultItemInfo = msg.Read<ResultItemInfo>();
        Account.SetResult(resultItemInfo);
        //Guild Attend Reward
        if (resultItemInfo.changePosts.Count != 0)
        {
            var page = Page.Open<PageReward>();
            List<RItemInfo> rewards = new List<RItemInfo>();
            foreach (var info in resultItemInfo.changePosts[0].postItemInfos)
            {
                RItemInfo itemInfo = new RItemInfo()
                {
                    itemId = info.itemId,
                    itemCount = info.itemCount,
                };
                rewards.Add(itemInfo);
            }
            page.Set(rewards, Str.Get("Label_376"), Str.Get("Msg_148"));

            //appsflyer 이벤트 길드출석보상
            //AppsFlyerManager.I().SendEventUnique("Guild_attendance");
        }
    }

    void _ReadChat(Msg msg, bool guildEntered = false)
    {
        List<ChatMsg> chats = msg.ReadList<ChatMsg>();

        if (guildEntered)
            Account.InitGuildChat();

        foreach (var chat in chats)
            Account.AddChat(chat);
    }

    void OnOccWarComChangeNot(Msg msg)
    {
        if (occWar == null || worldData == null)
            return;

        OccWarComChangeNotInfo not = msg.Read<OccWarComChangeNotInfo>();
        if (not.isPingChange)
        {
            Account.occWarCommanderPingBoardEventId = not.isActive ? not.boardEventId : 0;
            BroadCaster.onComPingChanged?.Invoke();
            foreach (var boardInfo in Account.occWarBoardEvents)
            {
                int worldIdx = worldData.hexIdByEventId[boardInfo.Key];

                Debug.Log($"OnOccupationWarChangeNot worldIdx : {worldIdx}, eventId : {boardInfo.Key}");
                world.OnOccupied(worldIdx);
            }
        }

        if (not.isMessageChange)
        {
            Account.occWarCommanderMessage = not.message;
            occWar.ChangeCommanderMessage(not.message);
        }
    }

    #if UNITY_EDITOR
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChatMsg chat = new ChatMsg();
            chat.type = EChatType.eNotice;
            chat.msg = $"<GetHeroFromGacha> <Nick:푸틴> <Hero:{UnityEngine.Random.Range(0, 64)}> <Type:1>";
            chat.noticeLifeTime = 5;
            Account.AddChat(chat);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChatMsg chat = new ChatMsg();
            chat.type = EChatType.eNotice;
            chat.msg = "동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리 나라 만세";
            chat.noticeLifeTime = 5;
            Account.AddChat(chat);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ChatMsg chat = new ChatMsg();
            chat.type = EChatType.eNotice;
            chat.msg = 
                "동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리 나라 만세 " +
                "무궁화 삼천리 화려 강산 대한 사람 대한으로 기리 보전하세 " +
                "한번 더! 동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리 나라 만세 " +
                "무궁화 삼천리 화려 강산 대한 사람 대한으로 기리 보전하세";

            chat.noticeLifeTime = 10;
            Account.AddChat(chat);
        }
    }
    #endif
}
