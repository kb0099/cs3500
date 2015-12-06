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
        /// Any player in the team should change its position.
        /// </summary>
        [TestMethod()]
        public void MoveTeamTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");
            c.Mass = 4000;
            w.SplitCube(c.uId, 0, 0);
            foreach(Cube p in w.teamCubes[c.uId])
            {
                double oldX = p.X, oldY = p.Y;
                w.MoveCube(p.uId, -10, -10);
                Assert.AreNotEqual<double>(oldX, p.X);
                Assert.AreNotEqual<double>(oldY, p.Y);
            }
        }

        /// <summary>
        /// Cube should consume food honoring the requirements
        /// </summary>
        [TestMethod()]
        public void EatFoodsTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");
            c.Mass = 4000;
            Cube f;
            w.AddFood(out f);
            f.X = c.X; f.Y = c.Y;
            w.EatFoods();
            Assert.AreEqual(0, w.foodCubes.Count);
            Assert.AreEqual(4001, c.Mass);
        }

        [TestMethod()]
        public void EatPlayersTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");
            c.Mass = 4000;
            Cube d = w.NextPlayer("b");
            d.Mass = 1000;
            d.X = c.X;
            d.Y = c.Y;
            w.EatPlayers();
            Assert.AreEqual(1, w.playerCubes.Count);
            Assert.AreEqual(5000, c.Mass);
        }

        [TestMethod()]
        public void SplitCubeTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");
            w.SplitCube(c.uId, 0, 0);
            Assert.IsTrue(w.teamCubes.Count > 0);
        }

        [TestMethod()]
        public void ApplyAttritionTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");
            c.Mass = 5000;
            int oldMass = c.Mass;
            for (int i = 0; i < 10; i++)
                w.ApplyAttrition();
            Assert.IsTrue(c.Mass < oldMass);
        }

        /// <summary>
        /// Rates of spawn are low, so, try to get at least 1 virus.
        /// Which verifies that they can come randomly in the world.
        /// </summary>
        [TestMethod()]
        public void HandleVirusesTest()
        {
            World w = CreateWorld();
            Cube c = w.NextPlayer("a");

            while(w.viruses.Count < 1)
            {
                w.HandleViruses();
            }

            // split the cube
            c.Mass = 5000;
            c.X = w.viruses[0].X;
            c.Y = w.viruses[0].Y;
            w.HandleViruses();

            Assert.AreEqual(2500, c.Mass);
            Assert.IsTrue(w.teamCubes.Count > 0);
        }
    }
}