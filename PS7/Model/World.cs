using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
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
        /// The player's unique ID.
        /// </summary>
        private int PlayerID;

        /// <summary>
        /// Collection of cubes in the world.
        /// </summary>
        private List<Cube> Cubes;

        /// <summary>
        /// The player's cube, which updates as new cube collecitons are set.
        /// </summary>
        private Cube Player;

        /// <summary>
        /// The comparer used to sort the collection of cubes.
        /// </summary>
        private IComparer<Cube> Comparer;

        /// <summary>
        /// Constructor of a World object.
        /// </summary>
        /// <param name="w">The width of the world.</param>
        /// <param name="h">The height of the world</param>
        /// <param name="id">The player's cube ID.</param>
        public World(double w, double h, int id)
        {
            this.Width = w;
            this.Height = h;
            this.PlayerID = id;
            this.Cubes = new List<Cube>();
            this.Comparer = new CubeMassComparer();
        }

        /// <summary>
        /// A method to set the cubes of the world.
        /// The old collection of cubes will be replaced with the input.
        /// </summary>
        public void SetCubes(IEnumerable<Cube> cubes)
        {
            // create a new list containing the cubes
            this.Cubes = new List<Cube>(cubes);
            // sort the list based on the cube sizes
            this.Cubes.Sort(Comparer);
            // search Cubes to get the player's cube
            Player = this.Cubes.Find(c => { return c.uid == PlayerID; });
        }

        /// <summary>
        /// A method to return the cubes of the world.
        /// The collection of cubes will be sorted by the smallest first.
        /// </summary>
        public IEnumerable<Cube> GetCubes()
        {
            return Cubes;
        }

        /// <summary>
        /// A method to return the player's cube. The player is identified by the ID given to the world upon
        /// construction.
        /// </summary>
        public Cube GetPlayerCube()
        {
            return Player;
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
