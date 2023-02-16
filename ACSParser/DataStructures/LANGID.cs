namespace ACSParser.DataStructures;

public struct LANGID
{
    public USHORT LanguageID; // 2 bytes

    // TODO: Parse not tested.
    public static LANGID Parse(BinaryReader reader)
    {
        LANGID langid = new LANGID();

        langid.LanguageID = reader.USHORT();

        return langid;
    }
}
