using System;
using Android.OS;

namespace Nyris.Ui.Android.Custom
{
    public abstract class Creator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object
    {
        public abstract Java.Lang.Object CreateFromParcel(Parcel source);

        public virtual Java.Lang.Object[] NewArray(int size)
        {
            return new T[size];
        }
    }
}
