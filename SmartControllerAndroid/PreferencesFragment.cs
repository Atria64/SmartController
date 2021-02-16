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
#if DEBUG
            var debugPreference = FindPreference("debug") as PreferenceCategory;
            debugPreference.Visible = true;
            var deletePreferences = FindPreference("deletePreferences") as PreferenceScreen;
            deletePreferences.PreferenceClick += (sender, e) =>
            {
                var preferences = PreferenceManager.GetDefaultSharedPreferences(Context);
                preferences.Edit().Clear().Commit();
                Toast.MakeText(Context, "設定をクリアしました。再起動してください。", ToastLength.Short).Show();
            };
#endif
        }
    }
}