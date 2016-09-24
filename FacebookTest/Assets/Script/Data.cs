using System;
using System.Collections.Generic;
using System.Text;

namespace Homura
{
    public class Data
    {
        public const int DATA_DEFAULT_SIZE = 256;
        public const int DATA_MIN_SIZE = 1;

        char[] mBuffer;
        int mUsingSize;
        int mBufferSize;
        int mReadPos;

        public Data()
        {
        }

        public ERROR_CODE Initialize(int _BufferSize)
        {
            int BufferSize = _BufferSize;
            if (DATA_MIN_SIZE > _BufferSize)
            {
                return ERROR_CODE.HEC_BELOW_ZERO_TO_SIZE;
            }

            BufferSize = OptimazationBufferSize(_BufferSize);
            
            mBuffer = new char[BufferSize];
            if (null == mBuffer)
            {
                return ERROR_CODE.HEC_FAIL_CREATE_BUFFER;
            }

            mReadPos = 0;
            mUsingSize = 0;
            mBufferSize = _BufferSize;

            mBuffer.Initialize();

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Initialize(char[] _Buffer, int _BufferSize)
        {
            if (null == _Buffer)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            if (_Buffer.Length > _BufferSize)
            {
                mBufferSize = OptimazationBufferSize(_Buffer.Length);
            }
            else
            {
                mBufferSize = OptimazationBufferSize(_BufferSize);
            }

            mBuffer = new char[mBufferSize];
            if (null == mBuffer)
            {
                return ERROR_CODE.HEC_FAIL_CREATE_BUFFER;
            }
            mBuffer.Initialize();

            Buffer.BlockCopy(_Buffer, 0, mBuffer, 0, mBuffer.Length);

            mUsingSize = _Buffer.Length;
            mReadPos = 0;

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Append(char[] _Data, int _Size)
        {
            if (null == _Data)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            if (1 > _Size)
            {
                return ERROR_CODE.HEC_BELOW_ZERO_TO_SIZE;
            }

            if (mBufferSize - mUsingSize < _Size)
            {
                ERROR_CODE ErrorCode = BufferSizeUp();
                if (ERROR_CODE.HEC_COMPLETE != ErrorCode)
                {
                    return ErrorCode;
                }
            }

            Buffer.BlockCopy(_Data, 0, mBuffer, mUsingSize, _Size);
            mUsingSize += _Size;

            return ERROR_CODE.HEC_COMPLETE;
        }

        public ERROR_CODE Pull(char[] _Data, int _Size)
        {
            if (null == _Data)
            {
                return ERROR_CODE.HEC_NULL_DATA;
            }

            if (mUsingSize - mReadPos < _Size)
            {
                return ERROR_CODE.HEC_BUFFER_OVER;
            }

            Buffer.BlockCopy(mBuffer, mReadPos, _Data, 0, _Size);

            mReadPos += _Size;

            return ERROR_CODE.HEC_COMPLETE;
        }

        public char[] GetBuffer()
        {
            return mBuffer;
        }

        public bool IsValid()
        {
            if(mUsingSize > mReadPos)
            {
                return true;
            }
            return false;
        }

        public void ResetReadPos()
        {
            mReadPos = 0;
        }

        ERROR_CODE BufferSizeUp()
        {
            char[] NewBuffer = new char[mBufferSize * 2];

            if (null == NewBuffer)
            {
                return ERROR_CODE.HEC_FAIL_CREATE_BUFFER;
            }

            Buffer.BlockCopy(mBuffer, 0, NewBuffer, 0, NewBuffer.Length);
            mBuffer = NewBuffer;
            mBufferSize <<= 1;

            return ERROR_CODE.HEC_COMPLETE;
        }

        int OptimazationBufferSize(int _Size)
        {
            int NewSize = 2;

            while(_Size > NewSize)
            {
               NewSize <<= 1;
            }

            return NewSize;
        }
    }
}