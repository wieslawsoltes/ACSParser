namespace ACSParser;

public static class BinaryReaderExtensions
{
    public static BOOL BOOL(this BinaryReader reader) => reader.ReadByte();
    public static BYTE BYTE(this BinaryReader reader) => reader.ReadByte();
    public static WCHAR WCHAR(this BinaryReader reader) => reader.ReadInt16();
    public static SHORT SHORT(this BinaryReader reader) => reader.ReadInt16();
    public static USHORT USHORT(this BinaryReader reader) => reader.ReadUInt16();
    public static LONG LONG(this BinaryReader reader) => reader.ReadInt32();
    public static ULONG ULONG(this BinaryReader reader) => reader.ReadUInt32();
}
