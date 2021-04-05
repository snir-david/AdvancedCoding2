using AdvancedCoding2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp.ViewModel
{
    class GraphViewModel: INotifyPropertyChanged
    {
        private IClientModel clientModel;
        private ViewModelController viewModelController;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public GraphViewModel(IClientModel c)
        {
            this.clientModel = c;
            clientModel.xmlParser();
            this.viewModelController = new ViewModelController(this.clientModel);

            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
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


    }
}
