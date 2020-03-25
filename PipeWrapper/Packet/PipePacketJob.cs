using System;
using System.Collections.Generic;
using System.Text;

namespace PipeWrapper.Packet
{
    public class PipePacketBase
    {
        /// <summary>
        /// Packet 코드
        /// </summary>
        public PipePacket mode { get; set; }
        /// <summary>
        /// 응답코드
        /// </summary>
        public PipePacketStatus status { get; set; }
        /// <summary>
        /// 처리 데이터 (PipePacketFor + xxx 클래스)
        /// </summary>
        public object data { get; set; }
    }
}
