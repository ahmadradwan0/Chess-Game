using Chess.TLDevProject.GameHeart.DataBaseModels;
using Chess.TLDevProject.GameHeart.GameEngine;
using Chess.TLDevProject.GameHeart.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Chess.TLDevProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] Match match)
        {
            var repo = new GameRepository.GameRepository();
            int matchId = await repo.InsertMatchAsync(match);

            return Ok(new
            {
                MatchId = matchId,
                Message = "Game started successfully.",
                StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            });
        }

        //======================================================================================

        [HttpPost("restart")]
        public async Task<IActionResult> RestartGame([FromBody] int matchId)
        {
            var repo = new GameRepository.GameRepository();
            bool success = await repo.ResetMatchAsync(matchId);

            if (!success)
                return BadRequest(new { Message = "Failed to restart the game." });

            return Ok(new
            {
                MatchId = matchId,
                Message = "Game restarted successfully.",
                StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            });
        }

        //======================================================================================

        //Get all users in the Data Base using the stored procedure in Game repo
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var repo = new GameRepository.GameRepository();
            var users = await repo.GetAllUsersAsync(); // implement this in your repository
            return Ok(users);
        }

        //======================================================================================

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var repo = new GameRepository.GameRepository();
            int newId = await repo.InsertUserAsync(user);

            return Ok(new
            {
                UserId = newId,
                Message = $"User '{user.UserName}' created successfully."
            });
        }

        //======================================================================================

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User user)
        {
            var repo = new GameRepository.GameRepository();
            int userId = await repo.GetUserByUSerNameAsync(user.UserName);

            if (userId <= 0)
            {
                return NotFound(new
                {
                    Message = $"User '{user.UserName}' not found. Please register first."
                });
            }

            return Ok(new
            {
                UserId = userId,
                Message = $"Welcome back, {user.UserName}!"
            });
        }

        //======================================================================================

        [HttpGet("matches/{userId}")]
        public async Task<IActionResult> GetUserMatches(int userId)
        {
            var repo = new GameRepository.GameRepository();
            var matches = await repo.GetMatchesByUserAsync(userId);

            return Ok(matches);
        }

        //======================================================================================

        [HttpGet("match/{matchId}")]
        public async Task<IActionResult> GetMatchLastFen(int matchId)
        {
            var repo = new GameRepository.GameRepository();
            string? fen = await repo.GetLastFenByMatchAsync(matchId);

            if (fen == null)
                return NotFound(new { Message = "No moves found for this match." });

            return Ok(new { MatchId = matchId, Fen = fen });
        }

        //======================================================================================

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromBody] GameRecord record)
        {
            var state = FENConverter.ExportFromFenStringToObj(record.FenString);
            var manager = new GameManager { GameState = state };

            var move = new MoveRecord(
                record.FromRow, record.FromCol, record.ToRow, record.ToCol,
                record.IsCapture, record.IsCastling, record.IsEnPassant,
                record.Promotion.HasValue ? (ChessPieceType?)record.Promotion.Value : null
            );

            bool success = manager.MakeAMove(move);
            if (!success)
                return BadRequest(new { Message = "Illegal move." });

            record.FenString = FENConverter.ExportFromObjToFenString(manager.GameState);



            record.MoveNumber = manager.GameState.MoveCounter;

            var repo = new GameRepository.GameRepository();
            int newId = await repo.InsertMoveAsync(record);

            GameStatusEvaluator.Evaluate(manager.GameState, out var status, out var result, out var message);

            if (result != ChessGameResult.None)
            {
                await repo.UpdateMatchResultAsync(record.MatchId, (byte)result, (byte)status);
            }
            return Ok(new
            {
                GameRecordId = newId,
                NewFen = record.FenString,
                Status = status.ToString(),
                Result = result.ToString(),
                Message = message
            });
        }

        //======================================================================================

        [HttpPost("undo")]
        public async Task<IActionResult> UndoMove([FromBody] GameRecord record)
        {
           
            var state = FENConverter.ExportFromFenStringToObj(record.FenString);

          
            var manager = new GameManager { GameState = state };

           
            bool undone = manager.UndoMove();
            if (!undone)
                return BadRequest(new { Message = "No moves to undo." });

            
            string newFen = FENConverter.ExportFromObjToFenString(manager.GameState);

           
            var repo = new GameRepository.GameRepository();
            await repo.DeleteLastMoveAsync(record.MatchId);

           
            return Ok(new
            {
                NewFen = newFen,
                Message = "Last move undone successfully",
                Status = "InProgress",
                Result = "None"
            });
        }

        //======================================================================================

        [HttpGet("match/{matchId}/records")]
        public async Task<IActionResult> GetMatchRecords(int matchId)
        {
            var repo = new GameRepository.GameRepository();
            var records = await repo.GetGameRecordsByMatchAsync(matchId);

            if (!records.Any())
                return NotFound(new { Message = "No records found for this match." });

            return Ok(records);
        }

        //======================================================================================


        [HttpGet("valid-moves")]
        public IActionResult GetValidMoves(string fen, int fromRow, int fromCol)
        {
            var state = FENConverter.ExportFromFenStringToObj(fen);
            var piece = state.Board[fromRow, fromCol];

            if (piece == null)
                return Ok(new List<object>());

            var allMoves = MoveGenerator.GenerateAllLegalMoves(state, state.SideToMove);

            var validMoves = allMoves
                .Where(m => m.FromRow == fromRow && m.FromCol == fromCol)
                .Select(m => new { m.ToRow, m.ToCol })
                .ToList();

            return Ok(validMoves);
        }

        //======================================================================================

    }
}