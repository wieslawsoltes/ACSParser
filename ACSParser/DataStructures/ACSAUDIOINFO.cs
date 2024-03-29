namespace ACSParser.DataStructures;

public class ACSAUDIOINFO
{
    public ACSLOCATOR AudioData; // 8 bytes
    public ULONG Checksum; // 4 bytes

    // TODO: Parse not tested.
    public static ACSAUDIOINFO Parse(BinaryReader reader)
    {
        ACSAUDIOINFO audioInfo = new ACSAUDIOINFO();

        audioInfo.AudioData = ACSLOCATOR.Parse(reader);
        audioInfo.Checksum = reader.ULONG();

        return audioInfo;
    }

    public void Write(BinaryWriter writer)
    {
        AudioData.Write(writer);
        writer.Write(Checksum);
    }
}
