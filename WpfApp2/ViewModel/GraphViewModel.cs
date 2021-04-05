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
        public event PropertyChangedEventHandler PropertyChanged;

        public List<String> nameList
        {
            get
            {
                return clientModel.HeaderNames;
            }
        } 

        public GraphViewModel(IClientModel c)
        {
            this.clientModel = c;
            clientModel.xmlParser();
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

    }
}
