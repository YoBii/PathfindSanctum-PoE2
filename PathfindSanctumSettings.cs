using System.Collections.Generic;
using System.Drawing;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;

namespace PathfindSanctum;

public class PathfindSanctumSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(true);
    public ToggleNode DebugEnable { get; set; } = new ToggleNode(false);
    [Menu("Custom Weight Settings")]
    public CustomWeightSettings CustomWeights { get; set; } = new CustomWeightSettings();

    [Menu("Advanced Settings")]
    public AdvancedSettings AdvancedSettings { get; set; } = new AdvancedSettings();

    [Menu("Style Settings")]
    public StyleSettings StyleSettings { get; set; } = new StyleSettings();

    [Menu("Debug Settings")]
    public DebugSettings DebugSettings { get; set; } = new DebugSettings();


    public Dictionary<string, ProfileContent> Profiles =
        new()
        {
            ["Default"] = ProfileContent.CreateDefaultProfile(),
            ["No-Hit"] = ProfileContent.CreateNoHitProfile()
        };

    public ListNode CurrentProfile { get; set; }
    public ButtonNode ResetProfile { get; set; } = new ButtonNode();
    public ColorNode TextColor { get; set; } = new ColorNode(Color.White);
    public ColorNode BackgroundColor { get; set; } = new ColorNode(Color.FromArgb(128, 0, 0, 0));
    public ColorNode BestPathColor { get; set; } = new(Color.Cyan);
    public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(5, 0, 10);

    public PathfindSanctumSettings()
    {
        CurrentProfile = new ListNode { Values = [.. Profiles.Keys], Value = "Default" };
        ResetProfile.OnPressed += () => ResetCurrentProfileToDefault();
    }

    private void ResetCurrentProfileToDefault()
    {
        var currentProfileName = CurrentProfile.Value;
        Profiles[currentProfileName] = currentProfileName switch
        {
            "Default" => ProfileContent.CreateDefaultProfile(),
            "No-Hit" => ProfileContent.CreateNoHitProfile(),
            _ => ProfileContent.CreateDefaultProfile()
        };
    }
}
