using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BF3LanLauncher;

public partial class MainWindow
{
    private const string GameExe = "bf3_offline.exe";
    private string _currentCoopMapLevel = "COOP_007/COOP_007";
    private string _currentMpMode = "ConquestLarge0";
    private string _currentMpMapLevel = "MP_001/MP_001";

    // Data bindings
    private List<CoopMapInfo> CoopMaps { get; set; } = [];
    private List<GameModeInfo> GameModes { get; set; } = [];
    private List<MapInfo>? CurrentMapList { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        LoadMapData();
        VerifyGameFiles();
    }

    private static void VerifyGameFiles()
    {
        if (File.Exists(GameExe))
            return;
        
        MessageBox.Show($"'{GameExe}' not found!\nPlease place this launcher in your Battlefield 3 game folder.", 
            "Missing File", MessageBoxButton.OK, MessageBoxImage.Error);
        
        Application.Current.Shutdown();
    }

    #region Data Initialization

    private void LoadMapData()
    {
        // Co-op maps
        CoopMaps =
        [
            new CoopMapInfo("OPERATION EXODUS", "COOP_007/COOP_007", "/Images/BF3_Coop_1.bmp"),
            new CoopMapInfo("FIRE FROM THE SKY", "COOP_006/COOP_006", "/Images/BF3_Coop_2.bmp"),
            new CoopMapInfo("EXFILTRATION", "COOP_009/COOP_009", "/Images/BF3_Coop_3.bmp"),
            new CoopMapInfo("HIT AND RUN", "COOP_002/COOP_002", "/Images/BF3_Coop_4.bmp"),
            new CoopMapInfo("DROP ʹEM LIKE LIQUID", "COOP_003/COOP_003", "/Images/BF3_Coop_5.bmp"),
            new CoopMapInfo("THE ELEVENTH HOUR", "COOP_010/COOP_010", "/Images/BF3_Coop_6.bmp")
        ];

        // Game modes
        GameModes =
        [
            new GameModeInfo("Conquest Large", "ConquestLarge0"),
            new GameModeInfo("Conquest Small", "ConquestSmall0"),
            new GameModeInfo("Conquest Assault (Large)", "ConquestAssaultLarge0"),
            new GameModeInfo("Conquest Assault (Small)", "ConquestAssaultSmall0"),
            new GameModeInfo("Conquest Assault #2", "ConquestAssaultSmall1"),
            new GameModeInfo("Rush", "RushLarge0"),
            new GameModeInfo("Squad Rush", "SquadRush0"),
            new GameModeInfo("Squad Deathmatch", "SquadDeathMatch0"),
            new GameModeInfo("Team Deathmatch", "TeamDeathMatch0"),
            new GameModeInfo("TDM Close Quarters", "TeamDeathMatchC0"),
            new GameModeInfo("Gun Master", "GunMaster0"),
            new GameModeInfo("Domination", "Domination0"),
            new GameModeInfo("Tank Superiority", "TankSuperiority0"),
            new GameModeInfo("Scavenger", "Scavenger0"),
            new GameModeInfo("Capture The Flag", "CaptureTheFlag0"),
            new GameModeInfo("Air Superiority", "AirSuperiority0")
        ];

        // Default map list (Conquest Large)
        CurrentMapList = GetMapsForMode("ConquestLarge0");
        MapComboBox.ItemsSource = CurrentMapList;
        MapComboBox.SelectedIndex = 0;
    }

    private List<MapInfo> GetMapsForMode(string mode)
    {
        var maps = new List<MapInfo>();

        // Base maps (available in most modes)
        var baseMaps = new[]
        {
            new MapInfo("Grand Bazaar", "MP_001/MP_001", "/Images/MP_001.bmp"),
            new MapInfo("Tehran Highway", "MP_003/MP_003", "/Images/MP_003.bmp"),
            new MapInfo("Caspian Border", "MP_007/MP_007", "/Images/MP_007.bmp"),
            new MapInfo("Seine Crossing", "MP_011/MP_011", "/Images/MP_011.bmp"),
            new MapInfo("Operation Firestorm", "MP_012/MP_012", "/Images/MP_012.bmp"),
            new MapInfo("Damavand Peak", "MP_013/MP_013", "/Images/MP_013.bmp"),
            new MapInfo("Noshahr Canals", "MP_017/MP_017", "/Images/MP_017.bmp"),
            new MapInfo("Kharg Island", "MP_018/MP_018", "/Images/MP_018.bmp"),
            new MapInfo("Operation Metro", "MP_Subway/MP_Subway", "/Images/MP_Subway.bmp"),
            new MapInfo("Gulf of Oman", "XP1_002/XP1_002", "/Images/XP1_002.bmp"),
            new MapInfo("Strike at Karkand", "XP1_001/XP1_001", "/Images/XP1_001.bmp"),
            new MapInfo("Sharqi Peninsula", "XP1_003/XP1_003", "/Images/XP1_003.bmp"),
            new MapInfo("Wake Island", "XP1_004/XP1_004", "/Images/XP1_004.bmp"),
            new MapInfo("Bandar Desert", "XP3_Desert/XP3_Desert", "/Images/XP3_Desert.bmp"),
            new MapInfo("Alborz Mountains", "XP3_Alborz/XP3_Alborz", "/Images/XP3_Alborz.bmp"),
            new MapInfo("Armored Shield", "XP3_Shield/XP3_Shield", "/Images/XP3_Shield.bmp"),
            new MapInfo("Death Valley", "XP3_Valley/XP3_Valley", "/Images/XP3_Valley.bmp"),
            new MapInfo("Markaz Monolith", "XP4_FD/XP4_FD", "/Images/XP4_FD.bmp"),
            new MapInfo("Azadi Palace", "XP4_Parl/XP4_Parl", "/Images/XP4_Parl.bmp"),
            new MapInfo("Epicenter", "XP4_Quake/XP4_Quake", "/Images/XP4_Quake.bmp"),
            new MapInfo("Talah Market", "XP4_Rubble/XP4_Rubble", "/Images/XP4_Rubble.bmp"),
            new MapInfo("Operation Riverside", "XP5_001/XP5_001", "/Images/XP5_001.bmp"),
            new MapInfo("Nebandan Flats", "XP5_002/XP5_002", "/Images/XP5_002.bmp"),
            new MapInfo("Kiasar Railroad", "XP5_003/XP5_003", "/Images/XP5_003.bmp"),
            new MapInfo("Sabalan Pipeline", "XP5_004/XP5_004", "/Images/XP5_004.bmp")
        };

        var cqMaps = new[]
        {
            new MapInfo("Scrapmetal", "XP2_Factory/XP2_Factory", "/Images/XP2_Factory.bmp"),
            new MapInfo("Operation 925", "XP2_Office/XP2_Office", "/Images/XP2_Office.bmp"),
            new MapInfo("Donya Fortress", "XP2_Palace/XP2_Palace", "/Images/XP2_Palace.bmp"),
            new MapInfo("Ziba Tower", "XP2_Skybar/XP2_Skybar", "/Images/XP2_Skybar.bmp")
        };

        // Filter maps based on game mode
        switch (mode)
        {
            case "ConquestLarge0":
            case "ConquestSmall0":
                maps.AddRange(baseMaps);
                break;
            case "ConquestAssaultLarge0":
            case "ConquestAssaultSmall0":
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_001")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_003")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_004")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Rubble")));
                break;
            case "ConquestAssaultSmall1":
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_001")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_003")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_004")));
                break;
            case "RushLarge0":
            case "SquadRush0":
                maps.AddRange(baseMaps);
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP1_001")));
                break;
            case "SquadDeathMatch0":
            case "TeamDeathMatch0":
            case "TeamDeathMatchC0":
                maps.AddRange(baseMaps);
                maps.AddRange(cqMaps);
                break;
            case "GunMaster0":
                maps.AddRange(cqMaps);
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_FD")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Parl")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Quake")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Rubble")));
                break;
            case "Domination0":
                maps.AddRange(cqMaps);
                break;
            case "TankSuperiority0":
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP3_Desert")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP3_Alborz")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP3_Shield")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP3_Valley")));
                break;
            case "Scavenger0":
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_FD")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Parl")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Quake")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP4_Rubble")));
                break;
            case "CaptureTheFlag0":
            case "AirSuperiority0":
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP5_001")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP5_002")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP5_003")));
                maps.Add(baseMaps.First(m => m.LevelPath.Contains("XP5_004")));
                break;
            default:
                maps.AddRange(baseMaps);
                break;
        }

        return maps;
    }

    #endregion

    #region Event Handlers

    private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // Reset any state if needed
    }

    private void CoopMapComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (CoopMapComboBox.SelectedItem is CoopMapInfo selected)
        {
            _currentCoopMapLevel = selected.LevelPath;
            CoopMapImage.Source = new BitmapImage(new Uri(selected.ImagePath, UriKind.Relative));
        }
    }

    private void GameModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (GameModeComboBox.SelectedItem is GameModeInfo mode)
        {
            _currentMpMode = mode.InternalName;
            CurrentMapList = GetMapsForMode(mode.InternalName);
            MapComboBox.ItemsSource = CurrentMapList;
            MapComboBox.SelectedIndex = 0;
        }
    }

    private void MapComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (MapComboBox.SelectedItem is MapInfo map)
        {
            _currentMpMapLevel = map.LevelPath;
            MpMapImage.Source = new BitmapImage(new Uri(map.ImagePath, UriKind.Relative));
        }
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        string ip = IpTextBox.Text.Trim();
        if (!IsValidIp(ip))
        {
            MessageBox.Show("Please enter a valid IP address.", "Invalid IP", MessageBoxButton.OK, MessageBoxImage.Warning);
            IpTextBox.Focus();
            return;
        }

        string args = $"-super layout.toc -Client.IsPresenceEnabled false -Core.EnableJuice 0 " +
                      $"-DisplayAsserts 0 -Persistence.AllUnlocksAlwaysUnlocked true " +
                      $"-Render.DebugRendererEnable 0 -Client.ServerIp {ip}";

        LaunchGame(args);
    }

    private void StartCoopServerButton_Click(object sender, RoutedEventArgs e)
    {
        string args = $"-super layout.toc -Client.IsPresenceEnabled false " +
                      $"-Game.Level Levels/{_currentCoopMapLevel} " +
                      $"-Render.DebugRendererEnable 0 -Core.EnableJuice 0 " +
                      $"-Online.Backend Backend_Lan -DisplayAsserts 0 " +
                      $"-Persistence.AllUnlocksAlwaysUnlocked true";

        LaunchGame(args);
    }

    private void StartMpServerButton_Click(object sender, RoutedEventArgs e)
    {
        string args = $"-super layout.toc -Client.IsPresenceEnabled false " +
                      $"-Game.Level Levels/{_currentMpMapLevel} " +
                      $"-Game.DisablePreRound 1 " +
                      $"-Game.DefaultLayerInclusion GameMode={_currentMpMode} " +
                      $"-Render.DebugRendererEnable 1 -Core.EnableJuice 0 " +
                      $"-Online.Backend Backend_Lan -DisplayAsserts 0 " +
                      $"-Persistence.AllUnlocksAlwaysUnlocked true";

        LaunchGame(args);
    }

    #endregion

    #region Helper Methods

    private static bool IsValidIp(string ip)
    {
        return !string.IsNullOrWhiteSpace(ip) && MyRegex().IsMatch(ip);
    }

    private static void LaunchGame(string arguments)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GameExe,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to launch game:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

    #region Window Chrome

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    [GeneratedRegex(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
    private static partial Regex MyRegex();

    #endregion
}

#region Data Models

public class CoopMapInfo(string displayName, string levelPath, string imagePath)
{
    public string DisplayName { get; } = displayName;
    public string LevelPath { get; } = levelPath;
    public string ImagePath { get; } = imagePath;
}

public class GameModeInfo(string displayName, string internalName)
{
    public string DisplayName { get; } = displayName;
    public string InternalName { get; } = internalName;
}

public class MapInfo(string displayName, string levelPath, string imagePath)
{
    public string DisplayName { get; } = displayName;
    public string LevelPath { get; } = levelPath;
    public string ImagePath { get; } = imagePath;
}

#endregion
