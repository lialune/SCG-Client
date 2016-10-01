using UnityEngine;
using System.Collections;

namespace Homura
{
    public class Server : MonoBehaviour
    {
        string IP;

        ERROR_CODE Initialize()
        {
            ERROR_CODE ErrorCode = Homura.RequestManager.Instance.Initialize();
            if(ERROR_CODE.HEC_COMPLETE != ErrorCode)
            {
                return ErrorCode;
            }

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