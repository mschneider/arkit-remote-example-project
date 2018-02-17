using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Utils
{
    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class ObjectSerializationExtension
    {

        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var jsonString = JsonUtility.ToJson(obj);
            var serializedObj = Encoding.Default.GetBytes(jsonString);

            File.WriteAllBytes(Application.persistentDataPath + "/serialize.bin", serializedObj);

            Debug.LogFormat("{0} path={1} type={2}", "Serialize", Application.persistentDataPath, obj.GetType());

            return serializedObj;
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }

            File.WriteAllBytes(Application.persistentDataPath + "/deserialize.bin", byteArray);

            Debug.LogFormat("{0} path={1} type={2}", "Deserialize", Application.persistentDataPath, typeof(T));

            return JsonUtility.FromJson<T>(Encoding.Default.GetString(byteArray));
        }
    }
}
