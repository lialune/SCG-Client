using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
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

        public Request(int _ID)
        {
            mID = _ID;
        }

        public int ID
        {
            get
            {
                return mID;
            }
        }

        public ERROR_CODE Initialize(REQUEST_TYPE _RequestType = REQUEST_TYPE.RT_NONE, int _BufferSize = Data.DATA_DEFAULT_SIZE)
        {
            ERROR_CODE ErrorCode = base.Initialize(_BufferSize);

            if (ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return (ERROR_CODE)ErrorCode;
            }

            if (REQUEST_TYPE.RT_NONE > _RequestType || REQUEST_TYPE.RT_COUNT <= _RequestType)
            {
                return ERROR_CODE.HEC_NOT_DEFINE_REQUEST_TYPE;
            }

            mRequestType = _RequestType;

            if (null == mParamList)
            {
                mParamList = new Dictionary<string, string>();
                if (null == mParamList)
                {
                    return ERROR_CODE.HEC_FAIL_NEW;
                }
            }
            else
            {
                mParamList.Clear();
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE AddParam(string _Key, string _Value)
        {
            if(null == _Key || null == _Value)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            try
            {
                mParamList.Add(_Key, _Value);
            }
            catch(System.ArgumentException)
            {
                return ERROR_CODE.HEC_OVERLAP_KEY;
            }
            
            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE RemoveParam(string _Key)
        {
            if(null == _Key)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            if(mParamList.Remove(_Key))
            {
                return ERROR_CODE.HEC_COMPLETE;
            }

            return ERROR_CODE.HEC_NOT_REGIST_KEY;
        }

        public Dictionary<string, string> ParamList
        {
            get
            {
                return mParamList;
            }
        }

        public ERROR_CODE GetRequest(out string _Str)
        {
            _Str = new string(GetBuffer());

            return ERROR_CODE.HEC_COMPLETE;
        }
    }

    public class RequestManager
    {
        enum REQUEST_STATE
        {
            RT_NOT_USING,
            RT_USING
        }

        class REQUEST_NODE
        {
            public REQUEST_STATE mState;
            public Request mRequest;

            public REQUEST_NODE(int _ID)
            {
                mRequest = new Request(_ID);
            }
        }

        const int DEFAULT_REQUEST_COUNT = 10;
        const int INIT_CREATE_REQUEST_MIN_COUNT = 1;

        static RequestManager mRequestManager = new RequestManager();

        int mUsigRequestCount;
        int mNextCreateRequestID;
        List<REQUEST_NODE> mRequestList = null;

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

        public ERROR_CODE Initialize(int _InitCreateReQuestCount = DEFAULT_REQUEST_COUNT)
        {
            if(INIT_CREATE_REQUEST_MIN_COUNT > _InitCreateReQuestCount)
            {
                return ERROR_CODE.HEC_BELOW_ZERO_TO_SIZE;
            }

            if(null == mRequestList)
            {
                mRequestList = new List<REQUEST_NODE>();

                if(null == mRequestList)
                {
                    return ERROR_CODE.HEC_FAIL_NEW;
                }
            }

            for (int i = 0; _InitCreateReQuestCount > i; ++i)
            {
                if(ERROR_CODE.HEC_COMPLETE != CreateRequest())
                {
                    return ERROR_CODE.HEC_FAIL_NEW;
                }
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Release()
        {
            if(null != mRequestList)
            {
                mRequestList.Clear();
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        //public void Update()
        //{

        //}

        //싱글 스레드라는 가정하에, ID는 함수 성공시에 증가
        ERROR_CODE CreateRequest(REQUEST_TYPE _RequestType = REQUEST_TYPE.RT_NONE, int _BufferSize = Data.DATA_DEFAULT_SIZE)
        {
            REQUEST_NODE NewRequest = new REQUEST_NODE(mNextCreateRequestID);

            if(null == NewRequest)
            {
                return ERROR_CODE.HEC_FAIL_NEW;
            }

            ERROR_CODE ErrorCode = NewRequest.mRequest.Initialize(_RequestType, _BufferSize);
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            NewRequest.mState = REQUEST_STATE.RT_NOT_USING;

            mRequestList.Add(NewRequest);

            ++mNextCreateRequestID;

            return ERROR_CODE.HEC_COMPLETE;
        }

        ERROR_CODE GetRequest(out Request _OutRequest)
        {
            _OutRequest = null;

            if(mRequestList.Count <= mUsigRequestCount)
            {
                ERROR_CODE ErrorCode = CreateRequest();

                if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
                {
                    return ErrorCode;
                }
            }

            if (mRequestList.Count > mUsigRequestCount + 1)
            {
                REQUEST_NODE Temp = mRequestList[mRequestList.Count];

                mRequestList[mRequestList.Count] = mRequestList[mUsigRequestCount];
                mRequestList[mUsigRequestCount] = Temp;
                mRequestList[mUsigRequestCount].mState = REQUEST_STATE.RT_USING;
            }
            _OutRequest = mRequestList[mUsigRequestCount].mRequest;
            ++mUsigRequestCount;

            return ERROR_CODE.HEC_COMPLETE;
        }

        ERROR_CODE ReturnRequest(int _ID)
        {
            if( 0 > _ID || mRequestList.Count <= _ID)
            {
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }

            for(int i = 0; mRequestList.Count > i; ++i)
            {
                if(_ID == mRequestList[i].mRequest.ID)
                {
                    REQUEST_NODE Temp = mRequestList[i];
                    mRequestList[i].mState = REQUEST_STATE.RT_NOT_USING;
                    mRequestList[i] = mRequestList[--mUsigRequestCount];
                    mRequestList[mUsigRequestCount] = Temp;
                    break;
                }
            }

            return ERROR_CODE.HEC_COMPLETE;
        }
    }
}
