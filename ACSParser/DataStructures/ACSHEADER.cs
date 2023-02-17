namespace ACSParser.DataStructures;

public class ACSHEADER
{
    public ULONG Signature; // 4 bytes
    public ACSLOCATOR CharacterInfo; // 8 bytes
    public ACSLOCATOR AnimationInfoList; // 8 bytes
    public ACSLOCATOR ImageInfoList; // 8 bytes
    public ACSLOCATOR AudioInfoList; // 8 bytes

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
