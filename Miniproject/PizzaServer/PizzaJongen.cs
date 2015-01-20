using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
namespace PizzaServer
{
    class PizzaJongen
    {
        private string currentLocation;
        private int counter = 1;
        private int step = 1;
        private string[] routeArray = new string[22];
        private Timer t;

        public PizzaJongen()
        {
            setRouteOne();
            currentLocation = routeArray[0];
            t = new Timer();
            t.Interval = 1000;
            t.Elapsed += t_Elapsed;
        }

        public void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            /* Zonder linq */
            //this.currentLocation = routeArray[counter];

            /* Met linq */
            var stringQuery = from str in routeArray select str;
            this.currentLocation = stringQuery.Skip(counter).First();

            if (counter == 21 || counter == 0)
            {
                step = -step;
            }
            counter += step;
            System.Console.WriteLine("Counter: " + counter + " step: " + step);
        }

        public async void startRoute()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            t.Start();
        }

        public void stopRoute()
        {
            t.Stop();
            t.Dispose();
        }

        public void setRouteOne()
        {
            routeArray[0] = "51.5930787575969@4.78132276201454";
            routeArray[1] = "51.5934464150774@4.78123171395641";
            routeArray[2] = "51.5938423505746@4.78118618992734";
            routeArray[3] = "51.5942921951585@4.78105539282971";
            routeArray[4] = "51.5947386949034@4.7810254451907";
            routeArray[5] = "51.5947386949034@4.78056624805882";
            routeArray[6] = "51.5947076881185@4.78001720800984";
            routeArray[7] = "51.5946580772186@4.77962788870236";
            routeArray[8] = "51.5945960635175@4.77935835995105";
            routeArray[9] = "51.5944348274985@4.77944820286815";
            routeArray[10] = "51.5942363823818@4.77944820286815";
            routeArray[11] = "51.5940193320426@4.77944820286823";
            routeArray[12] = "51.5938146836299@4.77945818541458";
            routeArray[13] = "51.5936534448372@4.77948813305363";
            routeArray[14] = "51.5934549963066@4.77952806323899";
            routeArray[15] = "51.5933185624389@4.77956799342439";
            routeArray[16] = "51.5931821281613@4.77942823777553";
            routeArray[17] = "51.5931821281613@4.77922858684866";
            routeArray[18] = "51.5932689500217@4.77895905809731";
            routeArray[19] = "51.5933185624389@4.77857972133622";
            routeArray[20] = "51.5933557717161@4.77821036712149";
            routeArray[21] = "51.5933743763434@4.77793085582382";

        }

        public string getNextLocation()
        {
            return currentLocation;
        }

    }
}
