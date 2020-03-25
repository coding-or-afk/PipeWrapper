using System;
using System.IO.Pipes;

namespace PipeWrapper
{
    public class PipeBaseForServer : PipeBase
    {
        // protected
        protected NamedPipeServerStream serverPipeStream;
        protected string PipeName { get; set; }
        protected void PipeConnected(IAsyncResult ar)
        {
            this.serverPipeStream.EndWaitForConnection(ar);
            EventHandler<EventArgs> connected = this.Connected;
            if (connected != null)
            {
                connected(this, new EventArgs());
            }

            this.asyncReaderStart(this);
        }

        // public
        public event EventHandler<EventArgs> Connected;
        public PipeBaseForServer(string pipeName, Action<PipeBase> asyncReaderStart)
        {
            this.asyncReaderStart = asyncReaderStart;
            this.PipeName = pipeName;
            this.serverPipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, CommonConstant.PipeMaxInstancesCount,
                PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            this.pipeStream = this.serverPipeStream;
            this.serverPipeStream.BeginWaitForConnection(new AsyncCallback(this.PipeConnected), null);
        }
        public PipeBaseForServer(string pipeName, Action<PipeBase> asyncReaderStart, PipeSecurity security)
        {
            this.asyncReaderStart = asyncReaderStart;
            this.PipeName = pipeName;
            this.serverPipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, CommonConstant.PipeMaxInstancesCount,
                PipeTransmissionMode.Byte, PipeOptions.Asynchronous, CommonConstant.PipeBufferSize, CommonConstant.PipeBufferSize, security);
            this.pipeStream = this.serverPipeStream;
            this.serverPipeStream.BeginWaitForConnection(new AsyncCallback(this.PipeConnected), null);
        }
        public void ClearConnectedEvent()
        {
            this.Connected = null;
        }
    }
}
