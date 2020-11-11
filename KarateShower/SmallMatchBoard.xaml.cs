using System.Windows;
using System.Windows.Controls;

namespace KarateShower
{
    /// <summary>
    /// SmallMatchBoard.xaml etkileşim mantığı
    /// </summary>
    public partial class SmallMatchBoard : UserControl
    {
        public SmallMatchBoard()
        {
            InitializeComponent();
            //CEZA BORDERLARI AYARLA
            bluec1borders = new Border[3];
            bluec2borders = new Border[3];
            redc1borders = new Border[3];
            redc2borders = new Border[3];
            bluec1borders[0] = bc1p1;
            bluec1borders[1] = bc1p2;
            bluec1borders[2] = bc1p3;
            bluec2borders[0] = bc2p1;
            bluec2borders[1] = bc2p2;
            bluec2borders[2] = bc2p3;
            redc1borders[0] = rc1p1;
            redc1borders[1] = rc1p2;
            redc1borders[2] = rc1p3;
            redc2borders[0] = rc2p1;
            redc2borders[1] = rc2p2;
            redc2borders[2] = rc2p3;
        }
        Border[] bluec1borders, bluec2borders, redc1borders, redc2borders; 
        public int BlueCeza1Count
        {
            set
            {
                for (int i = 0; i < value; i++)
                    bluec1borders[i].Visibility = Visibility.Visible;
            }
        }
        public int BlueCeza2Count
        {
            set
            {
                for (int i = 0; i < value; i++)
                    bluec2borders[i].Visibility = Visibility.Visible;
            }
        }
        public int RedCeza1Count
        {
            set
            {
                for (int i = 0; i < value; i++)
                    redc1borders[i].Visibility = Visibility.Visible;
            }
        }
        public int RedCeza2Count
        {
            set
            {
                for (int i = 0; i < value; i++)
                    redc2borders[i].Visibility = Visibility.Visible;
            }
        }

        public int BlueScore
        {
            set { bluescoretxt.Content = value.ToString(); }
        }

        public int RedScore
        {
            set { redscoretxt.Content = value.ToString(); }
        }

        public int Tatami
        {
            set { tatamitxt.Content = "TATAMI: " + value.ToString(); }
        }

        public string Hour
        {
            set { hourtxt.Content = "SAAT: " + value; }
        }

        public int Senshui
        {
            set
            {
                if (value == 1)
                    bluefirstpoint.Visibility = Visibility.Visible;
                else if (value == 2)
                    redfirstpoint.Visibility = Visibility.Visible;
            }
        }

        public int Winner
        {
            set
            {
                if (value == 1)
                    bluewinborder.Visibility = Visibility.Visible;
                else if (value == 2)
                    redwinborder.Visibility = Visibility.Visible;
            }
        }
    }
}
