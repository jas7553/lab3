using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace lab3.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic user data model.
    /// </summary>
    public class User
    {
        public User(String firstName, String lastName, String email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.FullName = this.FirstName + " " + this.LastName;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }

        public override string ToString()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
    
    /// <summary>
    /// Generic message data model.
    /// </summary>
    public class Message
    {
        public Message(String msgType, String msg, String ts, String firstName, String lastName, String email)
        {
            this.MsgType = msgType;
            this.Msg = msg;
            this.Ts = ts;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            if (this.MsgType.Equals("from"))
            {
                this.Alignment = "Left";
            }
            else
            {
                this.Alignment = "Right";
            }
        }

        public string MsgType { get; private set; }
        public string Msg { get; private set; }
        public string Ts { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Alignment { get; private set; }

        public override string ToString()
        {
            return this.FirstName + " " + this.LastName + " said: " + this.Msg;
        }
    }

    /// <summary>
    /// Generic location data model.
    /// </summary>
    public class Location
    {
        public Location(String firstName, String lastName, String email, String latitude, String longitude, String accuracy, String lastUpdated)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Accuracy = accuracy;
            this.LastUpdated = lastUpdated;
        }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public string Accuracy { get; private set; }
        public string LastUpdated { get; private set; }

        public override string ToString()
        {
            return this.FirstName + " " + this.LastName + " is at (" + this.Latitude + ", " + this.Longitude + ")";
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get { return this._users; }
        }

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get { return this._messages; }
        }

        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        public ObservableCollection<Location> Locations
        {
            get { return this._locations; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();

            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();

            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<User> GetUserAsync(string email)
        {
            await _sampleDataSource.GetUsersAsyncc();

            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Users.Where(user => user.Email.Equals(email)).Select(user => user);
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<IEnumerable<Message>> GetMessageAsync(string email)
        {
            await _sampleDataSource.GetMessagesAsyncc();

            var messages =
                from m in _sampleDataSource.Messages
                where m.Email.Equals(email)
                select m;

            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Messages.Where(message => message.Email.Equals(email)).Select(message => message);
            return messages.ToList();
        }

        public static async Task<IEnumerable<User>> GetUsersAsync()
        {
            await _sampleDataSource.GetUsersAsyncc();

            return _sampleDataSource.Users;
        }

        public static async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            await _sampleDataSource.GetLocationsAsyncc();

            return _sampleDataSource.Locations;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                SampleDataGroup group = new SampleDataGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["Subtitle"].GetString(),
                                                            groupObject["ImagePath"].GetString(),
                                                            groupObject["Description"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new SampleDataItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["Subtitle"].GetString(),
                                                       itemObject["ImagePath"].GetString(),
                                                       itemObject["Description"].GetString(),
                                                       itemObject["Content"].GetString()));
                }
                this.Groups.Add(group);
            }
        }
        
        private async Task GetUsersAsyncc()
        {
            if (this._users.Count != 0)
                return;

            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "getUsers")
                    };
            string jsonText = await API.sendCommand(parameters);
            if (jsonText == null)
            {
                return;
            }
            JsonArray jsonArray = JsonArray.Parse(jsonText);

            List<User> userz = new List<User>();
            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                User user = new User(groupObject["first_name"].GetString(),
                                     groupObject["last_name"].GetString(),
                                     groupObject["email"].GetString());
                userz.Add(user);
            }
            userz = userz.OrderBy(o => o.FullName).ToList();
            foreach (User u in userz)
            {
                this.Users.Add(u);
            }
        }

        private async Task GetMessagesAsyncc()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "getMessages")
                    };

            string jsonText = await API.sendCommand(parameters);
            if (jsonText == null)
            {
                return;
            }
            JsonArray jsonArray = JsonArray.Parse(jsonText);

            this.Messages.Clear();
            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                Message message = new Message(groupObject["msg_type"].GetString(),
                                           groupObject["message"].GetString(),
                                           groupObject["ts"].GetString(),
                                           groupObject["first_name"].GetString(),
                                           groupObject["last_name"].GetString(),
                                           groupObject["email"].GetString());
                this.Messages.Insert(0, message);
            }
        }

        private async Task GetLocationsAsyncc()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("command", "getLocations")
                    };

            string jsonText = await API.sendCommand(parameters);
            if (jsonText == null)
            {
                return;
            }
            JsonArray jsonArray = JsonArray.Parse(jsonText);

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                Location location = new Location(groupObject["first_name"].GetString(),
                                           groupObject["last_name"].GetString(),
                                           groupObject["email"].GetString(),
                                           groupObject["latitude"].GetString(),
                                           groupObject["longitude"].GetString(),
                                           groupObject["accuracy"].GetString(),
                                           groupObject["lastUpdated"].GetString());
                try
                {
                    if (Convert.ToDecimal(location.Latitude) >= -90 &&
                        Convert.ToDecimal(location.Latitude) <= 90 &&
                        Convert.ToDecimal(location.Longitude) >= -180 &&
                        Convert.ToDecimal(location.Latitude) <= 180)
                    {
                        this.Locations.Insert(0, location);
                    }
                }
                catch (FormatException)
                {
                    // better ignore this, just to be safe
                }
            }
        }
    }
}