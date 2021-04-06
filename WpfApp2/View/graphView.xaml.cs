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

namespace DesktopFGApp.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class graphView : Window
    {
        private GraphViewModel graphViewModel;
        private string attName , corrName;
        private List<int> l = new List<int>() { 1 , 2 , 3 ,4 , 1};
        private PlotModel pml = new PlotModel();

        
        public graphView(IClientModel c)
        {
            InitializeComponent();
            this.graphViewModel = new GraphViewModel(c);
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
            
        }


        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // gets the name of the button
            setUpModel(pml);
            setUpModel(graphViewModel.VM_PlotModel);
            LoadData();
            attName = (sender as Button).Content.ToString();
            graphViewModel.VM_chosen = attName;
            corrName = graphViewModel.FindCorralativeFeature(attName);
            graphViewModel.VM_corralative = corrName;

            
        }

        private void setUpModel(PlotModel pm)
        {
            pm.LegendTitle = attName;
            pm.LegendOrientation = LegendOrientation.Horizontal;
            pm.LegendPlacement = LegendPlacement.Outside;
            pm.LegendPosition = LegendPosition.TopRight;
            pm.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            pm.LegendBorder = OxyColors.Black;

            var dateAxis = new LinearAxis() {Maximum = 100 , Minimum = 0 ,Position = AxisPosition.Bottom ,MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 , Title = "time"  };
            pm.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis() { Maximum = 100, Minimum = 0 , Position = AxisPosition.Left , MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = attName };
            pm.Axes.Add(valueAxis);
        }
        private void LoadData()
        {
            //List<Measurement> measurements = Data.GetData();

            //var dataPerDetector = measurements.GroupBy(m => m.DetectorId).ToList();

            foreach (int i in l)
            {
                var lineSerie = new LineSeries
                {
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = OxyColors.Black,
                    MarkerType = MarkerType.None,
                    CanTrackerInterpolatePoints = false,
                    //Title = string.Format("Detector {0}),
                    
                };

                lineSerie.Points.Add(new DataPoint(i, 5));
                pml.Series.Add(lineSerie);
                graphViewModel.VM_PlotModel = pml;
            }
        }
    }
}
