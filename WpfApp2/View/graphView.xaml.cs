using AdvancedCoding2;
using DesktopFGApp.ViewModel;
using OxyPlot;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;

namespace DesktopFGApp.View
{
    /// <summary>
    /// Interaction logic for graphView.xaml
    /// </summary>
    public partial class graphView : Window
    {
        /***Data Members***/
        private GraphViewModel graphViewModel;
        private ViewModelController vmCon;
        private string attName, corrName;
        private OxyPlot.Wpf.PlotView Attpv, Corrpv, RegLinepv;
        /***Methods***/
        public graphView(IClientModel c, ViewModelController vmc)
        {
            InitializeComponent();
            this.vmCon = vmc;
            this.graphViewModel = new GraphViewModel(c, vmc, attPlot, corrPlot, LRPlot);
            this.DataContext = graphViewModel;
            StackPanel stackPanel = new StackPanel();
            //creating buttons
            setupButtons(stackPanel);
            scorllButtons.Content = stackPanel;
            Attpv = attPlot;
            Corrpv = corrPlot;
            RegLinepv = LRPlot;
            Loaded += GraphView_Loaded;
        }

        private void GraphView_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left ;
            this.Top = desktopWorkingArea.Top;
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
            graphViewModel.LoadLineDataGraph(graphViewModel.VM_currLine, Attpv, graphViewModel.VM_attChooseFloatList, graphViewModel.VM_AttPlotModel);
            attPlot.InvalidatePlot(true);
            //corrPlot
            graphViewModel.SetUpModel(graphViewModel.VM_CorrPlotModel);
            graphViewModel.VM_CorrPlotModel.Series.Clear();
            graphViewModel.LoadLineDataGraph(graphViewModel.VM_currLine, Corrpv, graphViewModel.VM_corrFloatList, graphViewModel.VM_CorrPlotModel);
            corrPlot.InvalidatePlot(true);
            //regLine or minCircle plot
            graphViewModel.SetUpModel(graphViewModel.VM_RegLinePlotModel);
            graphViewModel.VM_RegLinePlotModel.Series.Clear();
            if (vmCon.isRegLine)
                graphViewModel.LoadScatterGraphData(graphViewModel.VM_currLine, RegLinepv, graphViewModel.VM_attChooseFloatList, graphViewModel.VM_corrFloatList, graphViewModel.VM_RegLinePlotModel);
            if (vmCon.isCircel)
                graphViewModel.LoadCircleGraphData(graphViewModel.VM_currLine, RegLinepv, graphViewModel.VM_attChooseFloatList, graphViewModel.VM_corrFloatList, graphViewModel.VM_RegLinePlotModel);
            LRPlot.InvalidatePlot(true);
        }
        private void setupButtons(StackPanel stackPanel)
        {
            foreach (string name in graphViewModel.VM_attsName)
            {
                Button b = new Button();
                b.Click += Button_Click;
                b.Content = name;
                stackPanel.Children.Add(b);
                foreach (KeyValuePair<string, List<int>> entry in vmCon.VM_AnomalyReport)
                {
                    if (entry.Key.Contains(b.Content.ToString()))
                    {
                        b.Background = Brushes.SkyBlue;
                    }
                }
                graphViewModel.buttonsList.Add(b);
            }
        }
    }
}
