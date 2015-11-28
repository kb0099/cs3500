using System;
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

            IsServer = true;
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
            return new Cube(r.Next(Width),
                r.Next(Height),
                NextPlayerColor(),
                NextUID(),
                0,
                false,
                name,
                PlayerStartMass);
        }

        /// <summary>
        /// Creates and assigns a new food cube or null if fails.
        /// </summary>
        /// <param name="food">Used as return value for the new food cube.</param>
        /// <returns>True if created else false.</returns>
        public bool AddFood(out Cube food)
        {
            food = null;
            if (this.foodCubes.Count > this.MaxFood)
                return false;
            food = new Cube(r.Next() % this.Width, r.Next() % this.Height,  Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)).ToArgb(), NextUID(), 0, true, "", FoodValue);
            foodCubes[food.uId] = food;
            return true;
        }
    }
}
