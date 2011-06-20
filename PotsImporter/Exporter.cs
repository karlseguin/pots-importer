using System.IO;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace PotsImporter
{
   public class Exporter
   {
      public static void ExportNodes(string file)
      {
         using (var writer = new StreamWriter(file, false))
         foreach(var document in Store.NodeCollection().FindAs<BsonDocument>(Query.Exists("tags", true)).SetFields("loc", "tags"))
         {
            var loc = document["loc"].AsBsonArray;
            var tags = document["tags"].AsBsonArray;
            writer.Write("{{loc:[{0},{1}],tags:{2}}}\n", loc[0].AsDouble, loc[1].AsDouble, tags.ToJson());
         }
      }

      public static void ExportTags(string file)
      {
         using (var writer = new StreamWriter(file, false))
         foreach (var document in Store.TagCollection().FindAllAs<BsonDocument>())
         {
            writer.Write("{{_id:{0},name:'{1}'}}\n", document["_id"].AsInt32, document["name"].AsString.Replace("'", "\\;"));
         }
      }
   }
}