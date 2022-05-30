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
using System.Windows.Shell;
using WPFAcrylics.Enums;
using WPFAcrylics.Structs;

namespace WPFAcrylics
{
    /// <summary>
    /// Acrylic-background window <b>without</b> a title bar.<br/>
    /// This is intended to allow you to create your own title bar using <see cref="WindowChrome"/>.<br/><br/>
    /// Also has the benefit of being drag-resizable.
    /// </summary>
    public class BasicAcrylWindow : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal void EnableBlur()
        {
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

            _ = SetWindowCompositionAttribute(Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        /// <summary>Commonly called <b>hWnd</b>; the is the Window Handle.</summary>
        public IntPtr Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();
        private IntPtr? _handle;

        /// <summary>
        /// Property for changing the color of the TransparentBackground
        /// </summary>
        public Brush TransparentBackground
        {
            get => _transparentBackground;
            set
            {
                _transparentBackground = value;
                NotifyPropertyChanged();
            }
        }

        private Brush _transparentBackground;



        /// <summary>
        /// Changes the opacity of the Acrylic transparent background
        /// </summary>
        public double AcrylOpacity
        {
            get => _acrylOpacity;
            set
            {
                _acrylOpacity = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }
        private double _noiseRatio;



        /// <summary>
        /// Event implementation
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));



        private readonly Grid _mainGrid;
        private readonly Grid _contentGrid;
        private readonly string _internalGridName;


        /// <summary>
        /// Constructor
        /// </summary>
        public BasicAcrylWindow()
        {
            _transparentBackground = default!;
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



        /// <summary>
        /// 
        /// </summary>
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
            _contentGrid.Children.Add((UIElement)arg2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Grid BuildBaseWindow()
        {

            // Transparent effect rectangle
            var rect = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            rect.SetBinding(Shape.FillProperty, new Binding
            {
                Path = new PropertyPath("TransparentBackground"),
                Source = this,
                FallbackValue = Brushes.LightGray,
                TargetNullValue = Brushes.LightGray
            });
            rect.SetBinding(OpacityProperty, new Binding
            {
                Path = new PropertyPath("AcrylOpacity"),
                Source = this,
                FallbackValue = 0.6,
                TargetNullValue = 0.6
            });

            // Add the noise effect to the rectangle
            var fx = new NoiseEffect.NoiseEffect();
            BindingOperations.SetBinding(fx, NoiseEffect.NoiseEffect.RatioProperty, new Binding
            {
                Path = new PropertyPath("NoiseRatio"),
                TargetNullValue = 0.1,
                FallbackValue = 0.1,
                Source = this,
            });
            rect.Effect = fx;


            _mainGrid.Children.Add(rect);

            var windowGrid = new Grid();
            windowGrid.RowDefinitions.Add(new RowDefinition());
            _mainGrid.Children.Add(windowGrid);

            windowGrid.Children.Add(_contentGrid);
            return _mainGrid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {

            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {

                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private void Win_SourceInitialized(object? sender, EventArgs e) => HwndSource.FromHwnd(Handle).AddHook(new HwndSourceHook(WindowProc));

    }
}
