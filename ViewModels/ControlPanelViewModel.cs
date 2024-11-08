using Common.Base;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ViewModels.Messaging;

namespace ViewModels
{
    public class ControlPanelViewModel : ViewModelBase
    {
        private readonly ISettingService _settingService;
        private readonly IBassService _bassService;
        private ObservableCollection<PlayInfoModel>? _playInfoList;
        private PlayInfoModel _playInfoModel;
        private float _volumePosition;

        public ControlPanelViewModel(ISettingService settingService, IBassService bassService)
        {
            _settingService = settingService;
            _bassService = bassService;

            VolumePosition = _settingService.PlaySetting!.Volume;
        }

        public PlayInfoModel PlayInfoModel
        {
            get => _playInfoModel;
            set => SetProperty(ref _playInfoModel, value);
        }

        private RelayCommand<object?>? _fileOpenCommand;
        public RelayCommand<object?>? FileOpenCommand
        {
            get
            {
                return _fileOpenCommand ??
                    (_fileOpenCommand = new RelayCommand<object?>(this.FileOpenExecute));
            }
        }

        private RelayCommand _play_pauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return _play_pauseCommand ??
                    (_play_pauseCommand = new RelayCommand(this.PlayPauseExecute, this.CanPlayPauseExecute));
            }
        }

        private bool CanPlayPauseExecute()
        {
            if (PlayInfoModel is null) return false;

            if (PlayInfoModel.IsPlaying is false)
            {
                return PlayInfoModel.CanPlay;
            }
            else
            {
                return PlayInfoModel.IsPlaying;
            }
        }

        private void PlayPauseExecute()
        {
            try
            {
                if (PlayInfoModel.CanPlay is true &&
                    PlayInfoModel.IsPlaying is false)
                {
                    this.Play();
                }
                else if (PlayInfoModel.CanPlay is false &&
                    PlayInfoModel.IsPlaying is true)
                {
                    _bassService.Pause();
                    PlayPauseCommand.NotifyCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        private void Play()
        {
            // 최초 OpenFile 이전에는 볼륨 세팅이 되지 않기 때문에 Play시 볼륨 세팅한다.
            _bassService.SetVolume(VolumePosition);
            _bassService.Play();
            PlayPauseCommand.NotifyCanExecuteChanged();

            WeakReferenceMessenger.Default.Send(new SetPlayInfoMessage(PlayInfoModel));
        }

        private void FileOpenExecute(object? filePaths)
        {
            if (filePaths is null) return;

            string[] filePathArr = (string[])filePaths;

            try
            {
                // PlayInfoModel의 데이터변경을 감지하여 View를 업데이트함
                ObservableCollection<PlayInfoModel> playInfoList = new ObservableCollection<PlayInfoModel>();
                foreach (var filePath in filePathArr)
                {
                    TagLib.Tag tag = _bassService.GetTag(filePath);
                    if (tag is null)
                    {
                        Logger.Log.Write($"음원 tag정보를 추출할 수 없습니다. - {filePath}");
                        continue;
                    }
                    PlayInfoModel playInfo = new PlayInfoModel();
                    playInfo.Id = Guid.NewGuid();
                    playInfo.FilePath = filePath;
                    playInfo.Tag = tag;
                    playInfo.SetPlayInfo();

                    playInfoList.Add(playInfo);
                    _playInfoList = playInfoList;
                }

                PlayInfoModel = playInfoList[0];

                var result = _bassService.OpenFile(PlayInfoModel!);

                if (result is true)
                {
                    WeakReferenceMessenger.Default.Send(new SetPlayInfoListMessage(playInfoList));
                    WeakReferenceMessenger.Default.Send(new SetPlayInfoMessage(playInfoList[0]));
                    PlayPauseCommand.NotifyCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public float VolumePosition
        {
            get => _volumePosition;
            set
            {
                if (_bassService.SetVolume(value) is true)
                {
                    _settingService.PlaySetting!.Volume = value;
                }
                SetProperty(ref _volumePosition, value);
            }
        }

        private RelayCommand _readyCangeChannelPostionCommand;
        public RelayCommand ReadyCangeChannelPostionCommand
        {
            get
            {
                return _readyCangeChannelPostionCommand ??
                    (_readyCangeChannelPostionCommand = new RelayCommand(this.ReadyCangeChannelPostionExecute));
            }
        }

        private RelayCommand<double> _updateChannelPositionCommand;
        public RelayCommand<double> UpdateChannelPositionCommand
        {
            get
            {
                return _updateChannelPositionCommand ??
                    (_updateChannelPositionCommand = new RelayCommand<double>(this.UpdateChannelPostionExecute));
            }
        }

        private RelayCommand<double> _changingChannelPostionCommand;
        public RelayCommand<double> ChangingChannelPostionCommand
        {
            get
            {
                return _changingChannelPostionCommand ??
                    (_changingChannelPostionCommand = new RelayCommand<double>(this.ChangingChannelPostionExecute));
            }
        }

        private void ChangingChannelPostionExecute(double value)
        {
            if (PlayInfoModel.InTimerPositionUpdate is true)
                PlayInfoModel.ChannelPosition = (long)value;
        }

        private void UpdateChannelPostionExecute(double value)
        {
            try
            {
                _bassService.SetChannelPosition((long)value);
                PlayInfoModel.InTimerPositionUpdate = false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        private void ReadyCangeChannelPostionExecute()
        {
            PlayInfoModel.InTimerPositionUpdate = true;
        }
    }
}
