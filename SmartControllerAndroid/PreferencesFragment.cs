using System;
using Android.OS;
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
#endif
        }
    }
}