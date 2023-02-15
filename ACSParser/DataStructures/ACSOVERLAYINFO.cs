namespace ACSParser;

public struct ACSOVERLAYINFO
{
    public BYTE OverlayType;
    public BOOL ReplaceTopImage;
    public USHORT ImageIndex;
    public BYTE Unknown;
    public BOOL RegionDataPresent;
    public SHORT XOffset;
    public SHORT YOffset;
    public USHORT Width;
    public USHORT Height;
    public DATABLOCK RegionData;

    public static ACSOVERLAYINFO Parse(BinaryReader reader)
    {
        var overlayInfo = new ACSOVERLAYINFO();

        overlayInfo.OverlayType = reader.BYTE();
        overlayInfo.ReplaceTopImage = reader.BOOL();
        overlayInfo.ImageIndex = reader.USHORT();
        overlayInfo.Unknown = reader.BYTE();
        overlayInfo.RegionDataPresent = reader.BOOL();
        overlayInfo.XOffset = reader.SHORT();
        overlayInfo.YOffset = reader.SHORT();
        overlayInfo.Width = reader.USHORT();
        overlayInfo.Height = reader.USHORT();

        if (overlayInfo.RegionDataPresent != 0x00)
        {
            overlayInfo.RegionData = DATABLOCK.Parse(reader);
        }

        return overlayInfo;
    }

    public static ACSOVERLAYINFO[] ParseList(BinaryReader reader)
    {
        BYTE overlayInfoCount = reader.BYTE();

        ACSOVERLAYINFO[] overlayInfo = new ACSOVERLAYINFO[overlayInfoCount];
        for (var i = 0; i < overlayInfoCount; i++)
        {
            overlayInfo[i] = ACSOVERLAYINFO.Parse(reader);
        }

        return overlayInfo;
    }
}
