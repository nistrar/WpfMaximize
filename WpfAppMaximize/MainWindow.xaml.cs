// Copyright © Selmo Technology GmbH. All Rights Reserved

using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WpfAppMaximize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _restoreLeft;
        private double _restoreTop;
        private double _restoreWidth;
        private double _restoreHeight;
        private bool _isSimulatedMaximized = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeSimulated();
            }
        }

        private void MaximizeSimulated()
        {
            _isSimulatedMaximized = true;

            _restoreLeft = this.Left;
            _restoreTop = this.Top;
            _restoreWidth = this.Width;
            _restoreHeight = this.Height;

            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Normal;

            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var workingArea = screen.WorkingArea;

            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;

            ToggleMaximizeRestoreButtons(true);
        }

        private void ToggleMaximizeRestoreButtons(bool isMaximized)
        {
            ButtonRestore.Visibility = isMaximized ? Visibility.Visible : Visibility.Collapsed;
            ButtonMaximize.Visibility = isMaximized ? Visibility.Collapsed : Visibility.Visible;
        }

        private void RestoreWindow(double left, double top)
        {
            _isSimulatedMaximized = false;

            ResizeMode = ResizeMode.CanResize;
            WindowState = WindowState.Normal;

            this.Left = left;
            this.Top = top;
            this.Width = _restoreWidth;
            this.Height = _restoreHeight;

            ToggleMaximizeRestoreButtons(false);
        }

        private void GridTitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                SwitchWindowState();
            }
        }

        private void SwitchWindowState()
        {
            if (_isSimulatedMaximized)
            {
                RestoreWindow(_restoreLeft, _restoreTop);
            }
            else
            {
                MaximizeSimulated();
            }
        }

        private void GridTitleBar_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_isSimulatedMaximized)
                {
                    var mousePos = System.Windows.Forms.Cursor.Position;
                    RestoreWindow(mousePos.X - (_restoreWidth / 2), mousePos.Y);
                }

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
        }

        private void ButtonMinimize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
            e.Handled = true;
        }

        private void ButtonRestore_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RestoreWindow(_restoreLeft, _restoreTop);
        }

        private void ButtonMaximize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void ButtonClose_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}