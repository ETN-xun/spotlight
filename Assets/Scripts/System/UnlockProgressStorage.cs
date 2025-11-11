using System.IO;
using UnityEngine;

/// <summary>
/// 将关卡解锁进度存储到 AppData（Application.persistentDataPath）下的文件。
/// 提供读取/写入/初始化和从旧 PlayerPrefs 的一次性迁移。
/// </summary>
public static class UnlockProgressStorage
{
    private const string FileName = "unlocked_levels.txt";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    /// <summary>
    /// 初始化存储：若文件不存在则写入默认值；并尝试从旧的 PlayerPrefs 迁移一次。
    /// </summary>
    public static void Initialize()
    {
        MigrateFromPlayerPrefsIfNeeded();
        if (!File.Exists(FilePath))
        {
            SaveUnlockedLevels(1);
        }
    }

    /// <summary>
    /// 读取已解锁的最大关卡编号（默认 1）。
    /// </summary>
    public static int LoadUnlockedLevels()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var text = File.ReadAllText(FilePath);
                if (int.TryParse(text, out var value))
                {
                    return Mathf.Clamp(value, 1, 999);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"读取解锁文件失败，使用默认值：{e.Message}");
        }
        return 1;
    }

    /// <summary>
    /// 写入已解锁的最大关卡编号（至少为 1）。
    /// </summary>
    public static void SaveUnlockedLevels(int unlocked)
    {
        try
        {
            unlocked = Mathf.Max(1, unlocked);
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(FilePath, unlocked.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"写入解锁文件失败：{e.Message}");
        }
    }

    /// <summary>
    /// 是否已存在解锁进度文件。
    /// </summary>
    public static bool HasUnlockedLevels()
    {
        return File.Exists(FilePath);
    }

    /// <summary>
    /// 若文件不存在且旧的 PlayerPrefs 有值，则迁移一次。
    /// </summary>
    public static void MigrateFromPlayerPrefsIfNeeded()
    {
        try
        {
            if (!File.Exists(FilePath) && PlayerPrefs.HasKey("UnlockedLevels"))
            {
                int val = PlayerPrefs.GetInt("UnlockedLevels", 1);
                SaveUnlockedLevels(val);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"迁移旧进度失败：{e.Message}");
        }
    }
}

