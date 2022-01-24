using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControllerAndroid
{
    internal interface IIntPreferenceControllerInterface
    {
        const int DEFAULT_VALUE = 0;
        Activity Activity { get; }
        string PreferenceKey { get; }
        int PreferenceValue
        {
            get => PreferenceManager.GetDefaultSharedPreferences(Activity).GetInt(PreferenceKey, DEFAULT_VALUE);
            set {
                var pm = PreferenceManager.GetDefaultSharedPreferences(Activity).Edit();
                pm.PutInt(PreferenceKey, (int)PreferenceValue).Apply();
            }
        }
    }
}