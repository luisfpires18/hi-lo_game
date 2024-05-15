namespace HiLoGame.Domain.Interfaces
{
    public interface IGameRoomRepository : IRepository<GameRoom>
    {
        GameRoom? GetRoomById(string roomId);

        bool TryAddPlayer(GameRoom room, Player player);
    }
}