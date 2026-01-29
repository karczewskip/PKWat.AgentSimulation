using System;
using System.Collections.Generic;
using System.Text;

namespace PKWat.AgentSimulation.NeuralNetwork;

public interface INeuralNetwork<INPUT, OUTPUT>
{
    void Learn(IReadOnlyCollection<(INPUT, OUTPUT)> trainingData);
    OUTPUT Predict(INPUT input);
}
