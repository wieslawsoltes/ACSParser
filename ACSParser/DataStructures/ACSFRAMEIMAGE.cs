namespace ACSParser;

public struct ACSFRAMEIMAGE
{
    public ULONG ImageIndex;
    public SHORT XOffset;
    public SHORT YOffset;

    public static ACSFRAMEIMAGE Parse(BinaryReader reader)
    {
        return new ACSFRAMEIMAGE
        {
            ImageIndex = reader.ULONG(),
            XOffset = reader.SHORT(),
            YOffset = reader.SHORT()
        };
    }
}
