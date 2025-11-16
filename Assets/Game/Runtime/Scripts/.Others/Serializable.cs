using System.Collections;
#pragma warning disable 0618

public abstract class Serializable
{
    public static implicit operator bool(Serializable s) { return s != null; }

    public Serializable() { }
    public Serializable (Msg msg) {
        Deserialize(msg);
    }
    
    public abstract void Serialize(Msg msg);
    public abstract void Deserialize(Msg msg);
}

