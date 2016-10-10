using System;

namespace Homura
{
    public class CallbackList : Singletone<CallbackList>
    {
        public ERROR_CODE Login(Response _Res)
        {
            ERROR_CODE ErrorCode = _Res.Parsing();
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            // 요청 결과 확인
            string Value;
            ErrorCode = _Res.GetValue("Reslut", out Value);
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            if(Value.Equals("Success"))
            {

            }
            else
            {

            }

            return ERROR_CODE.HEC_COMPLETE;
        }
    }
}
