namespace ACSParser;

public struct ACSLOCATOR
{
    public ULONG Offset;
    public ULONG Size;

    public static ACSLOCATOR Parse(BinaryReader reader)
    {
        ACSLOCATOR locator = new ACSLOCATOR();

        locator.Offset = reader.ULONG();
        locator.Size = reader.ULONG();

        return locator;
    }
}
