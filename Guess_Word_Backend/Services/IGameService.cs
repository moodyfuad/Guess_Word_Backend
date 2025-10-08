using WordleServer.Dtos;

namespace WordleServer.Services
{
    public interface IGameService
    {
        Task<CreateGameResponseDto> CreateGameAsync(CancellationToken ct = default);
        Task<JoinGameResponseDto> JoinGameAsync(JoinGameRequestDto dto, CancellationToken ct = default);
        Task SubmitSecretAsync(SubmitSecretRequestDto dto, CancellationToken ct = default);
        Task<GuessResultDto> SubmitGuessAsync(SubmitGuessRequestDto dto, CancellationToken ct = default);
        Task<GameStateDto> GetGameStateAsync(string gameKey, CancellationToken ct = default);
    }
}
