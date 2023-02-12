using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct LANGID
{
    public ushort LanguageID;

    public static LANGID Parse(BinaryReader reader)
    {
        LANGID langid = new LANGID();
        langid.LanguageID = reader.ReadUInt16();
        return langid;
    }
}
