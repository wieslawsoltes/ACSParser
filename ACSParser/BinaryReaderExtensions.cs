using System.Runtime.InteropServices;
using System.Text;

namespace ACSParser;

public static class BinaryReaderExtensions
{
    public static string ReadStringNullTerminated(this BinaryReader reader)
    {
        List<byte> bytes = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            bytes.Add(b);
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    
    public static string ReadNullTerminatedString(this BinaryReader reader, int maxLength)
    {
        var buffer = new List<byte>();
        byte b;

        while ((b = reader.ReadByte()) != 0 && buffer.Count < maxLength)
        {
            buffer.Add(b);
        }

        return Encoding.ASCII.GetString(buffer.ToArray());
    }
    
    public static T[] ReadStructs<T>(this BinaryReader reader, int count) where T : struct
    {
        T[] array = new T[count];
        int sizeOfT = Marshal.SizeOf<T>();
        byte[] buffer = new byte[sizeOfT];
        for (int i = 0; i < count; i++)
        {
            buffer = reader.ReadBytes(sizeOfT);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            array[i] = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
        }
        return array;
    }

    public static List<T> ReadListWithSize<T>(this BinaryReader reader) where T : struct
    {
        int count = reader.ReadInt32();
        List<T> list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            T item = reader.ReadStruct<T>();
            list.Add(item);
        }
        return list;
    }

    public static T ReadStruct<T>(this BinaryReader reader) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] buffer = reader.ReadBytes(size);
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            IntPtr ptr = handle.AddrOfPinnedObject();
            return Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            handle.Free();
        }
    }

}
