using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using ViewModels;

namespace MainEntry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string _title = "WPF Music";

        public App()
        {
            new Bootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // IoC 컨테이너에 등록된 ShellViewModel를 가져오고 null이 아닌 값으로 간주하도록 보장(!)
            ShellViewModel shellViewModel = Ioc.Default.GetService<ShellViewModel>()!;
            shellViewModel.CurrentDataContext = Ioc.Default.GetService<MainViewModel>();

            var shellWindow = new WPFMusicShell();
            shellWindow.DataContext = shellViewModel;
            shellWindow.Width = shellViewModel.WindowWidth;
            shellWindow.Height = shellViewModel.WindowHeight;
            shellWindow.ShowDialog();

            // ShellViewModel 정리
            shellViewModel.Cleanup();
        }

        public static string ProductTitle
        {
            get { return _title; }
        }
    }
}
