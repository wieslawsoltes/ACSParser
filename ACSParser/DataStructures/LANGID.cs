namespace ACSParser.DataStructures;

public struct LANGID
{
    public USHORT LanguageID;

    public static LANGID Parse(BinaryReader reader)
    {
        LANGID langid = new LANGID();

        langid.LanguageID = reader.USHORT();

        return langid;
    }
}
