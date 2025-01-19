using System.IO;
using System.Text.Json;

namespace Vigil.Config
{
  /// <summary>
  /// Manages configuration data for a given type <typeparamref name="T"/> with thread safety.
  /// Handles loading, saving, and updating configuration files.
  /// </summary>
  /// <typeparam name="T">The type of the configuration data.</typeparam>
  /// <example>
  /// Example usage:
  /// <code>
  /// // Define a configuration class
  /// public class MyConfig
  /// {
  ///     public string Setting1 { get; set; } = "DefaultValue";
  ///     public int Setting2 { get; set; } = 42;
  /// }
  ///
  /// // Use ConfigManager to manage it
  /// var defaultConfig = new MyConfig();
  /// var configManager = new ConfigManager<MyConfig>("config.json", defaultConfig);
  ///
  /// // Access the configuration
  /// var config = configManager.GetConfig();
  /// Console.WriteLine(config.Setting1);
  ///
  /// // Update the configuration
  /// configManager.UpdateConfig(cfg =>
  /// {
  ///     cfg.Setting1 = "NewValue";
  /// });
  /// </code>
  /// </example>
  public class ConfigManager<T>
  {
    private readonly string _path;
    private T? _configData;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private readonly bool _usedDefaultConfig;

    public ConfigManager(string path, T defaultConfig)
    {
      _path = path;
      _usedDefaultConfig = !File.Exists(_path);
      if (_usedDefaultConfig)
      {
        _configData = defaultConfig;
        SaveConfig();
      }
      else
      {
        try
        {
          LoadConfig();
        }
        catch
        {
          _configData = defaultConfig;
          _usedDefaultConfig = true;
          SaveConfig();
        }
      }
    }

    public bool UsedDefaultConfig()
    {
      return _usedDefaultConfig;
    }

    private void LoadConfig()
    {
      _lock.EnterWriteLock();
      try
      {
        if (File.Exists(_path))
        {
          var json = File.ReadAllText(_path);
          _configData = JsonSerializer.Deserialize<T>(json);
        }
        else
        {
          _configData = Activator.CreateInstance<T>();
        }
      }
      finally
      {
        _lock.ExitWriteLock();
      }
    }

    public T GetConfig()
    {
      _lock.EnterReadLock();
      try
      {
        if (_configData == null)
        {
          throw new InvalidOperationException("Config data is null.");
        }
        return _configData;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public void UpdateConfig(Action<T> updateAction)
    {
      _lock.EnterWriteLock();
      try
      {
        if (_configData == null)
        {
          throw new InvalidOperationException("Config data is null.");
        }
        updateAction(_configData);
        SaveConfig();
      }
      finally
      {
        _lock.ExitWriteLock();
      }
    }

    private void SaveConfig()
    {
      var directory = Path.GetDirectoryName(_path);
      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }
      var json = JsonSerializer.Serialize(_configData);
      File.WriteAllText(_path, json);
    }
  }
}