using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace Models;

public class PlayInfoModel : ObservableObject, IEquatable<PlayInfoModel>
{
    private Guid _id;

    private TagLib.Tag? _tag;
    private string _albumText = "unknown album";
    private string _artistText = "unknown artist";
    private string _titleText = "unknown title";
    private string _yearText = "unknown year";
    private string _genreText = "unknown genre";
    private string _trackText = "unknown track";
    private string _discText = "unknown disc";
    private BitmapImage? _albumImage;
    private long _channelLength;
    private long _channelPosition;

    // 컨트롤 패널
    private bool _isPlaying;
    private bool _canPlay;

    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public TagLib.Tag? Tag
    {
        get => _tag;
        set => SetProperty(ref _tag, value);
    }

    public BitmapImage? AlbumImage
    {
        get => _albumImage;
        set => SetProperty(ref _albumImage, value);
    }

    public string ArtistText
    {
        get => string.IsNullOrWhiteSpace(_artistText) ? "unknown artist" : _artistText;
        set => SetProperty(ref _artistText, value);
    }

    public string TitleText
    {
        get => string.IsNullOrWhiteSpace(_titleText) ? "unknown title" : _titleText;
        set => SetProperty(ref _titleText, value);
    }

    public string YearText
    {
        get => string.IsNullOrWhiteSpace(_yearText) ? "unknown year" : _yearText;
        set => SetProperty(ref _yearText, value);
    }

    public string GenreText
    {
        get => string.IsNullOrWhiteSpace(_genreText) ? "unknown genre" : _genreText;
        set => SetProperty(ref _genreText, value);
    }

    public string TrackText
    {
        get => string.IsNullOrWhiteSpace(_trackText) ? "unknown track" : _trackText;
        set => SetProperty(ref _trackText, value);
    }

    public string DiscText
    {
        get => string.IsNullOrWhiteSpace(_discText) ? "unknown disc" : _discText;
        set => SetProperty(ref _discText, value);
    }

    public string AlbumText
    {
        get => string.IsNullOrWhiteSpace(_albumText) ? "unknown album" : _albumText;
        set => SetProperty(ref _albumText, value);
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

    public long ChannelLength
    {
        get => _channelLength;
        set => SetProperty(ref _channelLength, value);
    }

    public long ChannelPosition
    {
        get => _channelPosition;
        set
        {
            if (InTimerPositionUpdate is false)
                SetProperty(ref _channelPosition, value);
        }
    }

    public bool InTimerPositionUpdate { get; set; }

    public string? FilePath { get; set; }

    public void SetPlayInfo()
    {
        if (Tag is null) return;

        AlbumText = Tag.Album;
        ArtistText = Tag.AlbumArtists.Length > 0 ? Tag.AlbumArtists[0] : string.Empty;
        TitleText = Tag.Title;
        YearText = Tag.Year.ToString(CultureInfo.InvariantCulture);
        GenreText = Tag.Genres.Length > 0 ? Tag.Genres[0] : string.Empty;
        TrackText = Tag.Track.ToString(CultureInfo.InvariantCulture);
        DiscText = Tag.Disc.ToString(CultureInfo.InvariantCulture);

        if (Tag.Pictures.Length > 0)
        {
            using (MemoryStream albumArtworkMemStream = new MemoryStream(Tag.Pictures[0].Data.Data))
            {
                BitmapImage albumImage = new BitmapImage();
                albumImage.BeginInit();
                albumImage.CacheOption = BitmapCacheOption.OnLoad;
                albumImage.StreamSource = albumArtworkMemStream;
                albumImage.EndInit();

                AlbumImage = albumImage;

                albumArtworkMemStream.Close();
            }
        }
    }

    public bool Equals(PlayInfoModel? other)
    {
        throw new NotImplementedException();
    }
}
