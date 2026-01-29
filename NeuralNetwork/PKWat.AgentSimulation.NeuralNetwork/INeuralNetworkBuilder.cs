using System.Numerics.Tensors;

namespace PKWat.AgentSimulation.NeuralNetwork;

public interface INeuralNetworkBuilder
{
    INeuralNetwork<INPUT, OUTPUT> Build<INPUT, OUTPUT>() 
        where INPUT : INeuralNetworkInput 
        where OUTPUT : INeuralNetworkOutput<OUTPUT>;
}
