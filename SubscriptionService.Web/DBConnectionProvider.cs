using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionService.Web
{
    public interface IDBConnectionProvider
    {
        Task<IDbConnection> CreateDBConnection();
    }

    public class DBConnectionProvider: IDBConnectionProvider
    {
        private readonly string _connectionString;
        public DBConnectionProvider(IOptions<DBConnectionStrings> dBConnectionStrings)
        {
            _connectionString = dBConnectionStrings.Value.UserDb;
        }

        public string ConnectionString { get; }

        public async Task<IDbConnection> CreateDBConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            return connection;
        }
    }
}
