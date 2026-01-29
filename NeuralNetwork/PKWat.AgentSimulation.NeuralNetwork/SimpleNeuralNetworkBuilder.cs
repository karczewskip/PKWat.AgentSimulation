using Microsoft.ML;
using Microsoft.ML.Data;
using System.Numerics.Tensors;

namespace PKWat.AgentSimulation.NeuralNetwork;

internal class SimpleNeuralNetworkBuilder : INeuralNetworkBuilder
{
    public INeuralNetwork<INPUT, OUTPUT> Build<INPUT, OUTPUT>() 
        where INPUT : INeuralNetworkInput 
        where OUTPUT : INeuralNetworkOutput<OUTPUT>
    {
        return new SimpleNeuralNetwork<INPUT, OUTPUT>();
    }
}

internal class SimpleNeuralNetwork<INPUT, OUTPUT> : INeuralNetwork<INPUT, OUTPUT>
        where INPUT : INeuralNetworkInput
        where OUTPUT : INeuralNetworkOutput<OUTPUT>
{
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private int _featureSize;

    public SimpleNeuralNetwork()
    {
        _mlContext = new MLContext();
    }

    public void Learn(IReadOnlyCollection<(INPUT, OUTPUT)> trainingData)
    {
        // Determine the feature vector size from the first item
        var firstItem = trainingData.First();
        _featureSize = firstItem.Item1.ToTensor().ToArray().Length;

        var transformedData = trainingData.Select(item => new TrainingDataRow
        {
            Features = item.Item1.ToTensor().ToArray(),
            Label = item.Item2.ToTensor().ToArray()[0]
        });

        // Create schema definition with explicit vector size
        var schemaDefinition = SchemaDefinition.Create(typeof(TrainingDataRow));
        schemaDefinition["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, _featureSize);

        var dataView = _mlContext.Data.LoadFromEnumerable(transformedData, schemaDefinition);

        var pipeline = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");

        _model = pipeline.Fit(dataView);
    }

    private class TrainingDataRow
    {
        public float[] Features { get; set; } = Array.Empty<float>();
        public float Label { get; set; }
    }

    private class PredictionInput
    {
        public float[] Features { get; set; } = Array.Empty<float>();
    }

    private class PredictionOutput
    {
        public float Score { get; set; }
    }

    public OUTPUT Predict(INPUT input)
    {
        if (_model == null)
        {
            throw new InvalidOperationException("The model has not been trained yet.");
        }

        // Create schema definition for prediction input with the same feature size as training
        var inputSchemaDefinition = SchemaDefinition.Create(typeof(PredictionInput));
        inputSchemaDefinition["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, _featureSize);

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<PredictionInput, PredictionOutput>(
            _model, 
            inputSchemaDefinition: inputSchemaDefinition);

        var inputData = new PredictionInput
        {
            Features = input.ToTensor().ToArray()
        };
        var prediction = predictionEngine.Predict(inputData);
        var outputTensor = Tensor.Create([prediction.Score]);
        return OUTPUT.FromTensor(outputTensor);
    }
}

public interface INeuralNetworkInput
{
    Tensor<float> ToTensor();
}

public interface INeuralNetworkOutput<OUTPUT>
{
    Tensor<float> ToTensor();
    static abstract OUTPUT FromTensor(Tensor<float> tensor);
    static abstract int RequiredTensorSize { get; }
}
