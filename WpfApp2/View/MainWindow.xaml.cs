using DesktopFGApp;
using DesktopFGApp.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Runtime;
using System.Windows.Media;
using WpfApp2.View;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Threading;

namespace AdvancedCoding2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /***Data Members***/
        public ViewModelController controllerViewModel;
        private IClientModel clientModel;
        private JoystickView joystickView;
        private graphView graphV;
        /***Methods***/
        public MainWindow()
        {
            InitializeComponent();
            //creating a client instance
            Client c = new Client("localhost", 5400);
            clientModel = c;
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
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
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
                graph_button.Visibility = Visibility.Visible;
                joystick_button.Visibility = Visibility.Visible;
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
            if (controllerViewModel.VM_fpath != null)
            {
                CSV_button.Visibility = Visibility.Hidden;
                //read csv and start playing simulator
                readCSVfile();
                controllerViewModel.splitAtt();
                //Play_Button_Click(this, null);
                AnomalyDll_button.Visibility = Visibility.Visible;
            }
        }
        private void OpenXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                controllerViewModel.VM_XMLPath = openFileDialog.FileName;
            if (controllerViewModel.VM_XMLPath != null)
            {
                controllerViewModel.copyXML();
                controllerViewModel.xmlPraser();
                XML_button.Visibility = Visibility.Hidden;
                AnomalyDll_button.Visibility = Visibility.Visible;
            }
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
        private void graph_button_Click(object sender, RoutedEventArgs e)
        {
            if (graphV == null) 
                graphV = new graphView(clientModel, controllerViewModel);
            graphV.Show();
            graph_button.Visibility = Visibility.Hidden;
            hide_graph_button.Visibility = Visibility.Visible;
        }
        private void hide_graph_button_Click(object sender, RoutedEventArgs e)
        {
            if (graphV != null)
            {
                graphV.Hide();
                graph_button.Visibility = Visibility.Visible;
                hide_graph_button.Visibility = Visibility.Hidden;
            }
        }
        private void close_joystick_button_Click(object sender, RoutedEventArgs e)
        {
            if (joystickView != null)
            {
                joystickView.Hide();
                joystick_button.Visibility = Visibility.Visible;
                close_joystick_button.Visibility = Visibility.Hidden;
            }
        }
        private void joystick_button_Click(object sender, RoutedEventArgs e)
        {
            if(joystickView == null)
                joystickView = new JoystickView(clientModel);
            joystickView.Show();
            joystick_button.Visibility = Visibility.Hidden;
            close_joystick_button.Visibility = Visibility.Visible;
        }
        private void anomaly_detector_algorithim(object sender, RoutedEventArgs e)
        {
           OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                controllerViewModel.VM_DLLPath = openFileDialog.FileName;
            if (controllerViewModel.VM_DLLPath != null)
            {
                controllerViewModel.pauseConnection();
                controllerViewModel.dllHandling();
                ticksInit();
                controllerViewModel.resumeConnection();
                //Play_Button_Click(this, null);
            }
        }
        private void ticksInit()
        {
            DoubleCollection ticksMarks = new DoubleCollection();
            foreach (KeyValuePair <string, List<int>> entry in controllerViewModel.VM_AnomalyReport)
            {
                foreach(int i in entry.Value)
                {
                    // ticks for slider - every timestamp
                    ticksMarks.Add(i);
                }
            }
            time_slider.Ticks = ticksMarks;
        }
    }
}
    
