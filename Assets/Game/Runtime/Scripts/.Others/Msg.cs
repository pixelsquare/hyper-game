using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public partial class Msg
{
    const int DecompressedPayload = 100000;

    public static implicit operator bool(Msg value) { return value != null; }
    const double Life = 8f;

    static int LastPoolCount = 16;
    static List<Msg> usingMsgs = new List<Msg>();
    static List<Msg> sleepMsgs = new List<Msg>();

    public double created;
    public int netUid;
    public bool reliable;
    public MsgId msgId;

    public DataStreamReader reader;
    public DataStreamWriter writer;
    public DataStreamWriter savedWriter;
    public DataStreamWriter willCompressWriter;

    bool willCompress = false;
    NativeArray<byte> nativeArraysFromReader;

    public static Msg Create()
    {
        Msg msg = null;

        while (usingMsgs.Count > 0 && usingMsgs[0].created < Time.realtimeSinceStartupAsDouble)
        {
            msg = usingMsgs[0];

            sleepMsgs.Add(msg);
            usingMsgs.RemoveAt(0);
        }

        int sleepCnt = sleepMsgs.Count;
        if (sleepCnt > 0)
        {
            int idx = sleepCnt - 1;
            msg = sleepMsgs[idx];
            sleepMsgs.RemoveAt(idx);
        }
        else
        {
            msg = new Msg();

            int curr = usingMsgs.Count + sleepMsgs.Count + 1;
            if (curr >= LastPoolCount)
            {
                Debug.LogWarning("Msg Pool Size Expanded to " + LastPoolCount.ToString());
                LastPoolCount *= 2;
            }
        }   

        msg.created = Time.realtimeSinceStartupAsDouble + Life;
        msg.msgId = MsgId.Unknown;

        usingMsgs.Add(msg);
        return msg;
    }

    public void BeginWrite(MsgId msgId, DataStreamWriter w, bool willCompress = false)
    {
        this.msgId = msgId;
        this.willCompress = willCompress;

        if (willCompress)
        {
            savedWriter = w;

            if (willCompressWriter.IsCreated == false)
                willCompressWriter = new DataStreamWriter(
                    new NativeArray<byte>(new byte[DecompressedPayload], Allocator.Persistent));

            willCompressWriter.Clear();
            writer = willCompressWriter;
        }
        else
        {
            writer = w;
            writer.WriteInt(netUid);
            writer.WriteUShort((ushort)msgId);
        }
    }

    public void FinishWrite()
    {
        if (writer.IsCreated == false)
        {
            Debug.LogError("You can't Finish Writer...Not Created..");
            return;
        }

        if (willCompress == false)
            return;
        
        using (MemoryStream ms = new MemoryStream())
        {
            int previous = writer.Length + 6;
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
            {
                NativeArray<byte> bytes = writer.AsNativeArray();
                ds.Write(bytes.ToArray(), 0, bytes.Length);
                ds.Close();
            }

            writer = savedWriter;
            writer.WriteInt(netUid);
            writer.WriteUShort((ushort)msgId);

            NativeArray<byte> tempArray = new NativeArray<byte>(ms.ToArray(), Allocator.Temp);
            writer.WriteBytes(tempArray);
            tempArray.Dispose();

            Debug.Log($"{msgId} Compressed {previous} >> {writer.Length} bytes");
        }

        willCompress = false;        
    }

    public void BeginRead(DataStreamReader r, bool reliable)
    {
        reader = r;
        this.reliable = reliable;
        netUid = reader.ReadInt();
        msgId = (MsgId)reader.ReadUShort();
    }

    public void Decompress()
    {
        int previous = reader.Length;
        NativeArray<byte> nativeBytes = new NativeArray<byte>(new byte[previous-6], Allocator.Temp);
        reader.ReadBytes(nativeBytes);
        
        byte[] compressedBytes = nativeBytes.ToArray();
        nativeBytes.Dispose();

        using (MemoryStream decompressed = new MemoryStream())
        {
            using (MemoryStream compressed = new MemoryStream(compressedBytes))
            {
                using (DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(decompressed);
                }
            }

            nativeArraysFromReader = new NativeArray<byte>(decompressed.ToArray(), Allocator.Temp);                
        }

        reader = new DataStreamReader(nativeArraysFromReader);

        Debug.Log($"{msgId} Decompressed {previous} >> {reader.Length} bytes");
    }
}