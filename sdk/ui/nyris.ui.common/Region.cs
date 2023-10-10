using System;
namespace Nyris.UI.Common
{
    public class Region
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public bool IsValid => Math.Abs(Left + Top + Right + Bottom) > 0;
    }
}
