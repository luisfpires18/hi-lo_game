namespace HiLoGame.UnitTests.Repositories
{
    using AutoFixture;
    using FluentAssertions;
    using HiLoGame.Domain;
    using HiLoGame.Infrastructure.Repositories;

    public class GameRoomRepositoryTests
    {
        public readonly GameRoomRepository gameRoomRepository;

        public readonly IFixture fixture;

        public GameRoomRepositoryTests()
        {
            this.gameRoomRepository = new GameRoomRepository();

            this.fixture = new Fixture();
        }

        public const string InvalidRoomId = "404-not-found";
        public const string ValidRoomId = "123-lp";
        public const string ValidRoomName = "RoomLp";

        public const string ValidPlayerId = "LP18";
        public const string ValidPlayerName = "LP";

        [Fact]
        public void GameRoomRepository_GetRoomById_ShouldReturnExistingRoom()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);

            this.gameRoomRepository.Insert(room);

            // Act
            var result = this.gameRoomRepository.GetRoomById(ValidRoomId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(ValidRoomId);
        }

        [Fact]
        public void GameRoomRepository_GetRoomById_ShouldReturnNullForNonexistentRoom()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);

            this.gameRoomRepository.Insert(room);

            // Act
            var result = this.gameRoomRepository.GetRoomById(InvalidRoomId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GameRoomRepository_TryAddPlayer_ShouldAddPlayerToRoomWithSpace()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);
            var player = new Player(ValidPlayerId, ValidPlayerName);

            // Act
            var result = this.gameRoomRepository.TryAddPlayer(room, player);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void GameRoomRepository_TryAddPlayer_ShouldNotAddPlayerToFullRoom()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);
            var player1 = new Player(ValidPlayerId, ValidPlayerName);
            var player2 = new Player(this.fixture.Create<string>(), this.fixture.Create<string>());
            room.Players.Add(player1);
            room.Players.Add(player2);

            var player3 = new Player(this.fixture.Create<string>(), this.fixture.Create<string>());

            // Act
            var result = this.gameRoomRepository.TryAddPlayer(room, player3);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void GameRoomRepository_TryAddPlayer_ShouldNotAddPlayerAlreadyInRoom()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);
            var player = new Player(ValidPlayerId, ValidPlayerName);
            room.Players.Add(player);

            // Act
            var result = this.gameRoomRepository.TryAddPlayer(room, player);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void GameRoomRepository_TryAddPlayer_ShouldSetPlayer2WhenRoomIsFull()
        {
            // Arrange
            var room = new GameRoom(ValidRoomId, ValidRoomName);
            var player1 = new Player(ValidPlayerId, ValidPlayerName);
            var player2 = new Player(this.fixture.Create<string>(), this.fixture.Create<string>());
            room.Players.Add(player1);

            // Act
            var result = this.gameRoomRepository.TryAddPlayer(room, player2);

            // Assert
            result.Should().Be(true);
            room.HiLoGame!.Player2.Should().Be(player2.Id);
        }
    }
}