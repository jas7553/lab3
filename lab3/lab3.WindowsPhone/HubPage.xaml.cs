using lab3.Common;
using lab3.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;
using Windows.Devices.Geolocation;
using Newtonsoft.Json.Linq;
//using Microsoft.Phone.Notification;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace lab3
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private Geolocator locator;

        public HubPage()
        {
            /*
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = "Lab3Channel";
            */

            InitializeComponent();

            /*
            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();
            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                apiSetPush(pushChannel.ChannelUri.ToString());
            }
            */

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            locator = new Geolocator();
            locator.MovementThreshold = 1;
            locator.StatusChanged += (sender, args) =>
            {
                Debug.WriteLine("StatusChanged");
            };
            locator.PositionChanged += (sender, args) =>
            {
                Debug.WriteLine("PositionChanged");
                Debug.WriteLine(args);
                apiSetLocation(args.Position.Coordinate.Latitude.ToString(), args.Position.Coordinate.Longitude.ToString(), args.Position.Coordinate.Accuracy.ToString());
            };
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
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
            Debug.WriteLine("starting!!!");
            var users = await SampleDataSource.GetUsersAsync();
            Debug.WriteLine("stopping");
            this.DefaultViewModel["Users"] = users;
            foreach (var item in users)
            {
                Debug.WriteLine(item);
            }

            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
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

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Details about the click event.</param>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Defaults about the click event.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
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

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            apiSetLocation();
        }

        private async void apiSetLocation()
        {
            Geoposition position = await locator.GetGeopositionAsync();
            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setLocation"),
                        new KeyValuePair<string, string>("email", "derp@derp.gov"),
                        new KeyValuePair<string, string>("password", "password"),
                        new KeyValuePair<string, string>("lat", position.Coordinate.Latitude.ToString()),
                        new KeyValuePair<string, string>("long", position.Coordinate.Longitude.ToString()),
                        new KeyValuePair<string, string>("acc", position.Coordinate.Accuracy.ToString()),
                    };
            Debug.WriteLine("sending...");
            var response = await API.sendCommand(parameters);
            Debug.WriteLine(response);
        }

        private async void apiSetLocation(string latitude, string longitude, string accuracy)
        {
            Geoposition position = await locator.GetGeopositionAsync();
            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setLocation"),
                        new KeyValuePair<string, string>("email", "derp@derp.gov"),
                        new KeyValuePair<string, string>("password", "password"),
                        new KeyValuePair<string, string>("lat", latitude),
                        new KeyValuePair<string, string>("long", longitude),
                        new KeyValuePair<string, string>("acc", accuracy),
                    };
            Debug.WriteLine("sending...");
            var response = await API.sendCommand(parameters);
            Debug.WriteLine(response);
        }

        private async void apiSetPush(string push)
        {
            Geoposition position = await locator.GetGeopositionAsync();
            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setPush"),
                        new KeyValuePair<string, string>("email", "derp@derp.gov"),
                        new KeyValuePair<string, string>("password", "password"),
                        new KeyValuePair<string, string>("pushUrl", push)
                    };
            Debug.WriteLine("sending...");
            var response = await API.sendCommand(parameters);
            Debug.WriteLine(response);
        }

        /*
        /// #pragma mark - PushChannel methods

        /// <summary>
        /// Event handler for when the push channel Uri is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
            System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
            apiSetPush(e.ChannelUri.ToString());
        }

        /// <summary>
        /// Event handler for when a push notification error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
        }

        /// <summary>
        /// Event handler for when a toast notification arrives while your application is running.  
        /// The toast will not display if your application is running so you must add this
        /// event handler if you want to do something with the toast notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
        }
        */
    }
}