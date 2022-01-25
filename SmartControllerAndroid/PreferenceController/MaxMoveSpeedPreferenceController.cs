using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControllerAndroid
{
    internal class MaxMoveSpeedPreferenceController : IIntPreferenceControllerInterface
    {
        Activity _activity;
        public MaxMoveSpeedPreferenceController(Activity activity)
        {
            _activity = activity;
        }

        public Activity Activity => _activity;

        public string PreferenceKey => "MaxMoveSpeed";
    }
}