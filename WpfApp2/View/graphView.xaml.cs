using AdvancedCoding2;
using DesktopFGApp.ViewModel;
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
        private GraphViewModel graphViewModel;

        public graphView(IClientModel c)
        {
            InitializeComponent();
            this.graphViewModel = new GraphViewModel(c);
            this.DataContext = graphViewModel;

            StackPanel stackPanel = new StackPanel();
            foreach(string name in graphViewModel.nameList)
            {
                stackPanel.Children.Add(new Button() { Content = name });
            }
            scorllButtons.Content = stackPanel;
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // gets the name of the button
            string name = (sender as Button).Content.ToString();
            Console.WriteLine("got here");
        }
    }
}
