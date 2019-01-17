using System;
using Android.OS;
using Android.Runtime;
using Java.Interop;

namespace Nyris.UI.Android.Models
{
    public class Links : Java.Lang.Object, IParcelable
    {
        public string Main { get; set; }

        public string Mobile { get; set; }

        public override string ToString()
        {
            return $"Main: {Main}, \n" +
                   $"Mobile: {Mobile}";
        }

        [ExportField("CREATOR")]
        public static ParcelableCreator InitializeCreator()
        {
            return new ParcelableCreator();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Main);
            dest.WriteString(Mobile);
        }

        public int DescribeContents()
        {
            return 0;
        }

        public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new Links
                {
                    Main = source.ReadString(),
                    Mobile = source.ReadString()
                };
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }
    }
}
