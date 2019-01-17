using System;
using Android.OS;
using Android.Runtime;
using Java.Interop;

namespace Nyris.Ui.Android.Models
{
    public class PredictedCategory : Java.Lang.Object, IParcelable
    {
        public string Name { get; set; }

        public float Score { get; set; }

        [ExportField("CREATOR")]
        public static ParcelableCreator InitializeCreator()
        {
            return new ParcelableCreator();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(Name);
            dest.WriteFloat(Score);
        }

        public int DescribeContents()
        {
            return 0;
        }

        public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new PredictedCategory
                {
                    Name = source.ReadString(),
                    Score = source.ReadFloat()
                };
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }
    }
}
