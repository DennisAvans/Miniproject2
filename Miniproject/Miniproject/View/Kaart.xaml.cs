using Miniproject.Common;
using Miniproject.Model;
using Miniproject.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
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
        private Ellipse _ellipse;
        private bool _delivered = false;
        private List<Geopoint> _route = new List<Geopoint>();
        private PizzaJongen _pizzaJongen = new PizzaJongen();

        public Kaart()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        }

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
            setToCurrentLocation();

            _route.Add(new Geopoint(new BasicGeoposition() { Latitude = 51.5807, Longitude = 4.7940 }));
            _route.Add(new Geopoint(new BasicGeoposition() { Latitude = 51.5853, Longitude = 4.7943 }));
            await GetRouteAndDirections(_route);

            await Task.Run(() => checkForPizza());
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void pizzaJongenLocationUpdate()
        {
            // locatie van pizzajongen hele tijd updaten via server
            _pizzaJongen._latitude = 0.0;
            _pizzaJongen._latitude = 0.0;
        }

        private async void setToCurrentLocation()
        {
            if (_geo == null)
                _geo = new Geolocator() { DesiredAccuracy = PositionAccuracy.High, ReportInterval = 1000 };

            var location = await getLocationAsync();
            await map.TrySetViewAsync(location.Coordinate.Point, 16, 0, 0, MapAnimationKind.Linear);

            _geo.PositionChanged += new TypedEventHandler<Geolocator, PositionChangedEventArgs>(geo_PositionChanged);
        }

        private async void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
               {
                   var location = await getLocationAsync();
                   map.Children.Remove(_ellipse);
                   _ellipse = new Ellipse();

                   _ellipse.Fill = new SolidColorBrush(Colors.Red);
                   _ellipse.Width = 10;
                   _ellipse.Height = 10;
                   map.Children.Add(_ellipse);
                   MapControl.SetLocation(_ellipse, location.Coordinate.Point);
                   await map.TrySetViewAsync(location.Coordinate.Point, map.ZoomLevel, 0, 0, MapAnimationKind.Linear);
               });
        }

        private async void checkForPizza()
        {
            double latitude;
            double longitude;
            double distance = 0.02; // 20 meter
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                while (_delivered == false)
                {
                    latitude = _pizzaJongen._latitude; // long en lat van de pizzabezorger
                    longitude = _pizzaJongen._longitude;
                    var location = await getLocationAsync();
                    if (distanceBetweenPlaces(longitude, latitude, location.Coordinate.Point.Position.Longitude, location.Coordinate.Point.Position.Latitude) <= distance)
                    {
                        // doe iets

                        _delivered = true;
                    }
                }
            });
        }

        private async Task GetRouteAndDirections(List<Geopoint> list)
        {
            // Get the route between the points.
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(list);

            Debug.WriteLine("Route is opgehaald!");

            //Display route with text
            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                //InstructionsLabel.Text += "\n";
                // Display the directions.

                foreach (MapRouteLeg leg in routeResult.Route.Legs)
                {
                    foreach (MapRouteManeuver maneuver in leg.Maneuvers)
                    {

                    }
                }
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.LightBlue;
                viewOfRoute.OutlineColor = Colors.DarkBlue;

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                map.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await map.TrySetViewBoundsAsync(
                    routeResult.Route.BoundingBox,
                    null,
                    Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);

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
