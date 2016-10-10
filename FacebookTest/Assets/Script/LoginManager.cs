using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

namespace Homura
{
    using PARAM_LIST = Dictionary<string, string>;

    public enum LOGIN_STATE
    {
        LS_NONE,
        LS_MODE_SELECT,
        LS_NONE_LOGIN_MODE,
        LS_LOGIN_MODE,
        LS_LOGIN_TYPE_SELECT,
        LS_LOGIN_FACEBOOK,
        LS_LOGIN_GOOGLE,
        LS_LOGIN_COUNT
    }

    public enum MOUSE_BUTTON
    {
        MB_LEFT,
        MB_RIGHT,
        MB_MIDDLE
    }

    class LoginManager : MonoBehaviour
    {
        LOGIN_STATE mLoginState;

        public LOGIN_STATE LoginState
        {
            get
            {
                return mLoginState;
            }
            set
            {
                mLoginState = value;
            }
        }

        ERROR_CODE Initialize()
        {
            mLoginState = LOGIN_STATE.LS_NONE;

            return ERROR_CODE.HEC_COMPLETE;
        }

        IEnumerator Login()
        {
            StreamReader sr = new StreamReader("Assets/Config/LoginSetting.txt");
            string Data = sr.ReadToEnd();
            PARAM_LIST ParamList = JsonReader.Deserialize<PARAM_LIST>(Data);

            // 로그인 모드에 대한 정보가 없음
            if("None" == ParamList["LoginMode"])
            {
                while(true)
                {
                    if(Input.GetMouseButtonDown((int)MOUSE_BUTTON.MB_LEFT))
                    {
                        // 비로그인 모드인지, 로그인 선택 버튼 띄우기
                        break;
                    }
                    yield return null;
                }
            }

        }

        void Awake()
        {
            Initialize();
            StartCoroutine(Login());
        }

        void Update()
        {
        }
    }
}
