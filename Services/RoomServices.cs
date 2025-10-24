using Core.Enums;
using Core.MappingExtensions;
using Core.Repositories;
using Core.Services.Abstraction;
using Shared.Dtos.PlayerDtos;
using Shared.Dtos.RoomDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class RoomServices : IRoomServices
    {
        private readonly IRepositoryManager _repos;

        public RoomServices(IRepositoryManager repositoryManager) {
            _repos = repositoryManager;
        }
        public async Task<RoomDto> CreateRoom(CreateRoomRequestDto dto, CancellationToken ct = default)
        {
            var creator = await _repos.Player.GetAsync(p => p.Id == dto.CreatorId, ct: ct);
            var room = await _repos.Room.GetAsync(r => r.CreatorId == dto.CreatorId);
            if(room is not null)
            {
                await _repos.Room.DeleteAsync(room,ct);
            }
            room = new()
            {
                Key = GenerateGameKey(4),
                Creator = creator!,
                CreatorId = creator!.Id,
                WordLength = dto.WordLength,
                State = RoomStates.Created,
                MaxAttempts = dto.MaxAttempts,
            };
            creator.CreatedRoom = room;
            await _repos.Player.UpdateAsync(creator!, ct);
            return room.To<RoomDto>();
        }
        public async Task<ResponseToInvitationResultDto> JoinRoom(JoinRoomRequestDto dto, CancellationToken ct = default)
        {
            var room = await _repos.Room.GetAsync(r => r.Key == dto.GameKey && r.JoinerId == null && r.Creator != null, ct: ct);
            if (room is null || room.State != RoomStates.Created)
            {
                return new ResponseToInvitationResultDto(false, "Room Not Found or Already Joined", null, null, null);
            }
            var joiner = await _repos.Player.GetAsync(p => p.Id == dto.JoinerId, ct: ct);
            if (joiner is null)
            {
                return new ResponseToInvitationResultDto(false, "Joiner Player Not Found", null, null, null);
            }
            room.Joiner = joiner;
            room.JoinerId = joiner.Id;
            room.State = RoomStates.WaitingForWord;
            await _repos.Room.UpdateAsync(room, ct);
            return new ResponseToInvitationResultDto(true, "Join Success", room.Creator.To<PlayerDto>(), joiner.To<PlayerDto>(), room.To<RoomDto>());

        }

        public async Task<SubmitWordResultDto> SubmitWord(SelectWordRequestDto dto, CancellationToken ct = default)
        {
            var room = await _repos.Room.GetAsync(
                r => r.Key.ToLower() == dto.Roomkey.ToLower(),
                [
                    r=> r.Joiner,
                    r=> r.Creator,
                ],
                ct: ct);
            if (room is null || room.State != RoomStates.WaitingForWord)
            {
                return new SubmitWordResultDto(
                    false,
                    "Room Not Found or Invalid State",
                    room.Creator.ConnectionId,
                    room.Joiner.ConnectionId,
                    null);
            }
            if (room.WordLength != dto.Word.Length)
            {
                return new SubmitWordResultDto(
                    false,
                    "Word Length Mismatch",
                     room.Creator.ConnectionId,
                    room.Joiner.ConnectionId,
                    null);
            }
            if (room.CreatorId == dto.Id)
            {
                room.CreatorWord = dto.Word;
            }
            else if (room.JoinerId == dto.Id)
            {
                room.JoinerWord = dto.Word;
            }

            if (room.CreatorWord != null && room.JoinerWord != null)
            {
                room.State = RoomStates.InProgress;
                room.Creator.PlayedCount += 1;
                room.Joiner!.PlayedCount += 1;
                await _repos.Player.UpdateAsync(room.Creator);
                await _repos.Player.UpdateAsync(room.Joiner);
            }
            await _repos.Room.UpdateAsync(room, ct);
            return new(
                true,
                "Word Submitted Successfully",
                room.Creator.ConnectionId,
                room.Joiner.ConnectionId,
                room.To<RoomDto>());
        }

        public async Task<SubmitWordResultDto> SubmitWordGuess(SendGuessRequestDto dto, CancellationToken ct = default)
        {
            var room = await _repos.Room.GetAsync(
                r => r.Key.ToLower() == dto.RoomKey.ToLower(),
                [
                    r=> r.Joiner,
                    r=> r.Creator,
                ],
                ct: ct);
            if (room is null || room.State != RoomStates.InProgress)
            {
                return new SubmitWordResultDto(
                    false,
                    "Room Not Found or Invalid State",
                    room.Creator.ConnectionId,
                    room.Joiner.ConnectionId,
                    null);
            }
            if (room.CreatorId == dto.SenderId && room.JoinerWord == dto.Word)
            {
                // implement win logic for creator
                room.Creator.WinCount += 1;
                await _repos.Player.UpdateAsync(room.Creator);
            }
            else if (room.JoinerId == dto.SenderId && room.CreatorWord == dto.Word)
            {
                // implement win logic for joiner
                room.Joiner.WinCount += 1;
                await _repos.Player.UpdateAsync(room.Joiner);
            }
            return new(
                true,
                "Guess Submitted Successfully",
                room.Creator.ConnectionId,
                room.Joiner.ConnectionId, room.To<RoomDto>());
        }

        public async Task<string> LeaveGame(LeaveRoomRequestDto dto, CancellationToken ct = default)
        {
            var room = await _repos.Room.GetAsync(r => r.Key == dto.RoomKey,
                [
                    r=> r.Creator,
                    r=> r.Joiner,
                ],
                ct);
            if (room is null) return string.Empty;
            if (room.CreatorId == dto.PlayerId&& room.Joiner != null)
            {
                room.Joiner.WinCount += 1;
                await _repos.Player.UpdateAsync(room.Joiner!);
                await _repos.Room.DeleteAsync(room, ct);
                return room.Joiner.ConnectionId!;
            }
            else if (room.JoinerId == dto.PlayerId && room.Creator != null)
            {
                room.Creator.WinCount += 1;
                await _repos.Player.UpdateAsync(room.Creator);
                await _repos.Room.DeleteAsync(room, ct);
                return room.Creator.ConnectionId!;
            }
            await _repos.Room.DeleteAsync(room, ct);
            return string.Empty;


        }
        private string GenerateGameKey(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[len];
            rng.GetBytes(bytes);
            for (int i = 0; i < len; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }
    }
}
