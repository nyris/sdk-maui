﻿using System;
using CoreGraphics;
using UIKit;

namespace Nyris.UI.iOS.ImageOperations
{
    public static class ImageHelper
    {

        public static UIImage ResizeWithRatio(UIImage originalImage, CGSize targetedSizeArea )
        {
            if (originalImage?.CGImage == null)
            {
                return null;
            }
            var aspectRatio = (float)originalImage.Size.Width / (float)originalImage.Size.Height;
            var targetArea = targetedSizeArea.Width * targetedSizeArea.Height;
            var destinationWidth    = Math.Sqrt(targetArea * aspectRatio);
            var destinationHeight   = destinationWidth / aspectRatio;
            var resizedImageBounds = new CGRect(0, 0, destinationWidth, destinationHeight);
            UIGraphics.BeginImageContextWithOptions(resizedImageBounds.Size, false, 1);
            originalImage.Draw(resizedImageBounds);
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resizedImage;
        }

        public static UIImage Crop(UIImage originalImage, CGRect cropFrame, CGRect contextFrame)
        {
            if (originalImage == null)
            {
                return null;
            }

            if (cropFrame.IsEmpty)
            {
                return originalImage;
            }
            
            var imageFrame = new CGRect(0,0, originalImage.Size.Width * UIScreen.MainScreen.Scale, originalImage.Size.Height * UIScreen.MainScreen.Scale);
            var projectedCropFrame = ImageHelper.ScaleRect(cropFrame, contextFrame, imageFrame);
            var croppedImage = ImageHelper.Crop(originalImage, projectedCropFrame);
            return croppedImage;
        }

        private static UIImage Crop(UIImage originalImage, CGRect cropFrame)
        {
            var croppedCgImage = originalImage?.CGImage?.WithImageInRect(cropFrame);
            if (croppedCgImage == null)
            {
                return null;
            }
            
            var croppedImage = new UIImage(croppedCgImage);
            return croppedImage;
        }

        public static CGRect ScaleRect(CGRect originalRect, CGRect parentRect, CGRect destinationRect)
        {


            // the cropping views rect displayed on the screen
            var cropRect = new CGRect(x: originalRect.X,
                                  y: originalRect.Y,
                                  width: originalRect.Width,
                                  height: originalRect.Height);

            var aspectWidth = destinationRect.Width / parentRect.Width;
            var aspectHeight = destinationRect.Height / parentRect.Height;

            var normalizedWidth = cropRect.Width * aspectWidth;
            var normalizedHeight = cropRect.Height * aspectHeight;

            var xPositionAspect = (destinationRect.Width * cropRect.X) / parentRect.Width;
            var yPositionAspect = (destinationRect.Height * cropRect.Y) / parentRect.Height;
        
            var result = new CGRect(xPositionAspect, yPositionAspect, normalizedWidth, normalizedHeight);
            return result;
        }
    }
}
