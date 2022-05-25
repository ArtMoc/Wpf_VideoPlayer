using System;
using Microsoft.Win32;
using System.Threading;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlaying = false;
        Thread thread;
        bool posChange = true;
        List<string> path;
        List<string> names;
        bool isFullScreen = false;
        public MainWindow()
        {
            InitializeComponent();
            Player.Volume = 0.5;
            FilmsLB.AllowDrop = true;
        }

        void PositionSliderMove()
        {
            while (isPlaying)
            {
                if (posChange)
                    Dispatcher.Invoke(new Action(() =>
                    PositionSlider.Value = Player.Position.TotalSeconds));
                Thread.Sleep(1000);
            }
        }

        private void PlayBTN_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                Player.Pause();
                //PlayBTN.Content = "Play";
                PlayIMG.Source = new BitmapImage(new Uri("d:\\1\\play.jpg"));
            }
            else
            {
                //PlayBTN.Background = Brushes.Bisque; new SolidColorBrush(Color.FromRgb(123,22,44));
                Player.Play();
                //PlayBTN.Content = "Pause";
                PlayIMG.Source = new BitmapImage(new Uri("d:\\1\\pause.jpg"));
            }
            isPlaying = !isPlaying;
            thread = new Thread(PositionSliderMove);
            thread.Start();
        }

        private void StopBTN_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            isPlaying = false;
            //PlayBTN.Content = "Play";
            PlayIMG.Source = new BitmapImage(new Uri("d:\\1\\play.jpg"));
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = Volume.Value / 100.0;
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            double duration = Player.NaturalDuration.TimeSpan.TotalSeconds;
            PositionSlider.Maximum = duration;
            Player.Play();
            isPlaying = true;
            PlayIMG.Source = new BitmapImage(new Uri("d:\\1\\pause.jpg"));
        }

        private void Player_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Volume.Value += e.Delta / 80;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isPlaying = false;
        }

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            posChange = false;
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Player.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            posChange = true;
        }

        private void PlayBTN1_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Position.TotalSeconds + 5 < Player.NaturalDuration.TimeSpan.TotalSeconds)
            {
                Player.Position = TimeSpan.FromSeconds(Player.Position.TotalSeconds + 5);
            }
            else
            {
                Player.Position = TimeSpan.FromSeconds(Player.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        private void StopBTN1_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Position.TotalSeconds > 5)
            {
                Player.Position = TimeSpan.FromSeconds(Player.Position.TotalSeconds - 5);
            }
            else
            {
                Player.Position = TimeSpan.FromSeconds(0);
            }
        }

        private void Player_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 1)
            {
                PlayBTN.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.ClickCount == 2)
            {
                MessageBox.Show("DoubleClick");
            }
        }

        private void Player_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayBTN.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        List<string> GetFileNames(List<string> path)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < path.Count; i++)
            {
                int j = path[i].Length - 1;
                for (; path[i][j] != '\\'; j--) ;
                temp.Add(path[i].Remove(0, j + 1));
            }
            return temp;
        }

        private void OpenBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "AVI|*.avi|MP4|*.mp4|ALL|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileNames.ToList<string>();
            }
            FilmsLB.Items.Clear();
            names = GetFileNames(path);
            for (int i = 0; i < path.Count; i++)
            {
                FilmsLB.Items.Add(names[i]);
            }
        }

        private void FilmsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FilmsLB.SelectedIndex > -1)
            {
                Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
                Player.Stop();
            }
        }

        private void NextBTN_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsLB.SelectedIndex < path.Count - 1)
            {
                FilmsLB.SelectedIndex += 1;
            }
            else
            {
                FilmsLB.SelectedIndex = 0;
            }

            Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsLB.SelectedIndex > 0)
            {
                FilmsLB.SelectedIndex -= 1;
            }
            else
            {
                FilmsLB.SelectedIndex = path.Count - 1;
            }

            Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
        }

        private void FilmsLB_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if(path==null)
            {
                path = new List<string>();
            }
            foreach(string file in files)
            {
                path.Add(file);
            }
            //path = files.ToList<string>();

            FilmsLB.Items.Clear();
            names = GetFileNames(path);
            for (int i = 0; i < path.Count; i++)
            {
                FilmsLB.Items.Add(names[i]);
            }
        }
    }
}
