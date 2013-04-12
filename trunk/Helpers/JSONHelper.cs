using System;
using System.IO;
using System.Text;

namespace HTML5Compiler.Helpers
{
    /// <summary>
    /// Descripción breve de JSONHelper
    /// </summary>
    public static class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Default.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }

    }
}