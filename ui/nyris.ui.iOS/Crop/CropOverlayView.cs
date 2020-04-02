using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Nyris.UI.iOS.Crop
{
    public class CropOverlayView : UIView
    {
        nfloat _screenWidth = UIScreen.MainScreen.Bounds.Width;
        nfloat _screenHeight = UIScreen.MainScreen.Bounds.Height;

        List<UIView> _outerLines;
        List<UIView> _horizontalLines;
        List<UIView> _verticalLines;


        List<UIView> _topLeftCornerLines;
        List<UIView> _topRightCornerLines;
        List<UIView> _bottomLeftCornerLines;
        List<UIView> _bottomRightCornerLines;

        List<UIButton> _cornerButtons;

        float _cornerLineDepth = 3.0f;
        float _cornerLineWidth = 22.5f;
        float _lineWidth = 1;
        float _outterGapRatio = 1 / 3;
        public float CornerButtonWidth => 60;
        public float OutterGap => CornerButtonWidth * _outterGapRatio;
        public bool IsResizable = false;
        public bool IsMovable = false;
        public CGSize MinimumSize = CGSize.Empty;
        public bool SelectionLock = false;

        public CropOverlayView(CGRect frame) : base(frame)
        {
            Setup();
        }

        public CropOverlayView(IntPtr handle) : base(handle)
        {
        }

        void Setup()
        {
            CreateLines();
        }
        
        void CreateLines()
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
                BackgroundColor = UIColor.Red,
            };
            button.AddGestureRecognizer(new UIPanGestureRecognizer(MoveCropOverlay));
            AddSubview(button);
            return button;
        }

        void GenerateLineFrame()
        {
            for (int i = 0; i < _outerLines.Count; i++)
            {
                var line = _outerLines[i];
                CGRect lineFrame;
                switch (i)
                {
                    case 0:
                        lineFrame = new CGRect(OutterGap, OutterGap, Bounds.Width - OutterGap * 2, _lineWidth);
                        break;

                    case 1:
                        lineFrame = new CGRect(Bounds.Width - _lineWidth - OutterGap, OutterGap, _lineWidth,
                            Bounds.Height);
                        break;

                    case 2:
                        lineFrame = new CGRect(OutterGap, Bounds.Height - _lineWidth - OutterGap,
                            Bounds.Width - OutterGap * 2, _lineWidth);
                        break;

                    case 3:
                        lineFrame = new CGRect(OutterGap, OutterGap, _lineWidth, Bounds.Height - OutterGap * 2);
                        break;
                    default:
                        lineFrame = CGRect.Empty;
                        break;
                }

                line.Frame = lineFrame;
            }
        }

        void GenerateCorners()
        {
            var corners = new List<List<UIView>>
            {
                _topLeftCornerLines,
                _topRightCornerLines,
                _bottomLeftCornerLines,
                _bottomRightCornerLines,
            };

            for (int i = 0; i < corners.Count; i++)
            {
                var corner = corners[i];

                var horizontalFrame = CGRect.Empty;
                var verticalFrame = CGRect.Empty;
                var buttonFrame = CGRect.Empty;
                var buttonSize = new CGSize(CornerButtonWidth, CornerButtonWidth);
                var buttonPadding = 10f;

                switch (i)
                {
                    case 0:
                        verticalFrame = new CGRect(OutterGap, OutterGap, _cornerLineDepth, _cornerLineWidth);
                        horizontalFrame = new CGRect(OutterGap, OutterGap, _cornerLineWidth, _cornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(- buttonPadding, - buttonPadding), buttonSize);
                        break;

                    case 1:
                        verticalFrame = new CGRect(Bounds.Width - _cornerLineDepth - OutterGap, OutterGap,
                            _cornerLineDepth, _cornerLineWidth);
                        horizontalFrame = new CGRect(Bounds.Width - _cornerLineWidth - OutterGap, OutterGap,
                            _cornerLineWidth, _cornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(Bounds.Width - CornerButtonWidth + buttonPadding, -buttonPadding), buttonSize);
                        break;

                    case 2:
                        verticalFrame = new CGRect(OutterGap, Bounds.Height - _cornerLineWidth - OutterGap,
                            _cornerLineDepth, _cornerLineWidth);
                        horizontalFrame = new CGRect(OutterGap, Bounds.Height - _cornerLineDepth - OutterGap,
                            _cornerLineWidth, _cornerLineDepth);
                        buttonFrame = new CGRect(new CGPoint(-buttonPadding, Bounds.Height - CornerButtonWidth + buttonPadding), buttonSize);
                        break;

                    case 3:
                        verticalFrame = new CGRect(
                            Bounds.Width - _cornerLineDepth - OutterGap,
                            Bounds.Height - _cornerLineWidth - OutterGap,
                            _cornerLineDepth,
                            _cornerLineWidth
                        );
                        horizontalFrame = new CGRect(
                            Bounds.Width - _cornerLineWidth - OutterGap,
                            Bounds.Height - _cornerLineDepth - OutterGap,
                            _cornerLineWidth,
                            _cornerLineDepth
                        );
                        buttonFrame = new CGRect(
                            new CGPoint(Bounds.Width - CornerButtonWidth + buttonPadding, Bounds.Height - CornerButtonWidth + buttonPadding),
                            buttonSize);
                        break;
                    default:
                        break;
                }


                corner[0].Frame = verticalFrame;
                corner[1].Frame = horizontalFrame;
                _cornerButtons[i].Frame = buttonFrame;
            }
        }

        void MoveCropOverlay(UIPanGestureRecognizer gestureRecognizer)
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
                        Math.Max(newFrame.Width, MinimumSize.Width + 2 * OutterGap),
                        Math.Max(newFrame.Height, MinimumSize.Height + 2 * OutterGap));
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

                xMovement = CanMoveHorizontaly(xMovement) ? xMovement : gestureRecognizer.View.Center.X - translation.X;
                yMovement = CanMoveVerticaly(yMovement) ? yMovement : gestureRecognizer.View.Center.Y - translation.Y;

                gestureRecognizer.View.Center = new CGPoint(xMovement, yMovement);
                gestureRecognizer.SetTranslation(CGPoint.Empty, this);
            }
        }

        bool CanMoveHorizontaly(nfloat position)
        {
            var width = Frame.Width - (2 * OutterGap);
            return position - (width * 0.5) >= 0 && position + (width * 0.5) <= _screenWidth;
        }

        bool CanMoveVerticaly(nfloat position)
        {
            var height = Frame.Height - (2 * OutterGap);
            return position - (height * 0.5) >= 0 && position + (height * 0.5) <= (_screenHeight);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            GenerateLineFrame();
            GenerateCorners();
            var lineThickness = _lineWidth / UIScreen.MainScreen.Scale;
            var vPadding = Bounds.Height / (_verticalLines.Count + 1);
            var hPadding = Bounds.Width / (_horizontalLines.Count + 1);

            for (int i = 0; i < _horizontalLines.Count; i++)
            {
                var hLine = _horizontalLines[i];
                var vLine = _verticalLines[i];

                var vSpacing = (vPadding * (i + 1)) + (lineThickness * i);
                var hSpacing = (hPadding * (i + 1)) + (lineThickness * i);

                hLine.Frame = new CGRect(OutterGap, vSpacing + OutterGap, Bounds.Width - OutterGap * 2, lineThickness);
                vLine.Frame = new CGRect(hSpacing + OutterGap, OutterGap, lineThickness, Bounds.Height - OutterGap * 2);
            }
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var currentView = base.HitTest(point, uievent);
            if (currentView == null)
            {
            }
            else
            {
            }


            if (!IsMovable && IsResizable && currentView != null)
            {
            }

            return currentView;
        }
    }
}