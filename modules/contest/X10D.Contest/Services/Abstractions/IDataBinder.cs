using Npgsql;

namespace X10D.Contest.Services
{
    internal interface IDataBinder
    {
        TModel Bind<TModel>(NpgsqlDataReader reader) where TModel : class;
    }
}