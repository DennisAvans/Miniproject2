using Miniproject.Common;
using Miniproject.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace Miniproject.View
{
    public sealed partial class Kaart : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Geolocator _geo = null;
        private Ellipse _mylocationMarker;
        private Ellipse _pizzalocationMarker;
        private bool _delivered = false;
        private List<Geopoint> _route = new List<Geopoint>();

        public Kaart()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        }

        private async void showCurrentLocation()
        {
            if (_geo == null)
                _geo = new Geolocator() { DesiredAccuracy = PositionAccuracy.High, ReportInterval = 1000 };

            var location = await getLocationAsync();
            // await map.TrySetViewAsync(location.Coordinate.Point, 16, 0, 0, MapAnimationKind.Linear);

            _geo.PositionChanged += new TypedEventHandler<Geolocator, PositionChangedEventArgs>(geo_PositionChanged);
        }

        private async void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var location = await getLocationAsync();
                map.Children.Remove(_mylocationMarker);
                _mylocationMarker = new Ellipse();

                _mylocationMarker.Fill = new SolidColorBrush(Colors.Red);
                _mylocationMarker.Width = 10;
                _mylocationMarker.Height = 10;
                map.Children.Add(_mylocationMarker);
                MapControl.SetLocation(_mylocationMarker, location.Coordinate.Point);

            });
        }

        #region navigationhelpers
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.sendData("bestelpizza");
            string x = await App.readData();
            showCurrentLocation();
            var point = new Geopoint(new BasicGeoposition() //locatie van de pizzaria
                  {
                      Latitude = 51.5931, 
                      Longitude = 4.7813
                  });

            await map.TrySetViewAsync(point, map.ZoomLevel, 0, 0, MapAnimationKind.Linear);
            await Task.Run(() => getpizzalocation());
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion

        private async void getpizzalocation()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                while (_delivered == false)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500)); // 500ms delay
                    App.sendData("updatepizza");
                    string coord = await App.readData();
                    string[] spliited = coord.Split('@');

                    var point = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = Convert.ToDouble(spliited[0]),
                        Longitude = Convert.ToDouble(spliited[1])
                    });
                    isPizzaNearby(point); // check if pizza is near
                    map.Children.Remove(_pizzalocationMarker);
                    _pizzalocationMarker = new Ellipse
                    {
                        Fill = new SolidColorBrush(Colors.Blue),
                        Width = 10,
                        Height = 10
                    };
                    map.Children.Add(_pizzalocationMarker);
                    MapControl.SetLocation(_pizzalocationMarker, point);
                    await map.TrySetViewAsync(point, 16, 0, 0, MapAnimationKind.Linear);
                }
            });
        }

        private async void isPizzaNearby(Geopoint point)
        {
            double latitude = point.Position.Latitude; // long en lat van de pizzabezorger;
            double longitude = point.Position.Longitude;
            double distance = 0.02; // 20 meter
            var location = await getLocationAsync();

            if (distanceBetweenPlaces(longitude, latitude, location.Coordinate.Point.Position.Longitude, location.Coordinate.Point.Position.Latitude) <= distance)
            {
                _delivered = true;

               MessageDialog dialog = new MessageDialog("Je pizza is er binnen enkele ogenblikken");
                await dialog.ShowAsync();

            }
        }

        public static double distanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
        {
            const double RADIUS = 6378.16;
            double dlon = degToRad(lon2 - lon1);
            double dlat = degToRad(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(degToRad(lat1)) * Math.Cos(degToRad(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }

        public static double degToRad(double x)
        {
            return x * 3.141592653589793 / 180;
        }

        private async Task<Geoposition> getLocationAsync()
        {
            return await _geo.GetGeopositionAsync();
        }

    }
}
