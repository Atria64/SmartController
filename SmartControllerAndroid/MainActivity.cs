using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Support.Constraints;
using Android.Support.V4.Content;
using ZXing;
using ZXing.Mobile;
using Java.Net;
using Java.IO;
using System.Threading.Tasks;
using System;

namespace SmartControllerAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        MobileBarcodeScanner scanner;
        Button qrButton;
        ConstraintLayout mainLayout;
        ConstraintLayout statusBar;
        TextView textView;
        Status nowStatus;
        string IpAddress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            qrButton = FindViewById<Button>(Resource.Id.qrButton);
            mainLayout = FindViewById<ConstraintLayout>(Resource.Id.mainLayout);
            statusBar = FindViewById<ConstraintLayout>(Resource.Id.statusBar);
            textView = FindViewById<TextView>(Resource.Id.statusTextView);

            nowStatus = Status.BAD;
            
            MobileBarcodeScanner.Initialize(Application);
            scanner = new MobileBarcodeScanner();
            qrButton.Click += async (sender,e) => {
                nowStatus = Status.UNKNOWN;
                UpdateStatus();
                //Tell our scanner to use the default overlay
                scanner.UseCustomOverlay = false;

                //We can customize the top and bottom text of the default overlay
                scanner.TopText = "QRコードを探しています";
                scanner.BottomText = "PCソフトを起動して表示された\nQRコードを読み込んでください";

                //Start scanning
                var result = await scanner.Scan();
                HandleScanResultAsync(result);
            };

            UpdateStatus();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.option, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        private void UpdateStatus()
        {
            switch (nowStatus)
            {
                case Status.BAD:
                    statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.badStatus);
                    textView.Text = "未接続";
                    break;
                case Status.UNKNOWN:
                    statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.unknownStatus);
                    textView.Text = "接続チェック中";
                    break;
                case Status.OK:
                    statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.okStatus);
                    textView.Text = "接続完了";
                    break;
            }
        }
        async void HandleScanResultAsync(ZXing.Result result)
        {
            var msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = "Found Barcode: " + result.Text;
                if (await SocketSendAsync(result.Text, "ping"))
                {
                    IpAddress = result.Text;
                    nowStatus = Status.OK;
                    qrButton.Visibility = ViewStates.Gone;
                    mainLayout.Touch += OnTouch;
                }
                else
                {
                    nowStatus = Status.BAD;
                    qrButton.Visibility = ViewStates.Visible;
                    mainLayout.Touch -= OnTouch;
                }
            }
            else
            {
                msg = "Scanning Canceled!";
                nowStatus = Status.BAD;
                mainLayout.Touch -= OnTouch;
            }

            UpdateStatus();
            RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }
        private async Task<bool> SocketSendAsync(string ip, string msg)
        {
            int port = 2001;
            return await Task.Run(() =>
            {
                try
                {
                    InetSocketAddress address = new InetSocketAddress(ip, port);
                    Socket socket = new Socket();
                    socket.Connect(address, 3000);
                    using PrintWriter pw = new PrintWriter(socket.OutputStream, true);
                    pw.Println(msg);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            });
        }

        float downX;
        float downY;

        private async void OnTouch(object sender, View.TouchEventArgs touchEventArgs)
        {
            switch (touchEventArgs.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    {
                        downX = touchEventArgs.Event.GetX();
                        downY = touchEventArgs.Event.GetY();
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        var moveX = touchEventArgs.Event.GetX();
                        var moveY = touchEventArgs.Event.GetY();
                        if((GeoLength(downX, downY, moveX, moveY) > 30)) await SocketSendAsync(IpAddress, $"mv {downX - moveX} {downY - moveY}");
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        var upX = touchEventArgs.Event.GetX();
                        var upY = touchEventArgs.Event.GetY();
                        //タップ時
                        if ((GeoLength(downX, downY, upX, upY) < 30))
                        {
                            //通常タップ
                            if (touchEventArgs.Event.EventTime - touchEventArgs.Event.DownTime < 500)
                            {
                                await SocketSendAsync(IpAddress, "lc");
                            }
                            else //ロングタップ
                            {
                                await SocketSendAsync(IpAddress, "rc");
                            }
                        }
                        break;
                    }
            }
        }

        private double GeoLength(double x1, double y1, double x2, double y2)
        {
            double ret = Math.Sqrt(Math.Pow(x2 - x1, 2) +
            Math.Pow(y2 - y1, 2));
            return ret;
        }
    }
}