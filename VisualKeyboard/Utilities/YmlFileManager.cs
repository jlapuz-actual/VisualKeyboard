namespace VisualKeyboard.Utilities
{
    using System.IO;
    using VisualKeyboard.Models;
    using YamlDotNet.Serialization;

    class YmlFileManager
    {
        private readonly ISerializer serializer;
        private readonly IDeserializer deserializer;

        public YmlFileManager()
        {
            serializer = new SerializerBuilder().Build();
            deserializer = new DeserializerBuilder().Build();
        }
        public string GetYML(object graph)
        {
            return serializer.Serialize(graph);
        }
        public object GetObject(string ymlString)
        {
            object returnObject;
            try
            {
                using (var stringReader = new StringReader(ymlString))
                {
                    returnObject = deserializer.Deserialize<GridModel>(stringReader);
                }

            }
            catch (YamlDotNet.Core.YamlException e)
            {
                throw e;
            }

            return returnObject;
        }
    }
}
