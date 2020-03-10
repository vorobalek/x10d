using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using X10D.Mvc.Attributes;

namespace X10D.Mvc.Formats
{
    [Format("xml")]
    [Serializable]
    public sealed class XmlFormat : IApiResponse
    {
        [XmlElement(IsNullable = false, Order = 0, Type = typeof(bool))]
        public bool ok { get; set; }

        [XmlElement(IsNullable = true, Order = 1024, Type = typeof(object))]
        public object result { get; set; }

        [XmlElement(IsNullable = false, Order = 2048, Type = typeof(int))]
        public int status_code { get; set; }

        [XmlElement(IsNullable = true, Order = 4096, Type = typeof(string))]
        public string description { get; set; }

        [XmlElement(IsNullable = true, Order = int.MaxValue, Type = typeof(double?))]
        public double? request_time { get; set; }

        public StringBuilder Pack()
        {
            var serializer = new XmlSerializer(typeof(XmlFormat));

            var sw = new StringWriter();
            var tw = new XmlTextWriter(sw);

            serializer.Serialize(tw, this);

            sw.Close();
            tw.Close();

            return new StringBuilder(sw.ToString());
        }
    }
}
