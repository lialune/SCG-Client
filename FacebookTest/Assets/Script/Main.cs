using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.j
using Facebook.Unity;

public class Main : MonoBehaviour
{
    public string IP;

    void Start()
    {
        //Socket Sock = null;
        //IPEndPoint EndPoint;
        //byte[] buf = new byte[1024];

        //Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //IPAddress addr = IPAddress.Parse("127.0.0.1");
        //EndPoint = new IPEndPoint(addr, 8080);

        //Sock.Connect(EndPoint);

        //Debug.Log("Connect");

        //Homura.Packet pac = new Homura.Packet();
        //pac.initialize(Homura.PACKET_HEADER.PH_C_TO_S_CONNECT_CHECK_REQ, 8);

        //Sock.Send(pac.getBuffer());

        //Sock.Close();

        // 웹 통신
        string uri = "http://" + IP + ":8080/SCG/now.jsp";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(uri);
        Request.ContentType = "application/x-www-form-urlencoded; charset=euc-kr;";
        Request.ContentLength = uri.Length;

        WebResponse Respone = Request.GetResponse();
        Stream DataStread = Respone.GetResponseStream();
        StreamReader Reader = new StreamReader(DataStread, System.Text.Encoding.Default);
        string Data = Reader.ReadToEnd();

        Reader.Close();
        Respone.Close();
    }

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public void ButtonEventLogin()
    {
        List<string> Perms = new List<string>() { "public_profile", "email" };

        FB.LogInWithReadPermissions(Perms, AuthCallback);
    }

    void InitCallBack()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
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

    void AuthCallback(ILoginResult _Result)
    {
        if (FB.IsLoggedIn)
        {
            Facebook.Unity.AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            Debug.Log(aToken.UserId);

            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
