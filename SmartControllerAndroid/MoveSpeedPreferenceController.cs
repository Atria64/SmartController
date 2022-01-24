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
    internal class MoveSpeedPreferenceController : IIntPreferenceControllerInterface
    {
        Activity _activity;
        public MoveSpeedPreferenceController(Activity activity)
        {
            _activity = activity;
        }

        public string PreferenceKey => "MoveSpeed";

        public Activity Activity => _activity;
    }
}