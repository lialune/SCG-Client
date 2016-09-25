using UnityEngine;
using System.Collections;

namespace Homura
{
    public class Log
    {
        static Log mInstance = new Log();

        public Log Instanec
        {
            get
            {
                return mInstance;
            }
        }

        public ERROR_CODE Initialize()
        {

            return ERROR_CODE.HEC_COMPLETE;
        }

        public void Update()
        {

        }
    }
}
