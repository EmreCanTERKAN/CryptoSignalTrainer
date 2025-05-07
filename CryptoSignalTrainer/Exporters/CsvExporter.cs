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

    public static void WriteFeaturesOnlyToCsv(List<ModelInput> data, string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, config);

        // Sadece özellikleri yazıyoruz
        csv.WriteHeader<FeaturesOnly>();
        csv.NextRecord();

        foreach (var item in data)
        {
            csv.WriteRecord(new FeaturesOnly(item));
            csv.NextRecord();
        }
    }

    private class FeaturesOnly
    {
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }
        public float Rsi { get; set; }
        public float Sma { get; set; }
        public float Macd { get; set; }
        public float MacdSignal { get; set; }
        public float MacdHistogram { get; set; }
        public float Ema { get; set; }
        public float Atr { get; set; }
        public float BollingerUpper { get; set; }
        public float BollingerLower { get; set; }
        public float BollingerMiddle { get; set; }
        public float StochasticRsiK { get; set; }
        public float StochasticRsiD { get; set; }
        public float Obv { get; set; }

        public FeaturesOnly(ModelInput input)
        {
            Open = input.Open;
            High = input.High;
            Low = input.Low;
            Close = input.Close;
            Volume = input.Volume;
            Rsi = input.Rsi;
            Sma = input.Sma;
            Macd = input.Macd;
            MacdSignal = input.MacdSignal;
            MacdHistogram = input.MacdHistogram;
            Ema = input.Ema;
            Atr = input.Atr;
            BollingerUpper = input.BollingerUpper;
            BollingerLower = input.BollingerLower;
            BollingerMiddle = input.BollingerMiddle;
            StochasticRsiK = input.StochasticRsiK;
            StochasticRsiD = input.StochasticRsiD;
            Obv = input.Obv;
        }
    }

}
