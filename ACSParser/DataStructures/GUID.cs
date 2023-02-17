namespace ACSParser.DataStructures;

public class GUID
{
    public ULONG Data1; // 4 bytes
    public USHORT Data2; // 2 bytes
    public USHORT Data3; // 2 bytes
    public BYTE[] Data4; // 8 bytes

    public static GUID Parse(BinaryReader reader)
    {
        GUID guid = new GUID();

        guid.Data1 = reader.ULONG();
        guid.Data2 = reader.USHORT();
        guid.Data3 = reader.USHORT();

        guid.Data4 = new BYTE[8];
        for (var i = 0; i < 8; i++)
        {
            guid.Data4[i] = reader.BYTE();
        }

        return guid;
    }
}
