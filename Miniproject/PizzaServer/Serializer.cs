using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PizzaServer
{
      public class Serializer
    {
        public Serializer()
        {
        }

        public void SerializeObject(string filename, object objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public object DeSerializeObject(string filename)
        {

            object objectToSerialize;

            Stream stream = File.Open(filename, FileMode.Open);
            if (stream.Length < 1)
            {
                stream.Close();
                return null;
            }
            else
            {
                BinaryFormatter b = new BinaryFormatter();
                stream.Position = 0;
                objectToSerialize = b.Deserialize(stream);
                stream.Close();
                return objectToSerialize;
            }
        }
    }
}
