using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Nyris.UI.Android.Custom;

[Register("io.nyris.customs.ImageCameraView")]
internal class ImageCameraView : View
{
    private Paint _paint;
    private Bitmap _bitmap;

    public ImageCameraView(Context context) : base(context)
    {
        Init();
    }

    public ImageCameraView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        Init();
    }

    public ImageCameraView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    private void Init()
    {
        _paint = new Paint
        {
            AntiAlias = true,
            FilterBitmap = true,
            Dither = true
        };
    }

    public void SetBitmap(Bitmap bitmap)
    {
        _bitmap = bitmap;
        RequestLayout();
        Invalidate();
    }

    protected override void OnDraw(Canvas canvas)
    {
        canvas.DrawBitmap(_bitmap, 0f, 0f, _paint);
    }
}