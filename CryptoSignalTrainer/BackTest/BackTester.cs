using CryptoSignalTrainer.Models;
using Microsoft.ML;

public class Backtester
{
    public static void RunBacktestWithRiskManagement(string modelPath, string backtestDataPath)
    {
        var context = new MLContext();
        ITransformer trainedModel = context.Model.Load(modelPath, out var _);
        var predictionEngine = context.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

        var data = context.Data.LoadFromTextFile<ModelInput>(backtestDataPath, hasHeader: true, separatorChar: ',');
        var dataList = context.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: false).ToList();

        decimal initialCapital = 1000m;
        decimal cash = initialCapital;
        decimal coin = 0m;

        decimal entryPrice = 0;
        bool inPosition = false;

        decimal takeProfitThreshold = 0.02m;   // +%2
        decimal stopLossThreshold = -0.015m;   // -%1.5

        int totalBuys = 0, totalSells = 0;

        foreach (var candle in dataList)
        {
            var prediction = predictionEngine.Predict(candle);
            decimal currentPrice = (decimal)candle.Close;

            if (!inPosition && prediction.PredictedLabel == 0) // BUY
            {
                entryPrice = currentPrice;
                coin = cash / currentPrice;
                cash = 0;
                inPosition = true;
                totalBuys++;
                Console.WriteLine($"BUY at {entryPrice:F2}");
            }
            else if (inPosition)
            {
                decimal change = (currentPrice - entryPrice) / entryPrice;

                bool hitStopLoss = change <= stopLossThreshold;
                bool hitTakeProfit = change >= takeProfitThreshold;
                bool modelSaysSell = prediction.PredictedLabel == 1;

                if (hitStopLoss || hitTakeProfit || modelSaysSell)
                {
                    cash = coin * currentPrice;
                    coin = 0;
                    inPosition = false;
                    totalSells++;
                    Console.WriteLine($"SELL at {currentPrice:F2} | Return: {(change * 100):F2}%");
                }
            }
        }

        // Son pozisyonu kapatma (coin elimizde kalıyor)
        decimal finalPrice = (decimal)dataList.Last().Close;
        decimal portfolioValue = cash + (coin * finalPrice);

        Console.WriteLine("\n---- Backtest with Risk Management ----");
        Console.WriteLine($"Initial Capital   : {initialCapital:F2} TL");
        Console.WriteLine($"Final Cash        : {cash:F2} TL");
        Console.WriteLine($"Final Coin        : {coin:F6} (worth {coin * finalPrice:F2} TL)");
        Console.WriteLine($"Portfolio Value   : {portfolioValue:F2} TL");
        Console.WriteLine($"Total Profit      : {(portfolioValue - initialCapital):F2} TL ({((portfolioValue - initialCapital) / initialCapital) * 100:F2}%)");
        Console.WriteLine($"Total BUYs        : {totalBuys}");
        Console.WriteLine($"Total SELLs       : {totalSells}");
    }
}
