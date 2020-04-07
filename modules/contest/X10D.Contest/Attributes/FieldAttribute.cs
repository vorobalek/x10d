using System;

namespace X10D.Contest.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public FieldAttribute() { }
        public FieldAttribute(string name)
        {
            Name = name;
        }
    }
}
