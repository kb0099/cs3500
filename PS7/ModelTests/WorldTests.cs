using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgCubio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio.Tests
{
    [TestClass()]
    public class WorldTests
    {
        [TestMethod()]
        public void WorldTestValidPath()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Resources\\world_parameters.xml");
            World w = new World(path);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void WorldTestInValidPath()
        {
            string path = "c:\\file.xml";
            World w = new World(path);
        }

        private World CreateWorld()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Resources\\world_parameters.xml");
            return new World(path);
        }

        /// <summary>
        /// Should not throw exception
        /// Player mass should be default.
        /// </summary>
        [TestMethod()]
        public void NextPlayerTest()
        {
            World w = CreateWorld();
            Cube player = w.NextPlayer("Jim");
            Assert.AreEqual(player.Mass, w.PlayerStartMass);
            Assert.AreEqual(w.playerCubes.Count, 1);
            Assert.IsFalse(player.food);
        }

        /// <summary>
        /// Adding food should return true until MAX food limit is reached.
        /// </summary>
        [TestMethod()]
        public void AddFoodTest()
        {
            World w = CreateWorld();
            Cube food;
            w.AddFood(out food);
            Assert.IsTrue(food.food);
            while (w.AddFood(out food))
            {
                // just keep adding food
            }
            Assert.AreEqual(w.foodCubes.Count, w.MaxFood);
            //System.Diagnostics.Debug.WriteLine("#: " + w.foodCubes.Count);
        }

        /// <summary>
        /// Player should change its position
        /// </summary>
        [TestMethod()]
        public void MoveCubeTest()
        {
            World w = CreateWorld();
            Cube p = w.NextPlayer("a");
            double oldX = p.X, oldY = p.Y;
            w.MoveCube(p.uId, -10, -10);
            Assert.AreNotEqual<double>(oldX, p.X);
            Assert.AreNotEqual<double>(oldY, p.Y);
        }

        /// <summary>
        /// Any player in the team should change its position
        /// </summary>
        [TestMethod()]
        public void MoveTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EatFoodsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EatPlayersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SplitCubeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ApplyAttritionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HandleVirusesTest()
        {
            Assert.Fail();
        }
    }
}