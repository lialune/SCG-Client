using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using JsonFx.Json;

namespace Homura
{

    public class FacebookModule
    {
        PARAM_LIST mParamList;
        Server mSv;

        public void Initialize()
        {
            if (!FB.IsInitialized)
            {
                mSv = GameObject.FindGameObjectWithTag("Server").GetComponent<Server>();
                FB.Init(InitCallBack, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }

            if (null == mParamList)
            {
                mParamList = new PARAM_LIST();
            }
            else
            {
                mParamList.mParamList.Clear();
            }
        }

        public void Login()
        {
            List<string> Perms = new List<string>() { "public_profile", "email" };

            FB.LogInWithReadPermissions(Perms, AuthCallback);
            mSv.LoginState = LOGIN_STATE.LS_LOGIN_WAIT;
        }

        void AuthCallback(ILoginResult _Result)
        {
            if (FB.IsLoggedIn)
            {
                Facebook.Unity.AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

                mParamList = JsonReader.Deserialize<PARAM_LIST>(aToken.ToJson());

                mSv.LoginState = LOGIN_STATE.LS_LOGIN_SUCCESS;
            }
            else
            {
                Log.Instance.Logged(Log.WARNING_LEVEL.WL_1, "User cancelled login");
                mSv.LoginState = LOGIN_STATE.LS_LOGIN_FAIL;
            }
        }

        public PARAM_LIST GetParamList()
        {
            return mParamList;
        }

        public string GetParam(string _Key)
        {
            return mParamList.mParamList[_Key];
        }

        void InitCallBack()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Log.Instance.Logged(Log.WARNING_LEVEL.WL_2, "Failed to Initialize the Facebook SDK");
            }
        }

        void OnHideUnity(bool _IsGameShown)
        {
            if (!_IsGameShown)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}