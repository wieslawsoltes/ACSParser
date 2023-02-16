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

        return str;
    }
}
