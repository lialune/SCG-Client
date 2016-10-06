using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using Facebook.Unity;

namespace Homura
{
    using PARAM_LIST = Dictionary<string, string>;
    using PAGE_NAME_LIST = Dictionary<REQUEST_TYPE, string>;
    using REQUEST_CALLBACK_FUNC_LIST = Dictionary<REQUEST_TYPE, REQUEST_CALLBACK_FUNC>;
    delegate ERROR_CODE REQUEST_CALLBACK_FUNC(Response _Responese);

    public enum LOGIN_MODULE_TYPE
    {
        LMT_FACEBOOK,
        LMT_GOOLE_PLUS,
    }

    public enum LOGIN_STATE
    {
        LS_NONE,
        LS_WAIT_LOGIN,
        LS_LOGIN_SUCCESS,
        LS_LOGIN_FAIL
    }

    public class Server : MonoBehaviour
    {
        string IP;
        string Port;
        string ProjectName;
        string DefaultUri;
        LOGIN_STATE mLoginState;
        PAGE_NAME_LIST mPageNameList;
        REQUEST_CALLBACK_FUNC_LIST mRequestCallbackList;
        FacebookModule mFaceBookModule;

        ERROR_CODE Initialize()
        {
            if(null == mPageNameList)
            {
                mPageNameList = new PAGE_NAME_LIST();
            }
            else
            {
                mPageNameList.Clear();
            }

            if (null == mRequestCallbackList)
            {
                mRequestCallbackList = new REQUEST_CALLBACK_FUNC_LIST();
            }
            else
            {
                mRequestCallbackList.Clear();
            }


            ERROR_CODE ErrorCode = Log.Instanec.Initialize(0f);
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                Debug.Log("Server.Initialize() Log.Instance.Initialize() Error! ErrorCode : " + ErrorCode);
                return ErrorCode;
            }

            Log.Instanec.SetSaveType(Log.WARNING_LEVEL.WL_1, Log.LOG_SAVE_TYPE.LST_CONSOLE);
            Log.Instanec.SetSaveType(Log.WARNING_LEVEL.WL_2, Log.LOG_SAVE_TYPE.LST_CONSOLE);
            Log.Instanec.SetSaveType(Log.WARNING_LEVEL.WL_3, Log.LOG_SAVE_TYPE.LST_CONSOLE);
            Log.Instanec.SetSaveType(Log.WARNING_LEVEL.WL_4, Log.LOG_SAVE_TYPE.LST_CONSOLE);

            ErrorCode = Homura.RequestManager.Instance.Initialize();
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            // 서버 세팅 읽기 시작
            StreamReader sr = new StreamReader("Assets/Config/Server.txt");
            string Data = sr.ReadToEnd();
            PARAM_LIST ParamList = JsonReader.Deserialize<PARAM_LIST>(Data);
            if (null == ParamList)
            {
                return ERROR_CODE.HEC_FAIL_FILE_READ;
            }
            IP = ParamList["IP"];
            if (null == IP || "" == IP)
            {
                return ERROR_CODE.HEC_FAIL_FILE_READ;
            }
            Port = ParamList["Port"];
            if (null == Port || "" == Port)
            {
                return ERROR_CODE.HEC_FAIL_FILE_READ;
            }
            ProjectName = ParamList["ProjectName"];
            if (null == ProjectName || "" == ProjectName)
            {
                return ERROR_CODE.HEC_FAIL_FILE_READ;
            }
            if(null == DefaultUri)
            {
                DefaultUri = "http://" + IP + ":" + Port + "/" + ProjectName + "/";
            }
            // 서버 세팅 읽기 완료

            if(null == mFaceBookModule)
            {
                mFaceBookModule = new FacebookModule();
            }
            else
            {
                mFaceBookModule.Initialize();
            }
            
            mLoginState = LOGIN_STATE.LS_NONE;

            return ERROR_CODE.HEC_COMPLETE;
        }

        // 비동기 메시지 전송용 함수
        IEnumerator RequestMessage(Request _Request)
        {
            if (null == _Request)
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() _Request null!");
                yield break;
            }

            // 요청 시작
            WWW WebObj = new WWW(DefaultUri + mPageNameList[_Request.GetRequestType()] + "/" + _Request);
            yield return WebObj;
            //요청 에러 여부 확인
            if (!string.IsNullOrEmpty(WebObj.error))
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() WebObj Error! ErrorMessage : " + WebObj.error);
                yield break;
            }

            // 전송받은 데이터로 Response객체 생성
            string Data = WebObj.text;
            Data.Replace("\r\n", "");

            Homura.Response Res = new Response();
            ERROR_CODE ErrorCode = Res.Initialize(_Request.GetRequestType(), Data);
            if (ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() Response.Initialize() Error! ErrorMessage : " + ErrorCode);
                yield break;
            }

            ErrorCode = mRequestCallbackList[Res.GetRequestType()](Res);
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() Response.Initialize() Error! Request Callback Func Error! ErrorMessage : " + ErrorCode);
            }
        }

        public ERROR_CODE AddPageName(REQUEST_TYPE _RequestType, string _PageName)
        {
            if(REQUEST_TYPE.RT_NONE >= _RequestType || REQUEST_TYPE.RT_COUNT <= _RequestType)
            {
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }

            if (string.IsNullOrEmpty(_PageName))
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            string Temp;
            if (mPageNameList.TryGetValue(_RequestType, out Temp))
            {
                return ERROR_CODE.HEC_OVERLAP_KEY;
            }

            mPageNameList.Add(_RequestType, _PageName);

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Login(LOGIN_MODULE_TYPE _Type)
        {
            switch(_Type)
            {
                case LOGIN_MODULE_TYPE.LMT_FACEBOOK:
                    {
                        mFaceBookModule.Initialize();
                        mFaceBookModule.Login();
                    }break;
                case LOGIN_MODULE_TYPE.LMT_GOOLE_PLUS:
                    {
                    }break;
                default:
                    {
                        return ERROR_CODE.HEC_NOT_VALID_VALUE;
                    }
            }
            return ERROR_CODE.HEC_COMPLETE;
        }

        public LOGIN_STATE LoginState
        {
            set
            {
                mLoginState = value;
            }
            get
            {
                return mLoginState;
            }
        }

        ERROR_CODE AddRequestCallBack(REQUEST_TYPE _RequestType, REQUEST_CALLBACK_FUNC _Func)
        {
            if (REQUEST_TYPE.RT_NONE >= _RequestType || REQUEST_TYPE.RT_COUNT <= _RequestType)
            {
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }

            if (null == _Func)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            REQUEST_CALLBACK_FUNC Temp;
            if (mRequestCallbackList.TryGetValue(_RequestType, out Temp))
            {
                return ERROR_CODE.HEC_OVERLAP_KEY;
            }

            mRequestCallbackList.Add(_RequestType, _Func);

            return ERROR_CODE.HEC_COMPLETE;
        }

        void Awake()
        {
            ERROR_CODE ErrorCode = Initialize();
            if (ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_4, "Fail Server.Initialize" + ErrorCode);
            }
        }

        void Update()
        {
            Log.Instanec.Update();

            switch(mLoginState)
            {
                case LOGIN_STATE.LS_LOGIN_SUCCESS:
                    {
                        Homura.Request Req;
                        RequestManager.Instance.GetRequest(out Req);

                        Req.Initialize(REQUEST_TYPE.RT_LOGIN);
                        Req.AddParam("Type", "F");
                        Req.AddParam("ID", mFaceBookModule.GetParam("ID"));

                        StartCoroutine(RequestMessage(Req));
                    }break;
            }
        }
    }
}