using System.IO;
using System.Xml.Serialization;
using Mapper.OSM;

namespace MapperTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //FileStream nodesFileStream = File.Open("../../map.osm", FileMode.Open);
            //FileStream nodesFileStream = File.Open("d:/CitiesSkylines/Osm/MsftMap.osm", FileMode.Open);
            FileStream nodesFileStream = File.Open("d:/CitiesSkylines/Osm/MsftAreaMap.osm", FileMode.Open);

            var nodesReader = new StreamReader(nodesFileStream);

            var serializer = new XmlSerializer(typeof(OsmDataResponse));
            var nodesOsm = (OsmDataResponse) serializer.Deserialize(nodesReader);

            System.Console.Write(nodesOsm);

            nodesReader.Dispose();
            System.Console.WriteLine("\nTest Completed");
            System.Console.ReadLine();
        }
    }
}
