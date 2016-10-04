using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using JsonFx.Json;
using Facebook.Unity;

namespace Homura
{
    using PARAM_LIST = Dictionary<string, string>;
    using PAGE_NAME_LIST = Dictionary<REQUEST_TYPE, string>;

    public class Server : MonoBehaviour
    {
        string IP;
        string Port;
        string ProjectName;
        string DefaultUri;
        PAGE_NAME_LIST mPageNameList;

        ERROR_CODE Initialize()
        {
            if(null == mPageNameList)
            {
                mPageNameList = new PAGE_NAME_LIST();
            }

            ERROR_CODE ErrorCode = Homura.RequestManager.Instance.Initialize();
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

            PARAM_LIST ParamList = JsonReader.Deserialize<PARAM_LIST>("./Config/Server.txt");
            if(null == ParamList)
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

            return ERROR_CODE.HEC_COMPLETE;
        }

        IEnumerator RequestMessage(Request _Request)
        {
            if (null != _Request)
            {
                // 요청 시작
                WWW WebObj = new WWW(DefaultUri + mPageNameList[_Request.GetRequestType()]);
                yield return WebObj;
                //요청 에러 여부 확인
                if (!string.IsNullOrEmpty(WebObj.error))
                {
                    Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() WebObj Error! ErrorMessage : " + WebObj.error);
                }
                
                // 전송받은 데이터로 Response객체 생성
                string Data = WebObj.text;
                Data.Replace("\r\n", "");

                Homura.Response Res = new Response();
                ERROR_CODE ErrorCode = Res.Initialize(_Request.GetRequestType(), Data);
                if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
                {
                    Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Server.RequestMessage() Response.Initialize() Error! ErrorMessage : " + ErrorCode);
                }
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

        void Start()
        {
            ERROR_CODE ErrorCode = Initialize();
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_4, "Fail Server.Initialize" + ErrorCode);
            }
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

        }
    }
}