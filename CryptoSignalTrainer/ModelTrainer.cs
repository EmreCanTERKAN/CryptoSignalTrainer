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

        // Tüm verideki sınıf dağılımını yaz
        var allDataPreview = context.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: false).ToList();
        var allClassCounts = allDataPreview.GroupBy(p => p.SignalLabel).Select(g => new { g.Key, Count = g.Count() }).ToList();

        Console.WriteLine("Tüm verideki sınıf dağılımı:");
        foreach (var c in allClassCounts)
            Console.WriteLine($"Label {c.Key}: {c.Count} adet");

        if (allClassCounts.Count < 2)
        {
            Console.WriteLine("⚠ Hata: Çok sınıflı model eğitmek için en az 2 farklı sınıf gerekir.");
            return;
        }

        // 2. Veri Böl (Train-Test)
        var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

        // Eğitim verisinde sınıf dağılımını kontrol et
        var trainPreview = context.Data.CreateEnumerable<ModelInput>(split.TrainSet, reuseRowObject: false).ToList();
        var trainClasses = trainPreview.GroupBy(p => p.SignalLabel).Select(g => g.Key).ToList();

        if (trainClasses.Count < 2)
        {
            Console.WriteLine("⚠ Hata: Eğitim verisinde yalnızca tek bir sınıf var. Model eğitilemez.");
            return;
        }

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
            .Append(context.Transforms.NormalizeMinMax("Features"))
            .Append(context.Transforms.Conversion.MapValueToKey("Label", nameof(ModelInput.SignalLabel)));

        // 4. Model Seçimi
        var trainer = context.MulticlassClassification.Trainers.LightGbm(
            labelColumnName: "Label",
            featureColumnName: "Features",
            numberOfLeaves: 20,
            learningRate: 0.1,
            numberOfIterations: 100);

        // 5. Pipeline Tamamla
        var trainingPipeline = pipeline
            .Append(trainer)
            .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // 6. Model Eğit
        var model = trainingPipeline.Fit(split.TrainSet);

        // 7. model

        // 8. Tahmin ve Değerlendirme
        var predictions = model.Transform(split.TestSet);
        var metrics = context.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label", predictedLabelColumnName: "PredictedLabel");

        Console.WriteLine($"\nMacroAccuracy: {metrics.MacroAccuracy:F2}");
        Console.WriteLine($"LogLoss: {metrics.LogLoss:F2}");

        // 9. Modeli Kaydet
        context.Model.Save(model, data.Schema, modelPath);
        Console.WriteLine("✅ Model başarıyla kaydedildi.");
    }
}
