using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X10D.Infrastructure;
using X10D.Infrastructure.Attributes;

namespace X10D.Contest.Services
{
    internal sealed class SqlProvider : ServicePrototype<ISqlProvider>
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
        public override Func<IServiceProvider, object> CustomFactory => 
            (serviceProvider) =>
            {
                return serviceProvider.GetService<ISqlProvider<SqlProvider>>();
            };
    }

    [Service(InterfaceType = typeof(ISqlProvider<>), ServiceLifetime = ServiceLifetime.Transient)]
    internal sealed class SqlProvider<T> : ServicePrototype<ISqlProvider<T>>, ISqlProvider<T>
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
        private ILogger Logger { get; }
        private IConfiguration Configuration { get; }
        private NpgsqlConnection Connection { get; }
        private IDataBinder DataBinder { get; }

        public Type OwnerType => typeof(T);
        public SqlProvider(ILogger<ISqlProvider> logger, IConfiguration configuration, IDataBinder dataBinder)
        {
            UID = Guid.NewGuid();
            QueryCount = 0;
            IsClosed = true;

            Logger = logger;
            Configuration = configuration;
            var connectionString = Environment.GetEnvironmentVariable("Contest.ConnectionString")
                ?? Configuration.GetConnectionString(Configuration["Contest.ConnectionString"]);
            Connection = new NpgsqlConnection(connectionString);
            DataBinder = dataBinder;
        }

        public Guid UID { get; private set; }
        public ulong QueryCount { get; private set; }
        public bool IsClosed { get; private set; }

        public async Task OpenConnectionAsync()
        {
            try
            {
                await Connection.OpenAsync();
                Logger.LogInformation($"Подключение к базе открыто. ({OwnerType})");
                IsClosed = false;
            }
            catch (Exception ex)
            {
                Logger.LogCritical($"Произошла ошибка при подключении к базе. ({OwnerType}) {ex.Message}");
            }
        }

        public async Task CloseConnectionAsync()
        {
            try
            {
                IsClosed = true;
                await Connection.CloseAsync();
                Logger.LogInformation($"Подключение к базе закрыто. ({OwnerType})");
            }
            catch (Exception ex)
            {
                Logger.LogCritical($"Произошла ошибка при закрытии соединения с базой. ({OwnerType}) {ex.Message}");
            }
        }

        public async Task<NpgsqlDataReader> ExecuteDataReaderAsync(string query, NpgsqlParameter[] parameters = null)
        {
            return await Query(query, parameters, true);
        }

        public Task ExecuteQueryAsync(string query, NpgsqlParameter[] parameters = null)
        {
            return Query(query, parameters, false);
        }

        private async Task<NpgsqlDataReader> Query(string query, NpgsqlParameter[] parameters, bool executeDataReader)
        {
            NpgsqlDataReader sDr = null;
            ulong queryNumber = ++QueryCount;
            try
            {
                using var command = new NpgsqlCommand(query, Connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                Logger.LogTrace($"Отправлен запрос ({OwnerType}) #{queryNumber}: {query}");
                if (executeDataReader)
                {
                    sDr = await command.ExecuteReaderAsync();
                    Logger.LogTrace($"Получен ответ на запрос ({OwnerType}) #{queryNumber}: {query}");
                }
                else
                {
                    await command.ExecuteNonQueryAsync();
                    Logger.LogTrace($"Выполнен запрос ({OwnerType}) #{queryNumber}: {query}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка выполнения SQL запроса ({OwnerType}) #{queryNumber} {query} : {ex.Message}");
            }
            return sDr;
        }

        public async Task<IEnumerable<TModel>> ExecuteModel<TModel>(string query, NpgsqlParameter[] parameters = null) where TModel : class
        {
            var reader = await ExecuteDataReaderAsync(query, parameters);
            var result = new List<TModel>();
            while (await reader.ReadAsync())
            {
                result.Add(DataBinder.Bind<TModel>(reader));
            }
            return result;
        }
    }
}
