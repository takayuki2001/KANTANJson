using System.Collections.Generic;
using System.Text;

namespace KANTANJson
{
    static class JsonParser
    {
        static public string KeyParse(string value)
        {
            string firstChar = value.Substring(0, 1);
            if (firstChar == "\"")
            {
                return value.Substring(1, value.Length - 2);
            }

            return value;
        }
        static public object ValueParse(string value)
        {

            if(value == "")
            {
                return null;
            }

            string firstChar = value.Substring(0, 1);
            if(firstChar == "\"")
            {
                return value.Substring(1, value.Length - 2);
            }
            else if(firstChar == "{")
            {
                ObjectJson oj = new ObjectJson();
                StringBuilder sb = new StringBuilder(value);

                JsonParser.ObjectParse(ref oj, ref sb);

                return oj;
            }
            else if (firstChar == "[")
            {
                ArrayJson aj = new ArrayJson();
                StringBuilder sb = new StringBuilder(value);

                JsonParser.ArryParse(ref aj, ref sb);

                return aj;
            }
            else
            {
                if(value.ToLower() == "true")
                {
                    return true;
                }
                else if(value.ToLower() == "false")
                {
                    return false;
                }
                else if (value.ToLower() == "null")
                {
                    return null;
                }
                else
                {

                    return double.Parse(value);
                    
                }
            }
        }

        static public void ObjectParse(ref ObjectJson objectJson, ref StringBuilder jsonSB)
        {
            int startIndex = 1;
            bool isStr = false;
            int nest = 0;
            string key = "";
            for (int i = startIndex; i < jsonSB.Length; i++)
            {
                if (!isStr)
                {
                    if (jsonSB[i] == ':' && nest == 0)
                    {
                        key = jsonSB.ToString(startIndex, i - startIndex);
                        startIndex = i + 1;
                    }

                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    if (jsonSB[i] == '[' || jsonSB[i] == '{')
                    {
                        nest++;
                    }

                    if (jsonSB[i] == ']' || jsonSB[i] == '}')
                    {
                        nest--;
                        if (nest < 0)
                        {
                            objectJson.Add(JsonParser.KeyParse(key), JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        }
                    }

                    if (nest == 0 && jsonSB[i] == ',')
                    {
                        objectJson.Add(JsonParser.KeyParse(key), JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        startIndex = i + 1;
                    }
                }
                else
                {
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        isStr = false;
                    }
                }
            }
        }
        static public void ArryParse(ref ArrayJson arrayJson, ref StringBuilder jsonSB)
        {
            int startIndex = 1;
            bool isStr = false;
            int nest = 0;
            for (int i = startIndex; i < jsonSB.Length; i++)
            {
                if (!isStr)
                {

                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    if (jsonSB[i] == '[' || jsonSB[i] == '{')
                    {
                        nest++;
                    }

                    if (jsonSB[i] == ']' || jsonSB[i] == '}')
                    {
                        nest--;
                        if (nest < 0)
                        {
                            arrayJson.Add(JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        }
                    }

                    if (nest == 0 && jsonSB[i] == ',')
                    {
                        arrayJson.Add(JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        startIndex = i + 1;
                    }
                }
                else
                {
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        isStr = false;
                    }
                }
            }
        }

        static public void SpaceRemover(ref StringBuilder jsonSB)
        {
            bool isStr = false;
            int rmLen = 0;
            for (int i = 0; i < jsonSB.Length; i++)
            {
                if (!isStr)
                {
                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    if (jsonSB[i] == ' ' || jsonSB[i] == '\n' || jsonSB[i] == '\r' || jsonSB[i] == '\t')
                    {
                        rmLen++;
                    }
                    else if(rmLen > 0)
                    {
                        i = i - rmLen;
                        jsonSB = jsonSB.Remove(i, rmLen);
                        rmLen = 0;
                    }

                }
                else
                {
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        isStr = false;
                    }
                }
            }
        }
    }
    public class ArrayJson : BaseJson
    {
        List<object> _Elements = new List<object>();
        public List<object> ObjectList { get { return _Elements; } }
        public object this[int index] { get { return _Elements[index]; } }
        public int Count { get { return _Elements.Count; } }
        
        public void Add(object o)
        {
            _Elements.Add(o);
        }

        public ObjectJson GetObjectJson(int index)
        {
            if (typeof(ObjectJson) == _Elements[index].GetType())
            {
                return (ObjectJson)_Elements[index];
            }

            return null;
        }

        public ArrayJson GetArrayJson(int index)
        {
            if (typeof(ArrayJson) == _Elements[index].GetType())
            {
                return (ArrayJson)_Elements[index];
            }

            return null;
        }

    }

    public class ObjectJson : BaseJson
    {
        List<string> _KeyList = new List<string>();
        Dictionary<string, object>_Dictionary = new Dictionary<string, object>();
        public Dictionary<string, object> ObjectDictionary { get { return _Dictionary; } }
        public Dictionary<string, object>.KeyCollection Keys { get { return _Dictionary.Keys; } }
        public List<string> KeyList { get { return _KeyList; } }

        public Dictionary<string , object>.ValueCollection Values { get { return _Dictionary.Values; } }
        public int Count { get { return _Dictionary.Count; } }
        public object this[string key] { get { return _Dictionary[key];} }
        public object this[int index] { get { return _Dictionary[_KeyList[index]]; } }
        public void Add(string key, object value)
        {
            _KeyList.Add(key);
            _Dictionary.Add(key, value);
        }

        public string GetKeyByIndex(int i)
        {
            return _KeyList[i];
        }

        public ObjectJson GetObjectJson(int index)
        {
            if (typeof(ObjectJson) == _Dictionary[_KeyList[index]].GetType())
            {
                return (ObjectJson)_Dictionary[_KeyList[index]];
            }

            return null;
        }

        public ArrayJson GetArrayJson(int index)
        {
            if (typeof(ArrayJson) == _Dictionary[_KeyList[index]].GetType())
            {
                return (ArrayJson)_Dictionary[_KeyList[index]];
            }

            return null;
        }

        public ObjectJson GetObjectJson(string key)
        {
            if (typeof(ObjectJson) == _Dictionary[key].GetType())
            {
                return (ObjectJson)_Dictionary[key];
            }

            return null;
        }

        public ArrayJson GetArrayJson(string key)
        {
            if (typeof(ArrayJson) == _Dictionary[key].GetType())
            {
                return (ArrayJson)_Dictionary[key];
            }

            return null;
        }
    }

    public class BaseJson
    {
        
    }

    public class RootJson
    {
        protected StringBuilder _JsonStr = null;
        public string JsonStr { get { return _JsonStr.ToString(); } }
        public BaseJson Content = null;
        protected RootJson()
        {

        }

        private void DeserializeJson(ref string jsonStr)
        {
            _JsonStr = new StringBuilder(jsonStr);
            JsonParser.SpaceRemover(ref _JsonStr);

            int startIndex = 0;

            if (_JsonStr[startIndex] == '[')
            {
                ArrayJson aj = new ArrayJson();

                JsonParser.ArryParse(ref aj,ref _JsonStr);

                Content = aj;
            }
            else
            {
                ObjectJson oj = new ObjectJson();

                JsonParser.ObjectParse(ref oj, ref _JsonStr);

                Content = oj;
            }

        }
        static public RootJson Deserialize(string jsonStr)
        {
            RootJson bj = new RootJson();
            bj.DeserializeJson(ref jsonStr);

            return bj;
        }
    }
}
