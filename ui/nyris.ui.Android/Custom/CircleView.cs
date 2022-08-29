using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using AndroidX.Core.Content;

namespace Nyris.UI.Android.Custom
{
    [Register("io.nyris.customs.CircleView")]
    public class CircleView : View
    {
        private readonly float strokeContainerCircleDp = 17f;
        private readonly float radiusContainerCircleDp = 41.5f;
        private readonly float radiusCircleDp = 17.5f;
        private float _strokeContainerCirclePx;
        private float _radiusContainerCirclePX;
        private float _radiusCirclePx;

        private Paint _containerPaint;
        private Paint _circlePaint;

        public event EventHandler AnimationEnd;
        public CircleView(Context context) : base(context)
        {
            Init();
        }

        public CircleView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public CircleView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        private void Init()
        {
            _strokeContainerCirclePx = TypedValue.ApplyDimension(ComplexUnitType.Dip, strokeContainerCircleDp, Resources.DisplayMetrics);
            _radiusContainerCirclePX = TypedValue.ApplyDimension(ComplexUnitType.Dip, radiusContainerCircleDp, Resources.DisplayMetrics);
            _radiusCirclePx = TypedValue.ApplyDimension(ComplexUnitType.Dip, radiusCircleDp, Resources.DisplayMetrics);

            _containerPaint = new Paint();
            _containerPaint.SetStyle(Paint.Style.Stroke);
            _containerPaint.Color = new Color(ContextCompat.GetColor(Context, Resource.Color.nyris_color_primary));
            _containerPaint.StrokeWidth = _strokeContainerCirclePx;
            _containerPaint.AntiAlias = true;
            _containerPaint.Dither = true;
            _containerPaint.FilterBitmap = true;

            _circlePaint = new Paint();
            _circlePaint.SetStyle(Paint.Style.Fill);
            _circlePaint.Color = new Color(ContextCompat.GetColor(Context, Resource.Color.nyris_color_primary));
            _circlePaint.AntiAlias = true;
            _circlePaint.Dither = true;
            _circlePaint.FilterBitmap = true;
        }

        public void StartAnimation(View vPosCam)
        {
            _strokeContainerCirclePx = TypedValue.ApplyDimension(ComplexUnitType.Dip, strokeContainerCircleDp, Resources.DisplayMetrics);
            _radiusContainerCirclePX = TypedValue.ApplyDimension(ComplexUnitType.Dip, radiusContainerCircleDp, Resources.DisplayMetrics);
            _radiusCirclePx = TypedValue.ApplyDimension(ComplexUnitType.Dip, radiusCircleDp, Resources.DisplayMetrics);

            PostDelayed(() =>
            {
                ScaleCircleAnimation();

                var valueAnimator = ValueAnimator.OfFloat(0f, vPosCam.Top - Top);
                valueAnimator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    var value = (float)e.Animation.AnimatedValue;
                    TranslationY = value;
                };
                valueAnimator.SetInterpolator(new DecelerateInterpolator(3f));
                valueAnimator.SetDuration(700);
                valueAnimator.Start();
            }, 500);

        }

        private void ScaleCircleAnimation()
        {
            var originalStroke = _strokeContainerCirclePx;
            var originalCircle = _radiusCirclePx;
            var valueAnimator = ValueAnimator.OfFloat(0f, 40f);
            valueAnimator.SetDuration(500);
            valueAnimator.SetInterpolator(new LinearInterpolator());
            valueAnimator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
            {
                var value = (float)e.Animation.AnimatedValue;
                _strokeContainerCirclePx = originalStroke - value;
                _radiusCirclePx = originalCircle + value;
                _containerPaint.StrokeWidth = _strokeContainerCirclePx;
                Invalidate();
                RequestLayout();
            };

            valueAnimator.AnimationEnd += delegate
            {
                AnimationEnd?.Invoke(this, null);
            };

            valueAnimator.Start();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            var x = Width;
            canvas.DrawCircle(x / 2, Width / 2, _radiusContainerCirclePX, _containerPaint);
            canvas.DrawCircle(x / 2, Width / 2, _radiusCirclePx, _circlePaint);
        }
    }
}
