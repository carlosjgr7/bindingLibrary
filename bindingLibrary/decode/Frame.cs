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

namespace bindingLibrary.decode
{
    public class Frame
    {
        public Byte[] data { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
        public long timestamp { get; set; }

    }
}