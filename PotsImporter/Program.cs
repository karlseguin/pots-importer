using System.IO;
using MongoDB.Driver.Builders;

namespace PotsImporter
{
   class Program
   {
      static void Main(string[] args)
      {
         Store.Intialize("mongodb://localhost", "pots");
         TagHelper.Intialize();
         Store.NodeCollection().EnsureIndex(IndexKeys.Ascending("nid"), IndexOptions.SetSparse(true));

         foreach (var file in Directory.GetFiles(args[0], "*.osm"))
         {
            System.Console.WriteLine("Processing {0}", file);
            NodeImporter.Import(file);
            WayImporter.Import(file);
            Exporter.ExportNodes(args[0] + Path.GetFileNameWithoutExtension(file) + ".json");
            Store.NodeCollection().RemoveAll();
            Store.Reconnect();
         }
         Exporter.ExportTags(args[0] + "tags.json");
      }
   }
}
