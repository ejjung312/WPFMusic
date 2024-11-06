using Common.Base;
using Common.Enums;
using CommunityToolkit.Mvvm.Input;
using Services;

namespace ViewModels;

public class ShellViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    // ? -> null을 가질 수 있음
    private object? _currentDataContext;

    private double _windowWidth;
    private double _windowHeight;
	private double _windowLeft = 500;
	private double _windowTop = 500;
	private bool _windowTopMost = false;

    public ShellViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public object? CurrentDataContext
    {
        get => _currentDataContext;
        set => SetProperty(ref _currentDataContext, value); // SetProperty: 값이 있으면 업데이트. ref: 참조 전달
    }
	
	public double WindowWidth
	{
		get => _windowWidth;
		set => OnPropertyChanged(nameof(WindowWidth));
	}
	
	public double WindowHeight
	{
		get => _windowHeight;
		set => OnPropertyChanged(nameof(WindowHeight));
	}

    public double WindowLeft
    {
        get => _windowLeft;
        set => OnPropertyChanged(nameof(WindowLeft));
    }

    public double WindowTop
    {
        get => _windowTop;
        set => OnPropertyChanged(nameof(WindowTop));
    }

    #region Commands
    private RelayCommand? _mainSettingCommand;
    public RelayCommand? MainSettingCommand
    {
        get
        {
            return _mainSettingCommand ?? (_mainSettingCommand = new RelayCommand(this.MainSettingExecute));
        }
    }
    #endregion

    #region Commands Execute Methods
    private void MainSettingExecute()
    {
        if (_dialogService.CheckActivate("설정") is true)
        {

        } 
        else
        {
            _dialogService.SetVM(new MainSettingPopupViewModel(), "설정", 500, 650, EDialogHostType.BasicType);
        }
    }
    #endregion
}