namespace ACSParser.DataStructures;

public struct RGBQUAD
{
    public BYTE Red;
    public BYTE Green;
    public BYTE Blue;
    public BYTE Reserved;

    public static RGBQUAD Parse(BinaryReader reader)
    {
        RGBQUAD rgbquad = new RGBQUAD();

        rgbquad.Red = reader.BYTE();
        rgbquad.Green = reader.BYTE();
        rgbquad.Blue = reader.BYTE();
        rgbquad.Reserved = reader.BYTE();

        return rgbquad;
    }
}
