namespace ACSParser.DataStructures;

public class ACSOVERLAYINFO
{
    public BYTE OverlayType; // 1 byte
    public BOOL ReplaceTopImage; // 1 byte
    public USHORT ImageIndex; // 2 bytes
    public BYTE Unknown; // 1 byte
    public BOOL RegionDataPresent; // 1 byte
    public SHORT XOffset; // 2 bytes
    public SHORT YOffset; // 2 bytes
    public USHORT Width; // 2 bytes
    public USHORT Height; // 2 bytes
    public DATABLOCK RegionData;

    // TODO: Parse not tested.
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
