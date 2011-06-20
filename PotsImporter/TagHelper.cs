using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PotsImporter
{
   public static class TagHelper
   {
      private static MongoCollection _collection;
      private static readonly ICollection<string> _blacklist = new HashSet<string> { "converted_by", "created_by" };
      private static readonly IDictionary<string, int> _tags = new Dictionary<string, int>(1000);
      private static int _id;

      public static void Intialize()
      {
         _collection = Store.TagCollection();
         foreach (var found in _collection.FindAllAs<BsonDocument>())
         {
            var id = found["_id"].AsInt32;
            _tags.Add(found["name"].AsString, id);
            if (id > _id) { _id = id; }
         }
      }

      public static int? GetIndex(string name)
      {
         if (_blacklist.Contains(name)) { return null; }

         if (!_tags.ContainsKey(name))
         {
            ++_id;
            _collection.Insert(new BsonDocument {{"_id", _id}, {"name", name}});
            _tags.Add(name, _id);
         }
         return _tags[name];
      }
   }
}