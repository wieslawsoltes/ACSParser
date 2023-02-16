namespace ACSParser.DataStructures;

public struct RGBQUAD
{
    public BYTE Red; // 1 byte
    public BYTE Green; // 1 byte
    public BYTE Blue; // 1 byte
    public BYTE Reserved; // 1 byte

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
