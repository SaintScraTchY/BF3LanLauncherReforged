namespace BF3LanReforged;

public class GameModeInfo(string displayName, string internalName)
{
    public string DisplayName { get; } = displayName;
    public string InternalName { get; } = internalName;
}