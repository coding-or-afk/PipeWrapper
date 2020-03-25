using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeWrapper
{
    public abstract class PipeBase
    {
        // protected
        protected PipeStream pipeStream;
        protected Action<PipeBase> asyncReaderStart;
        protected void StartByteReaderAsync(Action<byte[]> packetReceived)
        {
            int count = 4;
            byte[] bDataLength = new byte[count];
            this.pipeStream.ReadAsync(bDataLength, 0, count).ContinueWith((Action<Task<int>>)(t =>
            {
                int len = t.Result;
                if (len == 0)
                {
                    EventHandler<EventArgs> pipeClosed = this.PipeClosed;
                    if (pipeClosed == null)
                        return;
                    pipeClosed((object)this, EventArgs.Empty);
                }
                else
                {
                    int int32 = BitConverter.ToInt32(bDataLength, 0);
                    byte[] data = new byte[int32];
                    this.pipeStream.ReadAsync(data, 0, int32).ContinueWith((Action<Task<int>>)(t2 =>
                    {
                        len = t2.Result;
                        if (len == 0)
                        {
                            EventHandler<EventArgs> pipeClosed = this.PipeClosed;
                            if (pipeClosed == null)
                                return;
                            pipeClosed((object)this, EventArgs.Empty);
                        }
                        else
                        {
                            packetReceived(data);
                            this.StartByteReaderAsync(packetReceived);
                        }
                    }));
                }
            }));
        }

        // public
        public event EventHandler<PipeBaseEventArgs> DataReceived;
        public event EventHandler<EventArgs> PipeClosed;
        public void Close()
        {
            this.pipeStream.WaitForPipeDrain();
            this.pipeStream.Close();
            this.pipeStream.Dispose();
            this.pipeStream = null;
            this.DataReceived = null;
        }
        public void StartByteReaderAsync()
        {
            this.StartByteReaderAsync((Action<byte[]>)(b =>
            {
                EventHandler<PipeBaseEventArgs> dataReceived = this.DataReceived;
                if (dataReceived == null)
                    return;
                dataReceived((object)this, new PipeBaseEventArgs(b, b.Length));
            }));
        }
        public void StartStringReaderAsync()
        {
            this.StartByteReaderAsync((Action<byte[]>)(b =>
            {
                string str = Encoding.UTF8.GetString(b).TrimEnd(new char[1]);
                EventHandler<PipeBaseEventArgs> dataReceived = this.DataReceived;
                if (dataReceived == null)
                    return;
                dataReceived((object)this, new PipeBaseEventArgs(str));
            }));
        }
        public void Flush()
        {
            this.pipeStream.Flush();
        }
        public Task WriteString(string str)
        {
            return this.WriteBytes(Encoding.UTF8.GetBytes(str));
        }
        public Task WriteBytes(byte[] bytes)
        {
            byte[] array = ((IEnumerable<byte>)BitConverter.GetBytes(bytes.Length)).Concat<byte>((IEnumerable<byte>)bytes).ToArray<byte>();
            return this.pipeStream.WriteAsync(array, 0, array.Length);
        }
        public void ClearReceivedHandler()
        {
            this.DataReceived = null;
        }
    }
}
