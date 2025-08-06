using System.Collections.Generic;

namespace AcsEditor.Models;

public class AcsAnimation : AcsBase
{
    public string? Name { get; set; }
    public string? ReturnAnimation { get; set; }
    public byte TransitionType { get; set; }
    public List<AcsFrame> Frames { get; set; } = new();
}