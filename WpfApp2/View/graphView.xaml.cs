using AdvancedCoding2;
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
using WpfApp2.ViewModel;

namespace DesktopFGApp.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class graphView : Window
    {
        public viewModelJoystick joystickVM;
        public graphView(IClientModel c)
        {
            InitializeComponent();
            //joystickVM = new viewModelJoystick(c);
            //DataContext = joystickVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // gets the name of the button
            string name = (sender as Button).Content.ToString();
            Console.WriteLine("got here");
        }
    }
}
