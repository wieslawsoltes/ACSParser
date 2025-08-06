using System.Collections.Generic;

namespace AcsEditor.Models;

public class AcsFrame : AcsBase
{
    public List<AcsFrameImage> Images { get; set; } = new();
    public ushort AudioIndex { get; set; }
    public ushort Duration { get; set; }
    public short ExitFrameIndex { get; set; }
    public List<AcsBranch> Branches { get; set; } = new();
    public List<AcsOverlay> Overlays { get; set; } = new();
}