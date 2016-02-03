using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GDAL_GUI_New
{
    class MyStringBuilder : INotifyPropertyChanged
    {
        private StringBuilder m_StringBuilder;

        public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MyStringBuilder()
        {
            m_StringBuilder = new StringBuilder();
        }

        public String Text
        {
            get { return this.m_StringBuilder.ToString(); }
            set
            {
                this.m_StringBuilder.Append(value);
                OnPropertyChanged("Text");
            }
        }
    }
}
