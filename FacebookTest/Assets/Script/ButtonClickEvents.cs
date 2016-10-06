using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
    class ButtonClickEvents :MonoBehaviour
    {
        public void FaceBookLogin()
        {
            Server Module = GameObject.FindGameObjectWithTag("Server").GetComponent<Homura.Server>();
            if(null != Module)
            {
                Module.Login(LOGIN_MODULE_TYPE.LMT_FACEBOOK);
            }
        }

        public void  GooglePlusLogin()
        {

        }
    }
}
