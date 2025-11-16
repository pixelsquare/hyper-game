using System;
using System.Text;
using UnityEngine;

public partial class Msg
{
    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public short ReadShort()
    {
        return reader.ReadShort();
    }

    public ushort ReadUShort()
    {
        return reader.ReadUShort();
    }

    public int ReadInt()
    {
        return reader.ReadInt();
    }

    public uint ReadUInt()
    {
        return reader.ReadUInt();
    }

    public long ReadLong()
    {
        return reader.ReadLong();
    }

    public ulong ReadULong()
    {
        return reader.ReadULong();
    }

    public float ReadFloat()
    {
        return reader.ReadFloat();
    }

    public double ReadDouble()
    {
        return reader.ReadDouble();
    }

    public bool ReadBool()
    {
        return reader.ReadByte() != 0;
    }

    static byte[] bytesForString = new byte[1024];

    public string ReadString()
    {
        int length = ReadUShort();
        if (length == 0)
            return "";

        try
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
                bytes[i] = reader.ReadByte();
            
            return Encoding.Unicode.GetString(bytes, 0, length);
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public DateTime ReadDateTime()
    {
        // 서버가 ServerTimeZone 기준 DateTime.Now를 보냈고, Db에 Kind가 떨어지더라도 서버의 기준은 명확하기에
        // Kind 정보가 있으면 무조건 서버 시간대로 맞춰 해석함
        DateTime rawTime = DateTime.FromBinary(ReadLong());

        if( rawTime.Kind == DateTimeKind.Unspecified )
            return rawTime;

        return rawTime.ToUniversalTime().AddHours(8);   // 명시적으로 정의할 필요가 있음 [ UTC+ (a) ]

    }

    public Vector3 ReadVector2()
    {
        return new Vector2(ReadFloat(), ReadFloat());
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Quaternion ReadQuaternion()
    {
        return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }

    public void Write(byte value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteByte(value);
    }

    public void Write(short value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteShort(value);
    }

    public void Write(ushort value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteUShort(value);
    }

    public void Write(int value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteInt(value);
    }

    public void Write(uint value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteUInt(value);
    }

    public void Write(long value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteLong(value);
    }

    public void Write(ulong value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteULong(value);
    }

    public void Write(float value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteFloat(value);
    }

    public void Write(double value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteDouble(value);
    }

    public void Write(bool value)
    {
        if (msgId == MsgId.Unknown)
            return;

        writer.WriteByte((byte)(value ? 1 : 0));
    }

    public void Write(string value)
    {
        if (msgId == MsgId.Unknown)
            return;

        int length = string.IsNullOrEmpty(value) ? 0 : value.Length;
        if (length == 0)
        {
            writer.WriteUShort(0);
            return;
        }
        
        if (length > 0)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            writer.WriteUShort((ushort)bytes.Length);
            foreach (byte b in bytes)
                writer.WriteByte(b);
        }
    }

    public void Write(DateTime value)
    {
        if (msgId == MsgId.Unknown)
            return;


        writer.WriteLong(value.ToBinary());
    }

    public void Write(Vector2 value)
    {
        if (msgId == MsgId.Unknown)
            return;

        Write(value.x);
        Write(value.y);
    }

    public void Write(Vector3 value)
    {
        if (msgId == MsgId.Unknown)
            return;

        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(Quaternion value)
    {
        if (msgId == MsgId.Unknown)
            return;

        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }
}
