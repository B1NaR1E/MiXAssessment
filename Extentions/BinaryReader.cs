using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiXAssessment.Extentions
{
    public static class BinaryReader
    {
        public static string ReadAscii(this System.IO.BinaryReader input)
        {
            List<byte> strBytes = new List<byte>();
            int b;
            while ((b = input.ReadByte()) != 0x00)
                strBytes.Add((byte)b);
            return Encoding.ASCII.GetString(strBytes.ToArray());
        }
    }
}
