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
}

public class AUDIO
{
    // TODO: Parse not tested.
    public static AUDIO Parse(BinaryReader reader)
    {
        AUDIO audio = new AUDIO();

        // TODO: RIFF Audio

        return audio;
    }
}
