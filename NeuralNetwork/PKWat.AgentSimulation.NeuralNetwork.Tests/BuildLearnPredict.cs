using System;
using System.Collections.Generic;
using System.Numerics.Tensors;
using System.Text;

namespace PKWat.AgentSimulation.NeuralNetwork.Tests;

[TestFixture]
public class BuildLearnPredict
{
    [Test]
    public void Test1()
    {
        var builder = new SimpleNeuralNetworkBuilder();
        var neuralNetwork = builder.Build<KartezianPoint, PredictionResult>();

        var slope = 2.0;
        var intercept = 1.0;

        var linearFunc = BuildLinearFunc(slope, intercept);

        var random = new Random(42);

        var trainingData = GenerateData(linearFunc, 1000, (-10, 10), random);

        neuralNetwork.Learn(trainingData);

        // Test predictions on points clearly above the line
        var pointAbove = new KartezianPoint(0, 10); // y=10, line at y=1, clearly above
        var predictionAbove = neuralNetwork.Predict(pointAbove);
        Assert.That(predictionAbove.IsAboveLine, Is.True, "Point (0, 10) should be classified as above the line");

        // Test predictions on points clearly below the line
        var pointBelow = new KartezianPoint(0, -10); // y=-10, line at y=1, clearly below
        var predictionBelow = neuralNetwork.Predict(pointBelow);
        Assert.That(predictionBelow.IsAboveLine, Is.False, "Point (0, -10) should be classified as below the line");

        // Test another point above
        var pointAbove2 = new KartezianPoint(5, 20); // y=20, line at y=2*5+1=11, above
        var predictionAbove2 = neuralNetwork.Predict(pointAbove2);
        Assert.That(predictionAbove2.IsAboveLine, Is.True, "Point (5, 20) should be classified as above the line");

        // Test another point below
        var pointBelow2 = new KartezianPoint(5, 5); // y=5, line at y=2*5+1=11, below
        var predictionBelow2 = neuralNetwork.Predict(pointBelow2);
        Assert.That(predictionBelow2.IsAboveLine, Is.False, "Point (5, 5) should be classified as below the line");
    }

    private record KartezianPoint(double X, double Y) : INeuralNetworkInput
    {
        public Tensor<float> ToTensor()
        {
            return Tensor.Create([(float)X, (float)Y]);
        }
    }

    private record PredictionResult(bool IsAboveLine) : INeuralNetworkOutput<PredictionResult>
    {
        public static PredictionResult FromTensor(Tensor<float> tensor)
        {
            var isAboveLine = tensor[0] > 0.5f;
            return new PredictionResult(isAboveLine);
        }

        public Tensor<float> ToTensor()
        {
            return Tensor.Create([IsAboveLine ? 1f : 0f]);
        }

        public static int RequiredTensorSize => 1;
    }

    private Func<float, float> BuildLinearFunc(double slope, double intercept)
    {
        return x => (float)(slope * x + intercept);
    }

    private IReadOnlyCollection<(KartezianPoint, PredictionResult)> GenerateData(
        Func<float, float> linearFunc,
        int numberOfPoints,
        (double Min, double Max) xRange,
        Random random)
    {
        var points = new List<(KartezianPoint, PredictionResult)>();
        for (int i = 0; i < numberOfPoints; i++)
        {
            var x = (float)(random.NextDouble() * (xRange.Max - xRange.Min) + xRange.Min);
            var y = linearFunc(x) + (float)(random.NextDouble() * 2 - 1); // Add noise
            var point = new KartezianPoint(x, y);
            var result = new PredictionResult(y > linearFunc(x) ? true : false);
            points.Add((point, result));
        }
        return points;
    }
}
