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
using Windows.UI.Xaml.Controls.Maps;

using System.Diagnostics;
using Windows.Devices.Geolocation;
using Newtonsoft.Json.Linq;
using Windows.Networking.PushNotifications; 

using Windows.UI;
using Windows.UI.Popups;

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

        //private Windows.UI.Xaml.Shapes.Rectangle fence;
        //private BasicGeoposition fencePos;
        private MapControl myMap;

        public HubPage()
        {
            /*
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = "Lab3Channel";
            */

            InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            registerPush();            

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        async private void registerPush()
        {
            PushNotificationChannel channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += channel_PushNotificationReceived;
            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setPush"),
                        new KeyValuePair<string, string>("pushUrl", channel.Uri.ToString())
                    };
            var response = await API.sendCommand(parameters);
        }

        void channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            throw new System.NotImplementedException();
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
            var users = await SampleDataSource.GetUsersAsync();
            this.DefaultViewModel["Users"] = users;

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
        async private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            String userEmail = ((User)e.ClickedItem).Email;
            IEnumerable<Message> messages = await SampleDataSource.GetMessageAsync(userEmail);
            Tuple<String, IEnumerable<Message>> args = new Tuple<string, IEnumerable<Message>>(userEmail, messages);
            if (!Frame.Navigate(typeof(UserPage), args))
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
        
        private async void apiSetLocation(string latitude, string longitude, string accuracy)
        {
            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "setLocation"),
                        new KeyValuePair<string, string>("lat", latitude),
                        new KeyValuePair<string, string>("long", longitude),
                        new KeyValuePair<string, string>("acc", accuracy),
                    };
            var response = await API.sendCommand(parameters);
        }

        async private void MyMap_OnLoaded(object sender, RoutedEventArgs e)
        {
            Geolocator locator = new Geolocator();
            locator.ReportInterval = 2000;
            locator.MovementThreshold = 1;
            locator.PositionChanged += (sender2, args) => {
                Geoposition pos = args.Position;
                string la = pos.Coordinate.Point.Position.Latitude.ToString();
                string lo = pos.Coordinate.Point.Position.Longitude.ToString();
                string ac = pos.Coordinate.Accuracy.ToString();
                apiSetLocation(la, lo, ac);
            };

            myMap = (MapControl)sender;
            myMap.Center = new Geopoint(new BasicGeoposition() { Altitude = 643, Latitude = 43.089863, Longitude = -77.669609 });
            myMap.ZoomLevel = 14;

            IEnumerable<Location> locations = await SampleDataSource.GetLocationsAsync();

            foreach (Location location in locations)
            {
                Double la = Convert.ToDouble(location.Latitude);
                Double lo = Convert.ToDouble(location.Longitude);
                Double al = 643;
                Geopoint pt = new Geopoint(new BasicGeoposition() { Latitude = la, Longitude = lo, Altitude = al });
                Windows.UI.Xaml.Shapes.Rectangle fence2 = new Windows.UI.Xaml.Shapes.Rectangle();
                fence2.Name = location.Email;
                fence2.Tapped += fence_Tapped;
                fence2.Width = 30;
                fence2.Height = 30;
                BitmapImage img2 = new BitmapImage(new Uri("ms-appx:///Assets/redpin.png"));
                fence2.Fill = new ImageBrush() { ImageSource = img2 };
                myMap.Children.Add(fence2);
                MapControl.SetLocation(fence2, pt);
                MapControl.SetNormalizedAnchorPoint(fence2, new Point(1.0, 0.5));
            }
        }

        async private void fence_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Windows.UI.Xaml.Shapes.Rectangle fench = (Windows.UI.Xaml.Shapes.Rectangle)sender;
            String userEmail = fench.Name;

            User user = await SampleDataSource.GetUserAsync(userEmail);
            if (user == null)
            {
                Debug.WriteLine("bad user: " + userEmail);
                return;
            }

            MessageDialog myDialog = new MessageDialog("Chat with " + user.FirstName + " " + user.LastName + "?");

            myDialog.Commands.Add(new UICommand("Yes", async (x) =>
            {
                IEnumerable<Message> messages = await SampleDataSource.GetMessageAsync(userEmail);
                Tuple<String, IEnumerable<Message>> args = new Tuple<string, IEnumerable<Message>>(userEmail, messages);
                if (!Frame.Navigate(typeof(UserPage), args))
                {
                    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
                }
            }));

            myDialog.Commands.Add(new UICommand("No", (x) => {}));

            myDialog.DefaultCommandIndex = 0;
            myDialog.CancelCommandIndex = 1;

            await myDialog.ShowAsync();
        }
    }
}