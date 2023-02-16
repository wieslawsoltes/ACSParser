namespace ACSParser.DataStructures;

public struct BALLOONINFO
{
    public BYTE NumTextLines;
    public BYTE CharsPerLine;
    public RGBQUAD ForegroundColor;
    public RGBQUAD BackgroundColor;
    public RGBQUAD BorderColor;
    public STRING FontName;
    public LONG FontHeight;
    public LONG FontWeight;
    public BOOL Italicized;
    public BYTE Unknown;

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
