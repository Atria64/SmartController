using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Support.Constraints;
using Android.Support.V4.Content;

namespace SmartControllerAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ConstraintLayout statusBar;
        TextView textView;
        Status nowStatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            var qrButton = FindViewById<Button>(Resource.Id.qrButton);
            statusBar = FindViewById<ConstraintLayout>(Resource.Id.statusBar);
            textView = FindViewById<TextView>(Resource.Id.statusTextView);
            nowStatus = Status.OK;
            qrButton.Click += (sender,e) =>{
                Toast.MakeText(this, "QRボタンタップ", ToastLength.Short).Show();
                nowStatus = Status.UNKNOWN;
                UpdateStatus();
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
    }
}