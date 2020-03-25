using PipeWrapper.Packet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace PipeWrapper
{
    // 2020-02-18 orseL
    //  : 사용자에 의해 pipe 가 종료되었을때 처리가 필요함
    public class PipeServer
    {
        // private
        private List<PipeBaseForServer> serverList = null;
        private PipeBaseForServer lastServer = null;
        private string pipeName = "";
        // 2020-01-30 orseL
        //  : 생성자에서 함수포인터를 인자값으로 추가
        //  : 각 프로젝트에서 pipe 서버 설치시 
        private PipeFunction pipeFunction = null;
        private PipeCloseFunction pipeCloseFunction = null;
        private void CreateServer()
        {
            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
            pipeSecurity.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            lastServer = new PipeBaseForServer(this.pipeName, p => p.StartByteReaderAsync(), pipeSecurity);
            lastServer.Connected += new EventHandler<EventArgs>(Connected);
            lastServer.DataReceived += new EventHandler<PipeBaseEventArgs>(DataReceived);
            lastServer.PipeClosed += new EventHandler<EventArgs>(PipeClosed);
        }
        private void Connected(object sender, EventArgs args)
        {
            // loop 구조의 pipe 서버가 아닌 한건씩 처리하고 종료후 새로생성
            serverList.Add(lastServer);
            lastServer.ClearConnectedEvent();
            this.CreateServer();
        }
        private void PipeClosed(object sender, EventArgs args)
        {
            this.pipeCloseFunction();
        }
        private void DataReceived(object sender, PipeBaseEventArgs args)
        {
            PipeBaseForServer server = (PipeBaseForServer)sender;
            byte[] reply = args.Data;
            this.pipeFunction(ref reply);
            server.WriteBytes(reply);
        }

        // pubilc
        public delegate void PipeFunction(ref byte[] reply);    // data received 함수포인터
        public delegate void PipeCloseFunction();               // pipe closed 함수포인터
        public PipeServer(string pipeName, PipeFunction pipeFunction, PipeCloseFunction pipeCloseFunction = null)
        {
            this.serverList = new List<PipeBaseForServer>();
            this.pipeName = pipeName;
            this.pipeFunction = pipeFunction;
            this.pipeCloseFunction = pipeCloseFunction;

            CreateServer();
        }
        public void Close()
        {
            try
            {
                for (int i = 0; i < this.serverList.Count; i++)
                {
                    this.serverList[i].Close();
                }
            }
            catch (Exception e)
            {
                CommonLog.WriteLog(LogLevel.Error, "Except during closing pipe server. err: " + e.Message);
            }
        }
    }
}
