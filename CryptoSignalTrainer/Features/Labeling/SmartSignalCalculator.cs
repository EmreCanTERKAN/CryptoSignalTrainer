using CryptoSignalTrainer.Models;

public enum TradeSignal
{
    Buy = 0,
    Sell = 1,
    Hold = 2
}

public enum PositionState
{
    None,
    Long,
    Short
}

public class SmartSignalCalculator
{
    private TradeSignal _lastSignal = TradeSignal.Hold;
    private PositionState _position = PositionState.None;
    private int _lastSignalBarIndex = -100; // Başlangıçta çok geçmişte
    private readonly int _cooldownBars;

    // Göstergelere göre dinamik ağırlıklar
    private readonly Dictionary<string, float> _weights = new()
    {
        { "Rsi", 2.5f },
        { "MacdHistogram", 2.0f },
        { "EmaCross", 2.0f },
        { "StochasticRsi", 2.0f },
        { "Bollinger", 1.5f },
        { "Obv", 2.0f },
        { "SmaCross", 1.5f },
        { "AtrVolatility", 1.0f }
    };

    public SmartSignalCalculator(int cooldownBars = 5)
    {
        _cooldownBars = cooldownBars;
    }

    public TradeSignal Calculate(ModelInput current, ModelInput previous, int currentBarIndex)
    {
        float score = 0;

        // RSI - Oversold / Overbought
        if (current.Rsi < 30) score += _weights["Rsi"];
        if (current.Rsi > 70) score -= _weights["Rsi"];

        // MACD Histogram Cross
        if (previous.MacdHistogram < 0 && current.MacdHistogram > 0)
            score += _weights["MacdHistogram"];
        if (previous.MacdHistogram > 0 && current.MacdHistogram < 0)
            score -= _weights["MacdHistogram"];

        // EMA Cross
        if (previous.Close < previous.Ema && current.Close > current.Ema)
            score += _weights["EmaCross"];
        if (previous.Close > previous.Ema && current.Close < current.Ema)
            score -= _weights["EmaCross"];

        // Stochastic RSI
        if (previous.StochasticRsiK < 20 && current.StochasticRsiK > previous.StochasticRsiK)
            score += _weights["StochasticRsi"];
        if (previous.StochasticRsiK > 80 && current.StochasticRsiK < previous.StochasticRsiK)
            score -= _weights["StochasticRsi"];

        // Bollinger Band Crossing
        if (previous.Close < previous.BollingerLower && current.Close > current.BollingerLower)
            score += _weights["Bollinger"];
        if (previous.Close > previous.BollingerUpper && current.Close < current.BollingerUpper)
            score -= _weights["Bollinger"];

        // OBV (On-Balance Volume) - Hacim trendi
        if (current.Obv > previous.Obv) score += _weights["Obv"] * 0.5f; // Alçalan hacim düşüş sinyali olabilir
        if (current.Obv < previous.Obv) score -= _weights["Obv"] * 0.5f;

        // SMA Cross
        if (previous.Close < previous.Sma && current.Close > current.Sma)
            score += _weights["SmaCross"];
        if (previous.Close > previous.Sma && current.Close < current.Sma)
            score -= _weights["SmaCross"];

        // ATR Volatilite Kontrolü - Yüksek volatilite riskli olabilir
        if (current.Atr > 2 * GetAverageAtr(current, previous)) // Yalnızca örnek hesaplama
        {
            score *= 0.5f; // Skoru azalt (aşırı dalgalanmada işlem yapmaktan kaçın)
        }

        // Cooldown kontrolü
        if (currentBarIndex - _lastSignalBarIndex < _cooldownBars)
            return TradeSignal.Hold;

        // Karar
        TradeSignal signal = TradeSignal.Hold;

        if (score >= 4.0f && _position != PositionState.Long)
        {
            signal = TradeSignal.Buy;
            _position = PositionState.Long;
            _lastSignalBarIndex = currentBarIndex;
        }
        else if (score <= -4.0f && _position != PositionState.Short)
        {
            signal = TradeSignal.Sell;
            _position = PositionState.Short;
            _lastSignalBarIndex = currentBarIndex;
        }

        // Aynı sinyalin tekrar etmemesi için kontrol
        if (signal == _lastSignal)
            return TradeSignal.Hold;

        _lastSignal = signal;
        return signal;
    }

    public void ResetPosition()
    {
        _position = PositionState.None;
    }

    public PositionState GetCurrentPosition()
    {
        return _position;
    }

    // Basit bir ATR ortalaması örneği (gerçek sistemde daha gelişmiş kullanılmalı)
    private float GetAverageAtr(ModelInput current, ModelInput previous)
    {
        return (current.Atr + previous.Atr) / 2;
    }
}