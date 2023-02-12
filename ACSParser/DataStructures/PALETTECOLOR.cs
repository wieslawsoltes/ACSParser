using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PALETTECOLOR
{
    public RGBQUAD Color;

    public static PALETTECOLOR Parse(BinaryReader reader)
    {
        var paletteColor = new PALETTECOLOR();
        paletteColor.Color = RGBQUAD.Parse(reader);
        return paletteColor;
    }
}
