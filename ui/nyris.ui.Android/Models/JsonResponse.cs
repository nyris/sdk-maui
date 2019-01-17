using System;
using Android.OS;
using Android.Runtime;
using Java.Interop;

namespace Nyris.UI.Android.Models
{
    public class JsonResponse : Java.Lang.Object, IParcelable
    {
        public string Content { get; set; }

        [ExportField("CREATOR")]
        public static ParcelableCreator InitializeCreator()
        {
            return new ParcelableCreator();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Content);
        }

        public int DescribeContents()
        {
            return 0;
        }

        public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new JsonResponse
                {
                    Content = source.ReadString()
                };
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }
    }
}
