using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System.Threading.Tasks;
using X10D.Infrastructure;

namespace X10D.Mvc.Formats
{
    internal sealed class DefaultFormatter : TextOutputFormatter
    {
        public DefaultFormatter()
        {
            SupportedMediaTypes.Add(Constants.DefaultFormatContentType);
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.UTF32);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            foreach (var prop in context.Object.GetType().GetProperties())
            {
                buffer.AppendLine($"{prop.Name}:\t{prop.GetValue(context.Object)}");
            }

            await response.WriteAsync(buffer.ToString());
        }
    }
}
