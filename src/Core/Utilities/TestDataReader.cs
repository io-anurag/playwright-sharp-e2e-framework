using AutomationFramework.Core.Constants;

namespace AutomationFramework.Core.Utilities;

/// <summary>
/// Resolves paths to test data files and delegates to type-specific readers.
/// Implements the Facade Pattern — tests call one class regardless of file format.
/// </summary>
public sealed class TestDataReader
{
    private readonly string _testDataRoot;
    private readonly JsonDataReader _jsonReader;
    private readonly CsvDataReader  _csvReader;

    public TestDataReader(string? testDataRoot = null)
    {
        _testDataRoot = testDataRoot ?? ResolveTestDataRoot();
        _jsonReader   = new JsonDataReader(_testDataRoot);
        _csvReader    = new CsvDataReader(_testDataRoot);
    }

    /// <summary>Reads and deserialises a JSON file from TestData/Json/.</summary>
    public T ReadJson<T>(string fileName) =>
        _jsonReader.Read<T>(Path.Combine("Json", fileName));

    /// <summary>Reads a JSON array from TestData/Json/.</summary>
    public IList<T> ReadJsonList<T>(string fileName) =>
        _jsonReader.ReadList<T>(Path.Combine("Json", fileName));

    /// <summary>Reads records from a CSV file in TestData/Csv/.</summary>
    public IList<T> ReadCsv<T>(string fileName) =>
        _csvReader.Read<T>(Path.Combine("Csv", fileName));

    /// <summary>Reads a raw JSON payload from TestData/Payloads/.</summary>
    public string ReadPayload(string fileName)
    {
        var path = Path.Combine(_testDataRoot, "Payloads", fileName);
        return File.ReadAllText(path);
    }

    private static string ResolveTestDataRoot()
    {
        // First check the output directory (copied by .csproj)
        var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.TestDataDirectory);
        if (Directory.Exists(outputPath))
            return outputPath;

        // Walk up to solution root
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return Path.Combine(dir.FullName, "tests", TestConstants.TestDataDirectory);
            dir = dir.Parent;
        }

        return outputPath;
    }
}
