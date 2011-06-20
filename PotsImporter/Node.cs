using System.Collections.Generic;
using System.Xml;

namespace PotsImporter
{
   public class Node
   {
      public int SourceId { get; set; }
      public double Longitude { get; set; }
      public double Latitude { get; set; }
      public IDictionary<int, string> Tags { get; set; }

      public static Node FromXml(XmlTextReader reader)
      {
         return new Node
                {
                   SourceId = int.Parse(reader.GetAttribute("id")),
                   Longitude = double.Parse(reader.GetAttribute("lon")),
                   Latitude = double.Parse(reader.GetAttribute("lat")),
                   Tags = new Dictionary<int, string>(5),
                };
      }

      public void AddTag(XmlTextReader reader)
      {
         var id = TagHelper.GetIndex(reader.GetAttribute("k"));
         if (id == null) { return; }
         Tags.Add(id.Value, reader.GetAttribute("v"));
      }
   }
}