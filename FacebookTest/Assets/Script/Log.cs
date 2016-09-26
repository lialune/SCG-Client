using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
    public class Log
    {
        public enum WARNING_LEVEL
        {
            WL_1, // 프로그램 진행에 아무런 영향이없거나 미미한 경우
            WL_2, // 프로그램 진행에 영향을 가할 수 있는 경우
            WL_3, // 프로그램 진행에 중대한 영향을 가할 수 있는 경우
            WL_4, // 프로그램 진행에 심각한 영향을 가할 수 있는 경우
            WL_COUNT
        }

        public enum LOG_SAVE_TYPE
        {
            LST_NONE,
            LST_CONSOLE = 0x01,
            LST_TEXT = 0x02,
            LST_DB = 0x04, //DB에 저장은 나중에 추가 할 수도 있음
        }

        struct LOG_NODE
        {
            public WARNING_LEVEL mLevel;
            public string mLog;
        }

        static Log mInstance = new Log();

        LOG_SAVE_TYPE[] mLogSaveType = new LOG_SAVE_TYPE[(int)WARNING_LEVEL.WL_COUNT];
        List<LOG_NODE> mLogList = null;
        // 해당 경로는 유니티 프로젝트/Asset이지만 추후에 다른곳으로 변경할 필요가 있음
        string mDefaultSavePath = Application.dataPath;
        string mNowSavePath;
        float mElapsedTime;
        float mSaveInterval;

        public Log Instanec
        {
            get
            {
                return mInstance;
            }
        }

        // Interval을 0으로 할 시, 프레임단위로 진행
        public ERROR_CODE Initialize(float _mSaveInterval)
        {
            if(null == mLogList)
            {
                if( null == (mLogList = new List<LOG_NODE>()) )
                {
                    return ERROR_CODE.HEC_FAIL_NEW;
                }
            }
            else
            {
                mLogList.Clear();
            }

            for(int i = 0; (int)WARNING_LEVEL.WL_COUNT > i; ++i)
            {
                mLogSaveType[i] = LOG_SAVE_TYPE.LST_NONE;
            }
            
            if(0 > _mSaveInterval)
            {
                return ERROR_CODE.HEC_NOT_VALID_VALUE;
            }

            if(0 < _mSaveInterval)
            {
                _mSaveInterval = mElapsedTime = _mSaveInterval;
            }

            return ERROR_CODE.HEC_COMPLETE;
        }

        void PathChechAndCreate()
        {
            System.DateTime Time = System.DateTime.Now;
            mNowSavePath = mDefaultSavePath + Time.Year + "/" + Time.Month + "/" + Time.Day;

            System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(mNowSavePath);
            if(!DI.Exists)
            {
                DI.Create();
            }

            System.IO.FileInfo FI = new System.IO.FileInfo(mNowSavePath + "/" + Time.Hour + Time.Minute + ".txt");
            if(!FI.Exists)
            {
                FI.Create();
            }
        }

        void LogText(WARNING_LEVEL _Level, string _Text)
        {
            // 여기부터
        }

        void UpdateInterval()
        {
            mElapsedTime -= Time.deltaTime;
            if(0f >= mElapsedTime)
            {
                List<LOG_NODE>.Enumerator Enumer = mLogList.GetEnumerator();
                while(Enumer.MoveNext())
                {
                    if( 0 != (LOG_SAVE_TYPE.LST_CONSOLE & mLogSaveType[(int)Enumer.Current.mLevel]) )
                    {
                        Debug.Log(Enumer.Current.mLevel + " : " + Enumer.Current.mLog);
                    }

                    if (0 != (LOG_SAVE_TYPE.LST_TEXT & mLogSaveType[(int)Enumer.Current.mLevel]))
                    {
                        Debug.Log(Enumer.Current.mLevel + " : " + Enumer.Current.mLog);
                    }
                }
            }
        }

        void UpdateFrame()
        {

        }
    }
}
