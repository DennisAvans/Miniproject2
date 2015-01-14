using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Miniproject.Model
{
    public class Bestellingen
    {
        private List<Pizza> _pizzas;

        const string fileName = "pizzas.json";

        public Bestellingen()
        {
            _pizzas = new List<Pizza>();
        }
        public async Task<int> getPizzasCount()
        {
            await ensureDataLoaded();
            return _pizzas.Count;
        }

        public async Task<List<Pizza>> getPizzas()
        {
            await ensureDataLoaded();
            return _pizzas;
        }

        private async Task ensureDataLoaded()
        {
            if (_pizzas.Count == 0)
                await loadPizzaDataAsync();
            return;
        }

        private async Task loadPizzaDataAsync()
        {
            if (_pizzas.Count != 0)
                return;
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<Pizza>));
            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    _pizzas = (List<Pizza>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                _pizzas = new List<Pizza>();
            }
        }

        public async void addPizza(Pizza pizza)
        {
            _pizzas.Add(pizza);
            await savePizzaDataAsync();
        }

        public async void delPizza(Pizza pizza)
        {
            _pizzas.Remove(pizza);
            await savePizzaDataAsync();
        }

        private async Task savePizzaDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<Pizza>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _pizzas);
            }
        }

    }
}
