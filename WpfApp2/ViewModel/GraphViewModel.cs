﻿using AdvancedCoding2;
using DesktopFGApp.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace DesktopFGApp.ViewModel
{
    class GraphViewModel : INotifyPropertyChanged
    {
        /***Data Members***/
        private IClientModel clientModel;
        private ViewModelController viewModelController;
        public event PropertyChangedEventHandler PropertyChanged;
        private Stopwatch timer;
        private long lastUpdate;
        private int attChooseIdx, corrChooseIdx;
        private float regMin, regMax, regFxMax, regFxMin;
        private string corrItemName;
        private List<string> attList;
        private List<float> floatAttList, floatCorrList;
        private PlotModel AttPlotModel, CorrPlotModel, regLinePlotModel;
        private OxyPlot.Wpf.PlotView VM_pvAtt, VM_pvCorr, VM_pvLR;
        public Circle c;

        /***Properties***/
        public PlotModel VM_AttPlotModel
        {
            get
            {
                return AttPlotModel;
            }
            set
            {
                if (VM_AttPlotModel != value)
                {
                    AttPlotModel = value;
                    onPropertyChanged("VM_AttPlotModel");

                }
            }
        }
        public PlotModel VM_CorrPlotModel
        {
            get
            {
                return CorrPlotModel;
            }
            set
            {
                if (VM_CorrPlotModel != value)
                {
                    CorrPlotModel = value;
                    onPropertyChanged("VM_CorrPlotModel");
                }
            }
        }
        public PlotModel VM_RegLinePlotModel
        {
            get
            {
                return regLinePlotModel;
            }
            set
            {
                if (VM_RegLinePlotModel != value)
                {
                    regLinePlotModel = value;
                    onPropertyChanged("VM_RegLinePlotModel");
                }
            }
        }
        public string VM_AttUserChoose
        {
            get
            {
                return clientModel.attributeChosen;
            }
            set
            {
                if (VM_AttUserChoose != value)
                {
                    clientModel.attributeChosen = value;
                    onPropertyChanged("VM_AttUserChoose");
                }
            }
        }
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
        public int VM_currLine
        {
            get
            {
                return clientModel.lineNumber;
            }
        }
        public List<List<string>> VM_ListOfListAtt
        {
            get
            {
                return clientModel.ListOfListOfAtt;
            }
        }
        public List<string> VM_attsName
        {
            get
            {
                return clientModel.HeaderNames;
            }
        }
        public List<float> VM_attChooseFloatList
        {
            get
            {
                return floatAttList;
            }
        }
        public List<float> VM_corrFloatList
        {
            get
            {
                return floatCorrList;
            }
        }
        /***Methods***/
        // the constructor of the Graph View Model - getting a client object and 3 plotviews for graphs
        public GraphViewModel(IClientModel c, ViewModelController vmc , OxyPlot.Wpf.PlotView Attpv, OxyPlot.Wpf.PlotView Corrpv, OxyPlot.Wpf.PlotView regLinepv)
        {
            //getting args from constructor
            clientModel = c;
                        viewModelController = vmc;

            VM_pvAtt = Attpv;
            VM_pvCorr = Corrpv;
            VM_pvLR = regLinepv;
            //setting up timer
            timer = new Stopwatch();
            lastUpdate = 0;
            timer.Start();

            clientModel.xmlParser();

            //Plot models inti
            VM_AttPlotModel = new PlotModel();
            VM_CorrPlotModel = new PlotModel();
            VM_RegLinePlotModel = new PlotModel();
            //init list for later
            floatAttList = new List<float>();
            floatCorrList = new List<float>();
            //onPropertyChanged methods
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
            // responsible of updating the graphs
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "lineNumber" && VM_AttUserChoose != null && VM_corralative != null && VM_pvLR != null)
                {
                    if (timer.ElapsedMilliseconds - lastUpdate > 500)
                    {
                        //setting up attPlotModel
                        VM_AttPlotModel.Series.Clear();
                        SetUpModel(VM_AttPlotModel);
                        LoadLineDataGraph(VM_currLine, VM_pvAtt, floatAttList,VM_AttPlotModel );
                        VM_pvAtt.InvalidatePlot(true);
                        //setting up corrPlotModel
                        VM_CorrPlotModel.Series.Clear();
                        SetUpModel(VM_CorrPlotModel);
                        LoadLineDataGraph(VM_currLine, VM_pvCorr, floatCorrList, VM_CorrPlotModel);
                        VM_pvCorr.InvalidatePlot(true);
                        //setting up regLinePlotModel or minCircle
                        VM_RegLinePlotModel.Series.Clear();
                        SetUpModel(VM_RegLinePlotModel);
                        if(viewModelController.isRegLine)
                            LoadScatterGraphData(VM_currLine, VM_pvLR, floatAttList, floatCorrList, VM_RegLinePlotModel);
                        if(viewModelController.isCircel)
                            LoadCircleGraphData(VM_currLine, VM_pvLR, floatAttList, floatCorrList, VM_RegLinePlotModel);
                            VM_pvLR.InvalidatePlot(true);
                        //updating lastUpdate timer
                        lastUpdate = timer.ElapsedMilliseconds;
                    }
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
        // gets a list of strings and returns a list of floats
        /************* only this class uses this function *************/
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
        public string FindCorralativeFeature(string attName)
        {
            int counter = 0;
            double max = 0;
            double result = 0;
            corrChooseIdx = 0;
            //finding attributes choseb index in list of list
            attChooseIdx = viewModelController.VM_headerNames.FindIndex(a => a.Contains(attName));
            // the values of the chosen feature
            attList = viewModelController.VM_currentAtt[attChooseIdx];
            floatAttList = stringToFloat(attList);
            //finding most corrleative attribute            
            foreach (List<string> list in viewModelController.VM_currentAtt)
            {
                if (counter == attChooseIdx)
                {
                    counter++;
                }
                else
                {
                    floatCorrList = stringToFloat(list);
                    result = Math.Abs(clientModel.pearson(floatAttList, floatCorrList, floatCorrList.Count));
                    if (result > max)
                    {
                        max = result;
                        corrChooseIdx = counter;
                    }
                    counter++;
                }
            }
            // takes care of the atts with vector 0
            if (corrChooseIdx == 0 && attName != viewModelController.VM_headerNames[0])
            {
                corrItemName = attName;
                VM_corralative = corrItemName;
                floatCorrList = floatAttList;
                return corrItemName;
            }
            corrItemName = viewModelController.VM_headerNames[corrChooseIdx];
            VM_corralative = corrItemName;
            List<Point> pointList = new List<Point>();
            for (int i = 0; i < attList.Count; i++)
            {
                pointList.Add(new Point(floatAttList[i], floatCorrList[i]));
            }
            if (viewModelController.isCircel)
            {
                c = ModelUtil.findMinCircle(pointList, (pointList.Count-1)); 
            }
            else if (viewModelController.isRegLine)
            {
                //finding reg Line
                Line regLine = clientModel.linear_reg(pointList, pointList.Count);
                //finding max and min values of attList;
                regMax = floatAttList.Max();
                regMin = floatAttList.Min();
                regFxMax = regLine.f(regMax);
                regFxMin = regLine.f(regMin);
            }
            return corrItemName;
        }
        // creates the chosen graph
        public void LoadLineDataGraph(int lineNumber, OxyPlot.Wpf.PlotView pv, List<float> valueList, PlotModel pm)
        {
           var lineSerie = new LineSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Black,
            };
            for (int i = 0; i < lineNumber; i++)
            {
                lineSerie.Points.Add(new DataPoint(i, valueList[i]));
            }
            //adding lineSeries to plotModel
            pm.Series.Add(lineSerie);
        }
        public void LoadScatterGraphData(int lineNumber, OxyPlot.Wpf.PlotView pv, List<float> attList, List<float> corrList, PlotModel pm)
        {
          //LineSeries for reg line
            var lineSeries = new LineSeries()
            {
                Color = OxyColors.Red,
                StrokeThickness = 2
            };
           //drawing line between to extrem points
            lineSeries.Points.Add(new DataPoint(regMax, regFxMax));
            lineSeries.Points.Add(new DataPoint(regMin, regFxMin));
            //all points besides last 300
            var scatterPoint = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Black
            };
            //last 30 seconds points - in red
            var scatter300Point = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Red
            };
            //Make 300 last points in red
            if (lineNumber > 300)
            {
                for (int i = 0; i < lineNumber - 300; i++)
                {
                    scatterPoint.Points.Add(new ScatterPoint(attList[i], corrList[i], 2));
                }
                for (int i = lineNumber - 300; i < lineNumber; i++)
                {
                    scatter300Point.Points.Add(new ScatterPoint(attList[i], corrList[i], 3));
                }
            } else
            {
                for (int i = 0; i < lineNumber; i++)
                {
                    scatter300Point.Points.Add(new ScatterPoint(attList[i], corrList[i], 3));
                }
            }
            //adding reg line and points to ploat model
            pm.Series.Add(scatter300Point);
            pm.Series.Add(scatterPoint);
            pm.Series.Add(lineSeries);
        }
        public void LoadCircleGraphData(int lineNumber, OxyPlot.Wpf.PlotView pv, List<float> attList, List<float> corrList, PlotModel pm)
        {
            //all points besides last 300
            var scatterPoint = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Black
            };
            //last 30 seconds points - in red
            var scatter300Point = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Red
            };
            //Make 300 last points in red
            if (lineNumber > 300)
            {
                for (int i = 0; i < lineNumber - 300; i++)
                {
                    scatterPoint.Points.Add(new ScatterPoint(attList[i], corrList[i], 2));
                }
                for (int i = lineNumber - 300; i < lineNumber; i++)
                {
                    scatter300Point.Points.Add(new ScatterPoint(attList[i], corrList[i], 3));
                }
            }
            else
            {
                for (int i = 0; i < lineNumber; i++)
                {
                    scatter300Point.Points.Add(new ScatterPoint(attList[i], corrList[i], 3));
                }
            }
            //adding reg line and points to ploat model
            pm.Series.Add(scatter300Point);
            pm.Series.Add(scatterPoint);
            if (c != null)
            {
                pm.Series.Add(new FunctionSeries((x) => Math.Sqrt(Math.Max(Math.Pow(c.radius, 2) - Math.Pow(x - c.center.x, 2), 0)) + c.center.y, -c.radius, c.radius, 0.1) { Color = OxyColors.Red });
                pm.Series.Add(new FunctionSeries((x) => (-Math.Sqrt(Math.Max(Math.Pow(c.radius, 2) - Math.Pow(x - c.center.x, 2), 0))) + c.center.y, -c.radius, c.radius, 0.1) { Color = OxyColors.Red });
            }
        }
        // sets up a given graph
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

