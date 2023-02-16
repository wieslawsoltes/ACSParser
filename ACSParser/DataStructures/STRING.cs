using System.Text;

namespace ACSParser.DataStructures;

public struct STRING
{
    public ULONG Count;
    public WCHAR[] Characters;

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

    public string AsString()
    {
        byte[] byteArray = new byte[Characters.Length * sizeof(short)];
        Buffer.BlockCopy(Characters, 0, byteArray, 0, byteArray.Length);
        return Encoding.Unicode.GetString(byteArray);
    }
}
