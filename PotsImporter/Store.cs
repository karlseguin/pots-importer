using MongoDB.Driver;

namespace PotsImporter
{
   public static class Store
   {
      private static MongoDatabase _database;

      public static void Intialize(string connectionString, string database)
      {
         _database = MongoServer.Create(connectionString).GetDatabase(database);
      }

      public static MongoCollection NodeCollection()
      {
         return GetCollection("nodes");
      }

      public static MongoCollection TagCollection()
      {
         return GetCollection("tags");
      }

      public static MongoCollection GetCollection(string name)
      {
         return _database.GetCollection(name);
      }

      public static MongoCollection<T> GetCollection<T>(string name)
      {
         return _database.GetCollection<T>(name);
      }

      public static void Reconnect()
      {
         _database.Server.Reconnect();
      }
   }
}