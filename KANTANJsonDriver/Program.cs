// See https://aka.ms/new-console-template for more information
using KANTANJson;

string jsonStr = "{\"num\":123,ABC:\"OK\",Bool:tRuE,Array:[123,1.23,\"123\"]}";
RootJson rj = RootJson.Deserialize(jsonStr);
Console.WriteLine(rj.JsonStr);


ObjectJson oj = (ObjectJson)rj.Content;
foreach (string key in oj.Keys)
{
    object obj = oj[key];
    Console.WriteLine( "[" + obj.GetType() + "]" + key + ":" + obj);
    if (obj.GetType() == typeof(ArrayJson))
    {
        ArrayJson aj = (ArrayJson)obj;
        for(int i = 0; i < aj.Count; i++)
        {
            Console.WriteLine("Array[" + i + "]" + aj[i].ToString());
        }
    }
}

