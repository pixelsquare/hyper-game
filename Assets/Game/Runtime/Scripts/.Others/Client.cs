using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Error;
using Unity.Networking.Transport.Utilities;

public abstract class Client : Access
{
    public const int MaxPayload = 30000;
    public const int MaxMsgSize = 1300;
    public static int LimitSendPerSec = 10;

    protected string targetIP = "192.168.0.1";
	protected int targetPort = 8777;

	public bool useUDP;
	public bool autoReconnect = false;		

	public UnityAction<Msg> onSomethingSend = null;
	public UnityAction<Msg> onSomethingReceived = null;
    
    [NonSerialized] public int netUid = -1;

    protected bool shutDownCalled = false;

    // Transport
    bool initialized = false;
    NetworkDriver universalNetwork;
    NetworkPipeline tcpPipeline;
    NetworkPipeline udpPipeline;
    NetworkConnection universalConnection;

    Coroutine connecting;

    protected Dictionary<MsgId, UnityAction<Msg>> handler =
		new Dictionary<MsgId, UnityAction<Msg>>();

    int sendHeat = 0;
    float endHeat = 0;

    NativeArray<byte> targetIPArray
    {
        get
        {
            NativeArray<byte> bytes = new NativeArray<byte>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            string[] ips = targetIP.Split('.');
            if (ips.Length >= 4)
            {
                bytes[0] = byte.Parse(ips[0]);
                bytes[1] = byte.Parse(ips[1]);
                bytes[2] = byte.Parse(ips[2]);
                bytes[3] = byte.Parse(ips[3]);
            }
            return bytes;
        }
    }

    public virtual bool isConnected {
        get {
            return 
                universalConnection != null &&
                universalConnection.IsCreated &&
                universalConnection.GetState(universalNetwork) == NetworkConnection.State.Connected;
        }
    }

    public virtual void Connect()
	{
        netUid = -1;        
        shutDownCalled = false;        

        if (handler.Count == 0)
			_RegisterHandler();

        if (initialized == false)
        {
            initialized = true;
            NetworkSettings settings = new NetworkSettings();

            settings.WithFragmentationStageParameters(
                payloadCapacity: MaxPayload
            );

            settings.WithNetworkConfigParameters(
                maxConnectAttempts: 0,
                disconnectTimeoutMS: 10000,
                heartbeatTimeoutMS: 4000,
                maxMessageSize: MaxMsgSize
            );

            /*
                connectTimeoutMS (1000)
                maxConnectAttempts (60) >> 8
                disconnectTimeoutMS (30000)
                heartBeatTimeoutMS (500) >> 1000
                reconnectionTimeoutMS (2000)
                maxFrameTimeMS (0)
                fixedFrameTimeMS (0),
                receiveQueueCapacity (512)
                sendQueueCapacity (512)
                maxMessageSize (1400)
            */

            universalNetwork = NetworkDriver.Create(settings);
            tcpPipeline = universalNetwork.CreatePipeline(
                typeof(FragmentationPipelineStage),
                typeof(ReliableSequencedPipelineStage)
            );

            if (useUDP)
                udpPipeline = universalNetwork.CreatePipeline(typeof(UnreliableSequencedPipelineStage));
        }
		
		if (connecting == null)
			connecting = StartCoroutine(_UniversalConnecting());
    }

	IEnumerator _UniversalConnecting()
	{
		while (Application.internetReachability == NetworkReachability.NotReachable)
			yield return new WaitForSecondsRealtime(.1f);

        if (isConnected)
        {
            connecting = null;
            yield break;
        }

        Debug.Log($"{GetType()} Try Connect Tcp {targetIP}:{targetPort}");
        NetworkEndpoint endPoint = NetworkEndpoint.LoopbackIpv4.WithPort((ushort)targetPort);

        NativeArray<byte> ipArray = targetIPArray;
        endPoint.SetRawAddressBytes(targetIPArray, NetworkFamily.Ipv4);
        ipArray.Dispose();

        universalConnection = universalNetwork.Connect(endPoint);        
        connecting = null;
	}

    protected virtual void _RegisterHandler()
    {
        handler.Add(MsgId.Error, _OnError);
        handler.Add(MsgId.ReportNetUid, _OnReportNetUid);
    }

    protected virtual void Update()
    {
        if (universalNetwork.IsCreated == false)
            return;
            
        /*
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Str.White($"{GetType()}.Update() Listening={universalNetwork.Listening}"));
        }
        #endif        
        */

        DataStreamReader reader;
        NetworkEvent.Type netType;

        universalNetwork.ScheduleUpdate().Complete();
        if (universalConnection != null && universalConnection.IsCreated)
        {
            while ((netType = universalConnection.PopEvent(universalNetwork, out reader)) != NetworkEvent.Type.Empty)
            {
                switch (netType)
                {
                    case NetworkEvent.Type.Connect:
                        _OnConnected();
                        break;

                    case NetworkEvent.Type.Disconnect:
                        OnDisconnected((DisconnectReason)reader.ReadByte());
                        break;

                    case NetworkEvent.Type.Data:
                        _OnDataReceived(reader, true);

                        if (universalConnection.IsCreated == false ||
                            universalNetwork.IsCreated == false)
                            return;
                        
                        break;
                }
            }
        }

        if (sendHeat > 0 && endHeat < Time.realtimeSinceStartup)
        {
            sendHeat = 0;
            endHeat = 0;
        }
    }

    protected void _OnDataReceived(DataStreamReader r, bool reliable)
    {
        Msg reader = Msg.Create();
        reader.BeginRead(r, reliable);
        ParseMsg(reader);
    }

    public void ParseMsg(Msg msg)
    {
        MsgId msgId = msg.msgId;
        if (onSomethingReceived != null)
            onSomethingReceived(msg);

        UnityAction<Msg> onReceived;
        if (handler.TryGetValue(msgId, out onReceived))
        {
            if (SpecificMsgIds.DoNotShowLog.Contains(msgId) == false)
            {
                string str = msgId.ToString().Replace("ReqAck", "Ack");
                Debug.Log($"Received {str} ({msg.reader.Length} bytes)");
            }

            onReceived.Invoke(msg);
        }
        else
            Debug.LogError($"Received Strange Msg ({msgId},{netUid}) ({msg.reader.Length} bytes)");
    }

    protected virtual void _OnReportNetUid(Msg msg)
    {
        netUid = msg.ReadInt();
        Debug.Log(Str.White($"{GetType().ToString()} set NetUid {netUid}"));
    }

    void _OnError(Msg msg) { OnError(msg); }

    protected virtual void OnError(Msg msg)
    {
        Debug.LogError($"{msg.msgId} Error");
    }

    void _OnConnected()
    {
        OnConnected();
    }

    protected abstract void OnConnected();

    protected virtual void OnConnectFailed() { }

    protected virtual void OnDisconnected(DisconnectReason reason) {
        if (autoReconnect && shutDownCalled == false)
			Connect();
	}

    public virtual Msg BeginWrite(MsgId msgId, bool reliable = true, bool willCompress = false)
    {
        if (useUDP == false && reliable == false)
            reliable = true;

        Msg msg = Msg.Create();
        msg.netUid = netUid;
        msg.reliable = reliable;

        if (universalConnection.IsCreated == false ||
            universalConnection.GetState(universalNetwork) != NetworkConnection.State.Connected)
        {
            Debug.LogError("Can not begin write with " + (reliable ? "Tcp" : "Udp"));
            return msg;
        }

        if (reliable)
        {
            sendHeat++;
            if (sendHeat == 1)
                endHeat = Time.realtimeSinceStartup + 1f;
            else if (sendHeat > LimitSendPerSec)
            {
                Debug.LogError($"Too many send times ({sendHeat}) per sec");
                return msg;
            }
        }

        int error = universalNetwork.BeginSend(reliable ? tcpPipeline : udpPipeline, universalConnection, out var writer);
        if (error == 0)        
            msg.BeginWrite(msgId, writer, willCompress);
        else
            Debug.LogError($"Fail to BeginSend ({netUid}, {msgId}) {(StatusCode)error}");

        return msg;
    }

    public void Send (Msg msg)
	{
        if (msg.msgId == MsgId.Unknown)
            return;

        if (msg.reliable && onSomethingSend != null)
            onSomethingSend(msg);
        
        int sent = universalNetwork.EndSend(msg.writer);
        if (sent < 0)
        {
            string str = msg.msgId.ToString().Replace("ReqAck", "Req");
            Debug.LogError($"Send Error (msg.reliable ? \"Tcp\" : \"Udp\") {str} {msg.writer.Length} bytes");
        }            
        else
        {
            if (SpecificMsgIds.DoNotShowLog.Contains(msg.msgId))
                return;

            string str = msg.msgId.ToString().Replace("ReqAck", "Req");
            Debug.Log($"Send {(msg.reliable ? "Tcp" : "Udp")} {str} ({msg.writer.Length} bytes)");
        }   
    }

	protected virtual void OnDestroy ()
    {
        initialized = false;
        if (universalNetwork.IsCreated)
        {
            universalNetwork.Disconnect(universalConnection);
            universalNetwork.Dispose();
        }
    }

    public void Disconnect()
    {
        shutDownCalled = true;
        if (universalNetwork.IsCreated && universalConnection.IsCreated)
            universalConnection.Disconnect(universalNetwork);

        Debug.Log(GetType().ToString() + " disconnect Called");
    }

    public void DisconnectSilently()
    {
        if (universalNetwork.IsCreated && universalConnection.IsCreated)
            universalConnection.Disconnect(universalNetwork);
    }
}
