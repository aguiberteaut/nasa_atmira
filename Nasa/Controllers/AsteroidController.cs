using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Nasa.Controllers
{
    [ApiController]
    [Route("asteroids")]
    public class AsteroidController : ControllerBase
    {
        
        static HttpClient httpClient = new HttpClient();

        public AsteroidController()
        {
        }

        [HttpGet]
        public async Task<IEnumerable<Response>> GetAsync(int days)
        {
            var logger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            }).CreateLogger("AsteroidController");
            try
            {
                ValidateDay(days);
                DateTime endDate;
                DateTime startDate = DateTime.UtcNow.Date;

                endDate = DateTime.UtcNow.Date.AddDays(days);

                var baseurl = "http://www.neowsapp.com/rest/v1/feed?";

                //var url = "http://www.neowsapp.com/rest/v1/feed?start_date=2021-12-09&end_date=2021-12-12&detailed=false&api_key=Na1sKwJGK1HVeOF4Yx8aLNp4u8ygT5GSSMF26HQ2";

                var startDateUrl = "start_date=" + startDate.Date.ToString("yyyy-MM-dd");

                var endDateUrl = "end_date=" + endDate.Date.ToString("yyyy-MM-dd");

                var apiKey = "api_key=DEMO_KEY";

                var url = baseurl + startDateUrl + "&" + endDateUrl + "&detailed=false&" + apiKey;

                var feed = await httpClient.GetFromJsonAsync<Asteroid>(url);

                List<Observation> potentiallyHazarous = new List<Observation>();

                foreach (var a in feed.near_earth_objects.Values)
                {
                    potentiallyHazarous.AddRange(a.Where(x => x.is_potentially_hazardous_asteroid));
                }

                return potentiallyHazarous
                    .Select(x =>
                    new Response
                    {
                        Name = x.name,
                        Diameter = calculateDiameter(x),
                        Velocity = x.close_approach_data.FirstOrDefault().relative_velocity.kilometers_per_hour,
                        Date = x.close_approach_data.FirstOrDefault().close_approach_date,
                        Planet = x.close_approach_data.FirstOrDefault().orbiting_body
                    });
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.LogError(e.Message, e.ParamName);
                throw new ArgumentOutOfRangeException(e.ParamName, e.Message);
            }
            
        }

        private void ValidateDay(int days)
        {
            if (days < 1 || days > 7)
                throw new ArgumentOutOfRangeException("Days", "Is not a valid value");
        }

        private string calculateDiameter(Observation asteroid)
        {
            float result = 0;

            float diameterMin = asteroid.estimated_diameter.kilometers.estimated_diameter_min;

            float diameterMax = asteroid.estimated_diameter.kilometers.estimated_diameter_max;

            result = (diameterMax + diameterMin) / 2;

            return result.ToString();
        }
    }
}
