using CryptoSignalTrainer;
using CryptoSignalTrainer.Data;
using CryptoSignalTrainer.Exporters;
using CryptoSignalTrainer.Features.PFI;

var candles = CandleLoader.LoadCandles("SOLUSDT_klines.csv");
var inputs = IndicatorCalculator.Calculate(candles);


//Sinyal üretmek..
//var labeled = new List<ModelInput>();
//var calculator = new SmartSignalCalculator(cooldownBars: 5);
//for (int i = 1; i < inputs.Count; i++)
//{
//    var signal = calculator.Calculate(inputs[i], inputs[i - 1], i);
//    inputs[i].SignalLabel = (int)signal;
//    Console.WriteLine($"{i}: {signal}");
//    labeled.Add(inputs[i]);

//}

CsvExporter.ExportLabeledData(inputs, "test.csv");
Console.WriteLine("başarıyla oluştu");

#region Backtest
string dataPath = "test.csv";
string modelPath = "traniner_model_v1.zip";
//ModelTrainer.TrainAdvanced(dataPath, modelPath);

FeatureImportanceCalculator.ComputePermutationFeatureImportance(modelPath, dataPath);
//Backtester.RunBacktestWithRiskManagement(modelPath, dataPath);
#endregion
