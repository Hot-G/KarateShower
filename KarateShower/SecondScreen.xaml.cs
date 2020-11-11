using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KarateShower
{
    /// <summary>
    /// SecondScreen.xaml etkileşim mantığı
    /// </summary>
    public partial class SecondScreen : Window
    {
        public SecondScreen(MainWindow frm)
        {
            InitializeComponent();
            bluec1bords = new Border[3];
            bluec1bords[0] = bluec1bord1;
            bluec1bords[1] = bluec1bord2;
            bluec1bords[2] = bluec1bord3;
            bluec2bords = new Border[3];
            bluec2bords[0] = bluec2bord1;
            bluec2bords[1] = bluec2bord2;
            bluec2bords[2] = bluec2bord3;
            redc1bords = new Border[3];
            redc1bords[0] = redc1bord1;
            redc1bords[1] = redc1bord2;
            redc1bords[2] = redc1bord3;
            redc2bords = new Border[3];
            redc2bords[0] = redc2bord1;
            redc2bords[1] = redc2bord2;
            redc2bords[2] = redc2bord3;
            frm1 = frm;
        }
        private MainWindow frm1;
        Border[] bluec1bords, bluec2bords, redc1bords, redc2bords;
        public void ShowScore(int blue, int red)
        {
            bluescoretxt.Content = blue.ToString();
            redscoretxt.Content = red.ToString();
        }

        public void ShowTime(string time)
        {
            timertxt.Content = time;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            frm1.RemoveSecondScreen();
        }

        public void ShowTatami(int tatami)
        {
            tatamitxt.Content = tatami.ToString();
        }

        public void ShowFlags(string b_bgurl, string b_country, string r_bgurl, string r_country)
        {
            blueflagimg.Background = new ImageBrush(new BitmapImage(new Uri(b_bgurl)));
            bluecountrytxt.Content = b_country;
            redflagimg.Background = new ImageBrush(new BitmapImage(new Uri(r_bgurl)));
            redcountrytxt.Content = r_country;
        }

        public void ShowPenal(int bc1, int bc2, int rc1, int rc2)
        {
            for(int i = 0; i < 3; i++)
            {
                bluec1bords[i].Background = (bc1 <= i)? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)): new SolidColorBrush(Color.FromArgb(0xFF, 205, 5, 5));
                bluec2bords[i].Background = (bc2 <= i) ? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) : new SolidColorBrush(Color.FromArgb(0xFF, 205, 5, 5));
                redc1bords[i].Background = (rc1 <= i) ? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) : new SolidColorBrush(Color.FromArgb(0xFF, 205, 5, 5));
                redc2bords[i].Background = (rc2 <= i) ? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) : new SolidColorBrush(Color.FromArgb(0xFF, 205, 5, 5));
            }
        }

        public void ShowFirstPoint(bool state, string team = "")
        {
            if (state)
            {
                if (team == "blue")
                    bluefirstpoint.Visibility = Visibility.Visible;
                else
                    redfirstpoint.Visibility = Visibility.Visible;
            }
            else
            {
                bluefirstpoint.Visibility = Visibility.Hidden;
                redfirstpoint.Visibility = Visibility.Hidden;
            }
        }

        public void ShowVictoryScreen(byte team)
        {
            switch (team)
            {
                case 0:
                    victorytxt.Content = "BERABERE";
                    victoryimg.Visibility = Visibility.Collapsed;
                    victoryframebord.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 124, 21, 107));
                    break;
                case 1:
                    victorytxt.Content = "AU KAZANDI !";
                    victoryimg.Background = blueflagimg.Background;
                    victoryframebord.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 50, 108, 232));
                    break;
                case 2:
                    victorytxt.Content = "AKA KAZANDI !";
                    victoryimg.Background = redflagimg.Background;
                    victoryframebord.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 178, 0, 0));
                    break;
            }      
            victorybord.Visibility = Visibility.Visible;
        }

        public void RemoveVictoryScreen()
        {
            victoryimg.Visibility = Visibility.Visible;
            victorybord.Visibility = Visibility.Collapsed;
        }
    }
}
