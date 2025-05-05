using CryptoSignalTrainer.Models;
using Microsoft.ML;

namespace CryptoSignalTrainer;
public class ModelTrainer
{
    public static void TrainAdvanced(string dataPath, string modelPath)
    {
        var context = new MLContext();


        // 1. Veri Yükle
        var data = context.Data.LoadFromTextFile<ModelInput>(dataPath, hasHeader: true, separatorChar: ',');

        var preview = context.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: false);
        var classCounts = preview.GroupBy(p => p.SignalLabel).Select(g => new { g.Key, Count = g.Count() });
        foreach (var c in classCounts)
            Console.WriteLine($"Label {c.Key}: {c.Count} adet");

        // 2. Veri Bölümle (Walk-Forward Validation)
        var trainTestSplit = context.Data.TrainTestSplit(data, testFraction: 0.2);

        // 3. Özellik Mühendisliği
        var pipeline = context.Transforms.Concatenate("Features",
                nameof(ModelInput.Open),
                nameof(ModelInput.High),
                nameof(ModelInput.Low),
                nameof(ModelInput.Close),
                nameof(ModelInput.Volume),
                nameof(ModelInput.Rsi),
                nameof(ModelInput.Sma),
                nameof(ModelInput.Macd),
                nameof(ModelInput.MacdSignal),
                nameof(ModelInput.MacdHistogram),
                nameof(ModelInput.Ema),
                nameof(ModelInput.Atr),
                nameof(ModelInput.BollingerUpper),
                nameof(ModelInput.BollingerLower),
                nameof(ModelInput.BollingerMiddle),
                nameof(ModelInput.StochasticRsiK),
                nameof(ModelInput.StochasticRsiD),
                nameof(ModelInput.Obv))
            .Append(context.Transforms.NormalizeMinMax("Features")) // Normalizasyon
            .Append(context.Transforms.Conversion.MapValueToKey("Label", nameof(ModelInput.SignalLabel)));

        // 4. Model Seçimi 
        #region LightGbm
        //var trainer = context.MulticlassClassification.Trainers.LightGbm(
        //    labelColumnName: "Label",
        //    featureColumnName: "Features",
        //    numberOfLeaves: 20,                       
        //    learningRate: 0.1,
        //    numberOfIterations: 100);
        #endregion
        #region Sdca
        //var trainer = context.MulticlassClassification.Trainers.SdcaMaximumEntropy(
        //    labelColumnName: "Label",
        //    featureColumnName: "Features");
        #endregion
        #region OneVersusAll
        var trainer = context.MulticlassClassification.Trainers.OneVersusAll(
            context.BinaryClassification.Trainers.FastTree(), labelColumnName: "Label");
        #endregion
        // 5. Pipeline Tamamla
        var trainingPipeline = pipeline.Append(trainer)
            .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // 6. Model Eğit
        var model = trainingPipeline.Fit(trainTestSplit.TrainSet);

        // 7. Tahmin ve Değerlendirme
        var predictions = model.Transform(trainTestSplit.TestSet);
        var metrics = context.MulticlassClassification.Evaluate(predictions);

        Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:F2}");
        Console.WriteLine($"LogLoss: {metrics.LogLoss:F2}");

        // 8. Model Kaydet
        context.Model.Save(model, data.Schema, modelPath);
    }
}
