using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nasa;
using Nasa.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class BaseTests
    {
        [TestMethod]
        public async Task GetAsteroidsValidDayValueAsync()
        {
            var controller = new AsteroidController();

            var response = await controller.GetAsync(5);

            Assert.IsTrue(response.Count() > 0);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task GetAsteroidsNotValidDayValueAsync()
        {
            var controller = new AsteroidController();

            var response = await controller.GetAsync(12);

            Assert.IsTrue(response == null);

        }
    }
}
