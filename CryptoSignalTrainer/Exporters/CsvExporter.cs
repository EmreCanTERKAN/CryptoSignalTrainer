using CryptoSignalTrainer.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace CryptoSignalTrainer.Exporters;
public static class CsvExporter
{
    public static void ExportLabeledData(List<ModelInput> data, string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.UTF8
        };

        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords(data);
    }
}
