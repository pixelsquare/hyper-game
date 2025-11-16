using System;
using System.Collections.Generic;
using Unity.Collections;

public partial class Msg
{
    public void Write(Serializable serializable)
    {
        if (msgId == MsgId.Unknown)
            return;

        serializable.Serialize(this);
    }

    public void Write(byte[] array, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        int length = array != null ? array.Length : 0;

        if (smallCapacity)
            Write((byte)length);
        else
            Write((ushort)length);

        if (array != null)
            foreach (byte i in array)
                Write(i);
    }

    public void Write(short[] array, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)array.Length);
        else
            Write((ushort)array.Length);

        foreach (short i in array)
            Write(i);
    }

    public void Write(int[] array, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)array.Length);
        else
            Write((ushort)array.Length);

        foreach (int i in array)
            Write(i);
    }

    public void Write(List<int> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (int i in list)
            Write(i);
    }

    public void Write(HashSet<int> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (int i in list)
            Write(i);
    }

    public void Write(List<ushort> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (ushort us in list)
            Write(us);
    }

    public void Write(List<short> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (short s in list)
            Write(s);
    }

    public void Write(List<byte> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (byte b in list)
            Write(b);
    }

    public void Write(List<float> list, bool smallCapacity = false)
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (float f in list)
            Write(f);
    }

    public void WriteList<T>(List<T> list, bool smallCapacity = false) where T : Serializable
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)list.Count);
        else
            Write((ushort)list.Count);

        foreach (T t in list)
            Write(t);
    }

    public void WriteArray<T>(T[] array, bool smallCapacity = false) where T : Serializable
    {
        if (msgId == MsgId.Unknown)
            return;

        if (smallCapacity)
            Write((byte)array.Length);
        else
            Write((ushort)array.Length);

        foreach (T t in array)
            Write(t);
    }

    public T Read<T>() where T : Serializable, new()
    {
        T t = new T();
        t.Deserialize(this);
        return t;
    }
    public List<T> ReadList<T>(bool smallCapacity = false) where T : Serializable, new()
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<T> list = new List<T>();
            for (int i = 0; i < cnt; i++)
                list.Add(Read<T>());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<" + typeof(T).ToString() + ">'!");
        }
    }

    public T[] ReadArray<T>(bool smallCapacity = false) where T : Serializable, new()
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            T[] array = new T[cnt];
            for (int i = 0; i < cnt; i++)
                array[i] = Read<T>();

            return array;
        }
        catch
        {
            throw new Exception("Could not read value of type 'Array<" + typeof(T).ToString() + ">'!");
        }
    }

    public byte[] ReadArrayByte(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            byte[] array = new byte[cnt];
            for (int i = 0; i < cnt; i++)
                array[i] = ReadByte();

            return array;
        }
        catch
        {
            throw new Exception("Could not read value of type 'Array<byte>'!");
        }
    }

    public short[] ReadArrayShort(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            short[] array = new short[cnt];
            for (int i = 0; i < cnt; i++)
                array[i] = ReadShort();

            return array;
        }
        catch
        {
            throw new Exception("Could not read value of type 'Array<short>'!");
        }
    }

    public int[] ReadArrayInt(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            int[] array = new int[cnt];
            for (int i = 0; i < cnt; i++)
                array[i] = ReadInt();

            return array;
        }
        catch
        {
            throw new Exception("Could not read value of type 'Array<int>'!");
        }
    }

    public List<int> ReadListInt(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<int> list = new List<int>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadInt());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<int>'!");
        }
    }

    public HashSet<int> ReadHashSetInt(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            HashSet<int> list = new HashSet<int>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadInt());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'HashSet<int>'!");
        }
    }

    public List<short> ReadListShort(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<short> list = new List<short>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadShort());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<short>'!");
        }
    }

    public List<ushort> ReadListUShort(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<ushort> list = new List<ushort>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadUShort());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<ushort>'!");
        }
    }

    public List<byte> ReadListByte(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<byte> list = new List<byte>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadByte());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<byte>'!");
        }
    }

    public List<float> ReadListFloat(bool smallCapacity = false)
    {
        try
        {
            int cnt = smallCapacity ? ReadByte() : ReadUShort();

            List<float> list = new List<float>();
            for (int i = 0; i < cnt; i++)
                list.Add(ReadFloat());

            return list;
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<float>'!");
        }
    }
}
