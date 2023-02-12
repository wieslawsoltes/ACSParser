using System.Runtime.InteropServices;

namespace ACSParser;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
public struct STATEINFO
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string StateName;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public string[] Animations;

    public static STATEINFO Parse(BinaryReader reader)
    {
        STATEINFO stateInfo = new STATEINFO();
        stateInfo.StateName = new string(reader.ReadChars(64)).TrimEnd('\0');

        ushort count = reader.ReadUInt16();
        stateInfo.Animations = new string[count];

        for (int i = 0; i < count; i++)
        {
            stateInfo.Animations[i] = new string(reader.ReadChars(64)).TrimEnd('\0');
        }

        return stateInfo;
    }
}
