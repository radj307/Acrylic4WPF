using System.Runtime.InteropServices;
using WPFAcrylics.Enums;

namespace WPFAcrylics.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }
}
