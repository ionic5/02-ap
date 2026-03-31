using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class AttributeStore
    {
        private readonly Dictionary<string, Attribute> _attributes;

        public AttributeStore()
        {
            _attributes = new Dictionary<string, Attribute>();
        }

        public void Set(string id, Attribute attribute)
        {
            _attributes[id] = attribute;
        }

        public Attribute Get(string id)
        {
            if (_attributes.TryGetValue(id, out var value))
                return value;
            return new Attribute();
        }

        public void Clear()
        {
            _attributes.Clear();
        }

        public void CopyTo(AttributeStore other)
        {
            foreach (var entry in _attributes)
                other.Set(entry.Key, entry.Value);
        }
    }
}
