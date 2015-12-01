﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;

namespace AgCubio
{
    /// <summary>
    /// This is another part of partial Wrold definition.
    /// Most of the functionality of the server are represented by this partial class.
    /// </summary>
    partial class World
    {
        /// <summary>
        /// The rate of updates per second the world should do.
        /// </summary>
        public int HeartbeatsPerSecond { get; private set; }

        /// <summary>
        /// The rate at which cubes can move at their smallest mass.
        /// </summary>
        public double TopSpeed { get; private set; } // TODO: figure out type, implement in code

        /// <summary>
        /// The rate at which cubes can move at their largest mass.
        /// </summary>
        public double LowSpeed { get; private set; } // TODO: figure out type, implement in code

        /// <summary>
        /// The rate at which cubes lose mass.
        /// </summary>
        public double AttritionRate { get; private set; } // TODO: figure out type, implement in code

        /// <summary>
        /// The default mass of food.
        /// </summary>
        public double FoodValue { get; private set; }

        /// <summary>
        /// The initial mass of players.
        /// </summary>
        public double PlayerStartMass { get; private set; }

        /// <summary>
        /// The maximum amount of food for the world. When world has less food, 1 food should be added per heartbeat.
        /// </summary>
        public int MaxFood { get; private set; }

        /// <summary>
        /// The mass that should not allow splitting when below this value.
        /// </summary>
        public double MinimumSplitMass { get; private set; }

        /// <summary>
        /// The distance a cube can be "thrown" when split.
        /// </summary>
        public double MaximumSplitDistance { get; private set; }

        /// <summary>
        /// The amount of cubes a player can split to at most.
        /// </summary>
        public int MaximumSplits { get; private set; }

        /// <summary>
        /// The distance cubes must be to have the larger eat the smaller.
        /// </summary>
        public double AbsorbDistanceDelta { get; private set; }

        private Random r = new Random();

        // Represents all the foods on the server
        public Dictionary<int, Cube> foodCubes = new Dictionary<int, Cube>();

        // Represents all the player cubes on the server
        public Dictionary<int, Cube> playerCubes = new Dictionary<int, Cube>();

        // Represents all the splitted cubes that belong to a single team/player.
        public Dictionary<int, LinkedList<Cube>> teamCubes = new Dictionary<int, LinkedList<Cube>>();

        /// <summary>
        /// Initializes a world from a config file.
        /// Exceptions are not caught in the constructor and need to be handeled by the caller.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        public World(string path)
        {
            XDocument xmlDoc;
            string name = string.Empty, val = string.Empty;
            xmlDoc = XDocument.Load(path);
            XElement p = xmlDoc.Element("parameters");

            this.Width = int.Parse(p.Element("width").Value);
            this.Height = int.Parse(p.Element("height").Value);
            this.HeartbeatsPerSecond = int.Parse(p.Element("heartbeats_per_second").Value);
            this.TopSpeed = int.Parse(p.Element("top_speed").Value);
            this.LowSpeed = int.Parse(p.Element("low_speed").Value);
            this.AttritionRate = int.Parse(p.Element("attrition_rate").Value);
            this.FoodValue = int.Parse(p.Element("food_value").Value);
            this.PlayerStartMass = int.Parse(p.Element("player_start_mass").Value);
            this.MaxFood = int.Parse(p.Element("max_food").Value);
            int.Parse(p.Element("min_split_mass").Value);
            int.Parse(p.Element("max_split_dist").Value);
            int.Parse(p.Element("max_splits").Value);
            double.Parse(p.Element("absorb_constant").Value);
        }


        private static readonly Color[] PLAYER_COLORS = {
            Color.Red, Color.Blue, Color.Black, Color.Violet, Color.LightPink, Color.Yellow, Color.Orange, Color.Pink
        };
        private static int nextColor = 0;
        private static int nextUID = -1;
        private static readonly Color VIRUS_COLOR = Color.Green;

        /// <summary>
        /// Returns next player color.
        /// </summary>
        /// <returns>Next available color.</returns>
        private static int NextPlayerColor()
        {
            return PLAYER_COLORS[(nextColor++) % (PLAYER_COLORS.Length)].ToArgb();
        }

        private static int NextUID()
        {
            return System.Threading.Interlocked.Increment(ref nextUID);
        }

        /// <summary>
        /// Creates a new player cube given the name.
        /// </summary>
        /// <param name="name">Name of the client cube</param>
        /// <returns>Newly generated cube.</returns>
        public Cube NextPlayer(string name)
        {
            Cube c;
            lock (this)
            {
                c = new Cube(r.Next(Width),
                r.Next(Height),
                NextPlayerColor(),
                NextUID(),
                0,
                false,
                name,
                PlayerStartMass);
                playerCubes[c.uId] = c;
            }
            return c;
        }

        /// <summary>
        /// Creates and assigns a new food cube or null if fails.
        /// </summary>
        /// <param name="food">Used as return value for the new food cube.</param>
        /// <returns>True if created else false.</returns>
        public bool AddFood(out Cube food)
        {
            food = null;
            if (this.foodCubes.Count >= this.MaxFood)
                return false;
            food = new Cube(r.Next() % this.Width, r.Next() % this.Height,  Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)).ToArgb(), NextUID(), 0, true, "", FoodValue);
            foodCubes[food.uId] = food;
            return true;
        }

        /// <summary>
        /// Moves the cube towards the given point and not to that point.
        /// The cube could be either player or food or virus.
        /// </summary>
        /// <param name="c">The cube</param>
        /// <param name="toX">Towards X co-ordinate</param>
        /// <param name="toY">Towards Y co-oprdinate</param>
        public void MoveCube(Cube c, int toX, int toY)
        {
            double h = Math.Sqrt(toX * toX + toY * toY);
            double speed = TopSpeed - c.Mass / 600;
            if (speed < LowSpeed) speed = LowSpeed;
            c.X = (toX - c.X) / h * speed * 20;
            c.Y = (toY - c.Y) / h * speed *20;
        }

        /// <summary>
        /// Handles eating of food cubes by the world player cubes.
        /// Side effect: Removes the food cubes which have been eaten from the world.
        /// </summary>
        /// <returns>All the cubes that are eaten.</returns>
        public IEnumerable<Cube> EatFoods()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles players eating other players. Also, removes dead players from world.
        /// </summary>
        /// <returns>All the players those have been eaten.</returns>
        public IEnumerable<Cube> EatPlayers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Splits the cube honoring the requirements.
        /// Side effect: Should add the splitted cubes to the  teamCubes.
        /// </summary>
        /// <param name="c">Cube to split</param>
        /// <param name="toX">Split towards X</param>
        /// <param name="toY">Split towards Y</param>
        public void SplitCube(Cube c, int toX, int toY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A method to apply attrition (or lose mass) to all player cubes. Larger cubes should lose mass faster than
        /// smaller cubes. There should be a minimum mass where attrition will not apply.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        public void ApplyAttrition()
        {
            // TODO: 
            // While it was an excellent idea of throwing exception if it is called by client.
            // It should not be our concern, since, placing this check in every single call is going to
            // negatively impact the performance.
            // So, I would recommend removing these extra checks, because it adds no extra benefit
            // Reason: 1) It's the failure of the  assignment specification itself.
            //         2) Exceptions will be automatically thrown when world is not properly initialized.
            //         3) Since, we had most variables like playerCubes from PS7 -- client-centric, I had to use
            //            new variables to represent playerCubes, foodCubes, and teamCubes the server has.
            //            This ensures that NullReferenceException is automatically thrown if unauthorized calls are made.
            //            So, manual checking and throwing should not be required.
            // 
            // One better way would probably have one World class independent of client and world.
            // Then, ClientWorld class and ServerWorld class can deerive from parent World class.
            // It should not be a big concern, now, since client was already completed.


            // TODO: implement
        }
    }
}
