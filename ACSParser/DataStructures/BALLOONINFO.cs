namespace ACSParser.DataStructures;

public struct BALLOONINFO
{
    public BYTE NumTextLines; // 1 byte
    public BYTE CharsPerLine; // 1 byte
    public RGBQUAD ForegroundColor; // 4 bytes
    public RGBQUAD BackgroundColor; // 4 bytes
    public RGBQUAD BorderColor; // 4 bytes
    public STRING FontName;
    public LONG FontHeight; // 4 bytes
    public LONG FontWeight; // 4 bytes
    public BOOL Italicized; // 1 byte
    public BYTE Unknown; // 1 byte

    public static BALLOONINFO Parse(BinaryReader reader)
    {
        BALLOONINFO info = new BALLOONINFO();

        info.NumTextLines = reader.BYTE();
        info.CharsPerLine = reader.BYTE();
        info.ForegroundColor = RGBQUAD.Parse(reader);
        info.BackgroundColor = RGBQUAD.Parse(reader);
        info.BorderColor = RGBQUAD.Parse(reader);
        info.FontName = STRING.Parse(reader);
        // TODO: FontHeight=-13
        info.FontHeight = reader.LONG();
        info.FontWeight = reader.LONG();
        info.Italicized = reader.BOOL();
        info.Unknown = reader.BYTE();

        return info;
    }
}
