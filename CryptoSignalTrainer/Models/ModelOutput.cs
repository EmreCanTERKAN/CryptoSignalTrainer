using Microsoft.ML.Data;

namespace CryptoSignalTrainer.Models;
public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public int PredictedLabel { get; set; }
}
