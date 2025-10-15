using Chess.TLDevProject.GameHeart.DataBaseModels;
using Chess.TLDevProject.GameHeart.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Chess.TLDevProject.GameRepository
{
    public class GameRepository
    {
        private readonly string _connStr;

        public GameRepository()
        {
            // Build a configuration object to read appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connStr = builder.GetConnectionString("ChessDb")!;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Users_GetAllUsers", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }

            return users;
        }


        public async Task<int> InsertUserAsync(User user)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Users_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserName", user.UserName);

            await conn.OpenAsync();

            // Read the single scalar value returned by SELECT SCOPE_IDENTITY()
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> GetUserByUSerNameAsync(string userName)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Users_GetUserByUserName", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserName", userName);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result == null ? -1 : Convert.ToInt32(result);
        }


        public async Task<int> InsertMoveAsync(GameRecord record)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GameRecords_InsertMove", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MatchID", record.MatchId);
            cmd.Parameters.AddWithValue("@FromRow", record.FromRow);
            cmd.Parameters.AddWithValue("@FromCol", record.FromCol);
            cmd.Parameters.AddWithValue("@ToRow", record.ToRow);
            cmd.Parameters.AddWithValue("@ToCol", record.ToCol);
            cmd.Parameters.AddWithValue("@IsCapture", record.IsCapture);
            cmd.Parameters.AddWithValue("@IsCastling", record.IsCastling);
            cmd.Parameters.AddWithValue("@IsEnPassant", record.IsEnPassant);
            cmd.Parameters.AddWithValue("@Promotion", (object?)record.Promotion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MoveNumber", record.MoveNumber);
            cmd.Parameters.AddWithValue("@FenString", record.FenString);


            await conn.OpenAsync();
            

            return Convert.ToInt32(await cmd.ExecuteNonQueryAsync());
        }

        public async Task<int> InsertMatchAsync(Match match)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Matches_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", match.UserId);

            await conn.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<bool> ResetMatchAsync(int matchId)
        {
            using (var connection = new SqlConnection(_connStr))
            using (var command = new SqlCommand("Matches_MatchReset", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MatchId", matchId);

                await connection.OpenAsync();
                int rows = await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task UpdateMatchResultAsync(int matchId, byte result, byte status)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Matches_MatchUpdateResultOrStatus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MatchID", matchId);
            cmd.Parameters.AddWithValue("@Result", result);
            cmd.Parameters.AddWithValue("@Status", status);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteLastMoveAsync(int matchId)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GameRecords_DeleteLastMove", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MatchID", matchId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Match>> GetMatchesByUserAsync(int userId)
        {
            var list = new List<Match>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("Matches_GetByUserId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Match
                {
                    MatchId = reader.GetInt32(reader.GetOrdinal("MatchId")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Status = reader.GetByte(reader.GetOrdinal("Status")),
                    Result = reader.GetByte(reader.GetOrdinal("Result"))
                });
            }

            return list;
        }

        public async Task<string?> GetLastFenByMatchAsync(int matchId)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GameRecords_GetLastFenByMatchID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MatchID", matchId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        public async Task<List<GameRecord>> GetGameRecordsByMatchAsync(int matchId)
        {
            var records = new List<GameRecord>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GameRecords_GetGameRecordsByMatchId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MatchID", matchId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                records.Add(new GameRecord
                {
                    MatchId = reader.GetInt32(reader.GetOrdinal("MatchID")),
                    MoveNumber = reader.GetInt32(reader.GetOrdinal("MoveNumber")),
                    FromRow = reader.GetByte(reader.GetOrdinal("FromRow")),
                    FromCol = reader.GetByte(reader.GetOrdinal("FromCol")),
                    ToRow = reader.GetByte(reader.GetOrdinal("ToRow")),
                    ToCol = reader.GetByte(reader.GetOrdinal("ToCol")),
                    IsCapture = reader.GetBoolean(reader.GetOrdinal("IsCapture")),
                    IsCastling = reader.GetBoolean(reader.GetOrdinal("IsCastling")),
                    IsEnPassant = reader.GetBoolean(reader.GetOrdinal("IsEnPassant")),
                    Promotion = reader.IsDBNull(reader.GetOrdinal("Promotion"))
                        ? null
                        : null,
                    FenString = reader.GetString(reader.GetOrdinal("FenString"))
                });
            }

            return records;
        }
    }
}
