﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X10D.Contest.Services
{
    internal interface ISqlProvider
    {
        Guid UID { get; }
        ulong QueryCount { get; }
        bool IsClosed { get; }
        Task OpenConnectionAsync();
        Task CloseConnectionAsync();
        Task<NpgsqlDataReader> ExecuteDataReaderAsync(string query, NpgsqlParameter[] parameters = null);
        Task ExecuteQueryAsync(string query, NpgsqlParameter[] parameters = null);
        Task<IEnumerable<TModel>> ExecuteModel<TModel>(string query, NpgsqlParameter[] parameters = null) where TModel : class;
    }

    internal interface ISqlProvider<T> : ISqlProvider
    {
        Type OwnerType { get; }
    }
}