using System;
using System.IO.Pipes;

namespace PipeWrapper
{
    public class PipeBaseForClient : PipeBase
    {
        // protected
        protected NamedPipeClientStream clientPipeStream;

        // public
        public PipeBaseForClient(string serverName, string pipeName, Action<PipeBase> asyncReaderStart)
        {
            this.asyncReaderStart = asyncReaderStart;
            this.clientPipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            this.pipeStream = this.clientPipeStream;
        }
        public void Connect()
        {
            this.clientPipeStream.Connect();
            this.asyncReaderStart(this);
        }
        public void Connect(int milliseconds)
        {
            this.clientPipeStream.Connect(milliseconds);
            this.asyncReaderStart(this);
        }
    }
}
