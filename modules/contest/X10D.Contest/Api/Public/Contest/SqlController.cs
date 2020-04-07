using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X10D.Contest.Services;
using X10D.Infrastructure;

namespace X10D.Contest.Api.Public.Contest
{
    public class SqlController : ContestApiControllerBase
    {
        public SqlController(IActivator activator) : base(activator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(string query)
        {
            var sqlProvider = Activator.GetServiceOrCreateInstance<ISqlProvider<SqlController>>();

            var result = new List<IEnumerable<object>>();
            await sqlProvider.OpenConnectionAsync();
            var reader = await sqlProvider.ExecuteDataReaderAsync(query);

            var schema = reader.GetColumnSchema();
            result.Add(schema.Select(s => s.ColumnName));

            while (await reader.ReadAsync())
            {
                var line = new List<object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    line.Add(reader[i]);
                }
                result.Add(line);
            }
            await sqlProvider.CloseConnectionAsync();

            return Ok(result);
        }
    }
}
