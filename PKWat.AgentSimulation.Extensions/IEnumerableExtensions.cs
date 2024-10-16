namespace PKWat.AgentSimulation.Extensions;

public static class IEnumerableExtensions
{
    public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
    {
        return new Queue<T>(enumerable);
    }
}