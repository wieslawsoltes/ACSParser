namespace ACSParser.DataStructures;

public struct ACSAUDIOINFO
{
    public ACSLOCATOR AudioData;
    public ULONG Checksum;

    public static ACSAUDIOINFO Parse(BinaryReader reader)
    {
        ACSAUDIOINFO audioInfo = new ACSAUDIOINFO();

        audioInfo.AudioData = ACSLOCATOR.Parse(reader);
        audioInfo.Checksum = reader.ULONG();

        return audioInfo;
    }
}
