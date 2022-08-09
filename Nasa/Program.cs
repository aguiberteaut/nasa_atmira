using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Nasa
{
    public class Program
    {
        static HttpClient httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        public static async Task getAsteroids()
        {
            var url = "http://www.neowsapp.com/rest/v1/feed?start_date=2021-12-09&end_date=2021-12-12&detailed=false&api_key=Na1sKwJGK1HVeOF4Yx8aLNp4u8ygT5GSSMF26HQ2";
            var feed = await httpClient.GetFromJsonAsync<Asteroid>(url);

            List<Observation> potentiallyHazarous = new List<Observation>();

            foreach (var a in feed.near_earth_objects.Values)
            {
                potentiallyHazarous.AddRange(a.Where(x => x.is_potentially_hazardous_asteroid));
            }


            if (potentiallyHazarous.Count > 0)
                Console.WriteLine("!!!!!!!!!!PAY ATTENTION!!!!!!!!!!");
            else
                Console.WriteLine("NO PROBLEM BUDDY");

            for (int i = 0; i < potentiallyHazarous.Count; i++)
            {
                Console.WriteLine("Asteroid number:" + i);
                Console.WriteLine("Asteroide:" + potentiallyHazarous[i].name);
                Console.WriteLine("Tamaño medio:" + calculateDiameter(potentiallyHazarous[i]) + " KM");
                Console.WriteLine("Velocidad: " + potentiallyHazarous[i].close_approach_data.FirstOrDefault().relative_velocity.kilometers_per_hour);
                Console.WriteLine("Fecha: " + potentiallyHazarous[i].close_approach_data.FirstOrDefault().close_approach_date);
                Console.WriteLine("Planeta: " + potentiallyHazarous[i].close_approach_data.FirstOrDefault().orbiting_body);
                Console.WriteLine("---------------------------------------------------");

            }

            Console.WriteLine("END");
        }

        private static float calculateDiameter(Observation asteroid)
        {
            float result = 0;

            float diameterMin = asteroid.estimated_diameter.kilometers.estimated_diameter_min;

            float diameterMax = asteroid.estimated_diameter.kilometers.estimated_diameter_max;

            result = (diameterMax + diameterMin) / 2;

            return result;
        }
    }
}
