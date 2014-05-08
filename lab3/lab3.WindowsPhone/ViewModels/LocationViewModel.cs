using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace lab3.ViewModels
{
    class LocationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (value != _firstName)
                {
                    _firstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (value != _lastName)
                {
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        private string _latitude;
        public string Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                if (value != _latitude)
                {
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        private string _longitude;
        public string Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (value != _longitude)
                {
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        private string _accuracy;
        public string Accuracy
        {
            get
            {
                return _accuracy;
            }
            set
            {
                if (value != _accuracy)
                {
                    _accuracy = value;
                    NotifyPropertyChanged("Accuracy");
                }
            }
        }

        private string _lastUpdated;
        public string LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                if (value != _lastUpdated)
                {
                    _lastUpdated = value;
                    NotifyPropertyChanged("LastUpdated");
                }
            }
        }
    }
}
