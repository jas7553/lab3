/**
 * Filename: UserViewModel.cs
 * Author: Jason A Smith <jas7553>
 * Assignment: CSCI-641-01 Lab 03
 * Date: 05/09/2014
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace lab3.ViewModels
{
    class UserViewModel : INotifyPropertyChanged
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
    }
}
