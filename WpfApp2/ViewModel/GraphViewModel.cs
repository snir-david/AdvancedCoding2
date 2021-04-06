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
        private PlotModel plotModel1, pml1 = new PlotModel();
        private PlotModel plotModel2, pml2 = new PlotModel();
        //private PlotModel plotModel3, pml3 = new PlotModel();
        private OxyPlot.Wpf.PlotView VM_pvAtt , VM_pvCorr; //, VM_pvLR;

        // property for the chart of the chosen.
        public PlotModel VM_PlotModel1
        {
            get
            {
                return plotModel1;
            }
            set
            {
                if (VM_PlotModel1 != value)
                {
                    plotModel1 = value;
                    onPropertyChanged("VM_PlotModel1");

                }
            }
        }

        // property for the chart of the corr.
        public PlotModel VM_PlotModel2
        {
            get
            {
                return plotModel2;
            }
            set
            {
                if (VM_PlotModel2 != value)
                {
                    plotModel2 = value;
                    onPropertyChanged("VM_PlotModel2");

                }
            }
        }

        // property for the Linear Reg.
        /*public PlotModel VM_PlotModel3
        {
            get
            {
                return plotModel3;
            }
            set
            {
                if (VM_PlotModel3 != value)
                {
                    plotModel3 = value;
                    onPropertyChanged("VM_PlotModel3");

                }
            }
        }*/
        // property of the list of att.
        public List<String> nameList
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }
        // property of the chosen att.
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
        // property for the corralative feature.
        public string VM_corralative
        {
            get
            {
                return clientModel.Corralative;
            }
            set
            {
                if (VM_corralative != value)
                {
                    clientModel.Corralative = value;
                    onPropertyChanged("VM_corralative");
                }
            }
        }
        // property of the current line we are in.
        public int VM_currLine
        {
            get
            {
                return clientModel.lineNumber;
            }
        }
        // property of the list of all the values of the atts.
        public List<List<string>> VM_attsList
        {
            get
            {
                return clientModel.CurrentAtt;
            }
        }
        // property of the list of the att's names.
        public List<string> VM_attsName
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }

        // the constructor of the Graph View Model.
        public GraphViewModel(IClientModel c, OxyPlot.Wpf.PlotView pv1 , OxyPlot.Wpf.PlotView pv2 /*, OxyPlot.Wpf.PlotView pv3*/)
        {
            this.clientModel = c;
            this.VM_pvAtt = pv1;
            this.VM_pvCorr = pv2;
            //this.VM_pvLR = pv3;

            clientModel.xmlParser();
            this.viewModelController = new ViewModelController(this.clientModel);
            VM_PlotModel1 = new PlotModel();
            VM_PlotModel2 = new PlotModel();
            //VM_PlotModel3 = new PlotModel();

            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
            
            // responsibles of updating the graph.
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "lineNumber" && VM_chosen != null && VM_corralative != null)
                {
                    pml1.Series.Clear();
                    SetUpModel(pml1);
                    LoadAttData(VM_currLine, VM_pvAtt);
                    VM_pvAtt.InvalidatePlot(true);

                    pml2.Series.Clear();
                    SetUpModel(pml2);
                    LoadCorrData(VM_currLine, VM_pvCorr);
                    VM_pvCorr.InvalidatePlot(true);

                    /*pml3.Series.Clear();
                    SetUpModel(pml3);                                                        
                    VM_pvLR.InvalidatePlot(true);*/
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

        // gets a list of strings and returns a list of floats.
        /************* only this class uses this function. *************/
        private List<float> stringToFloat(List<string> list)
        {
            List<float> result = new List<float>();
            foreach (string s in list)
            {
                result.Add(float.Parse(s));
            }
            return result;
        }
        
        // finds the corralated feature.
        /************ the graph view uses it ***********/
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
            // takes care of the atts with vector 0.
            if(indexOfCorr == 0 && s != viewModelController.VM_headerNames[0])
            {
                corrFeature = s;
                VM_corralative = corrFeature;
                return corrFeature;
            }

            corrFeature = viewModelController.VM_headerNames[indexOfCorr];
            VM_corralative = corrFeature;
            return corrFeature;
        }

        // creates the chosen graph.
        public void LoadAttData(int lineNumber, OxyPlot.Wpf.PlotView pv)
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

            pml1.Series.Add(lineSerie);
            VM_PlotModel1 = pml1;
        }
        
        // creates the corralative graph.
        // ********** maybe we can put it in the view graph ***********/
        public void LoadCorrData(int lineNumber, OxyPlot.Wpf.PlotView pv)
        {
            int idx = VM_attsName.FindIndex(a => a.Contains(VM_corralative));
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

            pml2.Series.Add(lineSerie);
            VM_PlotModel2 = pml2;
        }

        // sets up a given "graph".
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

