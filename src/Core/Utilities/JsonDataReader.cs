using AutomationFramework.Core.Exceptions;
using Newtonsoft.Json;

namespace AutomationFramework.Core.Utilities;

/// <summary>
/// Reads and deserialises JSON test-data files.
/// Uses Newtonsoft.Json for broad compatibility with complex object graphs.
/// </summary>
public sealed class JsonDataReader
{
    private readonly string _root;

    public JsonDataReader(string root)
    {
        _root = root;
    }

    /// <summary>Deserialises a single object from <paramref name="relativePath"/>.</summary>
    public T Read<T>(string relativePath)
    {
        var fullPath = Path.Combine(_root, relativePath);
        EnsureExists(fullPath);

        var json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<T>(json)
               ?? throw new TestException($"JSON file '{fullPath}' deserialised to null.");
    }

    /// <summary>Deserialises a JSON array from <paramref name="relativePath"/>.</summary>
    public IList<T> ReadList<T>(string relativePath)
    {
        var fullPath = Path.Combine(_root, relativePath);
        EnsureExists(fullPath);

        var json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<List<T>>(json)
               ?? throw new TestException($"JSON array in '{fullPath}' deserialised to null.");
    }

    private static void EnsureExists(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Test data file not found: '{path}'");
    }
}
