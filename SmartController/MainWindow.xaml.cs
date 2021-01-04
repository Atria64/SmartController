using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;

namespace SmartController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int PORT = 2001;
        private string IpAddress;
        public MainWindow()
        {
            InitializeComponent();
            NativeMethods.AllocConsole();
            IpAddress = GetThisIp();

            BarcodeWriter qrcode = new BarcodeWriter
            {
                // 出力するコードの形式をQRコードに選択
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    // QRコードの信頼性
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M,
                    // 日本語を表示したい場合シフトJISを指定
                    //CharacterSet = "Shift_JIS",
                    // デフォルト
                    CharacterSet = "ISO-8859-1",
                    // QRコードのサイズ決定
                    Height = 160,
                    Width = 160,
                    // QRコード周囲の余白の大きさ
                    Margin = 4
                }
            };

            var bmp = qrcode.Write(IpAddress);
            using Stream st = new MemoryStream();
            bmp.Save(st, ImageFormat.Bmp);
            st.Seek(0, SeekOrigin.Begin);
            image.Source = BitmapFrame.Create(st, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            WaitContact();
        }

        private string GetThisIp()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);
            //adrListから192.xxx系を取り出す。
            var ls = adrList.Where(x => x.ToString().StartsWith("192"));
            if (ls.Count() > 0)
            {
                return ls.ToList()[0].ToString();
            }
            else return null;
        }

        private async void WaitContact()
        {
            var server = new Server(IpAddress, PORT);
            if (await server.ConnectCheckAsync())
            {
                Console.WriteLine("Check completed.");
                this.Visibility = Visibility.Hidden;
                await server.StartAsync();
            }
        }
    }
}
