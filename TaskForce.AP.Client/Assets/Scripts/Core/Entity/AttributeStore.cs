using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class AttributeStore
    {
        private readonly Dictionary<string, Variant> _attributes;

        public AttributeStore()
        {
            _attributes = new Dictionary<string, Variant>();
        }

        public void Set(string id, Variant attribute)
        {
            _attributes[id] = attribute;
        }

        public Variant Get(string id)
        {
            if (_attributes.TryGetValue(id, out var value))
                return value;
            return new Variant();
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
