public enum EChatType
{
    eChannel,
    eGuild,
    eWhisper,
    eOccupationWar,
    eSystem,
    eAll,
    eNotice,
    eSystemWhisper,
    eNoticeFromGacha,                    // 챗 창에는 보여주지 않음
}

public class ChatMsg : Serializable
{
    public int accUid;              // 채팅을 보낸 유저 accUid
    public EChatType type;

    public string nick;             // 채팅을 보낸 유저 nick
    public string msg;
    public int whisperFromNetUid;   // 귓속말을 보낸 유저 netUid
    public string toWhisperNick;    // 귓속말을 보낼 유저의 nick
    public short profileId = -1;
    public int guildUid;            // 서버에서만 사용
    public short noticeLifeTime;    // 단위/초
    public int profileBorderItemId;
    public WriterGenderType writerGenderType;

    public override void Serialize(Msg msg)
    {
        msg.Write((byte)type);

        if (type == EChatType.eNotice)
        {
            msg.Write(this.msg);
            msg.Write(this.noticeLifeTime);
        }
        else
        {
            msg.Write(accUid);
            msg.Write(nick);
            msg.Write(this.msg);
            msg.Write(this.whisperFromNetUid);
            msg.Write(this.toWhisperNick);
            msg.Write(this.profileId);
            msg.Write(this.profileBorderItemId);
            msg.Write((byte)this.writerGenderType);
        }
    }

    public override void Deserialize(Msg msg)
    {
        type = (EChatType)msg.ReadByte();
        if (type == EChatType.eNotice)
        {
            this.msg = msg.ReadString();
            noticeLifeTime = msg.ReadShort();
        }
        else
        {
            accUid = msg.ReadInt();
            nick = msg.ReadString();
            this.msg = msg.ReadString();
            whisperFromNetUid = msg.ReadInt();
            toWhisperNick = msg.ReadString();
            profileId = msg.ReadShort();
            profileBorderItemId = msg.ReadInt();
            writerGenderType = (WriterGenderType)msg.ReadByte();
        }
    }

    public ChatMsg CreateInstance(string msg)
    {
        ChatMsg chat = new ChatMsg();
        chat.accUid = accUid;
        chat.type = type;
        chat.msg = msg;
        chat.nick = nick;
        chat.whisperFromNetUid = whisperFromNetUid;
        chat.profileId = profileId;
        chat.profileBorderItemId = profileBorderItemId;
        chat.writerGenderType = writerGenderType;
        return chat;
    }
}

