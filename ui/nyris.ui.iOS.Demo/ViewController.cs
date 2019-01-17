using System;

using UIKit;
using Foundation;
using Nyris.UI.iOS.Camera;


namespace Nyris.UI.iOS.Demo
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(CameraManager)));
            var storyboard = UIStoryboard.FromName("CameraController", bundle);
            var cameraController = storyboard.InstantiateInitialViewController();
            if (cameraController != null)
            {
                this.PresentViewController(cameraController, true, null);
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
