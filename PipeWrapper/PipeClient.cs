using System;
using System.Text;
using System.Threading;
using System.Diagnostics;

using PipeWrapper.Packet;
using Newtonsoft.Json.Linq;

namespace PipeWrapper
{
    public class PipeClient
    {
        // private
        private PipeBaseForClient client = null;
        private string pipeName = "";
        private object locker = new object();
        private string replyData;
        private PipePacketStatus pipeStatus = PipePacketStatus.Error;
        private bool CheckStatusSuccess()
        {
            lock (locker)
            {
                if (this.pipeStatus == PipePacketStatus.Success)
                {
                    return true;
                }

                return false;
            }
        }
        private void OnReceive(object sender, PipeBaseEventArgs args)
        {
            byte[] data = args.Data;
            string received = Encoding.UTF8.GetString(data);
            this.replyData = received;
            SetStatus(PipePacketStatus.Success);
            //CommonLog.WriteLog(LogLevel.Debug, "Client received: " + received);
        }
        private void SetStatus(PipePacketStatus pipeStatus)
        {
            lock (locker)
            {
                this.pipeStatus = pipeStatus;
            }
        }

        // public
        public JObject ReceiveData(ref PipePacketStatus status)
        {
            JObject whole = JObject.Parse(this.replyData);
            status = (PipePacketStatus)Convert.ToInt32(((string)whole["status"]));

            return whole;
        }
        public PipeClient(string pipeName)
        {
            this.pipeName = pipeName;
            client = new PipeBaseForClient(".", this.pipeName, p => p.StartByteReaderAsync());
            client.Connect();
            client.DataReceived += OnReceive;
        }
        public PipeClient(string pipeName, int timeoutMilliseconds)
        {
            this.pipeName = pipeName;
            client = new PipeBaseForClient(".", this.pipeName, p => p.StartByteReaderAsync());
            client.Connect(timeoutMilliseconds);
            client.DataReceived += OnReceive;
        }
        public void SendData(byte[] data, bool closeConnection = false)
        {
            SetStatus(PipePacketStatus.Error);
            byte[] request = data;
            client.WriteBytes(request);
            if (closeConnection == true) client.Close();
        }
        public void SendData(string data, bool closeConnection = false)
        {
            SetStatus(PipePacketStatus.Error);
            byte[] request = Encoding.UTF8.GetBytes(data);
            client.WriteBytes(request);
            if (closeConnection == true) client.Close();
        }
        public void Close()
        {
            this.Release();
            client.ClearReceivedHandler();
            client.Close();
        }
        public void Release()
        {
            ////this.RecvData = null;
        }
        public bool Wait(int timeout = 10)
        {
            timeout *= 40;
            while (timeout > 0)
            {
                Thread.Sleep(25);
                if (CheckStatusSuccess() == true)
                {
                    return true;
                }

                timeout -= 1;
            }

            if (this.pipeStatus == PipePacketStatus.Pending)
            {
                client.Close();
                client = new PipeBaseForClient(".", pipeName, p => p.StartByteReaderAsync());
                client.Connect();
                client.DataReceived += OnReceive;
            }

            return false;
        }
    }
}
