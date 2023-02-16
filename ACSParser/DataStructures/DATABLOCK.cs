namespace ACSParser.DataStructures;

public struct DATABLOCK
{
    public ULONG Size; // 4 bytes
    public BYTE[] Data; // Data.Length * 1 byte

    // TODO: Parse not tested.
    public static DATABLOCK Parse(BinaryReader reader)
    {
        DATABLOCK datablock = new DATABLOCK();

        datablock.Size = reader.ULONG();
        datablock.Data = reader.ReadBytes((int)datablock.Size);

        return datablock;
    }
}
