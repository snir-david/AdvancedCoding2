using AdvancedCoding2;
using System.Windows;
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    /// <summary>
    /// Interaction logic for JoystickView.xaml
    /// </summary>
    public partial class JoystickView : Window
    {
        private viewModelJoystick joystickVM;

        public JoystickView(IClientModel c)
        {
            InitializeComponent();
            joystickVM = new viewModelJoystick(c);
            DataContext = joystickVM;
        }
    }
}
