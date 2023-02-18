using System.Text;

namespace ACSParser.DataStructures;

public class STRING
{
    public ULONG Count; // 4 bytes
    public WCHAR[] Characters; // Characters.Length * 2 bytes

    public static STRING Parse(BinaryReader reader)
    {
        var str = new STRING();

        str.Count = reader.ULONG();

        str.Characters = new WCHAR[str.Count];
        for (var i = 0; i < str.Count; i++)
        {
            str.Characters[i] = reader.WCHAR();
        }

        if (str.Count > 0)
        {
            // Null Terminator (0x0000)
            reader.WCHAR(); 
        }

        return str;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(Count);

        for (var i = 0; i < Characters.Length; i++)
        {
            writer.Write(Characters[i]);
        }

        if (Count > 0)
        {
            writer.Write((WCHAR)0x0000);
        }
    }

    public string AsString()
    {
        byte[] byteArray = new byte[Characters.Length * sizeof(short)];
        Buffer.BlockCopy(Characters, 0, byteArray, 0, byteArray.Length);
        return Encoding.Unicode.GetString(byteArray);
    }
}
