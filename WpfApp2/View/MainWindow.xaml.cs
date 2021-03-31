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
            controllerViewModel = new ViewModelController(new Client("localhost", 5400));
            this.DataContext = controllerViewModel;

        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            controllerViewModel.pauseConnection();
            controllerViewModel.VM_playSpeed = 0;
        }
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
                controllerViewModel.connect();
                controllerViewModel.VM_playSpeed = 1;
                controllerViewModel.VM_TransSpeed = 100;
            
        }
        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            if(controllerViewModel.VM_playSpeed  - 0.1 > 0.1)
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
    }
}
