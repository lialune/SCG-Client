using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

namespace Homura
{

    public enum LOGIN_STATE
    {
        LS_NONE,
        LS_MODE_SELECT,
        LS_NONE_LOGIN_MODE,
        LS_LOGIN_MODE_SELECT,
        LS_LOGIN_TYPE_SELECT,
        LS_LOGIN_FACEBOOK,
        LS_LOGIN_GOOGLE,
        LS_LOGIN_WAIT,
        LS_LOGIN_SUCCESS,
        LS_LOGIN_FAIL,
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
        PARAM_LIST mLoginConfigParamList;

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
            StreamReader sr = new StreamReader("Assets/Config/LoginSetting.txt");
            string Data = sr.ReadToEnd();
            mLoginConfigParamList = JsonReader.Deserialize<PARAM_LIST>(Data);
        }
        
        IEnumerator Login()
        {
            StreamReader sr = new StreamReader("Assets/Config/LoginSetting.txt");
            string Data = sr.ReadToEnd();
            PARAM_LIST ParamList = JsonReader.Deserialize<PARAM_LIST>(Data);

            while (true)
            {
                if (Input.GetMouseButtonDown((int)MOUSE_BUTTON.MB_LEFT))
                {
                    switch(ParamList.mParamList["LoginMode"])
                    {
                        // 로그인 모드에 대한 정보가 없음
                        case "None":
                            {
                                //비로그인 모드인지, 로그인 선택 버튼 띄우기
                                GameObject Button = GameObject.FindGameObjectWithTag("LoginModeSelectButton");
                                if (null == Button)
                                {
                                    Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "Not Find LoginModeSelectButton!!");
                                    yield break;
                                }
                                Button.SetActive(true);

                                // 처음의 Press Touch텍스트 비활성화
                                GameObject TextPanel = GameObject.FindGameObjectWithTag("PressTouchPanel");
                                if (null == TextPanel)
                                {
                                    Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "Not Find PressTouch Text Panel");
                                    yield break;
                                }
                                TextPanel.SetActive(false);

                                //로그인 상태를 Mode Select로 변경
                                mLoginState = LOGIN_STATE.LS_LOGIN_MODE_SELECT;
                            }
                            break;
                        case "Facebook":
                            {
                                Server sv = GameObject.FindGameObjectWithTag("Server").GetComponent<Server>();
                                if (null == sv)
                                {
                                    Log.Instance.Logged(Log.WARNING_LEVEL.WL_3, "LoginManager.Login() Not Find Server!!");
                                    yield break;
                                }
                                sv.Login(LOGIN_MODULE_TYPE.LMT_FACEBOOK);

                                // 로그인 요청에 대한 결과가 나올 때 까지 대기
                                while(true)
                                {
                                    if(LOGIN_STATE.LS_LOGIN_SUCCESS == sv.LoginState)
                                    {
                                        break;
                                    }
                                    else if(LOGIN_STATE.LS_LOGIN_FAIL == sv.LoginState)
                                    {
                                        // 처음의 Press Touch텍스트 활성화
                                        GameObject TextPanel = GameObject.FindGameObjectWithTag("PressTouchPanel");
                                        if (null == TextPanel)
                                        {
                                            Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "Not Find PressTouch Text Panel");
                                            yield break;
                                        }
                                        TextPanel.SetActive(true);

                                        ParamList.mParamList["LoginMode"] = "None";
                                        break;
                                    }
                                }

                            }break;
                        case "Google":
                            {
                            }break;
                    }
                }
                yield return null;
            }

        }

        ERROR_CODE LoginModeSelect()
        {
            switch (mLoginConfigParamList.mParamList["LoginMode"])
            {
                // 로그인 모드에 대한 정보가 없음
                case "None":
                    {
                        //비로그인 모드인지, 로그인 선택 버튼 띄우기
                        GameObject Button = GameObject.FindGameObjectWithTag("LoginModeSelectButton");
                        if (null == Button)
                        {
                            Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "Not Find LoginModeSelectButton!!");
                        }
                        Button.SetActive(true);

                        // 처음의 Press Touch텍스트 비활성화
                        GameObject TextPanel = GameObject.FindGameObjectWithTag("PressTouchPanel");
                        if (null == TextPanel)
                        {
                            Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "Not Find PressTouch Text Panel");
                        }
                        TextPanel.SetActive(false);

                        //로그인 상태를 Mode Select로 변경
                        mLoginState = LOGIN_STATE.LS_LOGIN_MODE_SELECT;
                    }
                    break;
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        void Awake()
        {
            Initialize();
            StartCoroutine(Login());
        }

        void Update()
        {
            switch (mLoginState)
            {
                case LOGIN_STATE.LS_NONE:
                    {
                        if (Input.GetMouseButtonDown((int)MOUSE_BUTTON.MB_LEFT))
                        {
                            LoginModeSelect();

                        }
                    }break;
            }
        }
    }
}
