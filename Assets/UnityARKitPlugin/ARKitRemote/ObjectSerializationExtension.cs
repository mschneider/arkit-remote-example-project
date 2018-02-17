using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
            using (var ms1 = new MemoryStream())
            {            
                var jsonString = JsonUtility.ToJson(obj);
                var serializedObj = System.Text.Encoding.Default.GetBytes(jsonString);

                File.WriteAllBytes(Application.persistentDataPath + "/serialize.bin", serializedObj);
                Debug.LogFormat("{0} path={1} type={2}", "Serialize", Application.persistentDataPath, obj.GetType());

                return serializedObj;

                #if False

                var bf = new DataContractJsonSerializer(obj.GetType());
                bf.Serialize(ms1, obj);
                bf.WriteObject(ms1,  obj);
                var serializedObj = ms1.ToArray();

                if (MatchesDesiredHeader(serializedObj))
                {
                    Debug.LogFormat("Header matches on {0} {1}", "Serialize", obj.GetType());   
                    return serializedObj;
                }
                else
                {
                    using (var ms2 = new MemoryStream())
                    {
                        ms2.Write(desiredHeader, 0, desiredHeader.Length);
                        ms2.Write(serializedObj, skippedInvalidHeaderLength, serializedObj.Length - skippedInvalidHeaderLength);
                        ms2.Seek(0, SeekOrigin.Begin);

                        if (MatchesDesiredHeader(ms2.ToArray()))
                        {
                            Debug.LogFormat("Header matches on {0} {1}", "HeaderReplacement", obj.GetType());
                        }

                        return ms2.ToArray();
                    }
                }
                #endif
            }
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }

            File.WriteAllBytes(Application.persistentDataPath + "/deserialize.bin", byteArray);

            Debug.LogFormat("{0} path={1} type={2}", "Deserialize", Application.persistentDataPath, typeof(T));

            return JsonUtility.FromJson<T>(System.Text.Encoding.Default.GetString(byteArray));

            #if False

            if (MatchesDesiredHeader(byteArray))
            {
                Debug.LogFormat("Header matches on {0} {1}", "Deserialize", typeof(T));

                using (var memStream = new MemoryStream())
                {
                    memStream.Write(desiredHeader, 0, desiredHeader.Length);
                    memStream.Write(byteArray, skippedInvalidHeaderLength, byteArray.Length - skippedInvalidHeaderLength);
                    memStream.Seek(0, SeekOrigin.Begin);
    
                    if (MatchesDesiredHeader(memStream.ToArray()))
                    {
                        Debug.LogFormat("Header matches on {0} {1}", "HeaderReplacement", typeof(T));
                    }

                    var binForm = new BinaryFormatter();
                    var obj = (T)binForm.Deserialize(memStream);
                    return obj;
                }
            }
            else // if not MatchesDesiredHeader(byteArray)
            {
                using (var memStream = new MemoryStream())
                {
                    memStream.Write(byteArray, 0, byteArray.Length);
                    memStream.Seek(0, SeekOrigin.Begin);

                    var binForm = new BinaryFormatter();
                    var obj = (T)binForm.Deserialize(memStream);
                    return obj;
                }
            }
            #endif
        }


        private static readonly byte[] desiredHeader =
            {
                0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x0C, 0x02, 0x00, 0x00, 0x00, 0x46, 0x41, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x2D, 
                0x43, 0x53, 0x68, 0x61, 0x72, 0x70, 0x2C, 0x20, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 
                0x30, 0x2E, 0x30, 0x2E, 0x30, 0x2E, 0x30, 0x2C, 0x20, 0x43, 0x75, 0x6C, 0x74, 0x75, 0x72, 0x65, 
                0x3D, 0x6E, 0x65, 0x75, 0x74, 0x72, 0x61, 0x6C, 0x2C, 0x20, 0x50, 0x75, 0x62, 0x6C, 0x69, 0x63, 
                0x4B, 0x65, 0x79, 0x54, 0x6F, 0x6B, 0x65, 0x6E, 0x3D, 0x6E, 0x75, 0x6C, 0x6C, 0x05, 0x01, 0x00,
                0x00, 0x00, 0x23
            };

        private const int skippedInvalidHeaderLength = 92;

        private static bool MatchesDesiredHeader(byte[] byteArray)
        {
            for (var i = 0; i < desiredHeader.Length; i++)
            {
                if (desiredHeader[i] == byteArray[i])
                    continue;
                else
                {
                    return false;
                }

            }

            return true;
        }
    }
}