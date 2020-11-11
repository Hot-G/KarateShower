using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KarateShower
{
    /// <summary>
    /// flagcontroller.xaml etkileşim mantığı
    /// </summary>
    public partial class flagcontroller : UserControl
    {
        public flagcontroller()
        {
            InitializeComponent();
            flagimg.Background = new ImageBrush(new BitmapImage(new Uri(_bgurl)));
        }
        private string _bgurl = System.IO.Directory.GetCurrentDirectory() + @"\flags\turkey.png";
        public string InnerText
        {
            get { return flagtxt.Content.ToString(); }
            set { flagtxt.Content = value; }
        }


        public string BackgroundUrl
        {
            get { return this._bgurl; }
            set
            {
                this._bgurl = value;
                flagimg.Background = new ImageBrush(new BitmapImage(new Uri(_bgurl)));
            }
        }

        public Dock FlagDock
        {
            set
            {
                this.flagimg.SetValue(DockPanel.DockProperty, value);
            }
        }
    }
}
