using CryptoSignalTrainer.Data;
using CryptoSignalTrainer.Features.Labeling;

var candles = CandleLoader.LoadCandles("candles.csv");
var inputs = IndicatorCalculator.Calculate(candles);

var labeledInputs = LabelGenerator.AddLabels(inputs);

Console.WriteLine("Open | Close | RSI  | Sinyal");
foreach (var item in labeledInputs)
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