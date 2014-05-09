using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Windows;
using Windows.Storage;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace lab3
{
    public sealed partial class MyUserControl1 : UserControl
    {
        public event EventHandler loginClicked;

        public MyUserControl1()
        {
            this.InitializeComponent();

            emailAddressTextBox.Text = "derp@derp.gov";
            passwordTextBox.Password = "password";
        }

        async private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string username = emailAddressTextBox.Text;
            string password = passwordTextBox.Password;

            if (username.Equals("") || password.Equals(""))
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Bad Login";
                return;
            }

            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setLocation"),
                        new KeyValuePair<string, string>("email", username),
                        new KeyValuePair<string, string>("password", password)
                    };
            var response = await API.sendCommand(parameters);
            if (response.Equals("Invalid user"))
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Bad Login";
            }
            else
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["username"] = username;
                localSettings.Values["password"] = password;

                this.loginClicked(null, null);
            }
        }
    }
}
