using System;
using System.Runtime.InteropServices;
using WPFAcrylics.Enums;


namespace WPFAcrylics.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
}
