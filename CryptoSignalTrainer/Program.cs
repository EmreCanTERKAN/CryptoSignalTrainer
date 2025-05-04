using CryptoSignalTrainer.Data;
using CryptoSignalTrainer.Features.Labeling;
using CryptoSignalTrainer.Models;

var candles = CandleLoader.LoadCandles("candles.csv");
var inputs = IndicatorCalculator.Calculate(candles);

var labeled = new List<ModelInput>();

for (int i = 1; i < inputs.Count; i++)
{
    var current = inputs[i];
    var previous = inputs[i - 1];

    current.SignalLabel = SignalCalculator.CalculateSignal(current, previous);
    labeled.Add(current);
}

Console.WriteLine("Open | Close | RSI  | Sinyal");
foreach (var item in labeled)
{
    string signalText = item.SignalLabel switch
    {
        0 => "Al",
        1 => "Sat",
        2 => "Bekle",
        _ => "Bilinmiyor"
    };

    Console.WriteLine($"{item.Open} | {item.Close,-6} | {item.Rsi:F2} | {signalText}");
}

Console.WriteLine(labeled.Count);