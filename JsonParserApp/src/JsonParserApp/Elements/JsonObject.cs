using System.Collections.Generic;
using System.Linq;

namespace JsonParserApp.Elements
{
    public sealed class JsonObject : IJsonElement
    {
        private readonly Dictionary<string, IJsonElement> _members = new();
        public void Put(string key, IJsonElement value) => _members[key] = value;
        public bool TryGet(string key, out IJsonElement value) => _members.TryGetValue(key, out value);
        public IReadOnlyDictionary<string, IJsonElement> Members => _members;
        public string ToJson() => "{" + string.Join(",", _members.Select(kv => $"\"{kv.Key}\":{kv.Value.ToJson()}")) + "}";
    }
}
