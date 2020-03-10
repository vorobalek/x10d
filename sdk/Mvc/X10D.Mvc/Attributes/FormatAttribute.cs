using System;

namespace X10D.Mvc.Attributes
{
    public class FormatAttribute : Attribute
    {
        public string Name { get; set; }

        public FormatAttribute(string name)
        {
            Name = name;
        }
    }
}
