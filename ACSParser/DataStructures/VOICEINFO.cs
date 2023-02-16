namespace ACSParser.DataStructures;

public struct VOICEINFO
{
    public GUID TTS_EngineID;
    public GUID TTS_ModeID;
    public ULONG Speed;
    public USHORT Pitch;
    public BOOL ExtraDataPresent;
    public LANGID LanguageID;
    public STRING LanguageDialect;
    public USHORT Gender;
    public USHORT Age;
    public STRING Style;

    public static VOICEINFO Parse(BinaryReader reader)
    {
        var info = new VOICEINFO();

        info.TTS_EngineID = GUID.Parse(reader);
        info.TTS_ModeID = GUID.Parse(reader);
        info.Speed = reader.ULONG();
        info.Pitch = reader.USHORT();
        info.ExtraDataPresent = reader.BOOL();

        if (info.ExtraDataPresent != 0x00)
        {
            info.LanguageID = LANGID.Parse(reader);
            info.LanguageDialect = STRING.Parse(reader);
            info.Gender = reader.USHORT();
            info.Age = reader.USHORT();
            info.Style = STRING.Parse(reader);
        }

        return info;
    }
}
