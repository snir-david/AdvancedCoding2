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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using AdvancedCoding2;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using WpfApp2.View;
using WpfApp2.ViewModel;
using DesktopFGApp.View;

namespace AdvancedCoding2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ViewModelController controllerViewModel;
        public MainWindow()
        {
            InitializeComponent();
            Client c = new Client("localhost", 5400);
            JoystickView joystick = new JoystickView(c);
            joystick.Show();
            controllerViewModel = new ViewModelController(c);

            this.DataContext = controllerViewModel;
            if (Directory.Exists("C:\\Program Files\\FlightGear 2020.3.6"))
            {
                if (!File.Exists("C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml"))
                {
                    XML_button.Visibility = Visibility.Visible;
                    CSV_button.Visibility = Visibility.Visible;
                    controllerViewModel.VM_FGPath = "C:\\Program Files\\FlightGear 2020.3.6";
                }
                else
                {
                    CSV_button.Visibility = Visibility.Visible;
                    controllerViewModel.VM_XMLPath = "C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml";
                    controllerViewModel.xmlPraser();
                }

            }
            else
            {
                Folder_button.Visibility = Visibility.Visible;
            }
            graphView g = new graphView(c);
            g.Show();
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            controllerViewModel.pauseConnection();
            controllerViewModel.VM_playSpeed = 0;
            play_button1.Visibility = Visibility.Visible;
            pause_button1.Visibility = Visibility.Hidden;
        }
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            controllerViewModel.connect();
            if (controllerViewModel.VM_fpath != null)
            {
                controllerViewModel.VM_playSpeed = 1;
                controllerViewModel.VM_TransSpeed = 100;
                pause_button1.Visibility = Visibility.Visible;
                play_button1.Visibility = Visibility.Hidden;
            }


        }
        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            if (controllerViewModel.VM_playSpeed - 0.1 > 0.1)
            {
                controllerViewModel.VM_playSpeed -= 0.1;
                controllerViewModel.VM_TransSpeed += 10;
            }

        }
        private void Forw_Button_Click(object sender, RoutedEventArgs e)
        {
            if (controllerViewModel.VM_playSpeed < 1.9)
            {
                controllerViewModel.VM_playSpeed += 0.1;
                controllerViewModel.VM_TransSpeed -= 10;
            }

        }

        private void time_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controllerViewModel.VM_lineNumber = (int)time_slider.Value;
            controllerViewModel.settingUpTime();
        }

        private void Openfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                controllerViewModel.VM_fpath = openFileDialog.FileName;
            CSV_button.Visibility = Visibility.Hidden;
            Play_Button_Click(this, null);
        }
        private void OpenXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                controllerViewModel.VM_XMLPath = openFileDialog.FileName;
            controllerViewModel.copyXML();
            controllerViewModel.xmlPraser();
            XML_button.Visibility = Visibility.Hidden;
        }

        private void Openfolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result.ToString() != string.Empty)
                controllerViewModel.VM_FGPath = folderBrowserDialog.SelectedPath;
            Folder_button.Visibility = Visibility.Hidden;
            CSV_button.Visibility = Visibility.Visible;
            string XMLFilePath = "\\data\\Protocol\\playback_small.xml";
            string dest = System.IO.Path.Combine(controllerViewModel.VM_FGPath, XMLFilePath);
            if (!File.Exists(dest))
            {
                XML_button.Visibility = Visibility.Visible;
            }
        }
    }
}
