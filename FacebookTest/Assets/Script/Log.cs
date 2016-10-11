using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Homura
{
    public class Log
    {
        public enum WARNING_LEVEL
        {
            WL_NONE = -1,
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
            LST_CONSOLE_AND_TEXT = LST_CONSOLE | LST_TEXT,
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
        string mDefaultSavePath = Application.dataPath + "/Log";
        string mNowSavePath;
        float mElapsedTime;
        float mSaveInterval;
        StreamWriter mFileWriter;

        public static Log Instance
        {
            get
            {
                return mInstance;
            }
        }

        // Interval을 0으로 할 시, 프레임단위로 진행 프레임 단위 이외를 원한다면 프레임 단위보다 큰값을 넣을 것
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

            else
            {
                mSaveInterval = mElapsedTime = _mSaveInterval;
            }

            if(null != mFileWriter)
            {
                mFileWriter.Close();
            }
            mFileWriter = null;

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Logged(WARNING_LEVEL _Level, string _Text)
        {
            if( WARNING_LEVEL.WL_1 > _Level)
            {
                return ERROR_CODE.HEC_NOT_VALID_WANING_LEVEL;
            }

            if(null == _Text)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            LOG_NODE NewLog = new LOG_NODE();
            NewLog.mLevel = _Level;
            NewLog.mLog = _Text;

            mLogList.Add(NewLog);

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE SetSaveType(WARNING_LEVEL _Level, LOG_SAVE_TYPE _SaveType)
        {
            if(WARNING_LEVEL.WL_NONE >= _Level || WARNING_LEVEL.WL_COUNT <= _Level)
            {
                return ERROR_CODE.HEC_NOT_VALID_KEY;
            }

            mLogSaveType[(int)_Level] = _SaveType;

            return ERROR_CODE.HEC_COMPLETE;
        }

        void PathChechAndCreate()
        {
            System.DateTime Time = System.DateTime.Now;
            mNowSavePath = mDefaultSavePath + "/" + Time.Year;
            DirectoryInfo DI = new System.IO.DirectoryInfo(mNowSavePath);
            if (!DI.Exists)
            {
                DI.Create();
            }

            mNowSavePath = mNowSavePath + "/" + Time.Month;
            DI = new System.IO.DirectoryInfo(mNowSavePath);
            if (!DI.Exists)
            {
                DI.Create();
            }

            mNowSavePath = mNowSavePath + "/" + Time.Day;
            DI = new System.IO.DirectoryInfo(mNowSavePath);
            if(!DI.Exists)
            {
                DI.Create();
            }

            mNowSavePath = mNowSavePath + "/" + Time.Hour + Time.Minute + ".txt";
            System.IO.FileInfo FI = new System.IO.FileInfo(mNowSavePath);
            if(!FI.Exists)
            {
                FI.Create();
                if (null != mFileWriter)
                {
                    mFileWriter.Close();
                }
            }
            mFileWriter = new StreamWriter(mNowSavePath);
            mFileWriter.AutoFlush = false;
        }

        void LogText(string _Text)
        {
            mFileWriter.WriteLine(_Text);
        }

        public void Update()
        {
            mElapsedTime -= Time.deltaTime;
            if(0f >= mElapsedTime)
            {
                //PathChechAndCreate();
                List<LOG_NODE>.Enumerator Enumer = mLogList.GetEnumerator();
                while(Enumer.MoveNext())
                {
                    if( 0 != (LOG_SAVE_TYPE.LST_CONSOLE & mLogSaveType[(int)Enumer.Current.mLevel]) )
                    {
                        Debug.Log(Enumer.Current.mLevel + " : " + Enumer.Current.mLog);
                    }

                    if (0 != (LOG_SAVE_TYPE.LST_TEXT & mLogSaveType[(int)Enumer.Current.mLevel]))
                    {
                        LogText(Enumer.Current.mLevel + " : " + Enumer.Current.mLog);
                    }
                }
                if(null != mFileWriter)
                {
                    mFileWriter.Flush();
                    mFileWriter.Close();
                }
                mElapsedTime = mSaveInterval;
                mLogList.Clear();
            }
        }
    }
}
