using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Utilities
{
    public static class Serialization
    {
        public static BinaryFormatter formatter;

        public static void Serialize(Stream serializationStream, object graph)
        {
            formatter = new BinaryFormatter();

            formatter.Serialize(serializationStream, graph);
        }

        public static object Deserialize(Stream serializationStream)
        {
            formatter = new BinaryFormatter();
            object graph = formatter.Deserialize(serializationStream);

            return graph;
        }

        public static T Deserialize<T>(Stream serializationStream)
        {
            return (T)Deserialize(serializationStream);
        }
    }
}
