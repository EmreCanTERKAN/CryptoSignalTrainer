//var candles = CandleLoader.LoadCandles("candles.csv");
//var inputs = IndicatorCalculator.Calculate(candles);

//var labeled = new List<ModelInput>();

//for (int i = 1; i < inputs.Count; i++)
//{
//    var current = inputs[i];
//    var previous = inputs[i - 1];

//    current.SignalLabel = SignalCalculator.CalculateSignal(current, previous);
//    labeled.Add(current);
//}

//CsvExporter.ExportLabeledData(labeled, "labeled_data.csv");
//Console.WriteLine("Labeled data exported to labeled_data.csv");

string dataPath = "labeled_data.csv";
string modelPath = "trainedModel_oneversusall.zip";
Backtester.RunBacktestWithRiskManagement(modelPath, dataPath);
