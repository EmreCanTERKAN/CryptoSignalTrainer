using Microsoft.ML.Data;

namespace CryptoSignalTrainer.Models;
public class ModelInput
{
    [LoadColumn(0)] public float Open { get; set; }
    [LoadColumn(1)] public float High { get; set; }
    [LoadColumn(2)] public float Low { get; set; }
    [LoadColumn(3)] public float Close { get; set; }
    [LoadColumn(4)] public float Volume { get; set; }

    [LoadColumn(5)] public float Rsi { get; set; }
    [LoadColumn(6)] public float Sma { get; set; }
    [LoadColumn(7)] public float Macd { get; set; }
    [LoadColumn(8)] public float MacdSignal { get; set; }
    [LoadColumn(9)] public float MacdHistogram { get; set; }

    [LoadColumn(10)] public float Ema { get; set; }
    [LoadColumn(11)] public float Atr { get; set; }
    [LoadColumn(12)] public float BollingerUpper { get; set; }
    [LoadColumn(13)] public float BollingerLower { get; set; }
    [LoadColumn(14)] public float BollingerMiddle { get; set; }
    [LoadColumn(15)] public float StochasticRsiK { get; set; }
    [LoadColumn(16)] public float StochasticRsiD { get; set; }
    [LoadColumn(17)] public float Obv { get; set; }

    [LoadColumn(18)]
    public int SignalLabel { get; set; } // 0=Buy, 1=Sell, 2=Hold
}