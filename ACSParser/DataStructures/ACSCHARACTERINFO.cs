using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSCHARACTERINFO
{
    public ushort MinorVersion;
    public ushort MajorVersion;
    public ACSLOCATOR LocalizedInfo;
    public Guid UniqueId;
    public ushort CharacterWidth;
    public ushort CharacterHeight;
    public byte TransparentColorIndex;
    public uint Flags;
    public ushort AnimationSetMajorVersion;
    public ushort AnimationSetMinorVersion;
    public VOICEINFO VoiceOutputInfo;
    public BALLOONINFO BalloonInfo;
    public PALETTECOLOR[] ColorTable;
    public bool IsSystemTrayIconEnabled;
    public TRAYICON SystemTrayIcon;
    public STATEINFO[] AnimationStates;

    public static ACSCHARACTERINFO Parse(BinaryReader reader)
    {
        var characterInfo = new ACSCHARACTERINFO();
        characterInfo.MinorVersion = reader.ReadUInt16();
        characterInfo.MajorVersion = reader.ReadUInt16();
        characterInfo.LocalizedInfo = ACSLOCATOR.Parse(reader);
        characterInfo.UniqueId = new Guid(reader.ReadBytes(16));
        characterInfo.CharacterWidth = reader.ReadUInt16();
        characterInfo.CharacterHeight = reader.ReadUInt16();
        characterInfo.TransparentColorIndex = reader.ReadByte();
        characterInfo.Flags = reader.ReadUInt32();
        characterInfo.AnimationSetMajorVersion = reader.ReadUInt16();
        characterInfo.AnimationSetMinorVersion = reader.ReadUInt16();
        characterInfo.VoiceOutputInfo = VOICEINFO.Parse(reader);
        characterInfo.BalloonInfo = BALLOONINFO.Parse(reader);
        characterInfo.ColorTable = reader.ReadStructs<PALETTECOLOR>(256);
        characterInfo.IsSystemTrayIconEnabled = reader.ReadBoolean();
        if (characterInfo.IsSystemTrayIconEnabled)
        {
            characterInfo.SystemTrayIcon = TRAYICON.Parse(reader);
        }
        characterInfo.AnimationStates = reader.ReadListWithSize<STATEINFO>().ToArray();
        return characterInfo;
    }
}
