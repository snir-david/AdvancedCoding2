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
            Loaded += JoystickView_Loaded;
        }

        private void JoystickView_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
    }
}
