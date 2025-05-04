using CsvHelper;
using System.Globalization;

namespace CryptoSignalTrainer.Data;
public static class CandleLoader
{
    public static List<CandleData> LoadCandles(string csvPath)
    {
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<CandleData>().ToList();
        return records;
    }
}