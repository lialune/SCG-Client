using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

namespace Homura
{
    public class Response : Homura.Data
    {
        PARAM_LIST mParamList;
        REQUEST_TYPE mRequestType;

        public Response()
        {
        }

        public ERROR_CODE Initialize(REQUEST_TYPE _RequestType, string _Data)
        {
            ERROR_CODE ErrorCode = base.Initialize(_Data.Length);
            if (ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            if (REQUEST_TYPE.RT_NONE > _RequestType || REQUEST_TYPE.RT_COUNT <= _RequestType)
            {
                return ERROR_CODE.HEC_NOT_DEFINE_REQUEST_TYPE;
            }

            mRequestType = _RequestType;

            if (null == mParamList)
            {
                mParamList = new PARAM_LIST();
                if(null == mParamList)
                {
                    return ERROR_CODE.HEC_FAIL_NEW;
                }
            }
            else
            {
                mParamList.mParamList.Clear();
            }

            ErrorCode = Append(_Data);
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Parsing()
        {
            if(null == GetBuffer())
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            string Data = new string(GetBuffer());
            if(null == Data)
            {
                return ERROR_CODE.HEC_FAIL_NEW;
            }

            mParamList = JsonReader.Deserialize<PARAM_LIST>(Data);

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE GetValue(string _Key, out string _Value)
        {
            if(null == _Key)
            {
                _Value = null;
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }

            string Value;
            if (!mParamList.mParamList.TryGetValue(_Key, out Value))
            {
                _Value = null;
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }
            else
            {
                _Value = Value;
                return ERROR_CODE.HEC_COMPLETE;
            }
            
        }

        public REQUEST_TYPE GetRequestType()
        {
            return mRequestType;
        }
    }
}