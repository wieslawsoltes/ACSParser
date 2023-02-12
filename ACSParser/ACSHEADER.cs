using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSHEADER
{
    public uint Signature;
    public ACSLOCATOR CharacterInfo;
    public ACSLOCATOR AnimationInfoList;
    public ACSLOCATOR ImageInfoList;
    public ACSLOCATOR AudioInfoList;

    public static ACSHEADER Parse(BinaryReader reader)
    {
        var header = new ACSHEADER();
        header.Signature = reader.ReadUInt32();
        header.CharacterInfo = ACSLOCATOR.Parse(reader);
        header.AnimationInfoList = ACSLOCATOR.Parse(reader);
        header.ImageInfoList = ACSLOCATOR.Parse(reader);
        header.AudioInfoList = ACSLOCATOR.Parse(reader);

        if (header.Signature != 0xABCDABC3)
        {
            throw new Exception("Invalid ACS signature");
        }

        return header;
    }
}