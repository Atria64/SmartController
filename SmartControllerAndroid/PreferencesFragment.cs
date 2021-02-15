using System;
using Android.OS;
using Android.Preferences;
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
        }
    }
}