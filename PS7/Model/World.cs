using System;
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
        /// Collection of cubes in the world. Cubes can be identified by their unique ID, which will be the key.
        /// </summary>
        private IEnumerable<Cube> Cubes;

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
            this.Cubes = new HashSet<Cube>();
        }

        /// <summary>
        /// A method to set the cubes of the world.
        /// The old collection of cubes will be replaced with the input.
        /// </summary>
        public void SetCubes(IEnumerable<Cube> cubes)
        {
            this.Cubes = cubes;
        }

        /// <summary>
        /// A method to return the cubes of the world.
        /// </summary>
        public IEnumerable<Cube> GetCubes()
        {
            return Cubes;
        }
    }
}
