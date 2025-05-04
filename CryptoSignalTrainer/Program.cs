using CryptoSignalTrainer.Data;

var candles = CandleLoader.LoadCandles("candles.csv");

Console.WriteLine($"Toplam {candles.Count} veri yüklendi.");
