using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace lab3.ViewModels
{
    class MessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _msgType;
        public string MsgType
        {
            get
            {
                return _msgType;
            }
            set
            {
                if (value != _msgType)
                {
                    _msgType = value;
                    NotifyPropertyChanged("MsgType");
                }
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        private string _ts;
        public string Ts
        {
            get
            {
                return _ts;
            }
            set
            {
                if (value != _ts)
                {
                    _ts = value;
                    NotifyPropertyChanged("Ts");
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
    }
}
