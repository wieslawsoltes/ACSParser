namespace ACSParser.DataStructures;

public class DATABLOCK
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

    public void Write(BinaryWriter writer)
    {
        writer.Write(Size);

        for (var i = 0; i < Data.Length; i++)
        {
            writer.Write(Data[i]);
        }
    }
}
