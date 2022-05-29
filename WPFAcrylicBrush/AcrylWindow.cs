using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WPFAcrylics
{
    /// <summary>
    /// A <see cref="Window"/> derivative that adds an acrylic blur effect to the window background.<br/>
    /// This variant of <see cref="BasicAcrylWindow"/> comes with a replica of the Windows 10 titlebar, and some properties to control it.
    /// </summary>
    public class AcrylWindow : BasicAcrylWindow
    {
        #region Fields
        private bool _nowFullScreen = false;
        #endregion Fields

        #region Properties
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
        #endregion Properties

        #region Methods
        protected override Grid BuildBaseWindow()
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
            windowGrid.RowDefinitions.Add(new RowDefinition
            {
                MaxHeight = 30
            });
            windowGrid.RowDefinitions.Add(new RowDefinition());
            _ = _mainGrid.Children.Add(windowGrid);

            Grid titleBar = BuildTitleBar();

            _ = windowGrid.Children.Add(titleBar);
            _ = windowGrid.Children.Add(_contentGrid);
            return _mainGrid;
        }

        private Grid BuildTitleBar()
        {

            // Build the close button
            var closeButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "\uE711",
                FontSize = 14,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                MinWidth = 45,
            };
            _ = closeButton.SetBinding(VisibilityProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("ShowCloseButton"),
                TargetNullValue = Visibility.Visible
            });
            Grid.SetColumn(closeButton, 2);
            closeButton.Click += (x, y) => Close();


            // Build the maximize/reset button
            var fullscreenButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "\uE922",
                FontSize = 12,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                MinWidth = 45
            };
            _ = fullscreenButton.SetBinding(VisibilityProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("ShowFullscreenButton"),
                TargetNullValue = Visibility.Visible
            });
            Grid.SetColumn(fullscreenButton, 1);

            fullscreenButton.Click += (x, y) =>
            {
                if (_nowFullScreen)
                {
                    WindowState = WindowState.Normal;
                    _nowFullScreen = false;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                    _nowFullScreen = true;
                }
            };



            // Build the minimize button
            var minimizeButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "\uE921",
                FontSize = 12,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                MinWidth = 45,
            };
            _ = minimizeButton.SetBinding(VisibilityProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("ShowMinimizeButton"),
                TargetNullValue = Visibility.Visible
            });
            Grid.SetColumn(minimizeButton, 0);
            minimizeButton.Click += (x, y) => WindowState = WindowState.Minimized;

            var titleBar = new Grid
            {
                MaxHeight = 30
            };
            var titleBarButtons = new Grid
            {
                MaxHeight = 30,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            titleBarButtons.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            titleBarButtons.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            titleBarButtons.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });


            // Build the drag button - workaround for still dragging/double clicking the window
            var dragButton = new Button
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = Brushes.Transparent
            };
            var res = new ResourceDictionary
            {
                Source = new Uri($"/{AssemblyName.GetName};component/XAML/StyleResources.xaml", UriKind.RelativeOrAbsolute)
            };
            dragButton.Style = res["NoHoverStyle"] as Style;
            dragButton.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                DragMove();
                if (e.ClickCount == 2)
                    OnTitleBarDoubleClick();
            };

            int frames = 0;
            bool inDrag = false;
            dragButton.PreviewMouseMove += (sender, e) =>
            {
                if (inDrag && e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                    inDrag = false;
                    return;
                }
                if (e.LeftButton == MouseButtonState.Pressed && _nowFullScreen)
                {
                    if (frames == 3)
                    {
                        Point p = Mouse.GetPosition(this);
                        WindowState = WindowState.Normal;
                        Top = p.Y - 5;
                        Left = p.X - Width / 2;
                        _nowFullScreen = false;
                        DragMove();
                        inDrag = true;
                    }
                    else
                    {
                        frames++;
                    }
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    frames = 0;
                }
            };

            // Add all buttons the scenery
            _ = titleBar.Children.Add(dragButton);
            _ = titleBar.Children.Add(titleBarButtons);
            _ = titleBarButtons.Children.Add(closeButton);
            _ = titleBarButtons.Children.Add(fullscreenButton);
            _ = titleBarButtons.Children.Add(minimizeButton);

            return titleBar;
        }


        private void OnTitleBarDoubleClick()
        {
            if (_nowFullScreen)
            {
                WindowState = WindowState.Normal;
                _nowFullScreen = false;
            }
            else
            {
                WindowState = WindowState.Maximized;
                _nowFullScreen = true;
            }
        }
        #endregion Methods
    }
}
