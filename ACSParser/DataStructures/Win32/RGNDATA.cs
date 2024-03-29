namespace ACSParser.DataStructures;

public class RGNDATA
{
    public RGNDATAHEADER Header;
    public RECT[] Rectangles;

    // TODO: Parse not tested.
    public static RGNDATA Parse(BinaryReader reader)
    {
        RGNDATA data = new RGNDATA();

        data.Header = RGNDATAHEADER.Parse(reader);

        if (data.Header.Count * 4 != data.Header.RgnSize)
        {
            throw new Exception("Invalid region data size.");
        }

        RECT[] rectangles = new RECT[data.Header.Count];
        for (var i = 0; i < data.Header.Count; i++)
        {
            rectangles[i] = RECT.Parse(reader);
        }
        data.Rectangles = rectangles;

        return data;
    }

    public void Write(BinaryWriter writer)
    {
        Header.Write(writer);

        for (var i = 0; i < Rectangles.Length; i++)
        {
            Rectangles[i].Write(writer);
        }
    }
}
