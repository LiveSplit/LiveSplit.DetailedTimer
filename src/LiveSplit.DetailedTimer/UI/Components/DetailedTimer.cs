using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components;

[GlobalFontConsumer(GlobalFont.TimerFont)]
public class DetailedTimer : IComponent
{
    public Timer InternalComponent { get; set; }
    public SegmentTimer SegmentTimer { get; set; }
    public SimpleLabel LabelSegment { get; set; }
    public SimpleLabel LabelBest { get; set; }
    public SimpleLabel SegmentTime { get; set; }
    public SimpleLabel BestSegmentTime { get; set; }
    public SimpleLabel SplitName { get; set; }
    public DetailedTimerSettings Settings { get; set; }
    public GraphicsCache Cache { get; set; }
    public string Comparison { get; set; }
    public string Comparison2 { get; set; }
    public string ComparisonName { get; set; }
    public string ComparisonName2 { get; set; }
    public bool HideComparison1 { get; set; }
    public bool HideComparison2 { get; set; }
    protected int FrameCount { get; set; }

    protected int IconWidth { get; set; }

    public Image ShadowImage { get; set; }
    protected Image OldImage { get; set; }

    public float PaddingTop => 0f;
    public float PaddingLeft => 7f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 7f;

    public float VerticalHeight => Settings.Height;

    public float HorizontalWidth => Settings.Width;

    public float MinimumWidth => 20;

    public float MinimumHeight => 20;

    public IDictionary<string, Action> ContextMenuControls => null;

    private readonly Regex SubsplitRegex = new(@"^{(.+)}\s*(.+)$", RegexOptions.Compiled);

    public DetailedTimer(LiveSplitState state)
    {
        InternalComponent = new Timer();
        SegmentTimer = new SegmentTimer();
        Settings = new DetailedTimerSettings()
        {
            CurrentState = state
        };
        IconWidth = 0;
        Cache = new GraphicsCache();
        LabelSegment = new SimpleLabel();
        LabelBest = new SimpleLabel();
        SegmentTime = new SimpleLabel();
        BestSegmentTime = new SimpleLabel();
        SplitName = new SimpleLabel();
        state.ComparisonRenamed += state_ComparisonRenamed;
    }

    private void state_ComparisonRenamed(object sender, EventArgs e)
    {
        var args = (RenameEventArgs)e;
        if (Settings.Comparison == args.OldName)
        {
            Settings.Comparison = args.NewName;
            ((LiveSplitState)sender).Layout.HasChanged = true;
        }

        if (Settings.Comparison2 == args.OldName)
        {
            Settings.Comparison2 = args.NewName;
            ((LiveSplitState)sender).Layout.HasChanged = true;
        }
    }

    public void DrawGeneral(Graphics g, LiveSplitState state, float width, float height)
    {
        Timer.DrawBackground(g, InternalComponent.TimerColor, Settings.BackgroundColor, Settings.BackgroundColor2, width, height, Settings.BackgroundGradient);

        int lastSplitOffset = state.CurrentSplitIndex == state.Run.Count ? -1 : 0;

        float originalDrawSize = Math.Min(Settings.IconSize, width - 14);
        Image icon = state.CurrentSplitIndex >= 0 ? state.Run[state.CurrentSplitIndex + lastSplitOffset].Icon : null;
        if (Settings.DisplayIcon && icon != null)
        {
            if (OldImage != icon)
            {
                ImageAnimator.Animate(icon, (s, o) => { });
                OldImage = icon;
            }

            float drawWidth = originalDrawSize;
            float drawHeight = originalDrawSize;
            if (icon.Width > icon.Height)
            {
                float ratio = icon.Height / (float)icon.Width;
                drawHeight *= ratio;
            }
            else
            {
                float ratio = icon.Width / (float)icon.Height;
                drawWidth *= ratio;
            }

            ImageAnimator.UpdateFrames(icon);

            g.DrawImage(
                icon,
                7 + ((originalDrawSize - drawWidth) / 2),
                ((height - originalDrawSize) / 2.0f) + ((originalDrawSize - drawHeight) / 2),
                drawWidth,
                drawHeight);

            IconWidth = (int)(originalDrawSize + 7.5f);
        }
        else
        {
            IconWidth = 0;
        }

        InternalComponent.Settings.ShowGradient = Settings.TimerShowGradient;
        InternalComponent.Settings.OverrideSplitColors = Settings.OverrideTimerColors;
        InternalComponent.Settings.TimerColor = Settings.TimerColor;
        InternalComponent.Settings.DigitsFormat = Settings.DigitsFormat;
        InternalComponent.Settings.Accuracy = Settings.Accuracy;
        InternalComponent.Settings.DecimalsSize = Settings.DecimalsSize;
        SegmentTimer.Settings.ShowGradient = Settings.SegmentTimerShowGradient;
        SegmentTimer.Settings.OverrideSplitColors = true;
        SegmentTimer.Settings.TimerColor = Settings.SegmentTimerColor;
        SegmentTimer.Settings.DigitsFormat = Settings.SegmentDigitsFormat;
        SegmentTimer.Settings.Accuracy = Settings.SegmentAccuracy;
        SegmentTimer.Settings.DecimalsSize = Settings.SegmentTimerDecimalsSize;

        if (state.CurrentSplitIndex >= 0)
        {
            var labelsFont = new Font(Settings.SegmentLabelsFont.FontFamily, Settings.SegmentLabelsFont.Size, Settings.SegmentLabelsFont.Style, Settings.SegmentLabelsFont.Unit);
            var timesFont = new Font(Settings.SegmentTimesFont.FontFamily, Settings.SegmentTimesFont.Size, Settings.SegmentTimesFont.Style, Settings.SegmentTimesFont.Unit);
            // The visible comparisons share the space next to the segment timer equally.
            int visibleComparisons = (HideComparison1 ? 0 : 1) + (HideComparison2 ? 0 : 1);
            float comparisonsY = height * ((100f - Settings.SegmentTimerSizeRatio) / 100f);
            float comparisonHeight = height * (Settings.SegmentTimerSizeRatio / 100f) / Math.Max(visibleComparisons, 1);
            float comparison2Y = HideComparison1 ? comparisonsY : comparisonsY + comparisonHeight;

            LabelSegment.Font = labelsFont;
            LabelSegment.X = 5 + IconWidth;
            LabelSegment.Y = comparisonsY;
            LabelSegment.Width = width - SegmentTimer.ActualWidth - 5 - IconWidth;
            LabelSegment.Height = comparisonHeight;
            LabelSegment.HorizontalAlignment = StringAlignment.Near;
            LabelSegment.VerticalAlignment = StringAlignment.Center;
            LabelSegment.ForeColor = Settings.SegmentLabelsColor;
            LabelSegment.HasShadow = state.LayoutSettings.DropShadows;
            LabelSegment.ShadowColor = state.LayoutSettings.ShadowsColor;
            LabelSegment.OutlineColor = state.LayoutSettings.TextOutlineColor;
            if (!HideComparison1)
            {
                LabelSegment.Draw(g);
            }

            LabelBest.Font = labelsFont;
            LabelBest.X = 5 + IconWidth;
            LabelBest.Y = comparison2Y;
            LabelBest.Width = width - SegmentTimer.ActualWidth - 5 - IconWidth;
            LabelBest.Height = comparisonHeight;
            LabelBest.HorizontalAlignment = StringAlignment.Near;
            LabelBest.VerticalAlignment = StringAlignment.Center;
            LabelBest.ForeColor = Settings.SegmentLabelsColor;
            LabelBest.HasShadow = state.LayoutSettings.DropShadows;
            LabelBest.ShadowColor = state.LayoutSettings.ShadowsColor;
            LabelBest.OutlineColor = state.LayoutSettings.TextOutlineColor;
            if (!HideComparison2)
            {
                LabelBest.Draw(g);
            }

            float offset = Math.Max(HideComparison1 ? 0 : LabelSegment.ActualWidth, HideComparison2 ? 0 : LabelBest.ActualWidth) + 10;

            if (!HideComparison1)
            {
                SegmentTime.Font = timesFont;
                SegmentTime.X = offset + IconWidth;
                SegmentTime.Y = comparisonsY;
                SegmentTime.Width = width - SegmentTimer.ActualWidth - offset - IconWidth;
                SegmentTime.Height = comparisonHeight;
                SegmentTime.HorizontalAlignment = StringAlignment.Near;
                SegmentTime.VerticalAlignment = StringAlignment.Center;
                SegmentTime.ForeColor = Settings.SegmentTimesColor;
                SegmentTime.HasShadow = state.LayoutSettings.DropShadows;
                SegmentTime.ShadowColor = state.LayoutSettings.ShadowsColor;
                SegmentTime.OutlineColor = state.LayoutSettings.TextOutlineColor;
                SegmentTime.IsMonospaced = true;
                SegmentTime.Draw(g);
            }

            if (!HideComparison2)
            {
                BestSegmentTime.Font = timesFont;
                BestSegmentTime.X = offset + IconWidth;
                BestSegmentTime.Y = comparison2Y;
                BestSegmentTime.Width = width - SegmentTimer.ActualWidth - offset - IconWidth;
                BestSegmentTime.Height = comparisonHeight;
                BestSegmentTime.HorizontalAlignment = StringAlignment.Near;
                BestSegmentTime.VerticalAlignment = StringAlignment.Center;
                BestSegmentTime.ForeColor = Settings.SegmentTimesColor;
                BestSegmentTime.HasShadow = state.LayoutSettings.DropShadows;
                BestSegmentTime.ShadowColor = state.LayoutSettings.ShadowsColor;
                BestSegmentTime.OutlineColor = state.LayoutSettings.TextOutlineColor;
                BestSegmentTime.IsMonospaced = true;
                BestSegmentTime.Draw(g);
            }

            SplitName.Font = Settings.SplitNameFont;
            SplitName.X = IconWidth + 5;
            SplitName.Y = 0;
            SplitName.Width = width - InternalComponent.ActualWidth - IconWidth - 5;
            SplitName.Height = height * ((100f - Settings.SegmentTimerSizeRatio) / 100f);
            SplitName.HorizontalAlignment = StringAlignment.Near;
            SplitName.VerticalAlignment = StringAlignment.Center;
            SplitName.ForeColor = Settings.SplitNameColor;
            SplitName.HasShadow = state.LayoutSettings.DropShadows;
            SplitName.ShadowColor = state.LayoutSettings.ShadowsColor;
            SplitName.OutlineColor = state.LayoutSettings.TextOutlineColor;
            if (Settings.ShowSplitName)
            {
                SplitName.Draw(g);
            }
        }
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight);
        Matrix oldMatrix = g.Transform;
        InternalComponent.Settings.TimerHeight = VerticalHeight * ((100f - Settings.SegmentTimerSizeRatio) / 100f);
        InternalComponent.DrawVertical(g, state, width, clipRegion);
        g.Transform = oldMatrix;
        g.TranslateTransform(0, VerticalHeight * ((100f - Settings.SegmentTimerSizeRatio) / 100f));
        SegmentTimer.Settings.TimerHeight = VerticalHeight * (Settings.SegmentTimerSizeRatio / 100f);
        SegmentTimer.DrawVertical(g, state, width, clipRegion);
        g.Transform = oldMatrix;
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height);
        Matrix oldMatrix = g.Transform;
        InternalComponent.Settings.TimerWidth = HorizontalWidth;
        InternalComponent.DrawHorizontal(g, state, height * ((100f - Settings.SegmentTimerSizeRatio) / 100f), clipRegion);
        g.Transform = oldMatrix;
        g.TranslateTransform(0, height * ((100f - Settings.SegmentTimerSizeRatio) / 100f));
        SegmentTimer.DrawHorizontal(g, state, height * (Settings.SegmentTimerSizeRatio / 100f), clipRegion);
        SegmentTimer.Settings.TimerWidth = HorizontalWidth;
        g.Transform = oldMatrix;
    }

    public string ComponentName => "Detailed Timer";

    public Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public void SetSettings(XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        int lastSplitOffset = state.CurrentSplitIndex == state.Run.Count ? -1 : 0;
        GeneralTimeFormatter formatter = Settings.SegmentTimesFormatter;
        TimingMethod timingMethod = state.CurrentTimingMethod;
        if (Settings.TimingMethod == "Real Time")
        {
            timingMethod = TimingMethod.RealTime;
        }
        else if (Settings.TimingMethod == "Game Time")
        {
            timingMethod = TimingMethod.GameTime;
        }

        if (state.CurrentSplitIndex >= 0)
        {
            Comparison = ResolveComparison(state, Settings.Comparison);
            Comparison2 = ResolveComparison(state, Settings.Comparison2);
            HideComparison1 = Settings.HideComparison1 || Comparison == NoneComparisonGenerator.ComparisonName;
            HideComparison2 = Settings.HideComparison2 || Comparison2 == NoneComparisonGenerator.ComparisonName
                || (!HideComparison1 && Comparison == Comparison2);

            ComparisonName = CompositeComparisons.GetShortComparisonName(Comparison);
            ComparisonName2 = CompositeComparisons.GetShortComparisonName(Comparison2);

            LabelSegment.Text = ComparisonName + ":";

            LabelBest.Text = ComparisonName2 + ":";

            if (!HideComparison1)
            {
                SegmentTime.Text = formatter.Format(GetSegmentTime(state, Comparison, timingMethod, lastSplitOffset));
            }

            if (!HideComparison2)
            {
                BestSegmentTime.Text = formatter.Format(GetSegmentTime(state, Comparison2, timingMethod, lastSplitOffset));
            }

            if (state.CurrentSplitIndex >= 0)
            {
                string name = state.Run[state.CurrentSplitIndex + lastSplitOffset].Name;

                bool isSubsplit = name.StartsWith("-") && state.CurrentSplitIndex + lastSplitOffset < state.Run.Count - 1;

                if (isSubsplit)
                {
                    SplitName.Text = name[1..];
                }
                else
                {
                    Match match = SubsplitRegex.Match(name);
                    SplitName.Text = match.Success ? match.Groups[2].Value : name;
                }
            }
            else
            {
                SplitName.Text = "";
            }
        }

        SegmentTimer.Settings.TimingMethod = Settings.TimingMethod;
        InternalComponent.Settings.TimingMethod = Settings.TimingMethod;
        SegmentTimer.Update(null, state, width, height, mode);
        InternalComponent.Update(null, state, width, height, mode);

        Image icon = state.CurrentSplitIndex >= 0 ? state.Run[state.CurrentSplitIndex + lastSplitOffset].Icon : null;

        Cache.Restart();
        Cache["SplitIcon"] = icon;
        if (Cache.HasChanged)
        {
            FrameCount = icon == null ? 0 : icon.GetFrameCount(new FrameDimension(icon.FrameDimensionsList[0]));
        }

        Cache["SplitName"] = SplitName.Text;
        Cache["LabelSegment"] = LabelSegment.Text;
        Cache["LabelBest"] = LabelBest.Text;
        Cache["SegmentTime"] = SegmentTime.Text;
        Cache["BestSegmentTime"] = BestSegmentTime.Text;
        Cache["HideComparison1"] = HideComparison1;
        Cache["HideComparison2"] = HideComparison2;
        Cache["SegmentTimerText"] = SegmentTimer.BigTextLabel.Text + SegmentTimer.SmallTextLabel.Text;
        Cache["InternalComponentText"] = InternalComponent.BigTextLabel.Text + InternalComponent.SmallTextLabel.Text;
        if (InternalComponent.BigTextLabel.Brush != null && invalidator != null)
        {
            Cache["TimerColor"] = InternalComponent.BigTextLabel.Brush is LinearGradientBrush brush
                ? brush.LinearColors[0].ToArgb()
                : InternalComponent.BigTextLabel.ForeColor.ToArgb();
        }

        if (invalidator != null && (Cache.HasChanged || FrameCount > 1))
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    private static string ResolveComparison(LiveSplitState state, string comparison)
    {
        // "None" can't be selected in the settings, so it's treated as a missing comparison.
        return comparison == "Current Comparison"
            || comparison == NoneComparisonGenerator.ComparisonName
            || !state.Run.Comparisons.Contains(comparison)
            ? state.CurrentComparison
            : comparison;
    }

    private static TimeSpan? GetSegmentTime(LiveSplitState state, string comparison, TimingMethod timingMethod, int lastSplitOffset)
    {
        if (comparison == BestSegmentsComparisonGenerator.ComparisonName)
        {
            return state.Run[state.CurrentSplitIndex + lastSplitOffset].BestSegmentTime[timingMethod];
        }

        if (state.CurrentSplitIndex == 0 || (state.CurrentSplitIndex == 1 && lastSplitOffset == -1))
        {
            return state.Run[0].Comparisons[comparison][timingMethod];
        }

        return state.Run[state.CurrentSplitIndex + lastSplitOffset].Comparisons[comparison][timingMethod]
            - state.Run[state.CurrentSplitIndex - 1 + lastSplitOffset].Comparisons[comparison][timingMethod];
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
