using Common.Controls;
using Models;
using System.IO;
using System.Windows;

namespace Services;

public interface IBassService : ISpectrumPlayer
{
    public PlayInfoModel? PlayInfo { get; }

    public void SetChannelPosition(long position);

    public bool SetVolume(float volume);

    public void Play();

    public TagLib.Tag? GetTag(string filePath);

    public bool OpenFile(PlayInfoModel playInfo);
}

public class BassService : IBassService
{
    private PlayInfoModel? _playInfo;

    public int ActiveStreamHandle => throw new NotImplementedException();

    public PlayInfoModel? PlayInfo => _playInfo;

    public int GetFFTData(float[] fftDataBuffer)
    {
        throw new NotImplementedException();
    }

    public int GetFFTFrequencyIndex(int frequency)
    {
        throw new NotImplementedException();
    }

    public void SetChannelPosition(long position)
    {
        throw new NotImplementedException();
    }

    public bool SetVolume(float volume)
    {
        return true;
    }

    public TagLib.Tag? GetTag(string filePath)
    {
        if (File.Exists(filePath))
        {
            var fs = TagLib.File.Create(filePath);
            if (fs is not null)
            {
                return fs.Tag;
            }
            else
            {
                return null;
            }
        } 
        else
        {
            return null;
        }
    }

    public bool OpenFile(PlayInfoModel playInfo)
    {
        throw new NotImplementedException();
    }

    public void Play()
    {
        // 여기서 시작
        if (_canPlay)
        {
            this.PlayCurrentStream();
            _canPlay = false;

            if (PlayInfo is not null)
            {
                PlayInfo.IsPause = false;
                PlayInfo.CanPlay = CanPlay;
                PlayInfo.IsPlaying = true;
                PlayInfo.CanStop = true;

                if (_sem.CurrentCount < 1)
                {
                    System.Threading.Thread.Sleep(500);
                    _sem.Release(1);
                }
            }
            else
            {
                if (_sem.CurrentCount < 1)
                {
                    System.Threading.Thread.Sleep(500);
                    _sem.Release(1);
                    System.Threading.Thread.Sleep(500);
                    this.ChannelPostionTaskEnd();
                }

                Logger.Log.Write("PlayInfo is null!");
                MessageBox.Show("PlayInfo is null!");
            }
        }
    }
}