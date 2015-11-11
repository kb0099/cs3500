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
        /// The player's cubes, which updates when new cubes belong to player's team.
        /// </summary>
        private List<Cube> PlayerCubes;

        /// <summary>
        /// The comparer to sort the collection of cubes.
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
            this.Comparer = new CubeMassComparer();
            this.Cubes = new List<Cube>(50);
            this.PlayerCubes = new List<Cube>(6);
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
            Predicate<Cube> remover = (cb) => cb.uId == c.uId;
            Cubes.RemoveAll(remover);
            PlayerCubes.RemoveAll(remover);
            // if the cube is alive (i.e. not mass 0), add it to appropriate collections
            if (c.Mass != 0)
            {
                // add cube to Cubes
                Cubes.Add(c);
                // if it is a player cube (player ID equals team ID), add it to PlayerCubes
                if (c.teamId == PlayerID) PlayerCubes.Add(c);
                // sort the Cubes
                Cubes.Sort(Comparer);
            }
            
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
        /// A method to return cubes belonging to the player.
        /// </summary>
        public IEnumerable<Cube> GetPlayerCubes()
        {
            return PlayerCubes;
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
