using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LOCALIZEDINFO
{
    public ushort LanguageId;
    public string CharacterName;
    public string CharacterDescription;
    public string CharacterExtraData;

    public static LOCALIZEDINFO Parse(BinaryReader reader)
    {
        LOCALIZEDINFO localizedInfo = new LOCALIZEDINFO();

        localizedInfo.LanguageId = reader.ReadUInt16();
        localizedInfo.CharacterName = reader.ReadStringNullTerminated();
        localizedInfo.CharacterDescription = reader.ReadStringNullTerminated();
        localizedInfo.CharacterExtraData = reader.ReadStringNullTerminated();

        return localizedInfo;
    }
}
