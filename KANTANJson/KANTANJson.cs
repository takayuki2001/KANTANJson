using System.Collections.Generic;
using System.Text;

namespace KANTANJson
{
    static class JsonParser
    {
        /// <summary>
        /// "key" -> key にする。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string KeyParse(string value)
        {
            string firstChar = value.Substring(0, 1);
            if (firstChar == "\"")
            {
                return value.Substring(1, value.Length - 2);
            }

            return value;
        }

        /// <summary>
        /// "str" -> str
        /// {"key":"val"} -> ObjectJson
        /// [a, b, c] -> ArrayJson
        /// s_true -> b_true
        /// s_false -> b_false
        /// s_null-> null
        /// s_1.0-> d_1.0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ObjectJsonに要素を適切に追加
        /// </summary>
        /// <param name="objectJson"></param>
        /// <param name="jsonSB"></param>
        static public void ObjectParse(ref ObjectJson objectJson, ref StringBuilder jsonSB)
        {
            int startIndex = 1;
            bool isStr = false;
            int nest = 0;
            string key = "";
            for (int i = startIndex; i < jsonSB.Length; i++)
            {
                //今見ている文字は"xxxx"のxではないのであれば
                if (!isStr)
                {
                    //key:value の:であればkeyを代入
                    if (jsonSB[i] == ':' && nest == 0)
                    {
                        key = jsonSB.ToString(startIndex, i - startIndex);
                        startIndex = i + 1;
                    }

                    //入りの"であれば"の中である事を代入
                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    //入りの{ or [ であれば入れ後なのでネストを深くする
                    if (jsonSB[i] == '[' || jsonSB[i] == '{')
                    {
                        nest++;
                    }

                    //ネストを浅くする
                    if (jsonSB[i] == ']' || jsonSB[i] == '}')
                    {
                        nest--;
                        //ネストが0を下回るのは最後の｝に到達したという事なのでKeyValueを追加
                        if (nest < 0)
                        {
                            objectJson.Add(JsonParser.KeyParse(key), JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        }
                    }

                    //ネストが0で,で区切られているのは次のKeyValueに行くという事なのでKeyValueを追加
                    if (nest == 0 && jsonSB[i] == ',')
                    {
                        objectJson.Add(JsonParser.KeyParse(key), JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        startIndex = i + 1;
                    }
                }
                else
                {
                    //\"のエスケープ処理
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        //文字列ここで終わり
                        isStr = false;
                    }
                }
            }
        }

        /// <summary>
        /// ArrayJsonに要素を追加
        /// </summary>
        /// <param name="arrayJson"></param>
        /// <param name="jsonSB"></param>
        static public void ArryParse(ref ArrayJson arrayJson, ref StringBuilder jsonSB)
        {
            int startIndex = 1;
            bool isStr = false;
            int nest = 0;
            for (int i = startIndex; i < jsonSB.Length; i++)
            {
                //今見ている文字は"xxxx"のxではないのであれば
                if (!isStr)
                {
                    //入りの"であれば"の中である事を代入
                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    //入りの{ or [ であれば入れ後なのでネストを深くする
                    if (jsonSB[i] == '[' || jsonSB[i] == '{')
                    {
                        nest++;
                    }

                    //ネストを浅くする
                    if (jsonSB[i] == ']' || jsonSB[i] == '}')
                    {
                        nest--;
                        //ネストが0を下回るのは最後の｝に到達したという事なのでKeyValueを追加
                        if (nest < 0)
                        {
                            arrayJson.Add(JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        }
                    }

                    //ネストが0で,で区切られているのは次のKeyValueに行くという事なのでKeyValueを追加
                    if (nest == 0 && jsonSB[i] == ',')
                    {
                        arrayJson.Add(JsonParser.ValueParse(jsonSB.ToString(startIndex, i - startIndex)));
                        startIndex = i + 1;
                    }
                }
                else
                {
                    //\"のエスケープ処理
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        //文字列ここで終わり
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
                //今見ている文字は"xxxx"のxではないのであれば
                if (!isStr)
                {
                    //入りの"であれば"の中である事を代入
                    if (jsonSB[i] == '"')
                    {
                        isStr = true;
                    }

                    //Jsonの空白・改行・タブで有れば何文字消すかカウントを増やす
                    if (jsonSB[i] == ' ' || jsonSB[i] == '\n' || jsonSB[i] == '\r' || jsonSB[i] == '\t')
                    {
                        rmLen++;
                    }
                    else if(rmLen > 0)
                    {
                        //消す文字が有れば消す
                        i = i - rmLen;
                        jsonSB = jsonSB.Remove(i, rmLen);
                        rmLen = 0;
                    }

                }
                else
                {
                    //\"のエスケープ処理
                    if (jsonSB[i] == '\\' && jsonSB[i + 1] == '"')
                    {
                        i++;
                    }
                    else if (jsonSB[i] == '"')
                    {
                        //文字列ここで終わり
                        isStr = false;
                    }
                }
            }
        }
    }


    public class ArrayJson : BaseJson
    {
        //Valueを格納するリスト
        List<object> _Elements = new List<object>();

        //リストのゲッタ
        public List<object> ObjectList { get { return _Elements; } }

        //indexを使ったゲッタ
        public object this[int index] { get { return _Elements[index]; } }

        //要素数のゲッタ
        public int Count { get { return _Elements.Count; } }
        
        public void Add(object o)
        {
            _Elements.Add(o);
        }

        //ObjectJsonに変換した上で取得
        public ObjectJson GetObjectJson(int index)
        {
            if (typeof(ObjectJson) == _Elements[index].GetType())
            {
                return (ObjectJson)_Elements[index];
            }

            return null;
        }

        //ArrayJsonに変換した上で取得
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
        //IndexでKeyを取るためのKeyList
        List<string> _KeyList = new List<string>();
        
        //KeyValue本体のDic
        Dictionary<string, object>_Dictionary = new Dictionary<string, object>();

        //Dicのゲッタ
        public Dictionary<string, object> ObjectDictionary { get { return _Dictionary; } }

        //DicのKey一覧のゲッタ
        public Dictionary<string, object>.KeyCollection Keys { get { return _Dictionary.Keys; } }

        //KeyListのゲッタ
        public List<string> KeyList { get { return _KeyList; } }

        //DicのValuesのゲッタ
        public Dictionary<string , object>.ValueCollection Values { get { return _Dictionary.Values; } }

        //Dicの要素数
        public int Count { get { return _Dictionary.Count; } }

        //DicをKeyで取得
        public object this[string key] { get { return _Dictionary[key];} }

        //Dicをindexで取得
        public object this[int index] { get { return _Dictionary[_KeyList[index]]; } }


        public void Add(string key, object value)
        {
            //KeyListとDicに同時保存
            _KeyList.Add(key);
            _Dictionary.Add(key, value);
        }

        public string GetKeyByIndex(int i)
        {
            return _KeyList[i];
        }

        //ObjectJsonに変換した上で取得（Index版）
        public ObjectJson GetObjectJson(int index)
        {
            if (typeof(ObjectJson) == _Dictionary[_KeyList[index]].GetType())
            {
                return (ObjectJson)_Dictionary[_KeyList[index]];
            }

            return null;
        }

        //ArrayJsonに変換した上で取得（Index版）
        public ArrayJson GetArrayJson(int index)
        {
            if (typeof(ArrayJson) == _Dictionary[_KeyList[index]].GetType())
            {
                return (ArrayJson)_Dictionary[_KeyList[index]];
            }

            return null;
        }

        //ObjectJsonに変換した上で取得（Key版）
        public ObjectJson GetObjectJson(string key)
        {
            if (typeof(ObjectJson) == _Dictionary[key].GetType())
            {
                return (ObjectJson)_Dictionary[key];
            }

            return null;
        }

        //ArrayJsonに変換した上で取得（Key版）
        public ArrayJson GetArrayJson(string key)
        {
            if (typeof(ArrayJson) == _Dictionary[key].GetType())
            {
                return (ArrayJson)_Dictionary[key];
            }

            return null;
        }
    }

    //ArrayJsonとObjectJsonを同じように扱う為の継承されるだけクラス
    public class BaseJson
    {
        
    }

    /// <summary>
    /// パースとかするクラス
    /// </summary>
    public class RootJson
    {
        protected StringBuilder _JsonStr = null;

        //JsonStrのゲッタ
        public string JsonStr { get { return _JsonStr.ToString(); } }
        public BaseJson Content = null;

        //勝手なインスタンス生成は許さない
        protected RootJson()
        {

        }

        //コンストラクタから呼び出されるInitializeメソッド
        private void DeserializeJson(ref string jsonStr)
        {
            _JsonStr = new StringBuilder(jsonStr);
            JsonParser.SpaceRemover(ref _JsonStr);

            int startIndex = 0;

            //Array or Objectどっちか判断しContentに代入
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

        //コンストラクタ
        static public RootJson Deserialize(string jsonStr)
        {
            RootJson bj = new RootJson();
            bj.DeserializeJson(ref jsonStr);

            return bj;
        }
    }
}
