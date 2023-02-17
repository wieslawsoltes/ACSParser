namespace ACSParser.DataStructures;

public class ACSLOCATOR
{
    public ULONG Offset; // 4 bytes
    public ULONG Size; // 4 bytes

    public static ACSLOCATOR Parse(BinaryReader reader)
    {
        ACSLOCATOR locator = new ACSLOCATOR();

        locator.Offset = reader.ULONG();
        locator.Size = reader.ULONG();

        return locator;
    }
}
