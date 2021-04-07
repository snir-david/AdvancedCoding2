using AdvancedCoding2;
using DesktopFGApp.ViewModel;
using OxyPlot;
using System.Windows;
using System.Windows.Controls;

namespace DesktopFGApp.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class graphView : Window
    {
        /***Data Members***/
        private GraphViewModel graphViewModel;
        private string attName, corrName;
        private OxyPlot.Wpf.PlotView Attpv, Corrpv, RegLinepv;
        /***Methods***/
        public graphView(IClientModel c)
        {
            InitializeComponent();
            this.graphViewModel = new GraphViewModel(c, attPlot, corrPlot, LRPlot);
            this.DataContext = graphViewModel;
            StackPanel stackPanel = new StackPanel();
            //creating buttons
            foreach (string name in graphViewModel.VM_attsName)
            {
                Button b = new Button();
                b.Click += Button_Click;
                b.Content = name;
                stackPanel.Children.Add(b);
            }
            scorllButtons.Content = stackPanel;
            Attpv = attPlot;
            Corrpv = corrPlot;
            RegLinepv = LRPlot;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // gets the name of the button
            attName = (sender as Button).Content.ToString();
            graphViewModel.VM_AttUserChoose = attName;
            corrName = graphViewModel.FindCorralativeFeature(attName);
            graphViewModel.VM_corralative = corrName;
            //setting up 3 plot models - attPlot
            graphViewModel.SetUpModel(graphViewModel.VM_AttPlotModel);
            graphViewModel.VM_AttPlotModel.Series.Clear();
            graphViewModel.LoadLineDataGraph(graphViewModel.VM_currLine, Attpv, graphViewModel.VM_attChooseFloatList);
            attPlot.InvalidatePlot(true);
            //corrPlot
            graphViewModel.SetUpModel(graphViewModel.VM_CorrPlotModel);
            graphViewModel.VM_CorrPlotModel.Series.Clear();
            graphViewModel.LoadLineDataGraph(graphViewModel.VM_currLine, Corrpv, graphViewModel.VM_corrFloatList);
            corrPlot.InvalidatePlot(true);
            //regLine plot
            graphViewModel.SetUpModel(graphViewModel.VM_RegLinePlotModel);
            graphViewModel.VM_RegLinePlotModel.Series.Clear();
            graphViewModel.LoadScatterGraphData(graphViewModel.VM_currLine, RegLinepv, graphViewModel.VM_attChooseFloatList, graphViewModel.VM_corrFloatList);
            LRPlot.InvalidatePlot(true);
        }
    }
}
