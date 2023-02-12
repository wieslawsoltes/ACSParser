using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class BALLOONINFO
{
    public byte NumTextLines;
    public byte CharsPerLine;
    public RGBQUAD ForegroundColor;
    public RGBQUAD BackgroundColor;
    public RGBQUAD BorderColor;
    public string FontName;
    public int FontHeight;
    public int FontWeight;
    public bool Italicized;
    public byte Unknown;

    public static BALLOONINFO Parse(BinaryReader reader)
    {
        BALLOONINFO info = new BALLOONINFO();

        info.NumTextLines = reader.ReadByte();
        info.CharsPerLine = reader.ReadByte();
        info.ForegroundColor = RGBQUAD.Parse(reader);
        info.BackgroundColor = RGBQUAD.Parse(reader);
        info.BorderColor = RGBQUAD.Parse(reader);
        info.FontName = reader.ReadNullTerminatedString(32);
        info.FontHeight = reader.ReadInt32();
        info.FontWeight = reader.ReadInt32();
        info.Italicized = reader.ReadBoolean();
        info.Unknown = reader.ReadByte();

        return info;
    }
}
