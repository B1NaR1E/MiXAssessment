using System.Text;

namespace MiXAssessment.Extentions;

internal static class BinaryReader
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

