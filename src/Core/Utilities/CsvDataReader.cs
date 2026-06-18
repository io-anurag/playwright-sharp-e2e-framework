using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace AutomationFramework.Core.Utilities;

/// <summary>
/// Reads and maps CSV test-data files to strongly-typed records.
/// Uses CsvHelper for robust parsing (handles quoted fields, custom delimiters, etc.).
/// </summary>
public sealed class CsvDataReader
{
    private readonly string _root;

    public CsvDataReader(string root)
    {
        _root = root;
    }

    /// <summary>
    /// Reads all records from a CSV file and maps them to <typeparamref name="T"/>.
    /// The CSV must have a header row matching the public properties of <typeparamref name="T"/>.
    /// </summary>
    public IList<T> Read<T>(string relativePath)
    {
        var fullPath = Path.Combine(_root, relativePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"CSV test data file not found: '{fullPath}'");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions     = TrimOptions.Trim,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(fullPath);
        using var csv    = new CsvReader(reader, config);
        return csv.GetRecords<T>().ToList();
    }
}
