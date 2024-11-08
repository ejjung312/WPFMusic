using Common.Base;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Properties
    public AlbumArtInfoViewModel? AlbumArtInfo
    {
        get => Ioc.Default.GetService<AlbumArtInfoViewModel>();
    }

    public ControlPanelViewModel? ControlPanel
    {
        get => Ioc.Default.GetService<ControlPanelViewModel>();
    }

    public PlayListViewModel? PlayList
    {
        get => Ioc.Default.GetService<PlayListViewModel>();
    }
    #endregion

    #region Commands
    private RelayCommand _unloadedCommand;
    public RelayCommand UnloadedCommand
    {
        get
        {
            return _unloadedCommand ?? (_unloadedCommand = new RelayCommand(UnloadedExecute));
        }
    }
    #endregion

    private void UnloadedExecute()
    {
        AlbumArtInfo?.Cleanup();
        ControlPanel?.Cleanup();
        PlayList?.Cleanup();

        this.Cleanup();
    }
}
