using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KarateShower
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer counterTimer;
        TimeSpan T_StaticTime = TimeSpan.FromMinutes(1);
        TimeSpan T_DynamicTime = TimeSpan.FromMinutes(1); // maç zamanı
        int bluepoint, redpoint, bluec1count, bluec2count, redc1count, redc2count, tatami = 1;
        Border[] bluec1bords, bluec2bords, redc1bords, redc2bords;  //CEZA BORDERLARI
        List<flagcontroller> stflagbuttons = new List<flagcontroller>();
        /******************* PENCERE AÇILDIĞINDA AYARLARI YAP *****************/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Volume = 200;
            SetupPenal();
            //TİMER I AYARLA
            counterTimer = new DispatcherTimer();
            counterTimer.Interval = TimeSpan.FromSeconds(0.25);
            counterTimer.Tick += TimerVideoTime_Tick;
            sayactxt.Content = T_StaticTime.ToString(@"mm\:ss");
            AddSecondScreen();
            //BAYRAKLARI ÇEK
            string[] flagsPath = Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + @"\flags");
            foreach (string newflag in flagsPath)
            {
                flagcontroller newflagbutton = new flagcontroller();
                FileInfo fileInfo = new FileInfo(newflag);
                string flagName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                newflagbutton.InnerText = flagName.ToUpper().Replace("İ", "I");
                newflagbutton.BackgroundUrl = newflag;
                newflagbutton.Width = 360;  newflagbutton.Height = 80;
                newflagbutton.Margin = new Thickness(10);
                newflagbutton.Visibility = Visibility.Collapsed;
                newflagbutton.MouseLeftButtonDown += Newflagbutton_MouseLeftButtonDown;
                stflagbuttons.Add(newflagbutton);
                stflaglist.Children.Add(newflagbutton);
            }
            Stflagsearchbtn_TextChanged(this.stflagsearchbtn, null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        /******* CEZA BORDERLARINI AYARLA *********/
        private void SetupPenal()
        {
            bluec1bords = new Border[3];
            bluec1bords[0] = bluec1box1;
            bluec1bords[1] = bluec1box2;
            bluec1bords[2] = bluec1box3;
            bluec2bords = new Border[3];
            bluec2bords[0] = bluec2box1;
            bluec2bords[1] = bluec2box2;
            bluec2bords[2] = bluec2box3;
            redc1bords = new Border[3];
            redc1bords[0] = redc1box1;
            redc1bords[1] = redc1box2;
            redc1bords[2] = redc1box3;
            redc2bords = new Border[3];
            redc2bords[0] = redc2box1;
            redc2bords[1] = redc2box2;
            redc2bords[2] = redc2box3;
        }
        /************* TİMER İLE SAYACI AZALT ****************/
        MatchState matchstate = MatchState.editing;
        private void TimerVideoTime_Tick(object sender, EventArgs e)
        {
            T_DynamicTime = (T_DynamicTime < TimeSpan.FromSeconds(0.25)) ? TimeSpan.FromSeconds(0) : T_DynamicTime - TimeSpan.FromSeconds(0.25);
            sayactxt.Content = T_DynamicTime.ToString(@"mm\:ss");
            //İKİNCİ EKRAN
            if (scndscreen != null)
                scndscreen.ShowTime(sayactxt.Content.ToString());

            if(T_DynamicTime.TotalSeconds == 15)
            {
                mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + @"\15scsound.mp3"));
                mediaPlayer.Play();
            }
            //SÜRE SIFIRA GELİRSE DURDUR
            if(T_DynamicTime.TotalSeconds == 0)
            {
                CalculateMatchScore();
                minustimebtn.Visibility = Visibility.Visible;
                addtimebtn.Visibility = Visibility.Visible;
                //counterTimer.IsEnabled = false;
            }

        }
        /***************** İKİNCİL EKRANI GÖSTER *****************/

        /****** İKİNCİ EKRANI GÖSTERME BUTONU *******/
        private void Addsecondbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddSecondScreen();
        }
        SecondScreen scndscreen = null;
        private void AddSecondScreen()
        {        
            if (System.Windows.Forms.Screen.AllScreens.Count() > 1 && scndscreen == null)
            {
                scndscreen = new SecondScreen(this);
                scndscreen.WindowStartupLocation = WindowStartupLocation.Manual;
                scndscreen.Left = System.Windows.Forms.Screen.AllScreens[1].Bounds.Location.X;
                scndscreen.Width = System.Windows.Forms.Screen.AllScreens[1].Bounds.Width;
                scndscreen.Height = System.Windows.Forms.Screen.AllScreens[1].Bounds.Height;
                scndscreen.Top = 0;
                scndscreen.ShowScore(bluepoint, redpoint);
                scndscreen.ShowTime(sayactxt.Content.ToString());
                scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                scndscreen.ShowFlags(blueflag.BackgroundUrl, blueflag.InnerText, redflag.BackgroundUrl, redflag.InnerText);
                scndscreen.ShowTatami(tatami);
                scndscreen.Show();
            }
        }

        public void RemoveSecondScreen()
        {
            scndscreen = null;
        }
        /**************** İLK PUANI ALANA SENSUI VER **************/
        bool getSensui;
        Team sensui = Team.none;
        private void TakeFirstPoint(Team team)
        {
            if (!getSensui)
            {
                if (team == Team.blue)
                {
                    bluefirstpoint.Visibility = Visibility.Visible;
                }
                else
                {
                    redfirstpoint.Visibility = Visibility.Visible;
                }
                getSensui = true;
                sensui = team;
                commands.Add("sensui");
                commands.Add("");
                if (scndscreen != null)
                    scndscreen.ShowFirstPoint(true, (team == Team.blue) ? "blue" : "red");
            }
        }
        /************** SÜREYİ BAŞLATIP DURDUR *************/
        private void Startstopbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeState(matchstate);
        }
        string matchStartingTime;
        private void ChangeState(MatchState state)
        {
            if (state == MatchState.paused)
            {
                startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/pause-icon.png")));
                matchstate = MatchState.starting;
                if (B_getPoint != R_getPoint)
                {
                    if (B_getPoint)
                        TakeFirstPoint(Team.blue);
                    else if (R_getPoint)
                        TakeFirstPoint(Team.red);
                }
                B_getPoint = false; R_getPoint = false;
                //ARADAKİ FARK 8 İSE OYUNU BİTİR
                if (bluepoint - redpoint >= 8)
                {
                    FinishMatch(Team.blue);
                    return;
                }

                else if (redpoint - bluepoint >= 8)
                {
                    FinishMatch(Team.red);
                    return;
                }
            }
            else if(state == MatchState.starting)
            {
                startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/play-icon.png")));
                matchstate = MatchState.paused;
            }
            else if(state == MatchState.editing)
            {
                startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/pause-icon.png")));
                matchstate = MatchState.starting;
                //MAÇIN BAŞLANGIÇ ZAMANI
                matchStartingTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                //SENSUIYI AYARLA
                if (B_getPoint != R_getPoint)
                {
                    if (B_getPoint)
                        TakeFirstPoint(Team.blue);
                    else if (R_getPoint)
                        TakeFirstPoint(Team.red);
                }
                B_getPoint = false; R_getPoint = false;
                //ARADAKİ FARK 8 İSE OYUNU BİTİR
                if (bluepoint - redpoint >= 8)
                {
                    FinishMatch(Team.blue);
                    return;
                }
                    
                else if (redpoint - bluepoint >= 8)
                {
                    FinishMatch(Team.red);
                    return;
                }
                    
            }
            counterTimer.IsEnabled = (matchstate == MatchState.starting)?true:false;
            minustimebtn.Visibility = (matchstate == MatchState.starting) ? Visibility.Hidden : Visibility.Visible;
            addtimebtn.Visibility = (matchstate == MatchState.starting) ? Visibility.Hidden : Visibility.Visible;
        }

        /////////////////// ZAMANI 1 ARTTIRIP 1 AZALT /////////////////7
        private void Minustimebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(T_DynamicTime.TotalSeconds > 0)
                T_DynamicTime -= TimeSpan.FromSeconds(1);
            sayactxt.Content = T_DynamicTime.ToString(@"mm\:ss");
            if (scndscreen != null)
                scndscreen.ShowTime(sayactxt.Content.ToString());
        }

        private void Addtimebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            T_DynamicTime += TimeSpan.FromSeconds(1);
            sayactxt.Content = T_DynamicTime.ToString(@"mm\:ss");
            if (scndscreen != null)
                scndscreen.ShowTime(sayactxt.Content.ToString());
        }

        ///////////////// RESTARTLA ////////////////////
        private void RestartScreen()
        {
            if (victoryboard.Visibility == Visibility.Visible)
                AddLastMatch(tatami, matchStartingTime, bluepoint, redpoint, bluec1count, bluec2count, redc1count, redc2count);
            bluepoint = 0;  redpoint = 0;   ShowScore();
            getSensui = false;      sensui = Team.none;
            counterTimer.IsEnabled = false;
            ResetDynamicTime();
            ShowScore();
            matchstate = MatchState.editing;
            startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/play-icon.png")));
            bluefirstpoint.Visibility = Visibility.Hidden;
            redfirstpoint.Visibility = Visibility.Hidden;
            victoryboard.Visibility = Visibility.Hidden;
            B_getPoint = false; R_getPoint = false;
            bluec1count = 0;    bluec2count = 0;    redc1count = 0;     redc2count = 0;
            for(int i = 0; i < 3; i++)
            {
                bluec1bords[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                bluec2bords[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                redc1bords[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                redc2bords[i].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }   //CEZA BORDERLERİNİ TEMİZLE 
            commands.Clear();
            //İKİNCİ EKRAN
            if (scndscreen != null)
            {
                scndscreen.ShowScore(bluepoint, redpoint);
                scndscreen.ShowTime(sayactxt.Content.ToString());
                scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                scndscreen.ShowFirstPoint(false);
                scndscreen.RemoveVictoryScreen();
            }

        }

        private void ResetDynamicTime()
        {
            T_DynamicTime = T_StaticTime;
            sayactxt.Content = T_StaticTime.ToString(@"mm\:ss");
        }

        enum MatchState
        {
            starting,
            paused,
            stopped,
            editing
        }

        /******************* CEZA VERME BUTONLARI **********************/

        private void GiveBlueC1_Click(object sender, MouseButtonEventArgs e)
        {
            if (bluec1count < 3)
            {
                bluec1count++;
                bluec1bords[bluec1count - 1].Background = new SolidColorBrush(Color.FromArgb(0xFF, 195, 0, 0));
                commands.Add("ceza");
                commands.Add("bc1");
                if (scndscreen != null)
                {
                    scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                    if (T_DynamicTime <= TimeSpan.FromSeconds(15) && sensui == Team.blue)
                    {
                        scndscreen.ShowFirstPoint(false);
                        bluefirstpoint.Visibility = Visibility.Hidden;
                        redfirstpoint.Visibility = Visibility.Hidden;
                        sensui = Team.none;
                    }
                }
            }

            else
            {
                FinishMatch(Team.red);
            }


        }

        private void GiveBlueC2_Click(object sender, MouseButtonEventArgs e)
        {
            if (bluec2count < 3)
            {
                bluec2count++;
                bluec2bords[bluec2count - 1].Background = new SolidColorBrush(Color.FromArgb(0xFF, 195, 0, 0));
                commands.Add("ceza");
                commands.Add("bc2");
                if (scndscreen != null)
                {
                    scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                    if (T_DynamicTime <= TimeSpan.FromSeconds(15) && sensui == Team.blue)
                    {
                        scndscreen.ShowFirstPoint(false);
                        bluefirstpoint.Visibility = Visibility.Hidden;
                        redfirstpoint.Visibility = Visibility.Hidden;
                        sensui = Team.none;
                    }
                }
            }

            else
            {
                FinishMatch(Team.red);
            }         
        }


        private void OpenSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (settingsGrid.Visibility == Visibility.Hidden)
            {
                settingsGrid.Visibility = Visibility.Visible;
                SetupSettings();
            }
            else
                settingsGrid.Visibility = Visibility.Hidden;
        }

        private void RestartButton_Click(object sender, MouseButtonEventArgs e)
        {
            RestartScreen();
        }

        List<string> commands = new List<string>();
        private void Backbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UndoFunc();
        }

        private void UndoFunc()
        {
            if(commands.Count >= 2)
            switch(commands[commands.Count - 2])
            {
                case "bpuan":
                    bluepoint -= Convert.ToInt32(commands[commands.Count - 1]);
                    ShowScore();
                    commands.RemoveAt(commands.Count - 1);
                    commands.RemoveAt(commands.Count - 1);
                    if (scndscreen != null)
                        scndscreen.ShowScore(bluepoint, redpoint);
                    break;
                case "rpuan":
                    redpoint -= Convert.ToInt32(commands[commands.Count - 1]);
                    ShowScore();
                    commands.RemoveAt(commands.Count - 1);
                    commands.RemoveAt(commands.Count - 1);
                    if (scndscreen != null)
                        scndscreen.ShowScore(bluepoint, redpoint);
                        break;
                case "ceza":
                    switch (commands[commands.Count - 1])
                    {
                        case "bc1":
                            bluec1bords[bluec1count - 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            bluec1count--;
                            break;
                        case "bc2":
                            bluec2bords[bluec2count - 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            bluec2count--;
                            break;
                        case "rc1":
                            redc1bords[redc1count - 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            redc1count--;
                            break;
                        case "rc2":
                            redc2bords[redc2count - 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            redc2count--;
                            break;
                    }
                    commands.RemoveAt(commands.Count - 1);
                    commands.RemoveAt(commands.Count - 1);
                    if (scndscreen != null)
                        scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                    break;
                case "sensui":
                    getSensui = false;
                    bluefirstpoint.Visibility = Visibility.Hidden;
                    redfirstpoint.Visibility = Visibility.Hidden;
                    sensui = Team.none;
                    commands.RemoveAt(commands.Count - 1);
                    commands.RemoveAt(commands.Count - 1);
                    if (scndscreen != null)
                        scndscreen.ShowFirstPoint(false);
                    UndoFunc();
                    break;
                case "bitis":
                    victoryboard.Visibility = Visibility.Hidden;
                    if (scndscreen != null)
                        scndscreen.RemoveVictoryScreen();
                    commands.RemoveAt(commands.Count - 1);
                    commands.RemoveAt(commands.Count - 1);
                    startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/play-icon.png")));
                    matchstate = MatchState.paused;
                    break;
            }
        }

        private void GiveRedC1_Click(object sender, MouseButtonEventArgs e)
        {
            if (redc1count < 3)
            {
                redc1count++;
                redc1bords[redc1count - 1].Background = new SolidColorBrush(Color.FromArgb(0xFF, 195, 0, 0));
                commands.Add("ceza");
                commands.Add("rc1");
                if (scndscreen != null)
                {
                    scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                    if (T_DynamicTime <= TimeSpan.FromSeconds(15) && sensui == Team.red)
                    {
                        scndscreen.ShowFirstPoint(false);
                        bluefirstpoint.Visibility = Visibility.Hidden;
                        redfirstpoint.Visibility = Visibility.Hidden;
                        sensui = Team.none;
                    }                      
                }
                    
            }

            else
            {
                FinishMatch(Team.blue);
            }

        }

        private void GiveRedC2_Click(object sender, MouseButtonEventArgs e)
        {
            if (redc2count < 3)
            {
                redc2count++;
                redc2bords[redc2count - 1].Background = new SolidColorBrush(Color.FromArgb(0xFF, 195, 0, 0));
                commands.Add("ceza");
                commands.Add("rc2");
                if (scndscreen != null)
                {
                    scndscreen.ShowPenal(bluec1count, bluec2count, redc1count, redc2count);
                    if (T_DynamicTime <= TimeSpan.FromSeconds(15) && sensui == Team.red)
                    {
                        scndscreen.ShowFirstPoint(false);
                        bluefirstpoint.Visibility = Visibility.Hidden;
                        redfirstpoint.Visibility = Visibility.Hidden;
                        sensui = Team.none;
                    }
                }
            }

            else
            {
                FinishMatch(Team.blue);
            }

        }

        enum Team
        {
            red,
            blue,
            none
        }

        /***************** PUAN EKLEME BUTONLARI ********************/
        bool B_getPoint, R_getPoint;
        private void AddBluePoint_Click(object sender, MouseButtonEventArgs e)
        {
            if(matchstate != MatchState.starting)
            {
                Border bord = (Border)sender;
                bluepoint += Convert.ToInt32(bord.Tag);
                commands.Add("bpuan");
                commands.Add(bord.Tag.ToString());
                B_getPoint = true;
                ShowScore();
            }
          /*  if (bluepoint - redpoint >= 7)
                FinishMatch(Team.blue);*/
        }

        private void AddRedPoint_Click(object sender, MouseButtonEventArgs e)
        {
            if (matchstate != MatchState.starting)
            {
                Border bord = (Border)sender;
                redpoint += Convert.ToInt32(bord.Tag);
                commands.Add("rpuan");
                commands.Add(bord.Tag.ToString());
                R_getPoint = true;
                ShowScore();
            }
         /*   if (redpoint - bluepoint >= 7)
                FinishMatch(Team.red); */
        }

        /*********** SKORU GÖSTER ***********/
        private void ShowScore()
        {
            bluescoretxt.Content = bluepoint;
            redscoretxt.Content = redpoint;
            if (scndscreen != null)
                scndscreen.ShowScore(bluepoint, redpoint);
        }

        private void CalculateMatchScore()
        {
            if (bluepoint == redpoint)
                FinishMatch(sensui);
            else if (bluepoint > redpoint)
                FinishMatch(Team.blue);
            else if (bluepoint < redpoint)
                FinishMatch(Team.red);
        }

        private void FinishMatch(Team team)
        {
            counterTimer.IsEnabled = false;
            startstopbtnicon.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons/stop-icon.png")));
            matchstate = MatchState.stopped;
            mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + @"\finishsound.mp3"));
            mediaPlayer.Play();

            if (team == Team.none)
            {
                victoryboard.Background = new SolidColorBrush(Color.FromArgb(0xFF, 120, 36, 241));
                victorytxt.Content = "BERABERE !";
                victoryboard.Visibility = Visibility.Visible;
                if (scndscreen != null)
                    scndscreen.ShowVictoryScreen(0);
                return;
            }
            switch (team)
            {
                case Team.blue:
                    victoryboard.Background = new SolidColorBrush(Color.FromArgb(0xFF, 1, 100, 199));
                    victorytxt.Content = "AU KAZANDI !";
                    if (scndscreen != null)
                        scndscreen.ShowVictoryScreen(1);
                    break;
                case Team.red:
                    victoryboard.Background = new SolidColorBrush(Color.FromArgb(0xFF, 195, 0, 0));
                    victorytxt.Content = "AKA KAZANDI !";
                    if (scndscreen != null)
                        scndscreen.ShowVictoryScreen(2);
                    break;
            }
            commands.Add("bitis");
            commands.Add("");
            victoryboard.Visibility = Visibility.Visible;
        }

        /***************** AYARLAR KISMI ****************/
        
        TimeSpan settingstime;
        private void Sttimeup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            settingstime += TimeSpan.FromSeconds(30);
            sttimetxt.Content = settingstime.ToString(@"mm\:ss");
        }

        private void Sttimedown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            settingstime = (settingstime - TimeSpan.FromSeconds(30) < TimeSpan.FromSeconds(0))? TimeSpan.FromSeconds(0): settingstime - TimeSpan.FromSeconds(30);
            sttimetxt.Content = settingstime.ToString(@"mm\:ss");
        }

        private void Sttatamiup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tatami++;
            sttatamitxt.Content = tatami.ToString();
        }

        private void Sttatamidown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tatami = (tatami == 0)? 0 : tatami - 1;
            sttatamitxt.Content = tatami.ToString();
        }


        private void Stcancelbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            settingsGrid.Visibility = Visibility.Hidden;
        }

        private void Stacceptbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            T_StaticTime = settingstime;
            ResetDynamicTime();     //ZAMANI AYARLA
            tatamicounttxt.Content = tatami.ToString();
            settingsGrid.Visibility = Visibility.Hidden;
            blueflag.InnerText = stblueflag.InnerText;
            blueflag.BackgroundUrl = stblueflag.BackgroundUrl;
            redflag.InnerText = stredflag.InnerText;
            redflag.BackgroundUrl = stredflag.BackgroundUrl;
            if(scndscreen != null)
            {
                scndscreen.ShowFlags(blueflag.BackgroundUrl, blueflag.InnerText, redflag.BackgroundUrl, redflag.InnerText);
                scndscreen.ShowTime(sayactxt.Content.ToString());
                scndscreen.ShowTatami(tatami);
            }
        }

        private void SetupSettings()
        {
            settingstime = T_StaticTime;    sttimetxt.Content = settingstime.ToString(@"mm\:ss");
            tatami = Convert.ToInt32(tatamicounttxt.Content);   sttatamitxt.Content = tatami.ToString();
            stblueflag.InnerText = blueflag.InnerText;
            stblueflag.BackgroundUrl = blueflag.BackgroundUrl;
            stredflag.InnerText = redflag.InnerText;
            stredflag.BackgroundUrl = redflag.BackgroundUrl;
            CloseSearchTab();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                if(MessageBox.Show("Uygulamadan çıkmak istiyor musunuz ?", "Uyarı", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    Application.Current.Shutdown();

            if (e.Key == Key.Space)
                ChangeState(matchstate);
        }
        /***************** SON MAÇLARI GÖRÜNTÜLEME KISMI **********************/
        private void Lastmatchbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastmatchborder.Visibility = lastmatchborder.Visibility == Visibility.Collapsed? Visibility.Visible: Visibility.Collapsed;
        }

        private void AddLastMatch(int tatami, string hour, int bluescore, int redscore, int bc1, int bc2, int rc1, int rc2)
        {
            SmallMatchBoard newmatchboard = new SmallMatchBoard
            {
                Tatami = tatami,
                Hour = hour,
                BlueScore = bluescore,
                RedScore = redscore,
                BlueCeza1Count = bc1,
                BlueCeza2Count = bc2,
                RedCeza1Count = rc1,
                RedCeza2Count = rc2,
                Senshui = (sensui == Team.none) ? 0 : (sensui == Team.blue) ? 1 : 2,
                //Winner = (winteam == Team.none) ? 0 : (winteam == Team.blue) ? 1 : 2,
                Margin = new Thickness(20)
            };
            lastmatchlist.Children.Insert(1, newmatchboard);
            if (lastmatchlist.Children.Count > 4)
            {
                lastmatchlist.Children.RemoveAt(lastmatchlist.Children.Count - 1);
            }
        }

        flagcontroller stselectedflag;
        private void StFlagClickEvent(object sender, MouseButtonEventArgs e)
        {
            stselectedflag = (flagcontroller)sender;
            stflaglist.Visibility = Visibility.Visible;
            stbackbtn.Visibility = Visibility.Visible;
            settingslastbtn.Visibility = Visibility.Collapsed;
            stflagsearchbtn.Focus();
        }

        private void BackBtn_Click(object sender, MouseButtonEventArgs e)
        {
            CloseSearchTab();
        }

        int searchcounter = 0;
        flagcontroller[] searchedflags = new flagcontroller[5];
        private void Stflagsearchbtn_TextChanged(object sender, TextChangedEventArgs e)
        {
            stflagsearchbtn.Text = stflagsearchbtn.Text.ToUpper();
            stflagsearchbtn.Select(stflagsearchbtn.Text.Length, 0);
            for (int i = 0; i < searchcounter; i++)
            {
                searchedflags[i].Visibility = Visibility.Collapsed;
            }
            searchcounter = 0;
            foreach (flagcontroller searchflag in stflagbuttons)
            {
                if (searchflag.InnerText.IndexOf(stflagsearchbtn.Text) != -1)
                {
                    searchflag.Visibility = Visibility.Visible;
                    searchedflags[searchcounter] = searchflag;
                    searchcounter++;
                    if (searchcounter > 4)
                        break;
                }
            }
        }

        private void Newflagbutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            flagcontroller lastflag = (flagcontroller)sender;
            stselectedflag.InnerText = lastflag.InnerText;
            stselectedflag.BackgroundUrl = lastflag.BackgroundUrl;
            CloseSearchTab();
        }

        private void CloseSearchTab()
        {
            stflaglist.Visibility = Visibility.Collapsed;
            stbackbtn.Visibility = Visibility.Hidden;
            settingslastbtn.Visibility = Visibility.Visible;
            stflagsearchbtn.Text = "";
        }

        //YAPIMCI KISMINI AÇ
        private void Producerbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            productorborder.Visibility = Visibility.Visible;
        }

        //YAPIMCI KISMINI KAPAT
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            productorborder.Visibility = Visibility.Collapsed;
        }
    }
}