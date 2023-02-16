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

        characterInfo.MinorVersion = reader.USHORT(); // 2 bytes
        characterInfo.MajorVersion = reader.USHORT(); // 2 bytes
        characterInfo.LocalizedInfo = ACSLOCATOR.Parse(reader); // 8 bytes
        characterInfo.UniqueId = GUID.Parse(reader); // 16 bytes
        characterInfo.CharacterWidth = reader.USHORT(); // 2 bytes
        characterInfo.CharacterHeight = reader.USHORT(); // 2 bytes
        characterInfo.TransparentColorIndex = reader.BYTE(); // 1 bytes

        characterInfo.Flags = reader.ULONG(); // 4 bytes
        // var flags = Convert.ToString(characterInfo.Flags, 2).PadLeft(32, '0');
        // Console.WriteLine($"Flags: {flags}b 0x{characterInfo.Flags:X8}");

        characterInfo.AnimationSetMajorVersion = reader.USHORT(); // 2 bytes
        characterInfo.AnimationSetMinorVersion = reader.USHORT(); // 2 bytes

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

    public bool IsVoiceOutputDisabled => Util.IsBitSet(Flags, 4);

    public bool IsVoiceOutputEnabled => Util.IsBitSet(Flags, 5);

    public bool IsWordBalloonDisabled => Util.IsBitSet(Flags, 8);

    public bool IsWordBalloonEnabled => Util.IsBitSet(Flags, 9);

    public bool IsSizeToTextEnabled => Util.IsBitSet(Flags, 16);

    public bool IsAutoHideDisabled => Util.IsBitSet(Flags, 17);

    public bool IsAutoPaceDisabled => Util.IsBitSet(Flags, 18);

    public bool IsStandardAnimationSetSupported => Util.IsBitSet(Flags, 20);
}
