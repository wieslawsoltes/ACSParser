namespace ACSParser.DataStructures;

public struct DATABLOCK
{
    public ULONG Size;
    public BYTE[] Data;

    public static DATABLOCK Parse(BinaryReader reader)
    {
        DATABLOCK datablock = new DATABLOCK();

        datablock.Size = reader.ULONG();
        datablock.Data = reader.ReadBytes((int)datablock.Size);

        return datablock;
    }
}
