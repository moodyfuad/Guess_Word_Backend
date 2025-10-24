namespace Shared.Dtos.PlayerDtos
{
    public class GetOnlinePlayersRequestDto 
    {
        public GetOnlinePlayersRequestDto(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
