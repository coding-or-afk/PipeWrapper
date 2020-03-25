namespace PipeWrapper
{
    public class PipeBaseEventArgs
    {
        public byte[] Data { get; protected set; }
        public int Len { get; protected set; }
        public string String { get; protected set; }
        public object ClientManager { get; set; }
        public PipeBaseEventArgs(string str)
        {
            this.String = str;
        }
        public PipeBaseEventArgs(byte[] data, int len)
        {
            this.Data = data;
            this.Len = len;
        }
    }
}
