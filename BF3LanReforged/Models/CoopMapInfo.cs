namespace BF3LanReforged;

public class CoopMapInfo(string displayName, string levelPath, string imagePath)
{
    public string DisplayName { get; } = displayName;
    public string LevelPath { get; } = levelPath;
    public string ImagePath { get; } = imagePath;
}