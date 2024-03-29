﻿/**
 * Filename: UserPage.xaml.cs
 * Author: Jason A Smith <jas7553>
 * Assignment: CSCI-641-01 Lab 03
 * Date: 05/09/2014
 */

using lab3.Common;
using lab3.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace lab3
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private User user;

        public UserPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        } 

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            Tuple<String, IEnumerable<Message>> args = (Tuple<String, IEnumerable<Message>>)e.NavigationParameter;
            this.user = await SampleDataSource.GetUserAsync(args.Item1);

            if (this.user == null)
            {
                Debug.WriteLine("bad user: " + this.user);
            }

            List<Message> msgs = args.Item2.ToList();
            msgs.Sort((x, y) => x.Ts.CompareTo(y.Ts));

            this.DefaultViewModel["Messages"] = msgs;
            Header.Text = this.user.ToString();
            
            chathistorylist.UpdateLayout();
            if (msgs.Count > 0)
            {
                chathistorylist.ScrollIntoView(msgs.Last());
            }

            //var item = await SampleDataSource.GetItemAsync((string)e.NavigationParameter);
            //this.DefaultViewModel["Item"] = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            String msg = MessageBox.Text;
            if (msg.Equals(""))
            {
                return;
            }

            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "sendMessage"),
                        new KeyValuePair<string, string>("message", msg),
                        new KeyValuePair<string, string>("to", this.user.Email),
                    };
            var response = await API.sendCommand(parameters);
            if (response == null)
            {
                return;
            }
            MessageBox.Text = "";

            IEnumerable<Message> messages = await SampleDataSource.GetMessageAsync(user.Email);
            List<Message> msgs = messages.ToList();
            msgs.Sort((x, y) => x.Ts.CompareTo(y.Ts));
            this.DefaultViewModel["Messages"] = msgs;

            chathistorylist.UpdateLayout();
            if (msgs.Count > 0)
            {
                chathistorylist.ScrollIntoView(msgs.Last());
            }
        }

        private void MessageBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString().Equals("Enter"))
            {
                Button_Click(sender, e);
            }
        }
    }
}