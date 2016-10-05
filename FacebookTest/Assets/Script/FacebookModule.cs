using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using JsonFx.Json;

namespace Homura
{
    using PARAM_LIST = Dictionary<string, string>;

    public class FacebookModule
    {
        PARAM_LIST mParamList;

        public void Initialize()
        {
            if (!FB.IsInitialized)
            {
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
                mParamList.Clear();
            }
        }

        public void Login()
        {
            List<string> Perms = new List<string>() { "public_profile", "email" };

            FB.LogInWithReadPermissions(Perms, AuthCallback);
        }

        void AuthCallback(ILoginResult _Result)
        {
            if (FB.IsLoggedIn)
            {
                Facebook.Unity.AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

                mParamList = JsonReader.Deserialize<PARAM_LIST>(aToken.ToJson());
            }
            else
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_1, "User cancelled login");
            }
        }

        public PARAM_LIST GetParamList()
        {
            return mParamList;
        }

        public string GetParam(string _Key)
        {
            return mParamList[_Key];
        }

        void InitCallBack()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Log.Instanec.Logged(Log.WARNING_LEVEL.WL_2, "Failed to Initialize the Facebook SDK");
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