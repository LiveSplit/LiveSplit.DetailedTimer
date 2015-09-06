using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class DetailedTimerSettings : UserControl
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float SegmentTimerSizeRatio { get; set; }

        public LiveSplitState CurrentState { get; set; }

        public bool TimerShowGradient { get; set; }
        public bool OverrideTimerColors { get; set; }
        public bool SegmentTimerShowGradient { get; set; }
        public bool ShowSplitName { get; set; }

        public float IconSize { get; set; }
        public bool DisplayIcon { get; set; }

        public float DecimalsSize { get; set; }
        public float SegmentTimerDecimalsSize { get; set; }

        public Color TimerColor { get; set; }
        public Color SegmentTimerColor { get; set; }
        public Color SegmentLabelsColor { get; set; }
        public Color SegmentTimesColor { get; set; }
        public Color SplitNameColor { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }

        public DeltasGradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return TimerSettings.GetBackgroundTypeString(BackgroundGradient); }
            set { BackgroundGradient = (DeltasGradientType)Enum.Parse(typeof(DeltasGradientType), value.Replace(" ", "")); }
        }

        private string timerFormat
        {
            get
            {
                return DigitsFormat + Accuracy;
            }
            set
            {
                var decimalIndex = value.IndexOf('.');
                DigitsFormat = value.Substring(0, decimalIndex);
                Accuracy = value.Substring(decimalIndex);
            }
        }
        public string DigitsFormat { get; set; }
        public string Accuracy { get; set; }
        private string segmentTimerFormat
        {
            get
            {
                return SegmentDigitsFormat + SegmentAccuracy;
            }
            set
            {
                var decimalIndex = value.IndexOf('.');
                SegmentDigitsFormat = value.Substring(0, decimalIndex);
                SegmentAccuracy = value.Substring(decimalIndex);
            }
        }
        public string SegmentDigitsFormat { get; set; }
        public string SegmentAccuracy { get; set; }
        public TimeAccuracy SegmentTimesAccuracy { get; set; }

        public string SegmentLabelsFontString => SettingsHelper.FormatFont(SegmentLabelsFont);
        public Font SegmentLabelsFont { get; set; }
        public string SegmentTimesFontString => SettingsHelper.FormatFont(SegmentTimesFont);
        public Font SegmentTimesFont { get; set; }
        public string SplitNameFontString => SettingsHelper.FormatFont(SplitNameFont);
        public Font SplitNameFont { get; set; }

        public string Comparison { get; set; }
        public string Comparison2 { get; set; }
        public bool HideComparison { get; set; }
        public string TimingMethod { get; set; }

        public LayoutMode Mode { get; set; }

        public DetailedTimerSettings()
        {
            InitializeComponent();

            Height = 75;
            Width = 200;
            SegmentTimerSizeRatio = 40;

            TimerShowGradient = true;
            OverrideTimerColors = false;
            SegmentTimerShowGradient = true;
            ShowSplitName = false;

            TimerColor = Color.FromArgb(170, 170, 170);
            SegmentTimerColor = Color.FromArgb(170, 170, 170);
            SegmentLabelsColor = Color.FromArgb(255, 255, 255);
            SegmentTimesColor = Color.FromArgb(255, 255, 255);
            SplitNameColor = Color.FromArgb(255, 255, 255);

            DigitsFormat = "1";
            Accuracy = ".23";
            SegmentDigitsFormat = "1";
            SegmentAccuracy = ".23";
            SegmentTimesAccuracy = TimeAccuracy.Hundredths;

            SegmentLabelsFont = new Font("Segoe UI", 10, FontStyle.Regular);
            SegmentTimesFont = new Font("Segoe UI", 10, FontStyle.Bold);
            SplitNameFont = new Font("Segoe UI", 11, FontStyle.Regular);

            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = DeltasGradientType.Plain;

            IconSize = 40f;
            DisplayIcon = false;

            DecimalsSize = 35f;
            SegmentTimerDecimalsSize = 35f;

            Comparison = "Current Comparison";
            Comparison2 = "Best Segments";
            HideComparison = false;
            TimingMethod = "Current Timing Method";

            chkShowGradientSegmentTimer.DataBindings.Add("Checked", this, "SegmentTimerShowGradient", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowGradientTimer.DataBindings.Add("Checked", this, "TimerShowGradient", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimerColors.DataBindings.Add("Checked", this, "OverrideTimerColors", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSplitName.DataBindings.Add("Checked", this, "ShowSplitName", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTimerColor.DataBindings.Add("BackColor", this, "TimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentTimerColor.DataBindings.Add("BackColor", this, "SegmentTimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentLabelsColor.DataBindings.Add("BackColor", this, "SegmentLabelsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentTimesColor.DataBindings.Add("BackColor", this, "SegmentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSplitNameColor.DataBindings.Add("BackColor", this, "SplitNameColor", false, DataSourceUpdateMode.OnPropertyChanged);
            trkSegmentTimerRatio.DataBindings.Add("Value", this, "SegmentTimerSizeRatio", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSegmentLabelsFont.DataBindings.Add("Text", this, "SegmentLabelsFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSegmentTimesFont.DataBindings.Add("Text", this, "SegmentTimesFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSplitNameFont.DataBindings.Add("Text", this, "SplitNameFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayIcon.DataBindings.Add("Checked", this, "DisplayIcon", false, DataSourceUpdateMode.OnPropertyChanged);
            trkIconSize.DataBindings.Add("Value", this, "IconSize", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison2.DataBindings.Add("SelectedItem", this, "Comparison2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkHideComparison.DataBindings.Add("Checked", this, "HideComparison", false, DataSourceUpdateMode.OnPropertyChanged);
            trkDecimalsSize.DataBindings.Add("Value", this, "DecimalsSize", false, DataSourceUpdateMode.OnPropertyChanged);
            trkSegmentDecimalsSize.DataBindings.Add("Value", this, "SegmentTimerDecimalsSize", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbDigitsFormat.DataBindings.Add("SelectedItem", this, "DigitsFormat", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbAccuracy.DataBindings.Add("SelectedItem", this, "Accuracy", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSegmentDigitsFormat.DataBindings.Add("SelectedItem", this, "SegmentDigitsFormat", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSegmentAccuracy.DataBindings.Add("SelectedItem", this, "SegmentAccuracy", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void cmbTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimingMethod = cmbTimingMethod.SelectedItem.ToString();
        }

        void cmbSegmentDigitsFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SegmentDigitsFormat = cmbSegmentDigitsFormat.SelectedItem.ToString();
        }

        void cmbSegmentAccuracy_SelectedIndexChanged(object sender, EventArgs e)
        {
            SegmentAccuracy = cmbSegmentAccuracy.SelectedItem.ToString();
        }

        void cmbDigitsFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            DigitsFormat = cmbDigitsFormat.SelectedItem.ToString();
        }

        void cmbAccuracy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Accuracy = cmbAccuracy.SelectedItem.ToString();
        }

        void chkSplitName_CheckedChanged(object sender, EventArgs e)
        {
            label9.Enabled = label10.Enabled = lblSplitNameFont.Enabled = btnSplitNameColor.Enabled
                = btnSplitNameFont.Enabled = chkSplitName.Checked;

        }

        void chkDisplayIcon_CheckedChanged(object sender, EventArgs e)
        {
            label7.Enabled = trkIconSize.Enabled = chkDisplayIcon.Checked;
        }

        void chkOverrideTimerColors_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = btnTimerColor.Enabled = chkOverrideTimerColors.Checked;
        }

        void chkHideComparison_CheckedChanged(object sender, EventArgs e)
        {
            cmbComparison2.Enabled = label13.Enabled = !chkHideComparison.Checked;
        }

        void cmbComparison2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison2 = cmbComparison2.SelectedItem.ToString();
        }

        void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison = cmbComparison.SelectedItem.ToString();
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedText = cmbGradientType.SelectedItem.ToString();
            btnColor1.Visible = selectedText != "Plain" && !selectedText.Contains("Delta");
            btnColor2.Visible = !selectedText.Contains("Delta");
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        void btnSegmentTimesHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracySegmentTimes();
        }

        void btnSegmentTimesSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracySegmentTimes();
        }

        void UpdateAccuracySegmentTimes()
        {
            if (btnSegmentTimesSeconds.Checked)
                SegmentTimesAccuracy = TimeAccuracy.Seconds;
            else if (btnSegmentTimesTenths.Checked)
                SegmentTimesAccuracy = TimeAccuracy.Tenths;
            else
                SegmentTimesAccuracy = TimeAccuracy.Hundredths;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            Height = SettingsHelper.ParseFloat(element["Height"]);
            Width = SettingsHelper.ParseFloat(element["Width"]);
            SegmentTimerSizeRatio = SettingsHelper.ParseFloat(element["SegmentTimerSizeRatio"]);
            TimerShowGradient = SettingsHelper.ParseBool(element["TimerShowGradient"]);
            SegmentTimerShowGradient = SettingsHelper.ParseBool(element["SegmentTimerShowGradient"]);
            TimerColor = SettingsHelper.ParseColor(element["TimerColor"]);
            SegmentTimerColor = SettingsHelper.ParseColor(element["SegmentTimerColor"]);
            SegmentLabelsColor = SettingsHelper.ParseColor(element["SegmentLabelsColor"]);
            SegmentTimesColor = SettingsHelper.ParseColor(element["SegmentTimesColor"]);
            TimingMethod = SettingsHelper.ParseString(element["TimingMethod"], "Current Timing Method");
            DecimalsSize = SettingsHelper.ParseFloat(element["DecimalsSize"], 35f);
            SegmentTimerDecimalsSize = SettingsHelper.ParseFloat(element["SegmentTimerDecimalsSize"], 35f);
            DisplayIcon = SettingsHelper.ParseBool(element["DisplayIcon"], false);
            IconSize = SettingsHelper.ParseFloat(element["IconSize"], 40f);
            ShowSplitName = SettingsHelper.ParseBool(element["ShowSplitName"], false);
            SplitNameColor = SettingsHelper.ParseColor(element["SplitNameColor"], Color.FromArgb(255, 255, 255));
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Transparent);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.Transparent);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], DeltasGradientType.Plain.ToString());
            Comparison = SettingsHelper.ParseString(element["Comparison"], "Current Comparison");
            Comparison2 = SettingsHelper.ParseString(element["Comparison2"], "Best Segments");
            HideComparison = SettingsHelper.ParseBool(element["HideComparison"], false);
            SegmentTimesAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["SegmentTimesAccuracy"]);

            if (version >= new Version(1, 3))
            {
                OverrideTimerColors = SettingsHelper.ParseBool(element["OverrideTimerColors"]);
                SegmentLabelsFont = SettingsHelper.GetFontFromElement(element["SegmentLabelsFont"]);
                SegmentTimesFont = SettingsHelper.GetFontFromElement(element["SegmentTimesFont"]);
                SplitNameFont = SettingsHelper.GetFontFromElement(element["SplitNameFont"]);
            }
            else
            {
                OverrideTimerColors = !SettingsHelper.ParseBool(element["TimerUseSplitColors"]);
                SegmentLabelsFont = new Font("Segoe UI", 10, FontStyle.Regular);
                SegmentTimesFont = new Font("Segoe UI", 10, FontStyle.Bold);
                SplitNameFont = new Font("Segoe UI", 11, FontStyle.Regular);
            }

            if (version >= new Version(1, 5))
            {
                timerFormat = element["TimerFormat"].InnerText;
                segmentTimerFormat = element["SegmentTimerFormat"].InnerText;
            }
            else
            {
                DigitsFormat = "1";
                SegmentDigitsFormat = "1";
                var timerAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["TimerAccuracy"]);
                if (timerAccuracy == TimeAccuracy.Hundredths)
                    Accuracy = ".23";
                else if (timerAccuracy == TimeAccuracy.Tenths)
                    Accuracy = ".2";
                else
                    Accuracy = "";
                var segmentTimerAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["SegmentTimerAccuracy"]);
                if (segmentTimerAccuracy == TimeAccuracy.Hundredths)
                    SegmentAccuracy = ".23";
                else if (segmentTimerAccuracy == TimeAccuracy.Tenths)
                    SegmentAccuracy = ".2";
                else
                    SegmentAccuracy = "";
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.5") ^
            SettingsHelper.CreateSetting(document, parent, "Height", Height) ^
            SettingsHelper.CreateSetting(document, parent, "Width", Width) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimerSizeRatio", SegmentTimerSizeRatio) ^
            SettingsHelper.CreateSetting(document, parent, "TimerShowGradient", TimerShowGradient) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTimerColors", OverrideTimerColors) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimerShowGradient", SegmentTimerShowGradient) ^
            SettingsHelper.CreateSetting(document, parent, "TimerFormat", timerFormat) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimerFormat", segmentTimerFormat) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimesAccuracy", SegmentTimesAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "TimerColor", TimerColor) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimerColor", SegmentTimerColor) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentLabelsColor", SegmentLabelsColor) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimesColor", SegmentTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentLabelsFont", SegmentLabelsFont) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimesFont", SegmentTimesFont) ^
            SettingsHelper.CreateSetting(document, parent, "SplitNameFont", SplitNameFont) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayIcon", DisplayIcon) ^
            SettingsHelper.CreateSetting(document, parent, "IconSize", IconSize) ^
            SettingsHelper.CreateSetting(document, parent, "ShowSplitName", ShowSplitName) ^
            SettingsHelper.CreateSetting(document, parent, "SplitNameColor", SplitNameColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "Comparison", Comparison) ^
            SettingsHelper.CreateSetting(document, parent, "Comparison2", Comparison2) ^
            SettingsHelper.CreateSetting(document, parent, "HideComparison", HideComparison) ^
            SettingsHelper.CreateSetting(document, parent, "TimingMethod", TimingMethod) ^
            SettingsHelper.CreateSetting(document, parent, "DecimalsSize", DecimalsSize) ^
            SettingsHelper.CreateSetting(document, parent, "SegmentTimerDecimalsSize", SegmentTimerDecimalsSize);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        void DetailedTimerSettings_Load(object sender, EventArgs e)
        {
            chkHideComparison_CheckedChanged(null, null);
            chkOverrideTimerColors_CheckedChanged(null, null);
            chkDisplayIcon_CheckedChanged(null, null);
            chkSplitName_CheckedChanged(null, null);
            cmbComparison.Items.Clear();
            cmbComparison.Items.Add("Current Comparison");
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);
            cmbComparison2.Items.Clear();
            cmbComparison2.Items.Add("Current Comparison");
            cmbComparison2.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison2.Items.Contains(Comparison2))
                cmbComparison2.Items.Add(Comparison2);
            btnSegmentTimesHundredths.Checked = SegmentTimesAccuracy == TimeAccuracy.Hundredths;
            btnSegmentTimesTenths.Checked = SegmentTimesAccuracy == TimeAccuracy.Tenths;
            btnSegmentTimesSeconds.Checked = SegmentTimesAccuracy == TimeAccuracy.Seconds;

            if (Mode == LayoutMode.Horizontal)
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 50;
                trkSize.Maximum = 500;
                trkSize.DataBindings.Add("Value", this, "Width", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSize.Text = "Width:";
            }
            else
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 20;
                trkSize.Maximum = 150;
                trkSize.DataBindings.Add("Value", this, "Height", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSize.Text = "Height:";
            }
        }

        private void btnSegmentLabelsFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(SegmentLabelsFont, 7, 20);
            dialog.FontChanged += (s, ev) => SegmentLabelsFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblSegmentLabelsFont.Text = SegmentLabelsFontString;
        }

        private void btnSegmentTimesFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(SegmentTimesFont, 7, 20);
            dialog.FontChanged += (s, ev) => SegmentTimesFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblSegmentTimesFont.Text = SegmentTimesFontString;
        }
        private void btnSplitNameFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(SplitNameFont, 7, 20);
            dialog.FontChanged += (s, ev) => SplitNameFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblSplitNameFont.Text = SplitNameFontString;
        }
    }
}
