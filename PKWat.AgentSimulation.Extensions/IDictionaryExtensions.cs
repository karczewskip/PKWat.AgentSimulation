namespace PKWat.AgentSimulation.Extensions;

public static class IDictionaryExtensions
{
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, Func<TKey, TValue, TValue> update)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = update(key, dictionary[key]);
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}