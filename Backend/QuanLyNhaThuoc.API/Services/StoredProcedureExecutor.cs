using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyNhaThuoc.API.Data;

namespace QuanLyNhaThuoc.API.Services;

public sealed class StoredProcedureExecutor
{
    private readonly ApplicationDbContext _context;

    public StoredProcedureExecutor(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExecuteAndGetIdAsync(
        string procedureName,
        string idColumn,
        params ProcedureParameter[] parameters)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = _context.Database.CurrentTransaction!.GetDbTransaction();

            foreach (var parameter in parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Name;
                dbParameter.DbType = parameter.Type;
                dbParameter.Size = parameter.Size;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }

            string createdId;
            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                    throw new InvalidOperationException(
                        $"Stored procedure '{procedureName}' returned no created record.");

                int idOrdinal;
                try
                {
                    idOrdinal = reader.GetOrdinal(idColumn);
                }
                catch (IndexOutOfRangeException exception)
                {
                    throw new InvalidOperationException(
                        $"Stored procedure '{procedureName}' did not return the expected '{idColumn}' column.",
                        exception);
                }

                if (await reader.IsDBNullAsync(idOrdinal))
                    throw new InvalidOperationException(
                        $"Stored procedure '{procedureName}' returned a null '{idColumn}'.");

                createdId = reader.GetString(idOrdinal);
            }

            await transaction.CommitAsync();
            return createdId;
        }
        catch (SqlException exception) when (exception.Number >= 50000)
        {
            throw new BusinessRuleException(exception.Message, exception);
        }
    }
}

public sealed record ProcedureParameter(string Name, DbType Type, object? Value, int Size = 0);
