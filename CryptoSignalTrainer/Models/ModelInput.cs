namespace CryptoSignalTrainer.Models;
public class ModelInput
{
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Close { get; set; }
    public float Volume { get; set; }

    public float Rsi { get; set; }
    public float Sma { get; set; }
    public float Macd { get; set; }

    public bool Label { get; set; } // Şimdilik boş, ileride etiketleyeceğiz
}