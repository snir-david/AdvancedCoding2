using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace AdvancedCoding2
{
    public class ViewModelController : INotifyPropertyChanged
    {
        private IClientModel clientModel;
        public event PropertyChangedEventHandler PropertyChanged;


        private int playSpeed;
        public int VM_playSpeed
        {
            get
            {
                return playSpeed;
            }
            set
            {
                playSpeed = value;
                clientModel.TransSpeed = playSpeed;
                onPropertyChanged("VM_playSpeed");
            }
        }

        //public ViewModelController() { }

        public ViewModelController(IClientModel m)
        {
            this.clientModel = m;
            clientModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                onPropertyChanged("VM_" + e.PropertyName);
            };
        }


        public void onPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }

}
