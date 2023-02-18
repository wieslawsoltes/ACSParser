namespace ACSParser.DataStructures;

public class VOICEINFO
{
    public GUID TTS_EngineID; // 16 bytes
    public GUID TTS_ModeID; // 16 bytes
    public ULONG Speed; // 4 bytes
    public USHORT Pitch; // 2 bytes
    public BOOL ExtraDataPresent; // 1 byte
    public LANGID LanguageID; // 2 bytes
    public STRING LanguageDialect;
    public USHORT Gender; // 2 bytes
    public USHORT Age; // 2 bytes
    public STRING Style;

    // TODO: Parse not tested.
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

    public void Write(BinaryWriter writer)
    {
        TTS_EngineID.Write(writer);
        TTS_ModeID.Write(writer);
        writer.Write(Speed);
        writer.Write(Pitch);
        writer.Write(ExtraDataPresent);

        if (ExtraDataPresent != 0x00)
        {
            LanguageID.Write(writer);
            LanguageDialect.Write(writer);
            writer.Write(Gender);
            writer.Write(Age);
            Style.Write(writer);
        }
    }
}
