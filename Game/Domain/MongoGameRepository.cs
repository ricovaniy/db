using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase database)
        {
            gameCollection = database.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(x => x.Id == gameId).SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(x => x.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection.Find(x => x.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var replaceResult = gameCollection.ReplaceOne(x => x.Id == game.Id && x.Status == GameStatus.WaitingToStart, game);
            return replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0;
        }
    }
}