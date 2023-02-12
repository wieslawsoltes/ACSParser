using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RGBQUAD
{
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Reserved;

    public static RGBQUAD Parse(BinaryReader reader)
    {
        RGBQUAD rgbquad = new RGBQUAD();

        rgbquad.Red = reader.ReadByte();
        rgbquad.Green = reader.ReadByte();
        rgbquad.Blue = reader.ReadByte();
        rgbquad.Reserved = reader.ReadByte();

        return rgbquad;
    }
}
