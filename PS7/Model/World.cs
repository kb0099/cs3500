using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    public partial class World
    {
        /// <summary>
        /// The width of the world.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the world.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The rate of updates per second the world should do.
        /// </summary>
        public object HeartbeatsPerSecond { get; private set; } // TODO: figure out the type and use case

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

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        private int PlayerID;

        /// <summary>
        /// Collection of cubes in the world.
        /// </summary>
        private Dictionary<int, Cube> Cubes;

        /// <summary>
        /// The player's cubes, which updates when new cubes belong to player's team.
        /// </summary>
        private Dictionary<int, Cube> PlayerCubes;

        /// <summary>
        /// The comparer to sort the collection of cubes.
        /// </summary>
        private IComparer<Cube> Comparer = new CubeMassComparer();

        /// <summary>
        /// A boolean to help identify the use case of the world (if the world should allow client or server features).
        /// This is set in a constructor.
        /// </summary>
        private bool IsServer;

        /// <summary>
        /// Client-based constructor of a World object. This object will only allow methods used for client operations;
        /// any attempt to call other methods will throw exceptions.
        /// </summary>
        /// <param name="w">The width of the world.</param>
        /// <param name="h">The height of the world.</param>
        /// <param name="id">The player id.</param>
        public World(double w, double h, int id)
        {
            this.IsServer = false;
            this.Width = (int)w;
            this.Height = (int)h;
            this.PlayerID = id;
            this.Cubes = new Dictionary<int, Cube>(50);
            this.PlayerCubes = new Dictionary<int, Cube>(6);
        }

        /// <summary>
        /// Server-based constructor of a World object. This object will only allow methods used for server operations;
        /// any attempt to call other methods will throw exceptions.
        /// </summary>
        /// <param name="w">The width of the world.</param>
        /// <param name="h">The height of the world.</param>
        /// <param name="heartbeat">How many updates should be made per second.</param>
        /// <param name="topSpeed">How fast cubes move when small.</param>
        /// <param name="lowSpeed">How fast cubes move when large.</param>
        /// <param name="attrition">How fast cubes lose mass.</param>
        /// <param name="foodMass">The default mass of food.</param>
        /// <param name="playerStartMass">The default starting mass of players.</param>
        /// <param name="maxFood">The maximum amount of food cubes the world should manage.</param>
        /// <param name="minSplitMass">The mass at which player cubes can't split when below this mass.</param>
        /// <param name="maxSplitDist">How far cubes are "thrown" when split.</param>
        /// <param name="maxSplits">How many cubes a player can split to.</param>
        /// <param name="absorbDelta">How close cubes should be to have the larger eat the smaller.</param>
        public World(double w, double h, object heartbeat, double topSpeed, double lowSpeed, double attrition, double foodMass, double playerStartMass, int maxFood, double minSplitMass, double maxSplitDist, int maxSplits, double absorbDelta)
        {
            this.IsServer = true;
            this.Width = (int)w;
            this.Height = (int)h;
            this.HeartbeatsPerSecond = heartbeat;
            this.TopSpeed = topSpeed;
            this.LowSpeed = lowSpeed;
            this.AttritionRate = attrition;
            this.FoodValue = foodMass;
            this.PlayerStartMass = playerStartMass;
            this.MaxFood = maxFood;
            this.MinimumSplitMass = minSplitMass;
            this.MaximumSplitDistance = maxSplitDist;
            this.MaximumSplits = maxSplits;
            this.AbsorbDistanceDelta = absorbDelta;
            this.Cubes = new Dictionary<int, Cube>(maxFood); // the amount of cubes can initialize to how many food cubes will be managed
            InitializeFood();
        }

        /// <summary>
        /// A method to add a cube to the world. If the cube is already in the world, it's information is updated.
        /// If the cube has 0 mass, it is removed from the world.
        /// This method is allowed for client and server operation.
        /// </summary>
        public void AddCube(Cube c)
        {
            // check cube is not null; if it is, don't do anything
            if (c == null) return;
            // remove cubes with the same unique ID in Cubes and PlayerCubes; this helps to remove old cube data to swap with new cube data
            Cubes.Remove(c.uId);
            if (!IsServer) PlayerCubes.Remove(c.uId);
            // if the cube is alive (i.e. not mass 0), add it to appropriate collections
            if (c.Mass != 0)
            {
                // add cube to Cubes
                Cubes.Add(c.uId, c);
                // if it is a player cube (player ID equals team ID), add it to PlayerCubes
                if (!IsServer && (c.teamId == PlayerID || c.uId == PlayerID)) PlayerCubes.Add(c.uId, c);
            }
        }

        /// <summary>
        /// A method to return the cubes of the world.
        /// The collection of cubes will be sorted by the smallest first.
        /// This method is allowed for client and server operation.
        /// </summary>
        public IEnumerable<Cube> GetCubes()
        {
            // sort the Cubes
            List<Cube> cs = new List<Cube>(Cubes.Values);
            cs.Sort(Comparer);
            // return the cubes
            return cs;
        }

        /// <summary>
        /// A method to get the number of cubes in the world.
        /// This method is allowed for client and server operation.
        /// </summary>
        public int NumberCubes()
        {
            return Cubes.Count;
        }

        /// <summary>
        /// A method to return cubes belonging to the player.
        /// This method is only allowed for client operation; it will throw an exception if the world was not
        /// constructed for client use.
        /// </summary>
        public IEnumerable<Cube> GetPlayerCubes()
        {
            if (IsServer) throw new Exception("The world was constructed for server use, but a client-based method was called."); // TODO: determine exception type to use
            return PlayerCubes.Values;
        }

        /// <summary>
        /// A method to return cubes belonging to a player. Cubes with the same unique id or team id as the given id
        /// will belong to the player. The cubes will be sorted with the smallest first.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Cube> GetPlayerCubes(int id)
        {
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
            // utilize GetCubes() to check from the bottom up until food cubes start (food should be smaller than the minimum player size)
            List<Cube> c = new List<Cube>(GetCubes());
            List<Cube> output = new List<Cube>();
            int i = c.Count - 1;
            Cube temp;
            while (i >= 0 && c[i].food == false)
            {
                temp = c[i];
                // add cube to return list if teamId or uId match
                if (temp.teamId == id || temp.uId == id) output.Add(temp);
                // decrement i to next largest cube
                i--;
            }
            // reverse output to keep sorting order
            output.Reverse();
            return output;
        }

        /// <summary>
        /// A method to return cubes that are player cubes. The cubes will be sorted with the smallest first.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cube> GetAllPlayerCubes()
        {
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
            // utilize GetCubes() to check from the bottom up until food cubes start (food should be smaller than the minimum player size)
            List<Cube> c = new List<Cube>(GetCubes());
            List<Cube> output = new List<Cube>();
            int i = c.Count - 1;
            Cube temp;
            while (i >= 0 && c[i].food == false)
            {
                temp = c[i];
                // add cube to return list
                output.Add(temp);
                // decrement i to next largest cube
                i--;
            }
            // reverse output to keep sorting order
            output.Reverse();
            return output;
        }

        /// <summary>
        /// A method to get the number of player-owned cubes in the world.
        /// This method is only allowed for client operation; it will throw an exception if the world was not
        /// constructed for client use.
        /// </summary>
        public int NumberPlayerCubes()
        {
            if (IsServer) throw new Exception("The world was constructed for server use, but a client-based method was called."); // TODO: determine exception type to use
            return PlayerCubes.Count;
        }

        /// <summary>
        /// Gets the leftmost x coordinate, topmost y coordinate, and maximum size dimension.
        /// This method is only allowed for client operation; it will throw an exception if the world was not
        /// constructed for client use.
        /// </summary>
        public void GetPlayerCubesParameters(out double left, out double top, out double sizeX, out double sizeY)
        {
            if (IsServer) throw new Exception("The world was constructed for server use, but a client-based method was called."); // TODO: determine exception type to use
            left = Width; // the farthest left edge of the player cubes
            top = Height; // the farthest top edge of the player cubes
            double farthestRight = 0, farthestBottom = 0; // helpers to find farthest right and bottom edges
            double size; // helper to save a cube's size
            foreach (Cube c in PlayerCubes.Values)
            {
                // find minimum X
                if (c.LeftEdge < left) left = c.LeftEdge;
                // find minimum Y
                if (c.TopEdge < top) top = c.TopEdge;
                size = c.Size;
                // find maximum farthestRight (X+Size)
                if (c.RightEdge > farthestRight) farthestRight = c.RightEdge;
                // find maximum farthestBottom (Y+Size)
                if (c.BottomEdge > farthestBottom) farthestBottom = c.BottomEdge;
            }
            sizeX = farthestRight - left;
            sizeY = farthestBottom - top;
        }

        /// <summary>
        /// A method to return a boolean about if the player is dead.
        /// This method is only allowed for client operation; it will throw an exception if the world was not
        /// constructed for client use.
        /// </summary>
        public bool PlayerDeath()
        {
            if (IsServer) throw new Exception("The world was constructed for server use, but a client-based method was called."); // TODO: determine exception type to use
            return PlayerCubes.Count < 1;
        }

        /// <summary>
        /// A method to split a player's cubes. This will split cubes larger than the minimum split mass. This will
        /// split cubes towards a requested x and y position. This will be restricted to only allow a player to split
        /// to the maximum splits allowed.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        /// <param name="id">The player's unique id.</param>
        /// <param name="x">The player's requested x position to split towards.</param>
        /// <param name="y">The player's requested y position to split towards.</param>
        public void SplitPlayer(int id, double x, double y)
        {
            // TODO: implement
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
            // get the player's cubes
            // if the team id is not set, it must now be set to all the player cubes
            // for each cube with greater mass than the minimum split mass:
            //   generate a new cube with same color, name, and team id; may set position in relation to original cube's location and requested position
            //   set its momentum in relation to the requested position and maximum split distance
            //   set masses to 1/2 the original cube mass
            //   a timer should be set to the split cube to have it merge back to the original cube
            //   TODO: determine which cubes should split first if maximum splits is being approached (larger first?)
        }

        /// <summary>
        /// A method to handle the absorbtion of cubes.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>The cube that has been eaten. The world will not have this cube anymore, so it should be notified to clients to help them remove the cube.</returns>
        public Cube AbsorbCubes(Cube c1, Cube c2)
        {
            // TODO: implement, better define how to use the method
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
            return null;
        }

        /// <summary>
        /// A method to move a player's cubes. The player cubes with the given id as a unique id or team id will move towards
        /// the requested position. Split cubes cannot move in a way that overlaps each other. Cubes cannot move
        /// outside the edges of the world. Movement speed is dictated in relation to top speed and low speed (which
        /// depend on the cube's mass).
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MovePlayer(int id, double x, double y)
        {
            // TODO: implement
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
        }

        /// <summary>
        /// A method to initialize the world with the maximum food.
        /// This method is only allowed for server initialization.
        /// </summary>
        private void InitializeFood()
        {
            // TODO: implement
            for (int i = 0; i < MaxFood; i++)
            {
                // call add food to add a food cube
            }
        }

        /// <summary>
        /// A method to add one food cube. Food will have a random color and location, and the default food mass. If
        /// the world has reached its maximum food capacity, nothing will be done.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        public bool AddFood()
        {
            // TODO: implement
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
            // check if max food has been reached; return false if so
            // generate cube with random color and position, default food mass, and unique id
            // add cube to world
            return false;
        }

        /// <summary>
        /// A method to apply attrition (or lose mass) to all player cubes. Larger cubes should lose mass faster than
        /// smaller cubes. There should be a minimum mass where attrition will not apply.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        public void ApplyAttrition()
        {
            // TODO: implement
            if (!IsServer) throw new Exception("The world was constructed for client use, but a server-based method was called."); // TODO: determine exception type to use
        }

        /// <summary>
        /// The private IComparer used for sorting cubes by their mass.
        /// </summary>
        private class CubeMassComparer : IComparer<Cube>
        {
            int IComparer<Cube>.Compare(Cube c1, Cube c2)
            {
                // 1 is c1>c2, 0 is c1==c2, -1 is c1<c2
                if (c1.Mass == c2.Mass) return 0;
                else if (c1.Mass > c2.Mass) return 1;
                else return -1;
            }
        }
    }
}
