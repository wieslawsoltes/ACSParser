using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ACSOVERLAYINFO
{
    public byte OverlayType;
    public bool ReplaceTopImage;
    public ushort ImageIndex;
    public byte Unknown;
    public bool RegionDataPresent;
    public short XOffset;
    public short YOffset;
    public ushort Width;
    public ushort Height;
    public DATABLOCK RegionData;

    public static ACSOVERLAYINFO Parse(BinaryReader reader)
    {
        var overlayInfo = new ACSOVERLAYINFO();
        overlayInfo.OverlayType = reader.ReadByte();
        overlayInfo.ReplaceTopImage = reader.ReadBoolean();
        overlayInfo.ImageIndex = reader.ReadUInt16();
        overlayInfo.Unknown = reader.ReadByte();
        overlayInfo.RegionDataPresent = reader.ReadBoolean();
        overlayInfo.XOffset = reader.ReadInt16();
        overlayInfo.YOffset = reader.ReadInt16();
        overlayInfo.Width = reader.ReadUInt16();
        overlayInfo.Height = reader.ReadUInt16();
        if (overlayInfo.RegionDataPresent)
        {
            overlayInfo.RegionData = DATABLOCK.Parse(reader);
        }
        return overlayInfo;
    }
}
