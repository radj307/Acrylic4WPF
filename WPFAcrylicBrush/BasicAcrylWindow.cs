using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WPFAcrylics
{

    internal enum AccentState
    {
        ACCENT_DISABLED = 1,
        ACCENT_ENABLE_GRADIENT = 0,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }


    /// <summary>
    /// 
    /// </summary>
    public class BasicAcrylWindow : Window, INotifyPropertyChanged
    {
        // ===== Blur things ========

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            int accentStructSize = Marshal.SizeOf(accent);

            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            _ = SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }



        /// <summary>
        /// Property for changing the color of the TransparentBackground
        /// </summary>
        public Brush? TransparentBackground
        {
            get => _transparentBackground;
            set
            {
                _transparentBackground = value;
                OnPropertyChanged();
            }
        }

        private Brush? _transparentBackground = null;



        /// <summary>
        /// Changes the opacity of the Acrylic transparent background
        /// </summary>
        public double AcrylOpacity
        {
            get => _acrylOpacity;
            set
            {
                _acrylOpacity = value;
                OnPropertyChanged();
            }
        }

        private double _acrylOpacity;



        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowMinimizeButton
        {
            get => _showMinimizeButton;
            set
            {
                _showMinimizeButton = value;
                OnPropertyChanged();
            }
        }

        private Visibility _showMinimizeButton;



        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowFullscreenButton
        {
            get => _showFullscreenButton;
            set
            {
                _showFullscreenButton = value;
                OnPropertyChanged();
            }
        }

        private Visibility _showFullscreenButton;



        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowCloseButton
        {
            get => _showCloseButton;
            set
            {
                _showCloseButton = value;
                OnPropertyChanged();
            }
        }

        private Visibility _showCloseButton;



        /// <summary>
        /// 
        /// </summary>
        public double NoiseRatio
        {
            get => _noiseRatio;
            set
            {
                _noiseRatio = value;
                OnPropertyChanged();
            }
        }
        private double _noiseRatio;



        /// <summary>
        /// Event implementation
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));



        protected readonly Grid _mainGrid;
        protected readonly Grid _contentGrid;
        protected readonly string _internalGridName;

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicAcrylWindow()
        {
            Loaded += MainWindow_Loaded;
            SourceInitialized += Win_SourceInitialized;

            AcrylOpacity = 0.6;
            _contentGrid = new Grid();
            Grid.SetRow(_contentGrid, 1);
            _internalGridName = "internalMainGrid";
            _mainGrid = new Grid
            {
                Name = _internalGridName
            };

            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;

            Content = BuildBaseWindow();
        }

        /// <param name="arg1">The old content</param>
        /// <param name="arg2">The new content</param>
        protected override void OnContentChanged(object arg1, object arg2)
        {
            // Do nothing if this is the initialize call
            if (arg2 is Grid grid && grid.Name == _internalGridName)
            {
                return;
            }

            Content = _mainGrid;
            _contentGrid.Children.Clear();
            _ = _contentGrid.Children.Add((UIElement)arg2);
        }
        protected virtual Grid BuildBaseWindow()
        {
            // Transparent effect rectangle
            var rect = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _ = rect.SetBinding(Shape.FillProperty, new Binding
            {
                Path = new PropertyPath("TransparentBackground"),
                Source = this,
                FallbackValue = Brushes.LightGray,
                TargetNullValue = Brushes.LightGray
            });
            _ = rect.SetBinding(OpacityProperty, new Binding
            {
                Path = new PropertyPath("AcrylOpacity"),
                Source = this,
                FallbackValue = 0.6,
                TargetNullValue = 0.6
            });

            // Add the noise effect to the rectangle
            var fx = new WPFAcrylics.NoiseEffect.NoiseEffect();
            _ = BindingOperations.SetBinding(fx, WPFAcrylics.NoiseEffect.NoiseEffect.RatioProperty, new Binding
            {
                Path = new PropertyPath("NoiseRatio"),
                TargetNullValue = 0.1,
                FallbackValue = 0.1,
                Source = this,
            });
            rect.Effect = fx;


            _ = _mainGrid.Children.Add(rect);

            var windowGrid = new Grid();
            windowGrid.RowDefinitions.Add(new RowDefinition());
            _ = _mainGrid.Children.Add(windowGrid);
            _ = windowGrid.Children.Add(_contentGrid);
            return _mainGrid;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) => EnableBlur();

        // ==== Magic code from here: https://blogs.msdn.microsoft.com/llobo/2006/08/01/maximizing-window-with-windowstylenone-considering-taskbar/
        // All credits go to: LesterLobo
        // Code for preventing window to go out of area when maximizing - because windows with no WindowStyle do that; WPF bug
        private static IntPtr WindowProc(
              IntPtr hwnd,
              int msg,
              IntPtr wParam,
              IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
            case 0x0024:
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;
            }

            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {

                var monitorInfo = new MONITORINFO();
                _ = GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new();

            /// <summary> Win32 </summary>
            public int Width => Math.Abs(right - left);
            /// <summary> Win32 </summary>
            public int Height => bottom - top;

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty =>
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    left >= right || top >= bottom;
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object? obj) => obj is Rect && this == (RECT)obj;

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) => (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) => !(rect1 == rect2);


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private void Win_SourceInitialized(object? sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

    }
}
