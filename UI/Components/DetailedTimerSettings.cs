using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.TimeFormatters;
using Fetze.WinFormsColor;
using System.Xml;
using System.Globalization;
using LiveSplit.Options;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

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

        public Color TimerColor { get; set; }
        public Color SegmentTimerColor { get; set; }
        public Color SegmentLabelsColor { get; set; }
        public Color SegmentTimesColor { get; set; }
        public Color SplitNameColor { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }

        public GradientType BackgroundGradient { get; set; }
        public String GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public String TimerFormat { get; set; }
        public String SegmentTimerFormat { get; set; }
        public TimeAccuracy SegmentTimesAccuracy { get; set; }

        public string SegmentLabelsFontString { get { return String.Format("{0} {1}", SegmentLabelsFont.FontFamily.Name, SegmentLabelsFont.Style); } }
        public Font SegmentLabelsFont { get; set; }
        public string SegmentTimesFontString { get { return String.Format("{0} {1}", SegmentTimesFont.FontFamily.Name, SegmentTimesFont.Style); } }
        public Font SegmentTimesFont { get; set; }
        public string SplitNameFontString { get { return String.Format("{0} {1}", SplitNameFont.FontFamily.Name, SplitNameFont.Style); } }
        public Font SplitNameFont { get; set; }

        public String Comparison { get; set; }
        public String Comparison2 { get; set; }
        public bool HideComparison { get; set; }
        public String TimingMethod { get; set; }

        public LayoutMode Mode { get; set; }

        public DetailedTimerSettings()
        {
            InitializeComponent();

            Height = 50;
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

            TimerFormat = "1.23";
            SegmentTimerFormat = "1.23";
            SegmentTimesAccuracy = TimeAccuracy.Hundredths;

            SegmentLabelsFont = new Font("Segoe UI", 7, FontStyle.Regular);
            SegmentTimesFont = new Font("Segoe UI", 7, FontStyle.Bold);
            SplitNameFont = new Font("Segoe UI", 8, FontStyle.Regular);

            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;

            IconSize = 40f;
            DisplayIcon = false;

            Comparison = "Current Comparison";
            Comparison2 = "Best Segments";
            HideComparison = false;

            chkShowGradientSegmentTimer.DataBindings.Add("Checked", this, "SegmentTimerShowGradient", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowGradientTimer.DataBindings.Add("Checked", this, "TimerShowGradient", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimerColors.DataBindings.Add("Checked", this, "OverrideTimerColors", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSplitName.DataBindings.Add("Checked", this, "ShowSplitName", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTimerColor.DataBindings.Add("BackColor", this, "TimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentTimerColor.DataBindings.Add("BackColor", this, "SegmentTimerColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentLabelsColor.DataBindings.Add("BackColor", this, "SegmentLabelsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSegmentTimesColor.DataBindings.Add("BackColor", this, "SegmentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSplitNameColor.DataBindings.Add("BackColor", this, "SplitNameColor", false, DataSourceUpdateMode.OnPropertyChanged);
            /*btnTimerHundredths.CheckedChanged += btnTimerHundredths_CheckedChanged;
            btnTimerSeconds.CheckedChanged += btnTimerSeconds_CheckedChanged;
            btnSegmentTimerSeconds.CheckedChanged += btnSegmentTimerSeconds_CheckedChanged;
            btnSegmentTimerHundredths.CheckedChanged += btnSegmentTimerHundredths_CheckedChanged;*/
            btnSegmentTimesSeconds.CheckedChanged += btnSegmentTimesSeconds_CheckedChanged;
            btnSegmentTimesHundredths.CheckedChanged += btnSegmentTimesHundredths_CheckedChanged;
            trkSegmentTimerRatio.DataBindings.Add("Value", this, "SegmentTimerSizeRatio", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSegmentLabelsFont.DataBindings.Add("Text", this, "SegmentLabelsFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSegmentTimesFont.DataBindings.Add("Text", this, "SegmentTimesFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            lblSplitNameFont.DataBindings.Add("Text", this, "SplitNameFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayIcon.DataBindings.Add("Checked", this, "DisplayIcon", false, DataSourceUpdateMode.OnPropertyChanged);
            trkIconSize.DataBindings.Add("Value", this, "IconSize", false, DataSourceUpdateMode.OnPropertyChanged);

            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.SelectedIndexChanged += cmbComparison_SelectedIndexChanged;
            cmbComparison2.SelectedIndexChanged += cmbComparison2_SelectedIndexChanged;
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison2.DataBindings.Add("SelectedItem", this, "Comparison2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkHideComparison.DataBindings.Add("Checked", this, "HideComparison", false, DataSourceUpdateMode.OnPropertyChanged);

            chkHideComparison.CheckedChanged += chkHideComparison_CheckedChanged;
            chkOverrideTimerColors.CheckedChanged += chkOverrideTimerColors_CheckedChanged;
            chkDisplayIcon.CheckedChanged += chkDisplayIcon_CheckedChanged;
            chkSplitName.CheckedChanged += chkSplitName_CheckedChanged;

            cmbTimerFormat.DataBindings.Add("SelectedItem", this, "TimerFormat", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimerFormat.SelectedIndexChanged += cmbTimerFormat_SelectedIndexChanged;
            cmbSegmentTimerFormat.DataBindings.Add("SelectedItem", this, "SegmentTimerFormat", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSegmentTimerFormat.SelectedIndexChanged += cmbSegmentTimerFormat_SelectedIndexChanged;

            cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.SelectedIndexChanged += cmbTimingMethod_SelectedIndexChanged;

            this.Load += DetailedTimerSettings_Load;
        }

        void cmbTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimingMethod = cmbTimingMethod.SelectedItem.ToString();
        }

        void cmbSegmentTimerFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SegmentTimerFormat = cmbSegmentTimerFormat.SelectedItem.ToString();
        }

        void cmbTimerFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimerFormat = cmbTimerFormat.SelectedItem.ToString();
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
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
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

        /*void btnSegmentTimerHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracySegmentTimer();
        }

        void btnSegmentTimerSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracySegmentTimer();
        }

        void btnTimerSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracyTimer();
        }

        void btnTimerHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracyTimer();
        }

        void UpdateAccuracyTimer()
        {
            if (btnTimerSeconds.Checked)
                TimerAccuracy = TimeAccuracy.Seconds;
            else if (btnTimerTenths.Checked)
                TimerAccuracy = TimeAccuracy.Tenths;
            else
                TimerAccuracy = TimeAccuracy.Hundredths;
        }
        void UpdateAccuracySegmentTimer()
        {
            if (btnSegmentTimerSeconds.Checked)
                SegmentTimerAccuracy = TimeAccuracy.Seconds;
            else if (btnSegmentTimerTenths.Checked)
                SegmentTimerAccuracy = TimeAccuracy.Tenths;
            else
                SegmentTimerAccuracy = TimeAccuracy.Hundredths;
        }*/
        void UpdateAccuracySegmentTimes()
        {
            if (btnSegmentTimesSeconds.Checked)
                SegmentTimesAccuracy = TimeAccuracy.Seconds;
            else if (btnSegmentTimesTenths.Checked)
                SegmentTimesAccuracy = TimeAccuracy.Tenths;
            else
                SegmentTimesAccuracy = TimeAccuracy.Hundredths;
        }

        private T ParseEnum<T>(XmlElement element)
        {
            return (T)Enum.Parse(typeof(T), element.InnerText);
        }

        private Color ParseColor(XmlElement colorElement)
        {
            return Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber));
        }

        private XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version;
            if (element["Version"] != null)
                version = Version.Parse(element["Version"].InnerText);
            else
                version = new Version(1, 0, 0, 0);
            Height = Single.Parse(element["Height"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
            Width = Single.Parse(element["Width"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
            SegmentTimerSizeRatio = Single.Parse(element["SegmentTimerSizeRatio"].InnerText.Replace(',', '.'),CultureInfo.InvariantCulture);
            TimerShowGradient = Boolean.Parse(element["TimerShowGradient"].InnerText);
            if (version >= new Version(1, 3))
                OverrideTimerColors = Boolean.Parse(element["OverrideTimerColors"].InnerText);
            else
                OverrideTimerColors = !Boolean.Parse(element["TimerUseSplitColors"].InnerText);
            SegmentTimerShowGradient = Boolean.Parse(element["SegmentTimerShowGradient"].InnerText);
            TimerColor = ParseColor(element["TimerColor"]);
            SegmentTimerColor = ParseColor(element["SegmentTimerColor"]);
            SegmentLabelsColor = ParseColor(element["SegmentLabelsColor"]);
            SegmentTimesColor = ParseColor(element["SegmentTimesColor"]);
            if (version >= new Version(1, 5))
            {
                TimerFormat = element["TimerFormat"].InnerText;
                SegmentTimerFormat = element["SegmentTimerFormat"].InnerText;
                TimingMethod = element["TimingMethod"].InnerText;
            }
            else
            {
                TimingMethod = "Current Timing Method";
                var timerAccuracy = ParseEnum<TimeAccuracy>(element["TimerAccuracy"]);
                if (timerAccuracy == TimeAccuracy.Hundredths)
                    TimerFormat = "1.23";
                else if (timerAccuracy == TimeAccuracy.Tenths)
                    TimerFormat = "1.2";
                else
                    TimerFormat = "1";
                var segmentTimerAccuracy = ParseEnum<TimeAccuracy>(element["SegmentTimerAccuracy"]);
                if (segmentTimerAccuracy == TimeAccuracy.Hundredths)
                    SegmentTimerFormat = "1.23";
                else if (segmentTimerAccuracy == TimeAccuracy.Tenths)
                    SegmentTimerFormat = "1.2";
                else
                    SegmentTimerFormat = "1";
            }
            SegmentTimesAccuracy = ParseEnum<TimeAccuracy>(element["SegmentTimesAccuracy"]);
            if (version >= new Version(1, 3))
            {
                SegmentLabelsFont = GetFontFromElement(element["SegmentLabelsFont"]);
                SegmentTimesFont = GetFontFromElement(element["SegmentTimesFont"]);
                SplitNameFont = GetFontFromElement(element["SplitNameFont"]);
                DisplayIcon = Boolean.Parse(element["DisplayIcon"].InnerText);
                IconSize = Single.Parse(element["IconSize"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
                ShowSplitName = Boolean.Parse(element["ShowSplitName"].InnerText);
                SplitNameColor = ParseColor(element["SplitNameColor"]);
                BackgroundColor = ParseColor(element["BackgroundColor"]);
                BackgroundColor2 = ParseColor(element["BackgroundColor2"]);
                GradientString = element["BackgroundGradient"].InnerText;
                Comparison = element["Comparison"].InnerText;
                Comparison2 = element["Comparison2"].InnerText;
                HideComparison = Boolean.Parse(element["HideComparison"].InnerText);
            }
            else
            {
                SegmentLabelsFont = new Font("Segoe UI", 7, FontStyle.Regular);
                SegmentTimesFont = new Font("Segoe UI", 7, FontStyle.Bold);
                SplitNameFont = new Font("Segoe UI", 8, FontStyle.Regular);
                DisplayIcon = false;
                IconSize = 40f;
                ShowSplitName = false;
                SplitNameColor = Color.FromArgb(255, 255, 255);
                BackgroundColor = Color.Transparent;
                BackgroundColor2 = Color.Transparent;
                BackgroundGradient = GradientType.Plain;
                Comparison = "Current Comparison";
                Comparison2 = "Best Segments";
                HideComparison = false;
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            parent.AppendChild(ToElement(document, "Version", "1.5"));
            parent.AppendChild(ToElement(document, "Height", Height));
            parent.AppendChild(ToElement(document, "Width", Width));
            parent.AppendChild(ToElement(document, "SegmentTimerSizeRatio", SegmentTimerSizeRatio));
            parent.AppendChild(ToElement(document, "TimerShowGradient", TimerShowGradient));
            parent.AppendChild(ToElement(document, "OverrideTimerColors", OverrideTimerColors));
            parent.AppendChild(ToElement(document, "SegmentTimerShowGradient", SegmentTimerShowGradient));
            parent.AppendChild(ToElement(document, "TimerFormat", TimerFormat));
            parent.AppendChild(ToElement(document, "SegmentTimerFormat", SegmentTimerFormat));
            parent.AppendChild(ToElement(document, "SegmentTimesAccuracy", SegmentTimesAccuracy));
            parent.AppendChild(ToElement(document, TimerColor, "TimerColor"));
            parent.AppendChild(ToElement(document, SegmentTimerColor, "SegmentTimerColor"));
            parent.AppendChild(ToElement(document, SegmentLabelsColor, "SegmentLabelsColor"));
            parent.AppendChild(ToElement(document, SegmentTimesColor, "SegmentTimesColor"));
            parent.AppendChild(CreateFontElement(document, "SegmentLabelsFont", SegmentLabelsFont));
            parent.AppendChild(CreateFontElement(document, "SegmentTimesFont", SegmentTimesFont));
            parent.AppendChild(CreateFontElement(document, "SplitNameFont", SplitNameFont));
            parent.AppendChild(ToElement(document, "DisplayIcon", DisplayIcon));
            parent.AppendChild(ToElement(document, "IconSize", IconSize));
            parent.AppendChild(ToElement(document, "ShowSplitName", ShowSplitName));
            parent.AppendChild(ToElement(document, SplitNameColor, "SplitNameColor"));
            parent.AppendChild(ToElement(document, BackgroundColor, "BackgroundColor"));
            parent.AppendChild(ToElement(document, BackgroundColor2, "BackgroundColor2"));
            parent.AppendChild(ToElement(document, "BackgroundGradient", BackgroundGradient));
            parent.AppendChild(ToElement(document, "Comparison", Comparison));
            parent.AppendChild(ToElement(document, "Comparison2", Comparison2));
            parent.AppendChild(ToElement(document, "HideComparison", HideComparison));
            parent.AppendChild(ToElement(document, "TimingMethod", TimingMethod));
            return parent;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorPickerDialog();
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.SelectedColorChanged += (s, x) => button.BackColor = picker.SelectedColor;
            picker.ShowDialog(this);
            button.BackColor = picker.SelectedColor;
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
            /*btnTimerHundredths.Checked = TimerAccuracy == TimeAccuracy.Hundredths;
            btnTimerTenths.Checked = TimerAccuracy == TimeAccuracy.Tenths;
            btnTimerSeconds.Checked = TimerAccuracy == TimeAccuracy.Seconds;
            btnSegmentTimerHundredths.Checked = SegmentTimerAccuracy == TimeAccuracy.Hundredths;
            btnSegmentTimerTenths.Checked = SegmentTimerAccuracy == TimeAccuracy.Tenths;
            btnSegmentTimerSeconds.Checked = SegmentTimerAccuracy == TimeAccuracy.Seconds;*/
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

        private Font ChooseFont(Font previousFont, int minSize, int maxSize)
        {
            var dialog = new FontDialog();
            dialog.Font = previousFont;
            /*dialog.MaxSize = (int)previousFont.SizeInPoints;
            dialog.MinSize = (int)previousFont.SizeInPoints;*/
            dialog.MinSize = minSize;
            dialog.MaxSize = maxSize;
            try
            {
                var result = dialog.ShowDialog(this);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.Font;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                MessageBox.Show("This font is not supported.", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return previousFont;
        }

        private Font GetFontFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(data);
                return (Font)bf.Deserialize(ms);
            }
            return null;
        }

        private XmlElement CreateFontElement(XmlDocument document, String elementName, Font font)
        {
            var element = document.CreateElement(elementName);

            if (font != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();

                    bf.Serialize(ms, font);
                    var data = ms.ToArray();
                    var cdata = document.CreateCDataSection(Convert.ToBase64String(data));
                    element.InnerXml = cdata.OuterXml;
                }
            }

            return element;
        }

        private void btnSegmentLabelsFont_Click(object sender, EventArgs e)
        {
            SegmentLabelsFont = ChooseFont(SegmentLabelsFont, 7, 20);
            lblSegmentLabelsFont.Text = SegmentLabelsFontString;
        }

        private void btnSegmentTimesFont_Click(object sender, EventArgs e)
        {
            SegmentTimesFont = ChooseFont(SegmentTimesFont, 7, 20);
            lblSegmentTimesFont.Text = SegmentTimesFontString;
        }
        private void btnSplitNameFont_Click(object sender, EventArgs e)
        {
            SplitNameFont = ChooseFont(SplitNameFont, 7, 20);
            lblSplitNameFont.Text = SplitNameFontString;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        private XmlElement ToElement(XmlDocument document, String name, float value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString(CultureInfo.InvariantCulture);
            return element;
        }
    }
}
