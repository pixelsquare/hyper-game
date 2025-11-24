namespace Santelmo.Rinsurv
{
    public interface IMessage
    {
        public string Type { get; set; }

        public object Sender { get; set; }

        public object Recipient { get; set; }

        public float Delay { get; set; }

        public int Id { get; set; }

        public object Data { get; set; }

        public bool IsSent { get; set; }

        public bool IsHandled { get; set; }

        public int FrameIndex { get; set; }

        public void Clear();

        public void Release();
    }
}
