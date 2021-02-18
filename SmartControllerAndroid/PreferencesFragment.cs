using System;
using Android.OS;
using Android.Widget;
using AndroidX.Preference;

namespace SmartControllerAndroid
{
    public class PreferencesFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);
            var moveSpeedSeekBarPreference = FindPreference("MoveSpeed") as SeekBarPreference;
            moveSpeedSeekBarPreference.Min = 1;
            var stopServerPreference = FindPreference("stopServer") as PreferenceScreen;
            stopServerPreference.PreferenceClick += async (sender, e) =>
            {
                string ipAddress = PreferenceManager.GetDefaultSharedPreferences(Context).GetString("IpAddress", null);
                if (ipAddress == null)
                {
                    Toast.MakeText(Context, "null", ToastLength.Short).Show();
                }
                else
                {
                    if(await new SocketManager(ipAddress).ServerStopAsync())
                    {
                        Toast.MakeText(Context, "サーバーを停止しました", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(Context, "サーバーの接続に失敗しました", ToastLength.Short).Show();
                    }
                    PreferenceManager.GetDefaultSharedPreferences(Context).Edit().PutInt("Status", (int)Status.BAD).Apply();
                }
            };
#if DEBUG
            var debugPreference = FindPreference("debug") as PreferenceCategory;
            debugPreference.Visible = true;
            var deletePreferences = FindPreference("deletePreferences") as PreferenceScreen;
            deletePreferences.PreferenceClick += (sender, e) =>
            {
                var preferences = PreferenceManager.GetDefaultSharedPreferences(Context);
                preferences.Edit().Clear().Commit();
                Toast.MakeText(Context, GetString(Resource.String.delete_preferences_description), ToastLength.Short).Show();
            };
#endif
        }
    }
}