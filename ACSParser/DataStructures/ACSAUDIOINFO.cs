using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSAUDIOINFO
{
    public ACSLOCATOR AudioData;
    public uint Checksum;

    public static ACSAUDIOINFO Parse(BinaryReader reader)
    {
        ACSAUDIOINFO audioInfo = new ACSAUDIOINFO();
        audioInfo.AudioData = ACSLOCATOR.Parse(reader);
        audioInfo.Checksum = reader.ReadUInt32();
        return audioInfo;
    }
}
