using System;
using System.Collections.Generic;
using System.Text;

namespace Homura
{
    // S : Server
    // C : Client
    public enum PACKET_HEADER
    {
        PH_C_TO_S_CONNECT_CHECK_REQ,
        PH_S_TO_C_CONNECT_CHECK_ANS,
        PH_ERROR_CHECK
    }

    public enum PACKET_ERROR_CODE
    {
        PEC_COMPLITE,
        PEC_NOT_DEFINE_PACKET_HEADER,
        PEC_FAIL_CREATE_BUFFER,
        PEC_NOT_DATA,
        PEC_ZERO_OR_MINUS_SIZE,
        PEC_OVER_PULL
    }

    // パケットの構造はHeader+Size+Dataです。
    class Packet
    {
        public const int PACKET_DEFAULT_SIZE = 1024;
        public const int PACKET_MIN_SIZE = sizeof(PACKET_HEADER) + sizeof(int);

        byte[] mBuffer;
        PACKET_HEADER mHeader;
        int mUsingSize;
        int mBufferSize;
        int mReadPos;

        public Packet()
        {
        }

        public PACKET_ERROR_CODE initialize(PACKET_HEADER _PacketHeader, int _BufferSize)
        {
            int BufferSize = _BufferSize;
            if (PACKET_MIN_SIZE > _BufferSize)
            {
                BufferSize = PACKET_MIN_SIZE;
            }

            mBuffer = new byte[BufferSize];
            if (null == mBuffer)
            {
                return PACKET_ERROR_CODE.PEC_FAIL_CREATE_BUFFER;
            }
            mReadPos = 0;
            mUsingSize = _BufferSize;
            mBufferSize = _BufferSize;

            if (0 > mHeader && PACKET_HEADER.PH_ERROR_CHECK <= mHeader)
            {
                return PACKET_ERROR_CODE.PEC_NOT_DEFINE_PACKET_HEADER;
            }
            mHeader = _PacketHeader;
            // 임시
            Buffer.BlockCopy(BitConverter.GetBytes((int)mHeader), 0, mBuffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(mUsingSize), 0, mBuffer, 4, 4);

            return PACKET_ERROR_CODE.PEC_NOT_DEFINE_PACKET_HEADER;
        }

        public PACKET_ERROR_CODE initialize(byte[] _Buffer, int _BufferSize)
        {
            if (null == _Buffer)
            {
                return PACKET_ERROR_CODE.PEC_NOT_DATA;
            }

            if (PACKET_MIN_SIZE > _BufferSize)
            {
                return PACKET_ERROR_CODE.PEC_ZERO_OR_MINUS_SIZE;
            }

            mBuffer = _Buffer;
            mBufferSize = _BufferSize;
            mUsingSize = _BufferSize;
            mReadPos = 8;

            setPacketHeader((PACKET_HEADER)BitConverter.ToInt32(mBuffer, 0));

            return PACKET_ERROR_CODE.PEC_COMPLITE;
        }

        public PACKET_ERROR_CODE setPacketHeader(PACKET_HEADER _PacketHeader)
        {
            if (0 > _PacketHeader && PACKET_HEADER.PH_ERROR_CHECK <= _PacketHeader)
            {
                return PACKET_ERROR_CODE.PEC_NOT_DEFINE_PACKET_HEADER;
            }

            mHeader = _PacketHeader;

            return PACKET_ERROR_CODE.PEC_FAIL_CREATE_BUFFER;
        }

        public PACKET_HEADER getPacketHeader()
        {
            return mHeader;
        }

        public PACKET_ERROR_CODE append(byte[] _Data, int _Size)
        {
            if (null == _Data)
            {
                return PACKET_ERROR_CODE.PEC_NOT_DATA;
            }

            if (0 >= _Size)
            {
                return PACKET_ERROR_CODE.PEC_ZERO_OR_MINUS_SIZE;
            }

            if (mBufferSize - mUsingSize < _Size)
            {
                PACKET_ERROR_CODE ErrorCode = BufferSizeUp();
                if (PACKET_ERROR_CODE.PEC_COMPLITE != ErrorCode)
                {
                    return ErrorCode;
                }
            }

            Buffer.BlockCopy(_Data, 0, mBuffer, mUsingSize, _Size);
            mUsingSize += _Size;

            return PACKET_ERROR_CODE.PEC_COMPLITE;
        }

        public PACKET_ERROR_CODE pull(byte[] _Data, int _Size)
        {
            if (mUsingSize - mReadPos < _Size)
            {
                return PACKET_ERROR_CODE.PEC_OVER_PULL;
            }

            Buffer.BlockCopy(mBuffer, mReadPos, _Data, 0, _Size);

            mReadPos += _Size;

            return PACKET_ERROR_CODE.PEC_COMPLITE;
        }

        public byte[] getBuffer()
        {
            return mBuffer;
        }

        PACKET_ERROR_CODE BufferSizeUp()
        {
            byte[] NewBuffer = new byte[mBufferSize * 2];

            if (null == NewBuffer)
            {
                return PACKET_ERROR_CODE.PEC_FAIL_CREATE_BUFFER;
            }

            Buffer.BlockCopy(mBuffer, 0, NewBuffer, 0, NewBuffer.Length);
            mBuffer = NewBuffer;
            mBufferSize *= 2;

            return PACKET_ERROR_CODE.PEC_COMPLITE;
        }
    }
}
