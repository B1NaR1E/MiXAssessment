using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiXAssessment
{
    internal class BufferedBinaryReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly byte[] _buffer;
        private readonly int _bufferSize;
        private int _bufferOffset;
        private int _numBufferedBytes;

        public BufferedBinaryReader(Stream stream, int bufferSize)
        {
            _stream = stream;
            _bufferSize = bufferSize;
            _buffer = new byte[bufferSize];
            _bufferOffset = bufferSize;
        }

        public async Task<bool> FillBufferAsync()
        {
            var numBytesUnread = _bufferSize - _bufferOffset;
            var numBytesToRead = _bufferSize - numBytesUnread;

            _bufferOffset = 0;

            _numBufferedBytes = numBytesUnread;

            if (numBytesUnread > 0)
            {
                Buffer.BlockCopy(_buffer, numBytesToRead, _buffer, 0, numBytesToRead);
            }

            while (numBytesToRead > 0)
            {
                var numBytesRead = await _stream.ReadAsync(_buffer, numBytesUnread, numBytesToRead);

                if (numBytesRead == 0)
                {
                    return false;
                }
                _numBufferedBytes += numBytesRead;
                numBytesToRead -= numBytesRead;
                numBytesUnread += numBytesRead;
            }
            return true;
        }

        //public bool CanRead { get { return _stream.P} }

        //public ushort ReadUInt16() 
        //{
        //    var tempBuffer = new byte[2];

        //    Buffer.BlockCopy(_buffer, _bufferOffset, tempBuffer, 0, tempBuffer.Length);

        //    var valll = BitConverter.ToUInt16(tempBuffer, 0);

        //    var val = _buffer[_bufferOffset] | _buffer[_bufferOffset + 1] << 8;

        //    _bufferOffset += tempBuffer.Length;
        //    return val;
        //}

        public int ReadInt32()
        {
            var tempBuffer = new byte[4];

            Buffer.BlockCopy(_buffer, _bufferOffset, tempBuffer, 0, tempBuffer.Length);

            var val = BitConverter.ToInt32(tempBuffer, 0);

            _bufferOffset += tempBuffer.Length;

            return val;
        }

        public string ReadAscii()
        {
            //List<byte> tempBuffer = new List<byte>();

            //int b;
            //int index = _bufferOffset;
            //while ((b = (int)_buffer[index]) != 0x00)
            //{
            //    tempBuffer.Add((byte)b);
            //    ++index;
            //}
            //_bufferOffset += tempBuffer.Count + 1;
            var tempBuffer = new byte[11];

            Buffer.BlockCopy(_buffer, _bufferOffset, tempBuffer, 0, tempBuffer.Length);
            _bufferOffset += tempBuffer.Length + 1;
            return Encoding.ASCII.GetString(tempBuffer.ToArray());
        }

        public float ReadSingle()
        {
            var tempBuffer = new byte[4];

            Buffer.BlockCopy(_buffer, _bufferOffset, tempBuffer, 0, tempBuffer.Length);

            var val = BitConverter.ToSingle(tempBuffer, 0);

            //var val = (float)(_buffer[_bufferOffset] | _buffer[_bufferOffset + 1] | _buffer[_bufferOffset + 2] | _buffer[_bufferOffset + 3] << 8);
            _bufferOffset += tempBuffer.Length;
            return val;
        }

        public ulong ReadUInt64()
        {
            var tempBuffer = new byte[8];

            Buffer.BlockCopy(_buffer, _bufferOffset, tempBuffer, 0, tempBuffer.Length);

            var val = BitConverter.ToUInt64(tempBuffer, 0);

            _bufferOffset += tempBuffer.Length;

            return val;
        }

        public void Dispose()
        {
            _stream.Close();
            //throw new NotImplementedException();
        }
    }
}
