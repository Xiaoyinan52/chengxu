using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WireTestProgram
{
  public  class TongjiClass: INotifyPropertyChanged
    {

        public string DesinItemID { get; set; }
        public string _MuKuaiName;
        public string MuKuaiName
        {
            get { return _MuKuaiName; }
            set { _MuKuaiName = value; OnPropertyChanged("MuKuaiName"); }
        }




        public string GroupName { get; set; }
        public string Xzuobiao { get; set; }
        public string Yzuobiao { get; set; }
        public string ZuoBiaoName { get; set; }


        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;


    }
}
