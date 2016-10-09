using System;

namespace Homura
{
    public class CallbackList : Singletone<CallbackList>
    {
        public ERROR_CODE Signin(Response _Res)
        {


            return ERROR_CODE.HEC_COMPLETE;
        }
    }
}
