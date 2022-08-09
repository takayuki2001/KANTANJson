# KANTANJson

## 型の説明
### RootJson
RootJsonをパースし、Json情報を保存しておく為のクラスです。
使い方以外の使い道は現状ありません。

### BaseJson
ObjectJsonとArrayJsonの継承元です。
明示的にどちらかに変換し使います。

### ObjectJson
ObjectJsonはJsonにおける{key:value,n...}
を表現します。内部ではDictionary<string,object>で表されます。

### ArrayJson
ArrayJsonはJsonにおける[value,n...]
を表現します。内部ではList<object>で表されます。

### object
Jsonにおける全要素を表すために使用されます。System.Objectです。

numberは常にdoubleです。

boolはboolです。

stringはstringです。

nullはnullです。

適当にGetType()を活用し明示変換しご利用ください。

## 使い方
RootJson rj = RootJson.Deserialize(jsonStr)
によりRootJsonを作ります。
rj.ContentにてBaseJsonを取得します。

GetType()により　ObjectJsonかArrayJsonであるかを判別し、明示的に変換します。

ArrayJsonであればArrayJson[index]にてobject型を取得します。
ObjectJsonであればObjectJson[string key]か[index]でValueを取得できます。
ObjectJsonであればKeyListでKeyが入ったListが取得できます。
