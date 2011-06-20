using System.Collections.Generic;
using System.Xml;

namespace PotsImporter
{
   public class Way
   {
      public int Id { get; set; }
      public IList<int> NodeIds { get; set; }
      public IDictionary<int, string> Tags { get; set; }

      public static Way FromXml(XmlTextReader reader)
      {
         return new Way
         {
            Id = int.Parse(reader.GetAttribute("id")),
            NodeIds = new List<int>(5),
            Tags = new Dictionary<int, string>(5),
         };
      }

      public void AddNode(XmlTextReader reader)
      {
         NodeIds.Add(int.Parse(reader.GetAttribute("ref")));
      }

      public void AddTag(XmlTextReader reader)
      {
         var id = TagHelper.GetIndex(reader.GetAttribute("k"));
         if (id == null) { return; }
         Tags.Add(id.Value, reader.GetAttribute("v"));
      }
   }
}