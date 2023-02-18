namespace ACSParser.DataStructures;

public class ACSIMAGEINFO // 12 bytes
{
    public ACSLOCATOR InfoLocation; // 8 bytes
    public ULONG Checksum; // 4 bytes

    // TODO: Parse not tested.
    public static ACSIMAGEINFO Parse(BinaryReader reader)
    {
        ACSIMAGEINFO imageInfo = new ACSIMAGEINFO();

        imageInfo.InfoLocation = ACSLOCATOR.Parse(reader);
        imageInfo.Checksum = reader.ULONG();

        return imageInfo;
    }

    public void Write(BinaryWriter writer)
    {
        InfoLocation.Write(writer);
        writer.Write(Checksum);
    }
}
