using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace Models;

public class PlayInfoModel : ObservableObject, IEquatable<PlayInfoModel>
{
    private string _titleText = "unknown title";
    private string _artistText = "unknown artist";
    private BitmapImage? _albumImage;

    // 컨트롤 패널
    private bool _isPlaying;
    private bool _canPlay;

    public BitmapImage? AlbumImage
    {
        get => _albumImage;
        set => SetProperty(ref _albumImage, value);
    }

    public string TitleText
    {
        get => string.IsNullOrWhiteSpace(_titleText) ? "unknown title" : _titleText;
        set => SetProperty(ref _titleText, value);
    }

    public string ArtistText
    {
        get => string.IsNullOrWhiteSpace(_artistText) ? "unknown artist" : _artistText;
        set => SetProperty(ref _artistText, value);
    }

    public bool IsPlaying 
    { 
        get => _isPlaying; 
        set => SetProperty(ref _isPlaying, value); 
    }

    public bool CanPlay
    {
        get => _canPlay;
        set => SetProperty(ref _canPlay, value);
    }

    public bool Equals(PlayInfoModel? other)
    {
        throw new NotImplementedException();
    }
}
