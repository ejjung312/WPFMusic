using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MainEntry;

/// <summary>
/// WPFMusicShell.xaml에 대한 상호 작용 논리
/// </summary>
public partial class WPFMusicShell : Window
{
    private Rect _originWindow;

    public WPFMusicShell()
    {
        InitializeComponent();

        Loaded += WPFMusicShell_Loaded;
    }

    private void WPFMusicShell_Loaded(object sender, RoutedEventArgs e)
    {
        WindowInteropHelper interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
        Tag = interopHelper.Handle;
    }

    private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Minimized;

            _originWindow.Width = Width;
            _originWindow.Height = Height;
        } 
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        Close();
    }

    private void MinimizeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = xMaximizeToggleButton.IsChecked == true ? WindowState.Normal : WindowState.Maximized;
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
