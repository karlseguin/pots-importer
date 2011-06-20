using System.Collections.Generic;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Linq;

namespace PotsImporter
{
   public class WayImporter
   {
      public static void Import(string file)
      {
         const int bufferSize = 10000;
         var ways = new List<Way>(bufferSize);
         Way lastWay = null;
         using (var reader = new XmlTextReader(file))
         {
            while (reader.Read())
            {
               if (reader.NodeType == XmlNodeType.Element)
               {
                  if (reader.Name == "way")
                  {
                     lastWay = Way.FromXml(reader);
                     ways.Add(lastWay);
                  }
                  else if (reader.Name == "nd" && lastWay != null)
                  {
                     lastWay.AddNode(reader);
                  }
                  else if (reader.Name == "tag" && lastWay != null)
                  {
                     lastWay.AddTag(reader);
                  }
                  else
                  {
                     lastWay = null;
                  }

                  if (ways.Count == bufferSize)
                  {
                     SaveWays(ways);
                     ways.Clear();
                  }
               }
            }
         }
      }

      private static void SaveWays(List<Way> ways)
      {
         var nodes = new List<Node>(ways.Count);
         var collection = Store.NodeCollection();
         foreach(var way in ways)
         {
            if (way.Tags.Count == 0)
            {
               continue;
            }
            var points = new List<Point>(way.NodeIds.Count);
            foreach (var node in collection.FindAs<BsonDocument>(Query.In("nid", way.NodeIds.Select(n => new BsonInt32(n)).ToArray())))
            {
               var loc = node["loc"].AsBsonArray;
               points.Add(new Point(loc[0].AsDouble, loc[1].AsDouble));
            }
            if (points.Count == 0)
            {
               continue;
            }
            var center = ComputeCentroid(points);
            if (double.IsNaN(center.X) || double.IsInfinity(center.X) || center.X > 180 || center.X < -180)
            {
               center.X = points[0].X;
            }
            if (double.IsNaN(center.Y) || double.IsInfinity(center.Y) || center.Y > 180 || center.Y < -180)
            {
               center.Y = points[0].Y;
            }
            nodes.Add(new Node
            {
               SourceId = way.Id,
               Tags = way.Tags,
               Longitude = center.X,
               Latitude = center.Y,
            });
         }
         NodeImporter.SaveNodes(nodes, "wid");
      }


      //from http://www.koders.com/csharp/fidFE1500191D74D1E5E79D0115683208202CED09B9.aspx?s=mdef%3Acompute
      public static Point ComputeCentroid(IList<Point> points) 
      {
         if (points.Count == 1)
         {
            return points[0];
         }
         if (points.Count == 2)
         {
            return new Point((points[0].X + points[1].X)/2, (points[0].Y + points[1].Y)/2);
         }
         var vertices = points.Count / 2;

         var area = 0.0;
         double centroidX = 0.0, centroidY = 0.0;

         for (var i = 0; i < vertices; ++i) 
         {
            int k;
            if (i < vertices - 1) { k = i + 1; } 
            else { k = 0; }

            var temp = (points[i].X * points[k].Y) - (points[k].X * points[i].Y);
            area += temp;

            centroidX += (points[k].X + points[i].X) * temp;
            centroidY += (points[k].Y + points[i].Y) * temp;
         }
         return new Point(centroidX / (area * 3.0), centroidY / (area * 3.0));
      }
   }
}