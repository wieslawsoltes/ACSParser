namespace ACSParser;

public struct LOCALIZEDINFO
{
    public LANGID LanguageId;
    public STRING CharacterName;
    public STRING CharacterDescription;
    public STRING CharacterExtraData;

    public static LOCALIZEDINFO Parse(BinaryReader reader)
    {
        LOCALIZEDINFO localizedInfo = new LOCALIZEDINFO();

        localizedInfo.LanguageId = LANGID.Parse(reader);
        localizedInfo.CharacterName = STRING.Parse(reader);
        localizedInfo.CharacterDescription = STRING.Parse(reader);
        localizedInfo.CharacterExtraData = STRING.Parse(reader);

        return localizedInfo;
    }
    
    public static LOCALIZEDINFO[] ParseList(BinaryReader reader)
    {
        USHORT localizedInfoCount = reader.USHORT();

        LOCALIZEDINFO[] localizedInfo = new LOCALIZEDINFO[localizedInfoCount];
        for (var i = 0; i < localizedInfoCount; i++)
        {
            localizedInfo[i] = LOCALIZEDINFO.Parse(reader);
        }

        return localizedInfo;
    }
}
