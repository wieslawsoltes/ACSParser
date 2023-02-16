namespace ACSParser.DataStructures;

public struct ACSFRAMEIMAGE
{
    public ULONG ImageIndex; // 4 bytes
    public SHORT XOffset; // 2 bytes
    public SHORT YOffset; // 2 bytes

    // TODO: Parse not tested.
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
