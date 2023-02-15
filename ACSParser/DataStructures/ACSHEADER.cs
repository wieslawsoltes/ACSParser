namespace ACSParser;

public struct ACSHEADER
{
    public ULONG Signature;
    public ACSLOCATOR CharacterInfo;
    public ACSLOCATOR AnimationInfoList;
    public ACSLOCATOR ImageInfoList;
    public ACSLOCATOR AudioInfoList;

    public static ACSHEADER Parse(BinaryReader reader)
    {
        var header = new ACSHEADER();

        header.Signature = reader.ULONG();
        if (header.Signature != 0xABCDABC3)
        {
            throw new Exception("Invalid ACS signature");
        }

        header.CharacterInfo = ACSLOCATOR.Parse(reader);
        header.AnimationInfoList = ACSLOCATOR.Parse(reader);
        header.ImageInfoList = ACSLOCATOR.Parse(reader);
        header.AudioInfoList = ACSLOCATOR.Parse(reader);

        return header;
    }
}
