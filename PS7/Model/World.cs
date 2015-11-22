using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    public class World
    {
        /// <summary>
        /// The width of the world.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// The height of the world.
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// The rate of updates per second the world should do.
        /// </summary>
        public object HeartbeatsPerSecond { get; private set; } // TODO: figure out the type and use case, implement in construction and methods

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
        public double FoodValue { get; private set; } // TODO: implement in code

        /// <summary>
        /// The initial mass of players.
        /// </summary>
        public double PlayerStartMass { get; private set; } // TODO: implement in code

        /// <summary>
        /// The maximum amount of food for the world. When world has less food, 1 food should be added per heartbeat.
        /// </summary>
        public int MaxFood { get; private set; } // TODO: implement in code

        /// <summary>
        /// The mass that should not allow splitting when below this value.
        /// </summary>
        public double MinimumSplitMass { get; private set; } // TODO: implement in code

        /// <summary>
        /// The distance a cube can be "thrown" when split.
        /// </summary>
        public double MaximumSplitDistance { get; private set; } // TODO: implement in code

        /// <summary>
        /// The amount of cubes a player can split to at most.
        /// </summary>
        public int MaximumSplits { get; private set; } // TODO: implement in code

        /// <summary>
        /// The distance cubes must be to have the larger eat the smaller.
        /// </summary>
        public double AbsorbDistanceDelta { get; private set; } // TODO: implement in code

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        private int PlayerID;

        ///// <summary>
        ///// Collection of food cubes in the world.
        ///// </summary>
        //public Dictionary<long, Cube> foodCubes;

        ///// <summary>
        ///// The player's cubes, which updates when new cubes belong to player's team.
        ///// </summary>
        //public Dictionary<long, Cube> playerCubes;
        //public Dictionary<long, LinkedList<Cube>> splitCubes;

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
        private IComparer<Cube> Comparer;

        /// <summary>
        /// Client-based constructor of a World object.
        /// </summary>
        /// <param name="w">The width of the world.</param>
        /// <param name="h">The height of the world</param>
        /// <param name="id">The player id.</param>
        public World(double w, double h, int id)
        {
            this.Width = w;
            this.Height = h;
            this.PlayerID = id;
            this.Comparer = new CubeMassComparer();
            this.Cubes = new Dictionary<int, Cube>(50);
            this.PlayerCubes = new Dictionary<int, Cube>(6);
        }

        /// <summary>
        /// A method to add a cube to the world. If the cube is already in the world, it's information is updated.
        /// If the cube has 0 mass, it is removed from the world.
        /// </summary>
        public void AddCube(Cube c)
        {
            // check cube is not null; if it is, don't do anything
            if (c == null) return;
            // remove cubes with the same unique ID in Cubes and PlayerCubes; this helps to remove old cube data to swap with new cube data
            Cubes.Remove(c.uId);
            PlayerCubes.Remove(c.uId);
            // if the cube is alive (i.e. not mass 0), add it to appropriate collections
            if (c.Mass != 0)
            {
                // add cube to Cubes
                Cubes.Add(c.uId, c);
                // if it is a player cube (player ID equals team ID), add it to PlayerCubes
                if (c.teamId == PlayerID || c.uId == PlayerID) PlayerCubes.Add(c.uId, c);
            }

        }

        /// <summary>
        /// A method to return the cubes of the world.
        /// The collection of cubes will be sorted by the smallest first.
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
        /// </summary>
        /// <returns></returns>
        public int NumberCubes()
        {
            return Cubes.Count;
        }

        /// <summary>
        /// A method to return cubes belonging to the player.
        /// </summary>
        public IEnumerable<Cube> GetPlayerCubes()
        {
            return PlayerCubes.Values;
        }

        /// <summary>
        /// A method to get the number of player-owned cubes in the world.
        /// </summary>
        /// <returns></returns>
        public int NumberPlayerCubes()
        {
            return PlayerCubes.Count;
        }

        /// <summary>
        /// Gets the leftmost x coordinate, topmost y coordinate, and maximum size dimension.
        /// </summary>
        public void GetPlayerCubesParameters(out double left, out double top, out double sizeX, out double sizeY)
        {
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
        /// </summary>
        public bool PlayerDeath()
        {
            return PlayerCubes.Count < 1;
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
