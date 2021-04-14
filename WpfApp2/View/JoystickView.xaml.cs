using AdvancedCoding2;
using System.ComponentModel;
using System.Windows;
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    /// <summary>
    /// Interaction logic for JoystickView.xaml
    /// </summary>
    public partial class JoystickView : Window
    {
        /***Data Members***/
        public viewModelJoystick joystickVM;
        /***Methods***/
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
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
