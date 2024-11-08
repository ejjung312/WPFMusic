using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.Controls;

public class AlbumArtworkPanelConstrol : Control
{
    public AlbumArtworkPanelConstrol()
    {
        DefaultStyleKey = typeof(AlbumArtworkPanelConstrol);
    }

    public ImageSource? AlbumArtImage
    {
        get { return GetValue(AlbumArtImageProperty) as ImageSource; }
        set { SetValue(AlbumArtImageProperty, value); }
    }

    public static readonly DependencyProperty AlbumArtImageProperty =
        DependencyProperty.Register("AlbumArtImage", typeof(ImageSource), typeof(AlbumArtworkPanelConstrol));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
}
