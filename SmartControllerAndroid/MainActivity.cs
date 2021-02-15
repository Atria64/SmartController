using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Support.V4.Content;
using ZXing;
using ZXing.Mobile;
using Java.Net;
using Java.IO;
using System.Threading.Tasks;
using System;
using System.Threading;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Android.Content;

namespace SmartControllerAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private readonly uint MoveSpeed = 5;
        SocketManager socketManager;
        MobileBarcodeScanner scanner;
        Button qrButton;
        ConstraintLayout mainLayout;
        ConstraintLayout statusBar;
        TextView textView;
        string IpAddress;
        static bool repeatFlag = false;

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

            MobileBarcodeScanner.Initialize(Application);
            scanner = new MobileBarcodeScanner();
            qrButton.Click += async (sender, e) => {
                UpdateStatus(Status.UNKNOWN);
                //Tell our scanner to use the default overlay
                scanner.UseCustomOverlay = false;

                //We can customize the top and bottom text of the default overlay
                scanner.TopText = "QRコードを探しています";
                scanner.BottomText = "PCソフトを起動して表示された\nQRコードを読み込んでください";

                //Start scanning
                var result = await scanner.Scan();
                HandleScanResultAsync(result);
            };

            UpdateStatus(Status.BAD);
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
            switch (item.ItemId)
            {
                case Resource.Id.settings:
                    var intent = new Intent(this, typeof(PreferencesActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_repository:
                    OpenRepositoryUri();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void UpdateStatus(Status nextStatus)
        {
            //UIはメインスレッドで操作する
            var handler = new Handler(Looper.MainLooper);
            handler.Post(() => {
                switch (nextStatus)
                {
                    case Status.BAD:
                        statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.badStatus);
                        textView.Text = "未接続";
                        mainLayout.Touch -= OnTouch;
                        qrButton.Visibility = ViewStates.Visible;
                        break;
                    case Status.UNKNOWN:
                        statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.unknownStatus);
                        textView.Text = "接続チェック中";
                        break;
                    case Status.OK:
                        statusBar.Background = ContextCompat.GetDrawable(this, Resource.Color.okStatus);
                        textView.Text = "接続完了";
                        mainLayout.Touch += OnTouch;
                        qrButton.Visibility = ViewStates.Gone;
                        break;
                }
            });
        }
        private async void HandleScanResultAsync(ZXing.Result result)
        {
            var msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = "Found Barcode: " + result.Text;
                if (await (new SocketManager(result.Text).PingAsync()))
                {
                    IpAddress = result.Text;
                    socketManager = new SocketManager(IpAddress);
                    UpdateStatus(Status.OK);
                }
                else
                {
                    UpdateStatus(Status.BAD);
                }
            }
            else
            {
                msg = "Scanning Canceled!";
                UpdateStatus(Status.BAD);
            }

            RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
        }

        float downX;
        float downY;
        float moveX;
        float moveY;

        private async void OnTouch(object sender, View.TouchEventArgs touchEventArgs)
        {
            switch (touchEventArgs.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    {
                        downX = touchEventArgs.Event.GetX();
                        downY = touchEventArgs.Event.GetY();
                        ButtonLongClicked();
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        moveX = touchEventArgs.Event.GetX();
                        moveY = touchEventArgs.Event.GetY();
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        repeatFlag = false;
                        var upX = touchEventArgs.Event.GetX();
                        var upY = touchEventArgs.Event.GetY();
                        //タップ時
                        if ((GeoLength(downX, downY, upX, upY) < 30))
                        {
                            //通常タップ
                            if (touchEventArgs.Event.EventTime - touchEventArgs.Event.DownTime < 500)
                            {
                                if (await socketManager.LeftClickAsync() is false)
                                {
                                    UpdateStatus(Status.BAD);
                                }
                            }
                            else //ロングタップ
                            {
                                if (await socketManager.RightClickAsync() is false)
                                {
                                    UpdateStatus(Status.BAD);
                                }
                            }
                        }
                        break;
                    }
            }
        }
        private void ButtonLongClicked()
        {
            repeatFlag = true;
            new System.Threading.Thread(new System.Threading.ThreadStart(async () => {
                while (repeatFlag)
                {
                    Thread.Sleep(50);
                    if ((GeoLength(downX, downY, moveX, moveY) > 30))
                    {
                        float xDifference = downX - moveX;
                        float yDifference = downY - moveY;
                        if (await socketManager.MoveCursorAsync(xDifference, yDifference, MoveSpeed) is false)
                        {
                            UpdateStatus(Status.BAD);
                        }
                    }
                }
            })).Start();
        }

        private double GeoLength(double x1, double y1, double x2, double y2)
        {
            double ret = Math.Sqrt(Math.Pow(x2 - x1, 2) +
            Math.Pow(y2 - y1, 2));
            return ret;
        }
        
        private void OpenRepositoryUri()
        {
            var link = GetString(Resource.String.repository_uri);
            var intent = new Intent(Intent.ActionDefault, Android.Net.Uri.Parse(link));
            StartActivity(intent);
        }
    }
}