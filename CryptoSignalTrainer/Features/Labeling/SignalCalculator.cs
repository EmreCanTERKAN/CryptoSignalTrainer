using CryptoSignalTrainer.Models;

namespace CryptoSignalTrainer.Features.Labeling;
public static class SignalCalculator
{
    public static int CalculateSignal(ModelInput current, ModelInput previous)
    {
        int score = 0;

        // RSI
        if (current.Rsi < 30) score++;
        if (current.Rsi > 70) score--;

        // MACD Histogram Sıfır kesişimi
        if (previous.MacdHistogram < 0 && current.MacdHistogram > 0) score++;
        if (previous.MacdHistogram > 0 && current.MacdHistogram < 0) score--;

        // EMA kesişimi
        if (previous.Close < previous.Ema && current.Close > current.Ema) score++;
        if (previous.Close > previous.Ema && current.Close < current.Ema) score--;

        // Stochastic RSI %K kesişimi
        if (previous.StochasticRsiK < 20 && current.StochasticRsiK > previous.StochasticRsiK) score++;
        if (previous.StochasticRsiK > 80 && current.StochasticRsiK < previous.StochasticRsiK) score--;

        // Bollinger bandları
        if (previous.Close < previous.BollingerLower && current.Close > current.BollingerLower) score++;
        if (previous.Close > previous.BollingerUpper && current.Close < current.BollingerUpper) score--;

        // Karar
        if (score >= 2)
            return 0; // Al
        if (score <= -2)
            return 1; // Sat
        return 2; // Bekle
    }

}
