using System.Collections.Generic;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PotsImporter
{
   public class NodeImporter
   {
      public static void Import(string file)
      {
         const int bufferSize = 10000;
         var nodes = new List<Node>(bufferSize);
         Node lastNode = null;
         using (var reader = new XmlTextReader(file))
         {
            while (reader.Read())
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  if (reader.Name == "node")
                  {
                     lastNode = Node.FromXml(reader);
                     nodes.Add(lastNode);
                  }
                  else if (reader.Name == "tag" && lastNode != null)
                  {
                     lastNode.AddTag(reader);
                  }
                  else
                  {
                     lastNode = null;
                  }

                  if (nodes.Count == bufferSize)
                  {
                     SaveNodes(nodes, "nid");
                     nodes.Clear();
                  }
               }
            }
         }
      }

      public static void SaveNodes(List<Node> nodes, string sourceName)
      {
         if (nodes == null || nodes.Count == 0) { return; }
         var collection = Store.NodeCollection();
         var documents = new BsonDocument[nodes.Count];

         for (var i = 0; i < nodes.Count; ++i)
         {
            var node = nodes[i];
            if (node.Latitude > 180 || node.Latitude < -180 || node.Longitude > 180 || node.Longitude < -180){ continue; }
            var document = new BsonDocument { { sourceName, node.SourceId }, { "loc", new BsonArray(new[] { node.Longitude, node.Latitude }) } };
            if (node.Tags.Count > 0)
            {
               var array = new BsonArray();
               foreach(var tag in node.Tags)
               {
                  array.Add(new BsonArray(new object[] {tag.Key, tag.Value}));
               }
               document.Add(new BsonElement("tags", array));
            }
            documents[i] = document;
         }
         collection.InsertBatch(documents, SafeMode.True);
      }
   }
}