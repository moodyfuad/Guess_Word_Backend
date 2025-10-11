using Guess_Word_Backend.Dtos;
using WordleServer.Dtos;

namespace Guess_Word_Backend.Services
{
    public interface IGameService
    {
        Task<CreateGameResponseDto> CreateGameRoomAsync(CreateGameRoomRequestDto requestDto, CancellationToken ct = default);
        Task<JoinGameResponseDto> JoinGameAsync(JoinGameRequestDto dto, CancellationToken ct = default);
        Task SendMessageAsync(string content,string reciverId, CancellationToken ct = default);
        //Task SubmitSecretAsync(SubmitSecretRequestDto dto, CancellationToken ct = default);
        //Task<GuessResultDto> SubmitGuessAsync(SubmitGuessRequestDto dto, CancellationToken ct = default);
        //Task<GameStateDto> GetGameStateAsync(string gameKey, CancellationToken ct = default);
    }
}
