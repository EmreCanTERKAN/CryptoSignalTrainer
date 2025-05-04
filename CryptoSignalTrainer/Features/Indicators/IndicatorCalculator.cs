using System;
using System.Collections.Generic;
using System.Linq;
using CryptoSignalTrainer.Data;
using CryptoSignalTrainer.Models;
using Skender.Stock.Indicators;

public static class IndicatorCalculator
{
    public static List<ModelInput> Calculate(List<CandleData> candles)
    {
        var quotes = candles.Select(c => new Quote
        {
            Date = c.OpenTime,
            Open = (decimal)c.Open,
            High = (decimal)c.High,
            Low = (decimal)c.Low,
            Close = (decimal)c.Close,
            Volume = (decimal)c.Volume
        }).ToList();

        // Göstergeleri hesapla
        var rsi = quotes.GetRsi(14).ToDictionary(x => x.Date);
        var sma = quotes.GetSma(14).ToDictionary(x => x.Date);
        var macd = quotes.GetMacd().ToDictionary(x => x.Date);
        var ema = quotes.GetEma(14).ToDictionary(x => x.Date);
        var atr = quotes.GetAtr(14).ToDictionary(x => x.Date);
        var bollinger = quotes.GetBollingerBands(20, 2).ToDictionary(x => x.Date);
        var stochRsi = quotes.GetStochRsi(14, 3, 3).ToDictionary(x => x.Date);
        var obv = quotes.GetObv().ToDictionary(x => x.Date);

        var result = new List<ModelInput>();

        foreach (var q in quotes)
        {
            if (!rsi.TryGetValue(q.Date, out var rsiVal) || rsiVal.Rsi is null)
                continue;

            if (!sma.TryGetValue(q.Date, out var smaVal) || smaVal.Sma is null)
                continue;

            if (!macd.TryGetValue(q.Date, out var macdVal) ||
                macdVal.Macd is null || macdVal.Signal is null || macdVal.Histogram is null)
                continue;

            if (!ema.TryGetValue(q.Date, out var emaVal) || emaVal.Ema is null)
                continue;

            if (!atr.TryGetValue(q.Date, out var atrVal) || atrVal.Atr is null)
                continue;

            if (!bollinger.TryGetValue(q.Date, out var bbVal) ||
                bbVal.UpperBand is null || bbVal.LowerBand is null || bbVal.Sma is null)
                continue;

            if (!stochRsi.TryGetValue(q.Date, out var stochRsiVal) ||
                stochRsiVal.StochRsi is null || stochRsiVal.Signal is null)
                continue;

            if (!obv.TryGetValue(q.Date, out var obvVal))
                continue;

            result.Add(new ModelInput
            {
                Open = (float)q.Open,
                High = (float)q.High,
                Low = (float)q.Low,
                Close = (float)q.Close,
                Volume = (float)q.Volume,

                Rsi = (float)rsiVal.Rsi.Value,
                Sma = (float)smaVal.Sma.Value,
                Macd = (float)macdVal.Macd.Value,
                MacdSignal = (float)macdVal.Signal.Value,
                MacdHistogram = (float)macdVal.Histogram.Value,

                Ema = (float)emaVal.Ema.Value,
                Atr = (float)atrVal.Atr.Value,
                BollingerUpper = (float)bbVal.UpperBand.Value,
                BollingerLower = (float)bbVal.LowerBand.Value,
                BollingerMiddle = (float)bbVal.Sma.Value,
                StochasticRsiK = (float)stochRsiVal.StochRsi.Value,
                StochasticRsiD = (float)stochRsiVal.StochRsi.Value,
                Obv = (float)obvVal.Obv
            });
        }

        return result;
    }
}