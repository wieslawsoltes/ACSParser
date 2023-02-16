namespace ACSParser.DataStructures;

public struct ACSCHARACTERINFO
{
    public USHORT MinorVersion;
    public USHORT MajorVersion;
    public ACSLOCATOR LocalizedInfo;
    public GUID UniqueId;
    public USHORT CharacterWidth;
    public USHORT CharacterHeight;
    public BYTE TransparentColorIndex;
    public ULONG Flags;
    public USHORT AnimationSetMajorVersion;
    public USHORT AnimationSetMinorVersion;
    public VOICEINFO VoiceOutputInfo;
    public BALLOONINFO BalloonInfo;
    public PALETTECOLOR[] ColorTable;
    public BOOL IsSystemTrayIconEnabled;
    public TRAYICON SystemTrayIcon;
    public STATEINFO[] AnimationStates;

    public static ACSCHARACTERINFO Parse(BinaryReader reader)
    {
        ACSCHARACTERINFO characterInfo = new ACSCHARACTERINFO();

        characterInfo.MinorVersion = reader.USHORT();
        characterInfo.MajorVersion = reader.USHORT();
        characterInfo.LocalizedInfo = ACSLOCATOR.Parse(reader);
        characterInfo.UniqueId = GUID.Parse(reader);
        characterInfo.CharacterWidth = reader.USHORT();
        characterInfo.CharacterHeight = reader.USHORT();
        characterInfo.TransparentColorIndex = reader.BYTE();
        characterInfo.Flags = reader.ULONG();
        characterInfo.AnimationSetMajorVersion = reader.USHORT();
        characterInfo.AnimationSetMinorVersion = reader.USHORT();

        if (characterInfo.IsVoiceOutputEnabled)
        {
            characterInfo.VoiceOutputInfo = VOICEINFO.Parse(reader);
        }

        if (characterInfo.IsWordBalloonEnabled)
        {
            characterInfo.BalloonInfo = BALLOONINFO.Parse(reader);
        }

        characterInfo.ColorTable = PALETTECOLOR.ParseList(reader);

        characterInfo.IsSystemTrayIconEnabled = reader.BOOL();
        if (characterInfo.IsSystemTrayIconEnabled != 0x00)
        {
            characterInfo.SystemTrayIcon = TRAYICON.Parse(reader);
        }

        characterInfo.AnimationStates = STATEINFO.ParseList(reader);

        return characterInfo;
    }

    public bool IsVoiceOutputEnabled => (Flags & 0x10) != 0;

    public bool IsWordBalloonDisabled => (Flags & 0x100) != 0;

    public bool IsWordBalloonEnabled => (Flags & 0x200) != 0;

    public bool IsSizeToTextEnabled => (Flags & 0x10000) != 0;

    public bool IsAutoHideDisabled => (Flags & 0x20000) != 0;

    public bool IsAutoPaceDisabled => (Flags & 0x40000) != 0;

    public bool IsStandardAnimationSetSupported => (Flags & 0x100000) != 0;
}
