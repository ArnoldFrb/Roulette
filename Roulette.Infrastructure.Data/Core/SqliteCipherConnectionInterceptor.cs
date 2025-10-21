using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Roulette.Infrastructure.Data.Core
{
    public class SqliteCipherConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly string _password;

        public SqliteCipherConnectionInterceptor(string password)
        {
            _password = password;
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            if (!string.IsNullOrEmpty(_password))
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA key = '{_password}';";
                command.ExecuteNonQuery();
            }
            base.ConnectionOpened(connection, eventData);
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(_password))
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA key = '{_password}';";
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }
    }
}
