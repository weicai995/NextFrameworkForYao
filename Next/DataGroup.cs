using System.Collections.Generic;
using Newtonsoft.Json;

namespace NextFrameForYao.Next;

public class DataGroup<T>
{
    private const string DEFAULT_GROUP = "Default";

    [JsonProperty("DataDic")]
    public Dictionary<string, Dictionary<string, T>> DataDic { get; set; } = new Dictionary<string, Dictionary<string, T>>();

    public void Set(string key, T value) => this.Set("Default", key, value);

    public void Set(string group, string key, T value)
    {
        Dictionary<string, T> group1 = this.GetGroup(group);
        if ((object) value == null || value.Equals((object) null))
        {
            if (!group1.ContainsKey(key))
                return;
            group1.Remove(key);
        }
        else
            group1[key] = value;
    }

    public T Get(string key) => this.Get("Default", key);

    public T Get(string group, string key)
    {
        Dictionary<string, T> group1 = this.GetGroup(group);
        return group1.ContainsKey(key) ? group1[key] : default (T);
    }

    public void AddRange(Dictionary<string, T> dic)
    {
        Dictionary<string, T> defaultGroup = this.GetDefaultGroup();
        foreach (KeyValuePair<string, T> keyValuePair in dic)
            defaultGroup[keyValuePair.Key] = keyValuePair.Value;
    }

    public bool HasGroup(string group) => this.DataDic.ContainsKey(group);

    public Dictionary<string, T> GetDefaultGroup() => this.GetGroup("Default");

    public Dictionary<string, T> GetGroup(string group)
    {
        Dictionary<string, T> group1;
        if (!this.DataDic.TryGetValue(group, out group1))
        {
            group1 = new Dictionary<string, T>();
            this.DataDic[group] = group1;
        }
        return group1;
    }

    public bool Has(string key) => this.GetDefaultGroup().ContainsKey(key);

    public bool Has(string group, string key) => this.HasGroup(group) && this.GetGroup(group).ContainsKey(key);
}