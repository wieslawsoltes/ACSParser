namespace ACSParser;

public struct RGNDATA
{
    public RGNDATAHEADER Header;
    public RECT[] Rectangles;

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
}
