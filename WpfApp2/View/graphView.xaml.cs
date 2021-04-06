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
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Threading;

namespace DesktopFGApp.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class graphView : Window
    {
        private GraphViewModel graphViewModel;
        private string attName , corrName;
        PlotModel pml = new PlotModel();
        OxyPlot.Wpf.PlotView pv;
        


        public graphView(IClientModel c)
        {
            InitializeComponent();
            this.graphViewModel = new GraphViewModel(c, AttPlot);
            this.DataContext = graphViewModel;
            StackPanel stackPanel = new StackPanel();
            foreach(string name in graphViewModel.nameList)
            {
                Button b = new Button();
                b.Click += Button_Click;
                b.Content = name;
                stackPanel.Children.Add(b);
            }
            scorllButtons.Content = stackPanel;
            pv = AttPlot;
            
        }


        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // gets the name of the button
            attName = (sender as Button).Content.ToString();
            graphViewModel.VM_chosen = attName;
            corrName = graphViewModel.FindCorralativeFeature(attName);
            graphViewModel.VM_corralative = corrName;
            graphViewModel.SetUpModel(pml);
           
            pml.Series.Clear();
            graphViewModel.LoadData(graphViewModel.VM_currLine, pv);
            AttPlot.InvalidatePlot(true);
            
        }

        
        
    }
}
