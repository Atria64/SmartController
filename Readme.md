# SmartController Version Alpha.
SmartControllerはAndroid端末でのWindowsPC操作を可能にするソフトウェアです。   
使用する場合、PC(Server)とAndroid(Client)が同一Wi-Fi内にある必要があります。
   
現在はアルファ版となっており、一部機能が実装されておらず、不安定です。

# SmartController (Server)
PC(Server)側ソフトウェア。**Windowsのみ対応。**
   
起動すると自動でQRコードが生成されます。

## Server側起動例
![スクリーンショット 2021-02-23 083026](https://user-images.githubusercontent.com/49768768/108812347-4d0a7b00-75f2-11eb-87ab-bb16312cbc45.png)

### 利用したもの
 - .NET Core 3.1 WPF
 - [ZXing.NET](https://github.com/micjahn/ZXing.Net)
 - [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/)

# SmartControllerAndroid (Client)
Android(Client)側ソフトウェア。QRコードを読み取って`Server`と接続を行います。   
接続後、スワイプ&タップでカーソル操作を行うことができます。

## Android側起動例
<img src="https://user-images.githubusercontent.com/49768768/108812455-7cb98300-75f2-11eb-8fd8-ae1f9f3b992c.png" width="300px">

### 利用したもの
 - Xamarin.AndroidX
 - [ZXing.NET.Mobile](https://github.com/Redth/ZXing.Net.Mobile)
 - [Xamarin.AndroidX.ConstraintLayout](https://www.nuget.org/packages/Xamarin.AndroidX.ConstraintLayout/)
 - [Xamarin.AndroidX.ConstraintLayout.Solver](https://www.nuget.org/packages/Xamarin.AndroidX.ConstraintLayout/)


 # コントリビューション/バグ報告
 
 Atria64/SmartController に対するバグ報告などは [issue](https://github.com/Atria64/SmartController/issues/new/choose)にお願いします。   
 また、`good first issue / help wanted / bug` などのラベルが付いたものの開発の手伝いをしていただけると嬉しいです。
 開発の手伝いをして頂ける場合、該当issueにコメントをしてくださると嬉しいです。