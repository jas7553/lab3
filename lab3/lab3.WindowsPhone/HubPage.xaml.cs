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
//using Microsoft.Phone.Notification;
using Windows.Networking.PushNotifications; 

using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;

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

        private Windows.UI.Xaml.Shapes.Rectangle fence;
        private BasicGeoposition fencePos;
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

        private void MyMap_OnLoaded(object sender, RoutedEventArgs e)
        {
            Geolocator locator = new Geolocator();
            locator.ReportInterval = 1000;
            locator.MovementThreshold = 1;
            locator.PositionChanged += (sender2, args) => {
                string la = args.Position.Coordinate.Latitude.ToString();
                string lo = args.Position.Coordinate.Longitude.ToString();
                string ac = args.Position.Coordinate.Accuracy.ToString();
                apiSetLocation(la, lo, ac);
            };

            myMap = (MapControl)sender;
            myMap.Center = new Geopoint(new BasicGeoposition() { Altitude = 643, Latitude = 43.089863, Longitude = -77.669609 });
            myMap.ZoomLevel = 14;
            //      MapIcon1 = new MapIcon();
            fencePos = new BasicGeoposition() { Latitude = 43.089863, Longitude = -77.669609 };
            Geopoint point = new Geopoint(fencePos);


            fence = new Windows.UI.Xaml.Shapes.Rectangle();
            //         fence.Tapped += fence_Tapped;

            fence.Width = 30;
            fence.Height = 30;
            BitmapImage img = new BitmapImage(new Uri("ms-appx:///Assets/redpin.png"));
            fence.Fill = new ImageBrush() { ImageSource = img };

            MapControl.SetLocation(fence, point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(1.0, 0.5));


            myMap.Children.Add(fence);

            fence.Tapped += async (o, args) =>
            {
                MessageDialog myDialog = new MessageDialog("You clicked on something");
                myDialog.ShowAsync();
            };

            var henr = new MapPolygon();

            List<BasicGeoposition> grid = new List<BasicGeoposition>();

            #region henrietta polygon
            grid.Add(new BasicGeoposition() { Latitude = 43.095392, Longitude = -77.597041 });
            grid.Add(new BasicGeoposition() { Latitude = 43.094800, Longitude = -77.613090 });
            grid.Add(new BasicGeoposition() { Latitude = 43.094211, Longitude = -77.628284 });
            grid.Add(new BasicGeoposition() { Latitude = 43.093760, Longitude = -77.638963 });
            grid.Add(new BasicGeoposition() { Latitude = 43.093439, Longitude = -77.647059 });
            grid.Add(new BasicGeoposition() { Latitude = 43.092589, Longitude = -77.665879 });
            grid.Add(new BasicGeoposition() { Latitude = 43.092254, Longitude = -77.673852 });
            grid.Add(new BasicGeoposition() { Latitude = 43.092045, Longitude = -77.680575 });
            grid.Add(new BasicGeoposition() { Latitude = 43.091267, Longitude = -77.680973 });
            grid.Add(new BasicGeoposition() { Latitude = 43.089991, Longitude = -77.681874 });
            grid.Add(new BasicGeoposition() { Latitude = 43.089835, Longitude = -77.682005 });
            grid.Add(new BasicGeoposition() { Latitude = 43.088922, Longitude = -77.682888 });
            grid.Add(new BasicGeoposition() { Latitude = 43.088594, Longitude = -77.683225 });
            grid.Add(new BasicGeoposition() { Latitude = 43.088339, Longitude = -77.683495 });
            grid.Add(new BasicGeoposition() { Latitude = 43.088207, Longitude = -77.683670 });
            grid.Add(new BasicGeoposition() { Latitude = 43.086354, Longitude = -77.685583 });
            grid.Add(new BasicGeoposition() { Latitude = 43.086060, Longitude = -77.685832 });
            grid.Add(new BasicGeoposition() { Latitude = 43.085322, Longitude = -77.686402 });
            grid.Add(new BasicGeoposition() { Latitude = 43.084465, Longitude = -77.687057 });
            grid.Add(new BasicGeoposition() { Latitude = 43.084229, Longitude = -77.687210 });
            grid.Add(new BasicGeoposition() { Latitude = 43.083980, Longitude = -77.687441 });
            grid.Add(new BasicGeoposition() { Latitude = 43.083517, Longitude = -77.687804 });
            grid.Add(new BasicGeoposition() { Latitude = 43.083318, Longitude = -77.687982 });
            grid.Add(new BasicGeoposition() { Latitude = 43.083242, Longitude = -77.688083 });
            grid.Add(new BasicGeoposition() { Latitude = 43.083066, Longitude = -77.688218 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082935, Longitude = -77.688384 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082845, Longitude = -77.688460 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082686, Longitude = -77.688653 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082535, Longitude = -77.688785 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082426, Longitude = -77.688926 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082383, Longitude = -77.689017 });
            grid.Add(new BasicGeoposition() { Latitude = 43.082146, Longitude = -77.689248 });
            grid.Add(new BasicGeoposition() { Latitude = 43.081778, Longitude = -77.689699 });
            grid.Add(new BasicGeoposition() { Latitude = 43.080904, Longitude = -77.690503 });
            grid.Add(new BasicGeoposition() { Latitude = 43.080806, Longitude = -77.690633 });
            grid.Add(new BasicGeoposition() { Latitude = 43.080668, Longitude = -77.690754 });
            grid.Add(new BasicGeoposition() { Latitude = 43.080525, Longitude = -77.690924 });
            grid.Add(new BasicGeoposition() { Latitude = 43.080249, Longitude = -77.691176 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079991, Longitude = -77.691434 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079895, Longitude = -77.691552 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079687, Longitude = -77.691735 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079458, Longitude = -77.691966 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079102, Longitude = -77.692440 });
            grid.Add(new BasicGeoposition() { Latitude = 43.079008, Longitude = -77.692597 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078798, Longitude = -77.693078 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078647, Longitude = -77.693548 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078488, Longitude = -77.694254 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078415, Longitude = -77.694666 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078349, Longitude = -77.695208 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078268, Longitude = -77.695684 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078098, Longitude = -77.697089 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077981, Longitude = -77.698504 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077901, Longitude = -77.699207 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077859, Longitude = -77.699813 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077696, Longitude = -77.701331 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077499, Longitude = -77.702642 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077427, Longitude = -77.702996 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077366, Longitude = -77.703235 });
            grid.Add(new BasicGeoposition() { Latitude = 43.077184, Longitude = -77.703753 });
            grid.Add(new BasicGeoposition() { Latitude = 43.076974, Longitude = -77.704240 });
            grid.Add(new BasicGeoposition() { Latitude = 43.076648, Longitude = -77.704904 });
            grid.Add(new BasicGeoposition() { Latitude = 43.076462, Longitude = -77.705203 });
            grid.Add(new BasicGeoposition() { Latitude = 43.076165, Longitude = -77.705574 });
            grid.Add(new BasicGeoposition() { Latitude = 43.075721, Longitude = -77.705999 });
            grid.Add(new BasicGeoposition() { Latitude = 43.075545, Longitude = -77.706119 });
            grid.Add(new BasicGeoposition() { Latitude = 43.075395, Longitude = -77.706180 });
            grid.Add(new BasicGeoposition() { Latitude = 43.074881, Longitude = -77.706315 });
            grid.Add(new BasicGeoposition() { Latitude = 43.074576, Longitude = -77.706363 });
            grid.Add(new BasicGeoposition() { Latitude = 43.074124, Longitude = -77.706347 });
            grid.Add(new BasicGeoposition() { Latitude = 43.073753, Longitude = -77.706288 });
            grid.Add(new BasicGeoposition() { Latitude = 43.073562, Longitude = -77.706231 });
            grid.Add(new BasicGeoposition() { Latitude = 43.073392, Longitude = -77.706156 });
            grid.Add(new BasicGeoposition() { Latitude = 43.073077, Longitude = -77.705971 });
            grid.Add(new BasicGeoposition() { Latitude = 43.072543, Longitude = -77.705552 });
            grid.Add(new BasicGeoposition() { Latitude = 43.072119, Longitude = -77.705322 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071829, Longitude = -77.705116 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071662, Longitude = -77.704945 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071446, Longitude = -77.704643 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071370, Longitude = -77.704505 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071323, Longitude = -77.704377 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071256, Longitude = -77.704025 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071148, Longitude = -77.702980 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071105, Longitude = -77.702348 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071099, Longitude = -77.700842 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071057, Longitude = -77.699419 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071069, Longitude = -77.698716 });
            grid.Add(new BasicGeoposition() { Latitude = 43.071024, Longitude = -77.698329 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070916, Longitude = -77.697920 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070802, Longitude = -77.697573 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070608, Longitude = -77.697079 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070464, Longitude = -77.696826 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070332, Longitude = -77.696543 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069972, Longitude = -77.695916 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069894, Longitude = -77.695746 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069767, Longitude = -77.695606 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069684, Longitude = -77.695550 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069306, Longitude = -77.695420 });
            grid.Add(new BasicGeoposition() { Latitude = 43.069037, Longitude = -77.695383 });
            grid.Add(new BasicGeoposition() { Latitude = 43.068468, Longitude = -77.695510 });
            grid.Add(new BasicGeoposition() { Latitude = 43.068195, Longitude = -77.695627 });
            grid.Add(new BasicGeoposition() { Latitude = 43.067854, Longitude = -77.695857 });
            grid.Add(new BasicGeoposition() { Latitude = 43.067568, Longitude = -77.696190 });
            grid.Add(new BasicGeoposition() { Latitude = 43.067393, Longitude = -77.696479 });
            grid.Add(new BasicGeoposition() { Latitude = 43.067165, Longitude = -77.696798 });
            grid.Add(new BasicGeoposition() { Latitude = 43.066900, Longitude = -77.697306 });
            grid.Add(new BasicGeoposition() { Latitude = 43.066713, Longitude = -77.697600 });
            grid.Add(new BasicGeoposition() { Latitude = 43.066200, Longitude = -77.698158 });
            grid.Add(new BasicGeoposition() { Latitude = 43.065970, Longitude = -77.698369 });
            grid.Add(new BasicGeoposition() { Latitude = 43.065724, Longitude = -77.698537 });
            grid.Add(new BasicGeoposition() { Latitude = 43.065555, Longitude = -77.698633 });
            grid.Add(new BasicGeoposition() { Latitude = 43.065086, Longitude = -77.698846 });
            grid.Add(new BasicGeoposition() { Latitude = 43.064532, Longitude = -77.698985 });
            grid.Add(new BasicGeoposition() { Latitude = 43.063857, Longitude = -77.699052 });
            grid.Add(new BasicGeoposition() { Latitude = 43.063465, Longitude = -77.699015 });
            grid.Add(new BasicGeoposition() { Latitude = 43.063081, Longitude = -77.698902 });
            grid.Add(new BasicGeoposition() { Latitude = 43.062723, Longitude = -77.698726 });
            grid.Add(new BasicGeoposition() { Latitude = 43.062466, Longitude = -77.698567 });
            grid.Add(new BasicGeoposition() { Latitude = 43.061728, Longitude = -77.697976 });
            grid.Add(new BasicGeoposition() { Latitude = 43.061490, Longitude = -77.697740 });
            grid.Add(new BasicGeoposition() { Latitude = 43.061285, Longitude = -77.697494 });
            grid.Add(new BasicGeoposition() { Latitude = 43.061076, Longitude = -77.697304 });
            grid.Add(new BasicGeoposition() { Latitude = 43.060761, Longitude = -77.696969 });
            grid.Add(new BasicGeoposition() { Latitude = 43.060444, Longitude = -77.696678 });
            grid.Add(new BasicGeoposition() { Latitude = 43.060079, Longitude = -77.696407 });
            grid.Add(new BasicGeoposition() { Latitude = 43.059808, Longitude = -77.696172 });
            grid.Add(new BasicGeoposition() { Latitude = 43.059350, Longitude = -77.695909 });
            grid.Add(new BasicGeoposition() { Latitude = 43.059002, Longitude = -77.695811 });
            grid.Add(new BasicGeoposition() { Latitude = 43.058686, Longitude = -77.695776 });
            grid.Add(new BasicGeoposition() { Latitude = 43.058508, Longitude = -77.695778 });
            grid.Add(new BasicGeoposition() { Latitude = 43.058209, Longitude = -77.695821 });
            grid.Add(new BasicGeoposition() { Latitude = 43.057632, Longitude = -77.696021 });
            grid.Add(new BasicGeoposition() { Latitude = 43.057330, Longitude = -77.696180 });
            grid.Add(new BasicGeoposition() { Latitude = 43.057065, Longitude = -77.696367 });
            grid.Add(new BasicGeoposition() { Latitude = 43.056764, Longitude = -77.696637 });
            grid.Add(new BasicGeoposition() { Latitude = 43.056638, Longitude = -77.696740 });
            grid.Add(new BasicGeoposition() { Latitude = 43.056565, Longitude = -77.696779 });
            grid.Add(new BasicGeoposition() { Latitude = 43.056356, Longitude = -77.696954 });
            grid.Add(new BasicGeoposition() { Latitude = 43.056001, Longitude = -77.697298 });
            grid.Add(new BasicGeoposition() { Latitude = 43.055759, Longitude = -77.697604 });
            grid.Add(new BasicGeoposition() { Latitude = 43.055504, Longitude = -77.697881 });
            grid.Add(new BasicGeoposition() { Latitude = 43.055248, Longitude = -77.698198 });
            grid.Add(new BasicGeoposition() { Latitude = 43.054402, Longitude = -77.699342 });
            grid.Add(new BasicGeoposition() { Latitude = 43.054266, Longitude = -77.699567 });
            grid.Add(new BasicGeoposition() { Latitude = 43.053883, Longitude = -77.700054 });
            grid.Add(new BasicGeoposition() { Latitude = 43.053823, Longitude = -77.700152 });
            grid.Add(new BasicGeoposition() { Latitude = 43.053536, Longitude = -77.700492 });
            grid.Add(new BasicGeoposition() { Latitude = 43.053427, Longitude = -77.700662 });
            grid.Add(new BasicGeoposition() { Latitude = 43.053120, Longitude = -77.701046 });
            grid.Add(new BasicGeoposition() { Latitude = 43.052422, Longitude = -77.701852 });
            grid.Add(new BasicGeoposition() { Latitude = 43.052327, Longitude = -77.701981 });
            grid.Add(new BasicGeoposition() { Latitude = 43.052088, Longitude = -77.702191 });
            grid.Add(new BasicGeoposition() { Latitude = 43.051248, Longitude = -77.703033 });
            grid.Add(new BasicGeoposition() { Latitude = 43.051031, Longitude = -77.703292 });
            grid.Add(new BasicGeoposition() { Latitude = 43.050573, Longitude = -77.703773 });
            grid.Add(new BasicGeoposition() { Latitude = 43.050383, Longitude = -77.703995 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049936, Longitude = -77.704450 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049577, Longitude = -77.704954 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049431, Longitude = -77.705261 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049291, Longitude = -77.705592 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049177, Longitude = -77.705933 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049077, Longitude = -77.706504 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049052, Longitude = -77.706881 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049052, Longitude = -77.707011 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049093, Longitude = -77.707285 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049122, Longitude = -77.707734 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049396, Longitude = -77.709838 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049432, Longitude = -77.710423 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049421, Longitude = -77.711183 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049320, Longitude = -77.712071 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049211, Longitude = -77.712820 });
            grid.Add(new BasicGeoposition() { Latitude = 43.049089, Longitude = -77.713488 });
            grid.Add(new BasicGeoposition() { Latitude = 43.048965, Longitude = -77.713941 });
            grid.Add(new BasicGeoposition() { Latitude = 43.048708, Longitude = -77.714654 });
            grid.Add(new BasicGeoposition() { Latitude = 43.048412, Longitude = -77.715424 });
            grid.Add(new BasicGeoposition() { Latitude = 43.048229, Longitude = -77.715824 });
            grid.Add(new BasicGeoposition() { Latitude = 43.047554, Longitude = -77.717572 });
            grid.Add(new BasicGeoposition() { Latitude = 43.046909, Longitude = -77.719459 });
            grid.Add(new BasicGeoposition() { Latitude = 43.046782, Longitude = -77.719847 });
            grid.Add(new BasicGeoposition() { Latitude = 43.046690, Longitude = -77.720203 });
            grid.Add(new BasicGeoposition() { Latitude = 43.046541, Longitude = -77.720635 });
            grid.Add(new BasicGeoposition() { Latitude = 43.046464, Longitude = -77.720813 });
            grid.Add(new BasicGeoposition() { Latitude = 43.045864, Longitude = -77.722630 });
            grid.Add(new BasicGeoposition() { Latitude = 43.045339, Longitude = -77.723778 });
            grid.Add(new BasicGeoposition() { Latitude = 43.045058, Longitude = -77.724313 });
            grid.Add(new BasicGeoposition() { Latitude = 43.044733, Longitude = -77.724784 });
            grid.Add(new BasicGeoposition() { Latitude = 43.044455, Longitude = -77.725098 });
            grid.Add(new BasicGeoposition() { Latitude = 43.044099, Longitude = -77.725462 });
            grid.Add(new BasicGeoposition() { Latitude = 43.043809, Longitude = -77.725721 });
            grid.Add(new BasicGeoposition() { Latitude = 43.043466, Longitude = -77.725936 });
            grid.Add(new BasicGeoposition() { Latitude = 43.042609, Longitude = -77.726364 });
            grid.Add(new BasicGeoposition() { Latitude = 43.042381, Longitude = -77.726461 });
            grid.Add(new BasicGeoposition() { Latitude = 43.041987, Longitude = -77.726706 });
            grid.Add(new BasicGeoposition() { Latitude = 43.041324, Longitude = -77.727011 });
            grid.Add(new BasicGeoposition() { Latitude = 43.040412, Longitude = -77.727359 });
            grid.Add(new BasicGeoposition() { Latitude = 43.040043, Longitude = -77.727477 });
            grid.Add(new BasicGeoposition() { Latitude = 43.039784, Longitude = -77.727542 });
            grid.Add(new BasicGeoposition() { Latitude = 43.038668, Longitude = -77.727932 });
            grid.Add(new BasicGeoposition() { Latitude = 43.038524, Longitude = -77.727964 });
            grid.Add(new BasicGeoposition() { Latitude = 43.037994, Longitude = -77.728159 });
            grid.Add(new BasicGeoposition() { Latitude = 43.037636, Longitude = -77.728260 });
            grid.Add(new BasicGeoposition() { Latitude = 43.037357, Longitude = -77.728360 });
            grid.Add(new BasicGeoposition() { Latitude = 43.037017, Longitude = -77.728536 });
            grid.Add(new BasicGeoposition() { Latitude = 43.036595, Longitude = -77.728717 });
            grid.Add(new BasicGeoposition() { Latitude = 43.036358, Longitude = -77.728850 });
            grid.Add(new BasicGeoposition() { Latitude = 43.036181, Longitude = -77.728997 });
            grid.Add(new BasicGeoposition() { Latitude = 43.035876, Longitude = -77.729171 });
            grid.Add(new BasicGeoposition() { Latitude = 43.035077, Longitude = -77.729845 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034565, Longitude = -77.730180 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034199, Longitude = -77.730320 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034028, Longitude = -77.730332 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033884, Longitude = -77.730305 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033632, Longitude = -77.730180 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033454, Longitude = -77.730040 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033350, Longitude = -77.729929 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033229, Longitude = -77.729674 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033154, Longitude = -77.729464 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033118, Longitude = -77.729222 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033124, Longitude = -77.729013 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033227, Longitude = -77.728615 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033490, Longitude = -77.727945 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033660, Longitude = -77.727418 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033858, Longitude = -77.726961 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033965, Longitude = -77.726670 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033990, Longitude = -77.726558 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034079, Longitude = -77.726356 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034504, Longitude = -77.725043 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034671, Longitude = -77.724385 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034750, Longitude = -77.723872 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034749, Longitude = -77.723632 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034649, Longitude = -77.723102 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034570, Longitude = -77.722834 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034411, Longitude = -77.722483 });
            grid.Add(new BasicGeoposition() { Latitude = 43.034049, Longitude = -77.721895 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033913, Longitude = -77.721765 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033652, Longitude = -77.721645 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033472, Longitude = -77.721622 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033145, Longitude = -77.721659 });
            grid.Add(new BasicGeoposition() { Latitude = 43.032850, Longitude = -77.721728 });
            grid.Add(new BasicGeoposition() { Latitude = 43.032672, Longitude = -77.721791 });
            grid.Add(new BasicGeoposition() { Latitude = 43.032339, Longitude = -77.721949 });
            grid.Add(new BasicGeoposition() { Latitude = 43.032137, Longitude = -77.722066 });
            grid.Add(new BasicGeoposition() { Latitude = 43.031243, Longitude = -77.722621 });
            grid.Add(new BasicGeoposition() { Latitude = 43.031024, Longitude = -77.722775 });
            grid.Add(new BasicGeoposition() { Latitude = 43.029215, Longitude = -77.723836 });
            grid.Add(new BasicGeoposition() { Latitude = 43.028613, Longitude = -77.724229 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027991, Longitude = -77.724748 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027862, Longitude = -77.724888 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027786, Longitude = -77.725006 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027674, Longitude = -77.725119 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027468, Longitude = -77.725416 });
            grid.Add(new BasicGeoposition() { Latitude = 43.027289, Longitude = -77.725733 });
            grid.Add(new BasicGeoposition() { Latitude = 43.026860, Longitude = -77.726677 });
            grid.Add(new BasicGeoposition() { Latitude = 43.026598, Longitude = -77.727119 });
            grid.Add(new BasicGeoposition() { Latitude = 43.026393, Longitude = -77.727408 });
            grid.Add(new BasicGeoposition() { Latitude = 43.026038, Longitude = -77.727970 });
            grid.Add(new BasicGeoposition() { Latitude = 43.025753, Longitude = -77.728328 });
            grid.Add(new BasicGeoposition() { Latitude = 43.025221, Longitude = -77.728814 });
            grid.Add(new BasicGeoposition() { Latitude = 43.024676, Longitude = -77.729149 });
            grid.Add(new BasicGeoposition() { Latitude = 43.024434, Longitude = -77.729325 });
            grid.Add(new BasicGeoposition() { Latitude = 43.024064, Longitude = -77.729557 });
            grid.Add(new BasicGeoposition() { Latitude = 43.023699, Longitude = -77.729739 });
            grid.Add(new BasicGeoposition() { Latitude = 43.023315, Longitude = -77.729811 });
            grid.Add(new BasicGeoposition() { Latitude = 43.022919, Longitude = -77.729782 });
            grid.Add(new BasicGeoposition() { Latitude = 43.022801, Longitude = -77.729753 });
            grid.Add(new BasicGeoposition() { Latitude = 43.022452, Longitude = -77.729620 });
            grid.Add(new BasicGeoposition() { Latitude = 43.022203, Longitude = -77.729495 });
            grid.Add(new BasicGeoposition() { Latitude = 43.021677, Longitude = -77.729161 });
            grid.Add(new BasicGeoposition() { Latitude = 43.021406, Longitude = -77.728958 });
            grid.Add(new BasicGeoposition() { Latitude = 43.021266, Longitude = -77.728832 });
            grid.Add(new BasicGeoposition() { Latitude = 43.020438, Longitude = -77.727848 });
            grid.Add(new BasicGeoposition() { Latitude = 43.020223, Longitude = -77.727614 });
            grid.Add(new BasicGeoposition() { Latitude = 43.019994, Longitude = -77.727273 });
            grid.Add(new BasicGeoposition() { Latitude = 43.019793, Longitude = -77.727031 });
            grid.Add(new BasicGeoposition() { Latitude = 43.019611, Longitude = -77.726708 });
            grid.Add(new BasicGeoposition() { Latitude = 43.019287, Longitude = -77.726221 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018943, Longitude = -77.725772 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018743, Longitude = -77.725447 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018589, Longitude = -77.725140 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018380, Longitude = -77.724835 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018036, Longitude = -77.724227 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017544, Longitude = -77.723588 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017208, Longitude = -77.723267 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017257, Longitude = -77.715582 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017494, Longitude = -77.694597 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017513, Longitude = -77.693895 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017755, Longitude = -77.667696 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017775, Longitude = -77.659980 });
            grid.Add(new BasicGeoposition() { Latitude = 43.017841, Longitude = -77.649526 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018105, Longitude = -77.624559 });
            grid.Add(new BasicGeoposition() { Latitude = 43.018481, Longitude = -77.594937 });
            grid.Add(new BasicGeoposition() { Latitude = 43.028537, Longitude = -77.589694 });
            grid.Add(new BasicGeoposition() { Latitude = 43.033773, Longitude = -77.586903 });
            grid.Add(new BasicGeoposition() { Latitude = 43.038393, Longitude = -77.584468 });
            grid.Add(new BasicGeoposition() { Latitude = 43.041427, Longitude = -77.582831 });
            grid.Add(new BasicGeoposition() { Latitude = 43.044682, Longitude = -77.581110 });
            grid.Add(new BasicGeoposition() { Latitude = 43.048621, Longitude = -77.579063 });
            grid.Add(new BasicGeoposition() { Latitude = 43.059867, Longitude = -77.573111 });
            grid.Add(new BasicGeoposition() { Latitude = 43.068959, Longitude = -77.568214 });
            grid.Add(new BasicGeoposition() { Latitude = 43.070735, Longitude = -77.567274 });
            grid.Add(new BasicGeoposition() { Latitude = 43.078225, Longitude = -77.563474 });
            grid.Add(new BasicGeoposition() { Latitude = 43.084872, Longitude = -77.559898 });
            grid.Add(new BasicGeoposition() { Latitude = 43.087412, Longitude = -77.558566 });
            grid.Add(new BasicGeoposition() { Latitude = 43.088765, Longitude = -77.557839 });
            grid.Add(new BasicGeoposition() { Latitude = 43.091911, Longitude = -77.556104 });
            grid.Add(new BasicGeoposition() { Latitude = 43.096846, Longitude = -77.553443 });
            grid.Add(new BasicGeoposition() { Latitude = 43.096731, Longitude = -77.557032 });
            grid.Add(new BasicGeoposition() { Latitude = 43.096324, Longitude = -77.568278 });
            grid.Add(new BasicGeoposition() { Latitude = 43.096211, Longitude = -77.572189 });
            grid.Add(new BasicGeoposition() { Latitude = 43.095992, Longitude = -77.578629 });
            grid.Add(new BasicGeoposition() { Latitude = 43.095935, Longitude = -77.580849 });
            grid.Add(new BasicGeoposition() { Latitude = 43.095392, Longitude = -77.597041 });
            henr.FillColor = Color.FromArgb(10, 0, 0, 255);
            henr.Path = new Geopath(grid);

            myMap.MapElements.Add(henr);

            #endregion
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            fencePos.Latitude += 0.001;
            Geopoint newPoint = new Geopoint(fencePos);
            MapControl.SetLocation(fence, newPoint);
        }
    }
}