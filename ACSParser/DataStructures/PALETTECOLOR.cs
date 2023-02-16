namespace ACSParser.DataStructures;

public struct PALETTECOLOR
{
    public RGBQUAD Color; // 4 bytes

    public static PALETTECOLOR Parse(BinaryReader reader)
    {
        PALETTECOLOR paletteColor = new PALETTECOLOR();

        paletteColor.Color = RGBQUAD.Parse(reader);

        return paletteColor;
    }

    public static PALETTECOLOR[] ParseList(BinaryReader reader)
    {
        ULONG palletColorCount = reader.ULONG();

        PALETTECOLOR[] palletColor = new PALETTECOLOR[palletColorCount];
        for (var i = 0; i < palletColorCount; i++)
        {
            palletColor[i] = PALETTECOLOR.Parse(reader);
        }

        return palletColor;
    }
}
