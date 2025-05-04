using CryptoSignalTrainer.Models;

namespace CryptoSignalTrainer.Features.Labeling;
public static class LabelGenerator
{
    // Risk / Odül Ayarları
    private const float TakeProfitPercent = 1.5f;
    private const float StopLossPercent = -1.0f;
    private const int MaxBarsToWait = 10;

    // RSI eşikleri
    private const float RsiOverbought = 70;
    private const float RsiOversold = 30;

    public enum SignalType
    {
        Buy = 0,
        Sell = 1,
        Hold = 2
    }

    public static List<ModelInput> AddLabels(this List<ModelInput> inputs)
    {
        if (inputs == null || inputs.Count < MaxBarsToWait + 1)
            return new List<ModelInput>();

        var labeledList = new List<ModelInput>();

        for (int i = 0; i < inputs.Count - MaxBarsToWait; i++)
        {
            var current = inputs[i];
            decimal entryPrice = (decimal)current.Close;

            decimal takeProfitUp = entryPrice * (1 + (decimal)(TakeProfitPercent / 100));
            decimal takeProfitDown = entryPrice * (1 - (decimal)(TakeProfitPercent / 100));
            decimal stopLossUp = entryPrice * (1 + (decimal)(StopLossPercent / 100));
            decimal stopLossDown = entryPrice * (1 - (decimal)(StopLossPercent / 100));

            bool hitBuyTarget = false;
            bool hitSellTarget = false;

            for (int j = i + 1; j < i + MaxBarsToWait && j < inputs.Count; j++)
            {
                var next = inputs[j];

                decimal high = (decimal)next.High;
                decimal low = (decimal)next.Low;

                if (high >= takeProfitUp)
                    hitBuyTarget = true;

                if (low <= takeProfitDown)
                    hitSellTarget = true;

                if (low <= stopLossUp)
                    break;

                if (high >= stopLossDown)
                    break;
            }

            // RSI bazlı filtreleme (opsiyonel ama güçlü)
            bool isRsiOverbought = current.Rsi > RsiOverbought;
            bool isRsiOversold = current.Rsi < RsiOversold;

            // Karar kuralları
            SignalType signal = SignalType.Hold;

            if (hitBuyTarget && !hitSellTarget && isRsiOversold)
                signal = SignalType.Buy;
            else if (hitSellTarget && !hitBuyTarget && isRsiOverbought)
                signal = SignalType.Sell;
            else
                signal = SignalType.Hold;

            current.SignalLabel = (int)signal;

            labeledList.Add(current);
        }

        return labeledList;
    }

}
