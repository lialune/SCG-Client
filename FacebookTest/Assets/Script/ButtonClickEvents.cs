using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
    class ButtonClickEvents :MonoBehaviour
    {
        public void FaceBookLogin()
        {
            Homura.FacebookModule Module = GameObject.FindGameObjectWithTag("LoginModule").GetComponent<Homura.FacebookModule>();
            if(null != Module)
            {
                Module.Initialize();
                Module.Login();
            }
        }

        public void  GooglePlusLogin()
        {

        }
    }
}
