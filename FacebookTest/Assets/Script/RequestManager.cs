using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
    public enum REQUEST_ERROR_CODE
    {
        REC_NOT_DEFINE_REQUEST_TYPE = DATA_ERROR_CODE.DEC_COUNT,
        REC_COMPLETE,
        REC_FAIL_NEW,
        REC_OVERLAP_KEY,
        REC_NOT_REGIST_KEY,
        REC_COUNT
    }

    public enum REQUEST_TYPE
    {
        RT_NONE,
        RT_COUNT
    }

    public class Request : Homura.Data
    {
        // KeyName, Value
        Dictionary<string, string> mParamList;
        REQUEST_TYPE mRequestType;
        int mID;

        public Request()
        {
        }

        public REQUEST_ERROR_CODE Initialize(REQUEST_TYPE _RequestType = REQUEST_TYPE.RT_NONE, int _BufferSize = Data.DATA_DEFAULT_SIZE)
        {
            DATA_ERROR_CODE ErrorCode = base.Initialize(_BufferSize);

            if (DATA_ERROR_CODE.DEC_COMPLETE != ErrorCode)
            {
                return (REQUEST_ERROR_CODE)ErrorCode;
            }

            if (REQUEST_TYPE.RT_NONE > _RequestType || REQUEST_TYPE.RT_COUNT <= _RequestType)
            {
                return REQUEST_ERROR_CODE.REC_NOT_DEFINE_REQUEST_TYPE;
            }

            mRequestType = _RequestType;

            if (null == mParamList)
            {
                mParamList = new Dictionary<string, string>();
                if (null == mParamList)
                {
                    return REQUEST_ERROR_CODE.REC_FAIL_NEW;
                }
            }
            else
            {
                mParamList.Clear();
            }

            return REQUEST_ERROR_CODE.REC_COMPLETE;
        }

        public REQUEST_ERROR_CODE AddParam(string _Key, string _Value)
        {
            if(null == _Key || null == _Value)
            {
                return (REQUEST_ERROR_CODE)DATA_ERROR_CODE.DEC_NULL_DATA;
            }

            try
            {
                mParamList.Add(_Key, _Value);
            }
            catch(System.ArgumentException)
            {
                return REQUEST_ERROR_CODE.REC_OVERLAP_KEY;
            }
            
            return REQUEST_ERROR_CODE.REC_COMPLETE;
        }

        public REQUEST_ERROR_CODE RemoveParam(string _Key)
        {
            if(null == _Key)
            {
                return (REQUEST_ERROR_CODE)DATA_ERROR_CODE.DEC_NULL_DATA;
            }

            if(mParamList.Remove(_Key))
            {
                return REQUEST_ERROR_CODE.REC_COMPLETE;
            }

            return REQUEST_ERROR_CODE.REC_NOT_REGIST_KEY;
        }

        public Dictionary<string, string> ParamList
        {
            get
            {
                return mParamList;
            }
        }

        public REQUEST_ERROR_CODE GetRequest(out string _Str)
        {
            _Str = new string(GetBuffer());

            return REQUEST_ERROR_CODE.REC_COMPLETE;
        }
    }

    public class RequestManager
    {
        public static RequestManager mRequestManager = new RequestManager();
        req

        RequestManager()
        {
        }

        public RequestManager Instance
        {
            get
            {
                return mRequestManager;
            }
        }

        void Update()
        {

        }
    }
}
