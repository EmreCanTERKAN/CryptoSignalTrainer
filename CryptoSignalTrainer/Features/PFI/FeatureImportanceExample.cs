using CryptoSignalTrainer.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CryptoSignalTrainer.Features.PFI;

public class FeatureImportanceCalculator
{
    public static void ComputePermutationFeatureImportance(string modelPath, string dataPath)
    {
        ITransformer trainedModel;
        DataViewSchema modelSchema;

        var mlContext = new MLContext();

        // 1. Modeli doğru türle yükle
        using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            trainedModel = mlContext.Model.Load(stream, out modelSchema);
        }

        // 2. Veriyi yükle
        var data = mlContext.Data.LoadFromTextFile<ModelInput>(dataPath, hasHeader: true, separatorChar: ',');

        // 3. Modeli veriye uygula
        var transformedData = trainedModel.Transform(data);

        // 5. PFI hesapla
        var pfiResults = mlContext.MulticlassClassification.PermutationFeatureImportance(
            trainedModel,
            transformedData,
            labelColumnName: "Label");



        var slotNames = new VBuffer<ReadOnlyMemory<char>>();
        transformedData.Schema["Features"].GetSlotNames(ref slotNames);
        var featureNames = slotNames.DenseValues().Select(v => v.ToString()).ToArray();

        var results = new List<(string FeatureName, double Importance)>();

        int i = 0;
        foreach (var kvp in pfiResults.OrderBy(k => k.Key))
        {
            var importance = kvp.Value.MacroAccuracy.Mean;
            var featureName = i < featureNames.Length ? featureNames[i] : kvp.Key;
            results.Add((featureName, importance));
            i++;
        }

        foreach (var result in results.OrderByDescending(r => Math.Abs(r.Importance)))
        {
            Console.WriteLine($"{result.FeatureName} => {result.Importance:F4}");
        }
    }
}
