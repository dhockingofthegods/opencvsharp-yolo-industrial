using System.Text.Json;

namespace industrial_camera_checking_products.Configuration;

public sealed class UserSettings
{
    public int CameraIndex { get; set; } = 0;
    public string? ModelPath { get; set; }
    public double ConfThreshold { get; set; } = 0.25;
    public double NmsThreshold { get; set; } = 0.45;
    public int InputW { get; set; } = 640;
    public int InputH { get; set; } = 640;

    private static string GetPath()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "industrial_camera_checking_products");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "settings.json");
    }

    public static UserSettings Load()
    {
        try
        {
            var path = GetPath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<UserSettings>(json);
                if (settings != null) return settings;
            }
        }
        catch { }
        return new UserSettings();
    }

    public void Save()
    {
        try
        {
            var path = GetPath();
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch { }
    }
}
