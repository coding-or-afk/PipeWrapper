using System;
using System.Collections.Generic;
using System.Text;

namespace PipeWrapper.Packet
{
    public enum PipePacketStatus : int
    {
        /// <summary>
        /// 성공
        /// </summary>
        Success = 0,
        /// <summary>
        /// 실패 (Exception)
        /// </summary>
        Error = 1,
        /// <summary>
        /// 권한 오류
        /// </summary>
        AccessDenied = 2,
        /// <summary>
        /// 통신 오류
        /// </summary>
        NetworkError = 3,
        /// <summary>
        /// 만료된 세션
        /// </summary>
        InvalidSession = 4,
        /// <summary>
        /// 잘못된 요청
        /// </summary>
        InvalidRequest = 5,
        /// <summary>
        /// 작업이 진행중 (완료될때까지 기다리기)
        /// </summary>
        Pending = 6,
        /// <summary>
        /// 파일이 존재하지 않음 (삭제됨)
        /// </summary>
        ObjectNameNotFound = 7,
        /// <summary>
        /// 잘못된 인자값
        /// </summary>
        InvalidParameter = 8,
        /// <summary>
        /// 서버 에러
        /// </summary>
        ServerError = 9,
        /// <summary>
        /// 공유 위반
        /// </summary>
        SharingVioration = 10,
        /// <summary>
        /// ??
        /// </summary>
        ObjectDownloading = 11,
        /// <summary>
        /// 이미있는 파일
        /// </summary>
        AlreadyExistFile = 12,
        /// <summary>
        /// ??
        /// </summary>
        TaskNotEnd = 13,
        /// <summary>
        /// ??
        /// </summary>
        NeedRequestFile = 14,
        /// <summary>
        /// ??
        /// </summary>
        NotImplement = 15,
        /// <summary>
        /// 실패 (결과값 실패)
        /// </summary>
        False = 16,
    }
}
