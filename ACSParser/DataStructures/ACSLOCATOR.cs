﻿namespace ACSParser.DataStructures;

public class ACSLOCATOR // 8 bytes
{
    public ULONG Offset; // 4 bytes
    public ULONG Size; // 4 bytes

    public static ACSLOCATOR Parse(BinaryReader reader)
    {
        ACSLOCATOR locator = new ACSLOCATOR();

        locator.Offset = reader.ULONG();
        locator.Size = reader.ULONG();

        return locator;
    }
    
    public void Write(BinaryWriter writer)
    {
        writer.Write(Offset);
        writer.Write(Size);
    }
}
