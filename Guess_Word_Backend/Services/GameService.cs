using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WordleServer.Dtos;
using WordleServer.Hubs;
using WordleServer.Models;
using WordleServer.Repositories;

namespace WordleServer.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repo;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly ILogger<GameService> _logger;

        public GameService(IGameRepository repo, IHubContext<GameHub, IGameClient> hubContext, ILogger<GameService> logger)
        {
            _repo = repo;
            _hubContext = hubContext;
            _logger = logger;
        }

        private static string GenerateGameKey()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[6];
            rng.GetBytes(bytes);
            for (int i = 0; i < 6; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }

        // Hashing secret with per-player salt (kept but not used for evaluation in demo)
        private static (string hash, string salt) HashSecret(string secret)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);
            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(secret + salt);
            var hashBytes = sha.ComputeHash(combined);
            return (Convert.ToBase64String(hashBytes), salt);
        }

        public async Task<CreateGameResponseDto> CreateGameAsync(CancellationToken ct = default)
        {
            var gameKey = GenerateGameKey();

            var game = new Game
            {
                GameKey = gameKey,
                WordLength = 5,
                MaxAttempts = 6,
                Phase = GamePhase.WaitingForPlayers,
            };

            await _repo.AddAsync(game, ct);
            await _repo.SaveChangesAsync(ct);

            return new CreateGameResponseDto
            {
                GameKey = game.GameKey,
                WordLength = game.WordLength,
                MaxAttempts = game.MaxAttempts
            };
        }

        public async Task<JoinGameResponseDto> JoinGameAsync(JoinGameRequestDto dto, CancellationToken ct = default)
        {
            var game = await _repo.GetByKeyAsync(dto.GameKey, ct);
            if (game == null)
                return new JoinGameResponseDto { Success = false, Message = "Game not found" };

            if (game.Players.Count >= 2)
                return new JoinGameResponseDto { Success = false, Message = "Game is full" };

            var playerIndex = game.Players.Count;

            var player = new Player
            {
                GameId = game.Id,
                ClientId = dto.ClientId,
                DisplayName = dto.DisplayName ?? $"Player{playerIndex + 1}",
                SecretHash = string.Empty,
                SecretSalt = string.Empty,
                HasSubmittedSecret = false,
                PlainSecret = null
            };

            game.Players.Add(player);

            if (game.Players.Count == 2)
            {
                game.Phase = GamePhase.WaitingForSecrets;
            }

            await _repo.UpdateAsync(game, ct);
            await _repo.SaveChangesAsync(ct);

            // Notify group (whoever is connected)
            await _hubContext.Clients.Group(game.GameKey).ReceiveGameStateAsync(await BuildGameStateDto(game));

            return new JoinGameResponseDto
            {
                Success = true,
                Message = "Joined",
                PlayerIndex = playerIndex
            };
        }

        public async Task SubmitSecretAsync(SubmitSecretRequestDto dto, CancellationToken ct = default)
        {
            var game = await _repo.GetByKeyAsync(dto.GameKey, ct);
            if (game == null) throw new InvalidOperationException("Game not found");
            if (game.Phase != GamePhase.WaitingForSecrets) throw new InvalidOperationException("Game not in secret submission phase");

            var player = game.Players.FirstOrDefault(p => p.ClientId == dto.ClientId);
            if (player == null) throw new InvalidOperationException("Player not found in game");

            if (dto.Secret.Length != game.WordLength)
                throw new InvalidOperationException($"Secret must be {game.WordLength} letters long");

            // *** DEMO: store plaintext secret ***
            player.PlainSecret = dto.Secret;
            // also keep hash for compatibility (not used further here)
            var (hash, salt) = HashSecret(dto.Secret);
            player.SecretHash = hash;
            player.SecretSalt = salt;
            player.HasSubmittedSecret = true;

            if (game.Players.All(p => p.HasSubmittedSecret))
            {
                game.Phase = GamePhase.InProgress;
                game.CurrentTurn = new Random().Next(0, 2);
            }

            await _repo.UpdateAsync(game, ct);
            await _repo.SaveChangesAsync(ct);

            await _hubContext.Clients.Group(game.GameKey).ReceiveGameStateAsync(await BuildGameStateDto(game));
        }

        public async Task<GuessResultDto> SubmitGuessAsync(SubmitGuessRequestDto dto, CancellationToken ct = default)
        {
            var game = await _repo.GetByKeyAsync(dto.GameKey, ct);
            if (game == null) throw new InvalidOperationException("Game not found");
            if (game.Phase != GamePhase.InProgress) throw new InvalidOperationException("Game not in progress");

            var playerIndex = game.Players.FindIndex(p => p.ClientId == dto.ClientId);
            if (playerIndex < 0) throw new InvalidOperationException("Player not part of game");

            if (game.CurrentTurn != playerIndex) throw new InvalidOperationException("Not your turn");

            if (dto.Guess.Length != game.WordLength) throw new InvalidOperationException($"Guess must be {game.WordLength} letters");

            var opponentIndex = 1 - playerIndex;
            var opponent = game.Players.ElementAtOrDefault(opponentIndex);
            if (opponent == null) throw new InvalidOperationException("Opponent not found");
            if (!opponent.HasSubmittedSecret || string.IsNullOrEmpty(opponent.PlainSecret))
                throw new InvalidOperationException("Opponent hasn't set secret yet");

            // Normalize inputs if needed (for Arabic). For demo we assume inputs are normalized on client.
            var secret = opponent.PlainSecret!;
            var guess = dto.Guess;

            var feedback = EvaluateGuess(guess, secret);

            // Persist and broadcast
            await PersistAndBroadcastGuessAsync(game, playerIndex, guess, feedback, ct);

            // Build and return GuessResultDto to the REST caller
            var resultDto = new GuessResultDto
            {
                Guess = guess,
                Feedback = feedback,
                PlayerIndex = playerIndex,
                IsWinningGuess = feedback.All(f => f == LetterState.Correct)
            };

            return resultDto;
        }

        public async Task<GameStateDto> GetGameStateAsync(string gameKey, CancellationToken ct = default)
        {
            var game = await _repo.GetByKeyAsync(gameKey, ct);
            if (game == null) throw new InvalidOperationException("Game not found");
            return await BuildGameStateDto(game);
        }

        private Task<GameStateDto> BuildGameStateDto(Game game)
        {
            var dto = new GameStateDto
            {
                Phase = game.Phase,
                CurrentTurn = game.CurrentTurn,
                WordLength = game.WordLength,
                MaxAttempts = game.MaxAttempts,
                Players = game.Players.Select(p => p.DisplayName ?? p.ClientId).ToList(),
                Guesses = game.Guesses
                    .OrderBy(g => g.CreatedAt)
                    .Select(g => new GuessResultDto
                    {
                        Guess = g.Word,
                        PlayerIndex = g.PlayerIndex,
                        Feedback = string.IsNullOrEmpty(g.FeedbackCsv)
                            ? new List<LetterState>()
                            : g.FeedbackCsv.Split(',').Select(s => Enum.Parse<LetterState>(s)).ToList(),
                        IsWinningGuess = !string.IsNullOrEmpty(g.FeedbackCsv)
                            && g.FeedbackCsv.Split(',').All(x => x == LetterState.Correct.ToString())
                    }).ToList()
            };

            return Task.FromResult(dto);
        }

        // Wordle evaluation logic (standard)
        private List<LetterState> EvaluateGuess(string guess, string secret)
        {
            var result = new List<LetterState>(new LetterState[guess.Length]);
            var secretCounts = new Dictionary<char, int>();

            for (int i = 0; i < secret.Length; i++)
            {
                var sch = secret[i];
                if (i < guess.Length && guess[i] == sch)
                {
                    result[i] = LetterState.Correct;
                }
                else
                {
                    secretCounts[sch] = secretCounts.TryGetValue(sch, out var c) ? c + 1 : 1;
                }
            }

            for (int i = 0; i < guess.Length; i++)
            {
                if (result[i] == LetterState.Correct) continue;
                var ch = guess[i];
                if (secretCounts.TryGetValue(ch, out var cnt) && cnt > 0)
                {
                    result[i] = LetterState.Present;
                    secretCounts[ch] = cnt - 1;
                }
                else
                {
                    result[i] = LetterState.Absent;
                }
            }

            return result;
        }

        private async Task PersistAndBroadcastGuessAsync(Game game, int playerIndex, string guess, List<LetterState> feedback, CancellationToken ct = default)
        {
            var feedbackCsv = string.Join(',', feedback.Select(f => f.ToString()));
            var g = new Guess
            {
                GameId = game.Id,
                PlayerIndex = playerIndex,
                Word = guess,
                FeedbackCsv = feedbackCsv,
                CreatedAt = DateTime.UtcNow
            };

            game.Guesses.Add(g);

            if (feedback.All(f => f == LetterState.Correct))
            {
                game.Phase = GamePhase.Finished;
            }
            else
            {
                game.CurrentTurn = 1 - playerIndex;
            }

            await _repo.UpdateAsync(game, ct);
            await _repo.SaveChangesAsync(ct);

            var resultDto = new GuessResultDto
            {
                Guess = guess,
                Feedback = feedback,
                PlayerIndex = playerIndex,
                IsWinningGuess = feedback.All(f => f == LetterState.Correct)
            };

            await _hubContext.Clients.Group(game.GameKey).ReceiveGuessResultAsync(resultDto);
            await _hubContext.Clients.Group(game.GameKey).ReceiveGameStateAsync(await BuildGameStateDto(game));
        }
    }
}
