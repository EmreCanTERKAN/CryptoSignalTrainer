using Microsoft.ML.Data;

namespace CryptoSignalTrainer.Models;
public class ModelInput
{
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Close { get; set; }
    public float Volume { get; set; }

    // Mevcut göstergeler
    public float Rsi { get; set; }
    public float Sma { get; set; }
    public float Macd { get; set; }
    public float MacdSignal { get; set; }
    public float MacdHistogram { get; set; }

    // Yeni eklenen göstergeler
    public float Ema { get; set; }
    public float Atr { get; set; }
    public float BollingerUpper { get; set; }
    public float BollingerLower { get; set; }
    public float BollingerMiddle { get; set; }
    public float StochasticRsiK { get; set; }
    public float StochasticRsiD { get; set; }
    public float Obv { get; set; }

    [ColumnName("Label")]
    public int SignalLabel { get; set; } // 0=Buy, 1=Sell, 2=Hold
}