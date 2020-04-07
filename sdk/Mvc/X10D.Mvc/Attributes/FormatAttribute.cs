using System;

namespace X10D.Mvc.Attributes
{
    public class FormatAttribute : Attribute
    {
        public string Name { get; set; }
        public string ContentType { get; set; }

        public FormatAttribute(string name) : this(name, "text/plain") { }

        public FormatAttribute(string name, string contentType)
        {
            Name = name;
            ContentType = contentType;
        }
    }
}
