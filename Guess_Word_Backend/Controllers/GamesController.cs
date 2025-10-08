using Microsoft.AspNetCore.Mvc;
using WordleServer.Dtos;
using WordleServer.Services;

namespace WordleServer.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _service;
        private readonly ILogger<GamesController> _logger;

        public GamesController(IGameService service, ILogger<GamesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateGameResponseDto>> CreateGame(CancellationToken ct)
        {
            var dto = await _service.CreateGameAsync(ct);
            return CreatedAtAction(nameof(GetGameState), new { gameKey = dto.GameKey }, dto);
        }

        [HttpPost("join")]
        public async Task<ActionResult<JoinGameResponseDto>> Join([FromBody] JoinGameRequestDto dto, CancellationToken ct)
        {
            var result = await _service.JoinGameAsync(dto, ct);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("secret")]
        public async Task<IActionResult> SubmitSecret([FromBody] SubmitSecretRequestDto dto, CancellationToken ct)
        {
            try
            {
                await _service.SubmitSecretAsync(dto, ct);
                return Ok(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("guess")]
        public async Task<IActionResult> SubmitGuess([FromBody] SubmitGuessRequestDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _service.SubmitGuessAsync(dto, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (NotImplementedException ex)
            {
                return StatusCode(501, new { success = false, message = "Server evaluation not implemented: " + ex.Message });
            }
        }

        [HttpGet("{gameKey}")]
        public async Task<IActionResult> GetGameState(string gameKey, CancellationToken ct)
        {
            try
            {
                var state = await _service.GetGameStateAsync(gameKey, ct);
                return Ok(state);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}
