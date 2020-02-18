using System.ComponentModel.DataAnnotations;

namespace X10D.Core.Data
{
    internal sealed class StoredCacheString
    {
        internal StoredCacheString()
        {
        }

        internal StoredCacheString(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [Key]
        internal string Key { get; set; }

        internal string Value { get; set; }
    }
}