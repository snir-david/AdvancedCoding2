using AdvancedCoding2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace DesktopFGApp.ViewModel
{
    class GraphViewModel: INotifyPropertyChanged
    {
        private IClientModel clientModel;
        private ViewModelController viewModelController;
        public event PropertyChangedEventHandler PropertyChanged;
        private List<string> attList;
        private PlotModel plotModel, pml = new PlotModel();
        private OxyPlot.Wpf.PlotView VM_pv;


        public PlotModel VM_PlotModel
        {
            get
            {
                return plotModel;
            }
            set
            {
                if (VM_PlotModel != value)
                {
                    plotModel = value;
                    onPropertyChanged("VM_PlotModel");

                }
            }
        }
        public List<String> nameList
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }
        public string VM_chosen
        {
            get
            {
                return clientModel.Chosen;
            }
            set
            {
                if (VM_chosen != value)
                {
                    clientModel.Chosen = value;
                    onPropertyChanged("VM_chosen");
                }
            }
        }
        public string VM_corralative
        {
            get
            {
                return clientModel.Chosen;
            }
            set
            {
                if (VM_corralative != value)
                {
                    clientModel.Chosen = value;
                    onPropertyChanged("VM_corralative");
                }
            }
        }
        public int VM_currLine
        {
            get
            {
                return clientModel.lineNumber;
            }
        }
        public List<List<string>> VM_attsList
        {
            get
            {
                return clientModel.CurrentAtt;
            }
        }
        public List<string> VM_attsName
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }

        public GraphViewModel(IClientModel c, OxyPlot.Wpf.PlotView pv)
        {
            this.clientModel = c;
            this.VM_pv = pv;
            clientModel.xmlParser();
            this.viewModelController = new ViewModelController(this.clientModel);
            VM_PlotModel = new PlotModel();

            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "lineNumber" && VM_chosen != null)
                {
                    pml.Series.Clear();
                    SetUpModel(pml);
                    LoadData(VM_currLine, VM_pv);
                    VM_pv.InvalidatePlot(true);
                }
            };
               
        }

        public void onPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private List<float> stringToFloat(List<string> list)
        {
            List<float> result = new List<float>();
            foreach (string s in list)
            {
                result.Add(float.Parse(s));
            }
            return result;
        }
        
        // finds the corralated feture
        public string FindCorralativeFeature(string s)
        {
            string corrFeature;
            List<string> valuesOfChosen = new List<string>();
            List<string> valuesOfCorr = new List<string>();

            int index = viewModelController.VM_headerNames.FindIndex(a => a.Contains(s));
            // the values of the chosen feature.
            valuesOfChosen = viewModelController.VM_currentAtt[index];
            List<float> valuesOfChosenInFloat = new List<float>();
            valuesOfChosenInFloat = stringToFloat(valuesOfChosen);

            int indexOfCorr = 0;
            int counter = 0;
            double max = 0;
            double result = 0;

            foreach (List<string> list in viewModelController.VM_currentAtt)
            {
               
                List<float> valuesInFloat = new List<float>();
                if (counter == index)
                {
                    counter++;
                    
                }
                else
                {
                    valuesInFloat = stringToFloat(list);
                    result = Math.Abs(clientModel.pearson(valuesOfChosenInFloat, valuesInFloat,valuesInFloat.Count));
                    if (result > max)
                    {
                        max = result;
                        indexOfCorr = counter;
                    }
                    counter++;
                }
            }
            corrFeature = viewModelController.VM_headerNames[indexOfCorr];
            return corrFeature;
        }

        public void LoadData(int lineNumber, OxyPlot.Wpf.PlotView pv)
        {
            int idx = VM_attsName.FindIndex(a => a.Contains(VM_chosen));
            attList = VM_attsList[idx];

            var lineSerie = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Black,
            };

            for (int i = 0; i < lineNumber; i++)
            {
                lineSerie.Points.Add(new DataPoint(i, Double.Parse(attList[i])));
            }

            pml.Series.Add(lineSerie);
            VM_PlotModel = pml;
        }

        public void SetUpModel(PlotModel pm)
        {
            pm = new PlotModel();
            pm.LegendOrientation = LegendOrientation.Horizontal;
            pm.LegendPlacement = LegendPlacement.Outside;
            pm.LegendPosition = LegendPosition.TopRight;
            pm.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            pm.LegendBorder = OxyColors.Black;

            //Creating Axis
            var timeAxis = new LinearAxis() { Position = AxisPosition.Bottom, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80, Title = "Time" };
            pm.Axes.Add(timeAxis);
            var valueAxis = new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            pm.Axes.Add(valueAxis);
        }
    }

}

