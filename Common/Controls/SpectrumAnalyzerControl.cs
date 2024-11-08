using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Common.Controls;

public class SpectrumAnalyzerControl : Control
{
    private readonly DispatcherTimer animationTimer;
    private Canvas spectrumCanvas;
    private ISpectrumPlayer soundPlayer;
    private double barWidth = 1;
    private int maximumFrequencyIndex = 2047;
    private int minimumFrequencyIndex;
    private double bandWidth = 1.0;
    private float[] channelPeakData;
    private int[] barIndexMax;
    private int[] barLogScaleIndexMax;
    private double[] barHeights;
    private double[] peakHeights;
    private readonly List<Shape> barShapes = new List<Shape>();
    private readonly List<Shape> peakShapes = new List<Shape>();

    public ISpectrumPlayer RegisterSoundPlayer
    {
        get { return (ISpectrumPlayer)GetValue(RegisterSoundPlayerProperty); }
        set { SetValue(RegisterSoundPlayerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RegisterSoundPlayer.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RegisterSoundPlayerProperty =
        DependencyProperty.Register("RegisterSoundPlayer", typeof(ISpectrumPlayer), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(null, OnRegisterSoundPlayerChanged));

    private static void OnRegisterSoundPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = d as SpectrumAnalyzerControl;

        spectrumAnalyzerControl.soundPlayer = (ISpectrumPlayer)e.NewValue;
        spectrumAnalyzerControl.UpdateBarLayout();
        spectrumAnalyzerControl.animationTimer.Start();
    }

    public bool IsPlaying
    {
        get { return (bool)base.GetValue(IsPlayingProperty); }
        set { base.SetValue(IsPlayingProperty, value); }
    }

    public static readonly DependencyProperty IsPlayingProperty =
      DependencyProperty.Register("IsPlaying", typeof(bool), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(false, OnIsPlayingChanged));

    private static void OnIsPlayingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;

        if ((bool)e.NewValue && !spectrumAnalyzerControl.animationTimer.IsEnabled)
            spectrumAnalyzerControl.animationTimer.Start();
    }

    #region BarSpacing
    public double BarSpacing
    {
        get
        {
            return (double)GetValue(BarSpacingProperty);
        }
        set
        {
            SetValue(BarSpacingProperty, value);
        }
    }

    public static readonly DependencyProperty BarSpacingProperty =
        DependencyProperty.Register("BarSpacingProperty", typeof(double), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(5.0d, OnBarSpacingChanged, OnCoerceBarSpacing));

    protected virtual double OnCoerceBarSpacing(double value)
    {
        value = Math.Max(value, 0);
        return value;
    }

    private static object OnCoerceBarSpacing(DependencyObject d, object baseValue)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = d as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceBarSpacing((double)baseValue);
        else
            return baseValue;
    }

    protected virtual void OnBarSpacingChanged(double oldValue, double newValue)
    {
        UpdateBarLayout();
    }

    private static void OnBarSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = d as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnBarSpacingChanged((double)e.OldValue, (double)e.NewValue);
    }
    #endregion

    #region BarCount
    public int BarCount
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (int)GetValue(BarCountProperty);
        }
        set
        {
            SetValue(BarCountProperty, value);
        }
    }

    public static readonly DependencyProperty BarCountProperty = DependencyProperty.Register("BarCount", typeof(int), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(32, OnBarCountChanged, OnCoerceBarCount));

    private static object OnCoerceBarCount(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceBarCount((int)value);
        else
            return value;
    }

    private static void OnBarCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnBarCountChanged((int)e.OldValue, (int)e.NewValue);
    }

    protected virtual int OnCoerceBarCount(int value)
    {
        value = Math.Max(value, 1);
        return value;
    }

    protected virtual void OnBarCountChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }
    #endregion

    #region MaximumFrequency
    public static readonly DependencyProperty MaximumFrequencyProperty = DependencyProperty.Register("MaximumFrequency", typeof(int), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(20000, OnMaximumFrequencyChanged, OnCoerceMaximumFrequency));

    private static object OnCoerceMaximumFrequency(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceMaximumFrequency((int)value);
        else
            return value;
    }

    private static void OnMaximumFrequencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnMaximumFrequencyChanged((int)e.OldValue, (int)e.NewValue);
    }

    protected virtual int OnCoerceMaximumFrequency(int value)
    {
        if ((int)value < MinimumFrequency)
            return MinimumFrequency + 1;
        return value;
    }

    protected virtual void OnMaximumFrequencyChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }

    public int MaximumFrequency
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (int)GetValue(MaximumFrequencyProperty);
        }
        set
        {
            SetValue(MaximumFrequencyProperty, value);
        }
    }
    #endregion

    #region Minimum Frequency
    public static readonly DependencyProperty MinimumFrequencyProperty = DependencyProperty.Register("MinimumFrequency", typeof(int), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(20, OnMinimumFrequencyChanged, OnCoerceMinimumFrequency));

    private static object OnCoerceMinimumFrequency(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceMinimumFrequency((int)value);
        else
            return value;
    }

    private static void OnMinimumFrequencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnMinimumFrequencyChanged((int)e.OldValue, (int)e.NewValue);
    }

    protected virtual int OnCoerceMinimumFrequency(int value)
    {
        if (value < 0)
            return value = 0;
        CoerceValue(MaximumFrequencyProperty);
        return value;
    }

    protected virtual void OnMinimumFrequencyChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }

    public int MinimumFrequency
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (int)GetValue(MinimumFrequencyProperty);
        }
        set
        {
            SetValue(MinimumFrequencyProperty, value);
        }
    }
    #endregion

    #region BarStyle
    public static readonly DependencyProperty BarStyleProperty = DependencyProperty.Register("BarStyle", typeof(Style), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnBarStyleChanged), new CoerceValueCallback(OnCoerceBarStyle)));

    private static object OnCoerceBarStyle(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceBarStyle((Style)value);
        else
            return value;
    }

    private static void OnBarStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnBarStyleChanged((Style)e.OldValue, (Style)e.NewValue);
    }

    protected virtual Style OnCoerceBarStyle(Style value)
    {
        return value;
    }

    protected virtual void OnBarStyleChanged(Style oldValue, Style newValue)
    {
        UpdateBarLayout();
    }

    public Style BarStyle
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (Style)GetValue(BarStyleProperty);
        }
        set
        {
            SetValue(BarStyleProperty, value);
        }
    }
    #endregion

    #region PeakStyle
    public static readonly DependencyProperty PeakStyleProperty = DependencyProperty.Register("PeakStyle", typeof(Style), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPeakStyleChanged), new CoerceValueCallback(OnCoercePeakStyle)));

    private static object OnCoercePeakStyle(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoercePeakStyle((Style)value);
        else
            return value;
    }

    private static void OnPeakStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnPeakStyleChanged((Style)e.OldValue, (Style)e.NewValue);
    }

    protected virtual Style OnCoercePeakStyle(Style value)
    {

        return value;
    }

    protected virtual void OnPeakStyleChanged(Style oldValue, Style newValue)
    {
        UpdateBarLayout();
    }

    public Style PeakStyle
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (Style)GetValue(PeakStyleProperty);
        }
        set
        {
            SetValue(PeakStyleProperty, value);
        }
    }
    #endregion

    #region ActualBarWidth
    public static readonly DependencyProperty ActualBarWidthProperty = DependencyProperty.Register("ActualBarWidth", typeof(double), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(0.0d, new PropertyChangedCallback(OnActualBarWidthChanged), new CoerceValueCallback(OnCoerceActualBarWidth)));

    private static object OnCoerceActualBarWidth(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceActualBarWidth((double)value);
        else
            return value;
    }

    private static void OnActualBarWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnActualBarWidthChanged((double)e.OldValue, (double)e.NewValue);
    }

    protected virtual double OnCoerceActualBarWidth(double value)
    {
        return value;
    }

    protected virtual void OnActualBarWidthChanged(double oldValue, double newValue)
    {

    }

    public double ActualBarWidth
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (double)GetValue(ActualBarWidthProperty);
        }
        protected set
        {
            SetValue(ActualBarWidthProperty, value);
        }
    }
    #endregion

    #region AveragePeaks
    public static readonly DependencyProperty AveragePeaksProperty = DependencyProperty.Register("AveragePeaks", typeof(bool), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(false, OnAveragePeaksChanged, OnCoerceAveragePeaks));

    private static object OnCoerceAveragePeaks(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceAveragePeaks((bool)value);
        else
            return value;
    }

    private static void OnAveragePeaksChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnAveragePeaksChanged((bool)e.OldValue, (bool)e.NewValue);
    }

    protected virtual bool OnCoerceAveragePeaks(bool value)
    {
        return value;
    }

    protected virtual void OnAveragePeaksChanged(bool oldValue, bool newValue)
    {

    }

    public bool AveragePeaks
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (bool)GetValue(AveragePeaksProperty);
        }
        set
        {
            SetValue(AveragePeaksProperty, value);
        }
    }
    #endregion

    #region BarHeightScaling
    public static readonly DependencyProperty BarHeightScalingProperty = DependencyProperty.Register("BarHeightScaling", typeof(BarHeightScalingStyles), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(BarHeightScalingStyles.Decibel, OnBarHeightScalingChanged, OnCoerceBarHeightScaling));

    private static object OnCoerceBarHeightScaling(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoerceBarHeightScaling((BarHeightScalingStyles)value);
        else
            return value;
    }

    private static void OnBarHeightScalingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnBarHeightScalingChanged((BarHeightScalingStyles)e.OldValue, (BarHeightScalingStyles)e.NewValue);
    }

    protected virtual BarHeightScalingStyles OnCoerceBarHeightScaling(BarHeightScalingStyles value)
    {
        return value;
    }

    protected virtual void OnBarHeightScalingChanged(BarHeightScalingStyles oldValue, BarHeightScalingStyles newValue)
    {

    }

    public BarHeightScalingStyles BarHeightScaling
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (BarHeightScalingStyles)GetValue(BarHeightScalingProperty);
        }
        set
        {
            SetValue(BarHeightScalingProperty, value);
        }
    }
    #endregion

    #region PeakFallDelay
    public static readonly DependencyProperty PeakFallDelayProperty = DependencyProperty.Register("PeakFallDelay", typeof(int), typeof(SpectrumAnalyzerControl), new UIPropertyMetadata(10, OnPeakFallDelayChanged, OnCoercePeakFallDelay));

    private static object OnCoercePeakFallDelay(DependencyObject o, object value)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            return spectrumAnalyzerControl.OnCoercePeakFallDelay((int)value);
        else
            return value;
    }

    private static void OnPeakFallDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        SpectrumAnalyzerControl spectrumAnalyzerControl = o as SpectrumAnalyzerControl;
        if (spectrumAnalyzerControl != null)
            spectrumAnalyzerControl.OnPeakFallDelayChanged((int)e.OldValue, (int)e.NewValue);
    }

    protected virtual int OnCoercePeakFallDelay(int value)
    {
        value = Math.Max(value, 0);
        return value;
    }

    protected virtual void OnPeakFallDelayChanged(int oldValue, int newValue)
    {

    }

    public int PeakFallDelay
    {
        // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        get
        {
            return (int)GetValue(PeakFallDelayProperty);
        }
        set
        {
            SetValue(PeakFallDelayProperty, value);
        }
    }
    #endregion

    private void UpdateBarLayout()
    {
        if (soundPlayer == null || spectrumCanvas == null)
            return;

        barWidth = Math.Max(((double)(spectrumCanvas.RenderSize.Width - (BarSpacing * (BarCount + 1))) / (double)BarCount), 1);
        maximumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MaximumFrequency) + 1, 2047);
        minimumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MinimumFrequency), 2047);
        bandWidth = Math.Max(((double)(maximumFrequencyIndex - minimumFrequencyIndex)) / spectrumCanvas.RenderSize.Width, 1.0);

        int actualBarCount;
        if (barWidth >= 1.0d)
            actualBarCount = BarCount;
        else
            actualBarCount = Math.Max((int)((spectrumCanvas.RenderSize.Width - BarSpacing) / (barWidth + BarSpacing)), 1);
        channelPeakData = new float[actualBarCount];

        int indexCount = maximumFrequencyIndex - minimumFrequencyIndex;
        int linearIndexBucketSize = (int)Math.Round((double)indexCount / (double)actualBarCount, 0);
        List<int> maxIndexList = new List<int>();
        List<int> maxLogScaleIndexList = new List<int>();
        double maxLog = Math.Log(actualBarCount, actualBarCount);
        for (int i = 1; i < actualBarCount; i++)
        {
            maxIndexList.Add(minimumFrequencyIndex + (i * linearIndexBucketSize));
            int logIndex = (int)((maxLog - Math.Log((actualBarCount + 1) - i, (actualBarCount + 1))) * indexCount) + minimumFrequencyIndex;
            maxLogScaleIndexList.Add(logIndex);
        }
        maxIndexList.Add(maximumFrequencyIndex);
        maxLogScaleIndexList.Add(maximumFrequencyIndex);
        barIndexMax = maxIndexList.ToArray();
        barLogScaleIndexMax = maxLogScaleIndexList.ToArray();

        barHeights = new double[actualBarCount];
        peakHeights = new double[actualBarCount];

        spectrumCanvas.Children.Clear();
        barShapes.Clear();
        peakShapes.Clear();

        double height = spectrumCanvas.RenderSize.Height;
        double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
        for (int i = 0; i < actualBarCount; i++)
        {
            double xCoord = BarSpacing + (barWidth * i) + (BarSpacing * i) + 1;
            Rectangle barRectangle = new Rectangle()
            {
                Margin = new Thickness(xCoord, height, 0, 0),
                Width = barWidth,
                Height = 0,
                Style = BarStyle
            };
            barShapes.Add(barRectangle);
            Rectangle peakRectangle = new Rectangle()
            {
                Margin = new Thickness(xCoord, height - peakDotHeight, 0, 0),
                Width = barWidth,
                Height = peakDotHeight,
                Style = PeakStyle
            };
            peakShapes.Add(peakRectangle);
        }

        foreach (Shape shape in barShapes)
            spectrumCanvas.Children.Add(shape);
        foreach (Shape shape in peakShapes)
            spectrumCanvas.Children.Add(shape);

        ActualBarWidth = barWidth;
    }

    public enum BarHeightScalingStyles
    {
        Decibel,
        Sqrt,
        Linear
    }
}
