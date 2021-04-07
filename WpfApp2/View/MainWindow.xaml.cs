using DesktopFGApp.View;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using WpfApp2.View;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdvancedCoding2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /***Data Members***/
        public ViewModelController controllerViewModel;
        /***Methods***/
        public MainWindow()
        {
            InitializeComponent();
            //creating a client instance
            Client c = new Client("localhost", 5400);
            //TODO - changing joystick view to buttons
            JoystickView joystick = new JoystickView(c);
            joystick.Show();
            controllerViewModel = new ViewModelController(c);
            this.DataContext = controllerViewModel;
            //checking if FG folder is in the "normal" place
            if (Directory.Exists("C:\\Program Files\\FlightGear 2020.3.6"))
            {
                //if FG folder in the right place checking if XML file in protocol
                if (!File.Exists("C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml"))
                {
                    //if XML not found - show CSV and XML load buttons
                    XML_button.Visibility = Visibility.Visible;
                    CSV_button.Visibility = Visibility.Visible;
                    controllerViewModel.VM_FGPath = "C:\\Program Files\\FlightGear 2020.3.6";
                }
                else
                {
                    //if XML found - only CSV button visible and than save XML path and parse XML
                    CSV_button.Visibility = Visibility.Visible;
                    controllerViewModel.VM_XMLPath = "C:\\Program Files\\FlightGear 2020.3.6\\data\\Protocol\\playback_small.xml";
                    controllerViewModel.xmlPraser();
                }
            }
            else
            {
                //FG folder is not in place - asking from user to choose right folder
                Folder_button.Visibility = Visibility.Visible;
            }
            //TODO - changing graph view to buttons
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
            //checking if playspeed is not under 0.1 (or else data will stop sending)
            if (controllerViewModel.VM_playSpeed - 0.1 > 0.1)
            {
                controllerViewModel.VM_playSpeed -= 0.1;
                controllerViewModel.VM_TransSpeed += 10;
            }
        }
        private void Forw_Button_Click(object sender, RoutedEventArgs e)
        {
            //checking if playspeed is not over 1.9 (or else all data will send in a second)
            if (controllerViewModel.VM_playSpeed < 1.9)
            {
                controllerViewModel.VM_playSpeed += 0.1;
                controllerViewModel.VM_TransSpeed -= 10;
            }
        }
        private void time_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //getting slider value and sending it to viewmodel for time view
            controllerViewModel.VM_lineNumber = (int)time_slider.Value;
            controllerViewModel.settingUpTime();
        }
        private void Openfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                controllerViewModel.VM_fpath = openFileDialog.FileName;
            CSV_button.Visibility = Visibility.Hidden;
            //read csv and start playing simulator
            readCSVfile();
            controllerViewModel.splitAtt();
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
        private void readCSVfile()
        {
            String[] csvLine = File.ReadAllLines(controllerViewModel.VM_fpath);
            controllerViewModel.VM_CSVcopy = csvLine;
        }
    }
}
