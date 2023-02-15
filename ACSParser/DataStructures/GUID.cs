namespace ACSParser;

public struct GUID
{
    public ULONG Data1;
    public USHORT Data2;
    public USHORT Data3;
    public BYTE[] Data4;

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
