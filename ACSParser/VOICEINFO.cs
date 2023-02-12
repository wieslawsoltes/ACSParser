using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class VOICEINFO
{
    public Guid TTS_EngineID;
    public Guid TTS_ModeID;
    public uint Speed;
    public ushort Pitch;
    public bool ExtraDataPresent;

    // Extra data
    public ushort LanguageID;
    public string LanguageDialect;
    public ushort Gender;
    public ushort Age;
    public string Style;

    public static VOICEINFO Parse(BinaryReader reader)
    {
        var info = new VOICEINFO();

        info.TTS_EngineID = new Guid(reader.ReadBytes(16));
        info.TTS_ModeID = new Guid(reader.ReadBytes(16));
        info.Speed = reader.ReadUInt32();
        info.Pitch = reader.ReadUInt16();
        info.ExtraDataPresent = reader.ReadBoolean();

        if (info.ExtraDataPresent)
        {
            info.LanguageID = reader.ReadUInt16();
            info.LanguageDialect = reader.ReadStringNullTerminated();
            info.Gender = reader.ReadUInt16();
            info.Age = reader.ReadUInt16();
            info.Style = reader.ReadStringNullTerminated();
        }

        return info;
    }
}