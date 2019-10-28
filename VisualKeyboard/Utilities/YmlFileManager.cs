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
            var stringReader = new StringReader(ymlString);
            var returnObject = deserializer.Deserialize<GridModel>(stringReader);

            return returnObject;
        }
    }
}
