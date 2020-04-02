using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Nyris.UI.iOS.Crop
{
    public class CropOverlayView : UIView
    {
        private readonly nfloat _screenWidth = UIScreen.MainScreen.Bounds.Width;
        private readonly nfloat _screenHeight = UIScreen.MainScreen.Bounds.Height;

        private List<UIView> _outerLines;
        private List<UIView> _horizontalLines;
        private List<UIView> _verticalLines;


        private List<UIView> _topLeftCornerLines;
        private List<UIView> _topRightCornerLines;
        private List<UIView> _bottomLeftCornerLines;
        private List<UIView> _bottomRightCornerLines;

        private List<UIButton> _cornerButtons;

        private const float CornerLineDepth = 3.0f;
        private const float CornerLineWidth = 22.5f;
        private const float LineWidth = 1;
        private const float OuterGapRatio = 1 / 3;
        private const float CornerButtonSize = 60;
        private float OuterGap => CornerButtonSize * OuterGapRatio;
        private CGSize _minimumSize = CGSize.Empty;
        
        public bool IsResizable = false;
        public bool IsMovable = false;

        public CropOverlayView(CGRect frame) : base(frame)
        {
            Setup();
        }

        public CropOverlayView(IntPtr handle) : base(handle)
        {
        }

        private void Setup()
        {
            CreateLines();
        }

        private void CreateLines()
        {
            _outerLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
                CreateLine(),
                CreateLine(),
            };

            _horizontalLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };
            _verticalLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };

            _topLeftCornerLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };
            _topRightCornerLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };
            _bottomLeftCornerLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };
            _bottomRightCornerLines = new List<UIView>
            {
                CreateLine(),
                CreateLine(),
            };

            _cornerButtons = new List<UIButton>
            {
                CreateButton(),
                CreateButton(),
                CreateButton(),
                CreateButton(),
            };


            var dragGesture = new UIPanGestureRecognizer(MoveCropOverlay);
            AddGestureRecognizer(dragGesture);
        }

        private UIView CreateLine(UIColor color = null)
        {
            var line = new UIView
            {
                BackgroundColor = color ?? UIColor.White
            };
            AddSubview(line);
            return line;
        }

        private UIButton CreateButton()
        {
            var button = new UIButton
            {
                BackgroundColor = UIColor.Clear,
            };
            button.AddGestureRecognizer(new UIPanGestureRecognizer(MoveCropOverlay));
            AddSubview(button);
            return button;
        }

        private void GenerateLineFrame()
        {
            for (var i = 0; i < _outerLines.Count; i++)
            {
                var line = _outerLines[i];
                CGRect lineFrame;
                switch (i)
                {
                    case 0:
                        lineFrame = new CGRect(OuterGap, OuterGap, Bounds.Width - OuterGap * 2, LineWidth);
                        break;

                    case 1:
                        lineFrame = new CGRect(Bounds.Width - LineWidth - OuterGap, OuterGap, LineWidth,
                            Bounds.Height);
                        break;

                    case 2:
                        lineFrame = new CGRect(OuterGap, Bounds.Height - LineWidth - OuterGap,
                            Bounds.Width - OuterGap * 2, LineWidth);
                        break;

                    case 3:
                        lineFrame = new CGRect(OuterGap, OuterGap, LineWidth, Bounds.Height - OuterGap * 2);
                        break;
                    default:
                        lineFrame = CGRect.Empty;
                        break;
                }

                line.Frame = lineFrame;
            }
        }

        private void GenerateCorners()
        {
            var corners = new List<List<UIView>>
            {
                _topLeftCornerLines,
                _topRightCornerLines,
                _bottomLeftCornerLines,
                _bottomRightCornerLines,
            };

            for (var i = 0; i < corners.Count; i++)
            {
                var corner = corners[i];

                var horizontalFrame = CGRect.Empty;
                var verticalFrame = CGRect.Empty;
                var buttonFrame = CGRect.Empty;
                var buttonSize = new CGSize(CornerButtonSize, CornerButtonSize);
                const float buttonPadding = 10f;

                switch (i)
                {
                    case 0:
                        verticalFrame = new CGRect(OuterGap, OuterGap, CornerLineDepth, CornerLineWidth);
                        horizontalFrame = new CGRect(OuterGap, OuterGap, CornerLineWidth, CornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(- buttonPadding, - buttonPadding), buttonSize);
                        break;

                    case 1:
                        verticalFrame = new CGRect(Bounds.Width - CornerLineDepth - OuterGap, OuterGap,
                            CornerLineDepth, CornerLineWidth);
                        horizontalFrame = new CGRect(Bounds.Width - CornerLineWidth - OuterGap, OuterGap,
                            CornerLineWidth, CornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(Bounds.Width - CornerButtonSize + buttonPadding, -buttonPadding), buttonSize);
                        break;

                    case 2:
                        verticalFrame = new CGRect(OuterGap, Bounds.Height - CornerLineWidth - OuterGap,
                            CornerLineDepth, CornerLineWidth);
                        horizontalFrame = new CGRect(OuterGap, Bounds.Height - CornerLineDepth - OuterGap,
                            CornerLineWidth, CornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(-buttonPadding, Bounds.Height - CornerButtonSize + buttonPadding), buttonSize);
                        break;

                    case 3:
                        verticalFrame = new CGRect(
                            Bounds.Width - CornerLineDepth - OuterGap,
                            Bounds.Height - CornerLineWidth - OuterGap,
                            CornerLineDepth,
                            CornerLineWidth
                        );
                        horizontalFrame = new CGRect(
                            Bounds.Width - CornerLineWidth - OuterGap,
                            Bounds.Height - CornerLineDepth - OuterGap,
                            CornerLineWidth,
                            CornerLineDepth
                        );
                        buttonFrame = new CGRect(
                            new CGPoint(Bounds.Width - CornerButtonSize + buttonPadding, Bounds.Height - CornerButtonSize + buttonPadding),
                            buttonSize);
                        break;
                }


                corner[0].Frame = verticalFrame;
                corner[1].Frame = horizontalFrame;
                _cornerButtons[i].Frame = buttonFrame;
            }
        }

        private void MoveCropOverlay(UIPanGestureRecognizer gestureRecognizer)
        {
            if (IsResizable && (gestureRecognizer.View is UIButton))
            {
                if (gestureRecognizer.State == UIGestureRecognizerState.Began ||
                    gestureRecognizer.State == UIGestureRecognizerState.Changed)
                {
                    var translation = gestureRecognizer.TranslationInView(this);
                    var newFrame = CGRect.Empty;
                    var button = (UIButton) gestureRecognizer.View;
                    if (ReferenceEquals(button, _cornerButtons[0]))
                    {
                        newFrame = new CGRect(
                            Frame.X + translation.X,
                            Frame.Y + translation.Y,
                            Frame.Width - translation.X,
                            Frame.Height - translation.Y);
                    }
                    else if (ReferenceEquals(button, _cornerButtons[1]))
                    {
                        newFrame = new CGRect(
                            Frame.X,
                            Frame.Y + translation.Y,
                            Frame.Width + translation.X,
                            Frame.Height - translation.Y);
                    }
                    else if (ReferenceEquals(button, _cornerButtons[2]))
                    {
                        newFrame = new CGRect(
                            Frame.X + translation.X,
                            Frame.Y,
                            Frame.Width - translation.X,
                            Frame.Height + translation.Y);
                    }
                    else if (ReferenceEquals(button, _cornerButtons[3]))
                    {
                        newFrame = new CGRect(
                            Frame.X,
                            Frame.Y,
                            Frame.Width + translation.X,
                            Frame.Height + translation.Y);
                    }

                    var minimumFrame = new CGRect(newFrame.X, newFrame.Y,
                        Math.Max(newFrame.Width, _minimumSize.Width + 2 * OuterGap),
                        Math.Max(newFrame.Height, _minimumSize.Height + 2 * OuterGap));
                    Frame = minimumFrame;
                    LayoutSubviews();
                    gestureRecognizer.SetTranslation(CGPoint.Empty, this);
                }
            }
            else if (IsMovable)
            {
                this.Move(gestureRecognizer);
            }
        }


        void Move(UIPanGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State == UIGestureRecognizerState.Began ||
                gestureRecognizer.State == UIGestureRecognizerState.Changed)
            {
                var translation = gestureRecognizer.TranslationInView(this);
                var xMovement = gestureRecognizer.View.Center.X + translation.X;
                var yMovement = gestureRecognizer.View.Center.Y + translation.Y;

                xMovement = CanMoveHorizontally(xMovement) ? xMovement : gestureRecognizer.View.Center.X - translation.X;
                yMovement = CanMoveVertically(yMovement) ? yMovement : gestureRecognizer.View.Center.Y - translation.Y;

                gestureRecognizer.View.Center = new CGPoint(xMovement, yMovement);
                gestureRecognizer.SetTranslation(CGPoint.Empty, this);
            }
        }

        private bool CanMoveHorizontally(nfloat position)
        {
            var width = Frame.Width - (2 * OuterGap);
            return position - (width * 0.5) >= 0 && position + (width * 0.5) <= _screenWidth;
        }

        private bool CanMoveVertically(nfloat position)
        {
            var height = Frame.Height - (2 * OuterGap);
            return position - (height * 0.5) >= 0 && position + (height * 0.5) <= (_screenHeight);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            GenerateLineFrame();
            GenerateCorners();
            var lineThickness = LineWidth / UIScreen.MainScreen.Scale;
            var vPadding = Bounds.Height / (_verticalLines.Count + 1);
            var hPadding = Bounds.Width / (_horizontalLines.Count + 1);

            for (var i = 0; i < _horizontalLines.Count; i++)
            {
                var hLine = _horizontalLines[i];
                var vLine = _verticalLines[i];

                var vSpacing = (vPadding * (i + 1)) + (lineThickness * i);
                var hSpacing = (hPadding * (i + 1)) + (lineThickness * i);

                hLine.Frame = new CGRect(OuterGap, vSpacing + OuterGap, Bounds.Width - OuterGap * 2, lineThickness);
                vLine.Frame = new CGRect(hSpacing + OuterGap, OuterGap, lineThickness, Bounds.Height - OuterGap * 2);
            }
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var currentView = base.HitTest(point, uievent);
            return currentView;
        }
    }
}