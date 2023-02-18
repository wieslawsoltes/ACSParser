namespace ACSParser.DataStructures;

public class STATEINFO
{
    public STRING StateName;
    public STRING[] Animations;

    public static STATEINFO Parse(BinaryReader reader)
    {
        var stateInfo = new STATEINFO();
        
        stateInfo.StateName = STRING.Parse(reader);

        var count = reader.USHORT();
 
        stateInfo.Animations = new STRING[count];

        for (var i = 0; i < count; i++)
        {
            stateInfo.Animations[i] = STRING.Parse(reader);
        }

        return stateInfo;
    }

    public static STATEINFO[] ParseList(BinaryReader reader)
    {
        USHORT stateInfoCount = reader.USHORT();

        STATEINFO[] stateInfo = new STATEINFO[stateInfoCount];
        for (var i = 0; i < stateInfoCount; i++)
        {
            stateInfo[i] = STATEINFO.Parse(reader);
        }

        return stateInfo;
    }

    public void Write(BinaryWriter writer)
    {
        StateName.Write(writer);

        for (var i = 0; i < Animations.Length; i++)
        {
            Animations[i].Write(writer);
        }
    }
}
