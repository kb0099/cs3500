using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Cube
    {
        /// <summary>
        /// The x location. It is assumed to be part of the center.
        /// </summary>
        [JsonProperty(PropertyName = "loc_x")]
        public double X;

        /// <summary>
        /// The y location. It is assumed to be part of the center.
        /// </summary>
        [JsonProperty(PropertyName = "loc_y")]
        public double Y;

        /// <summary>
        /// The ARGB color contained in one int. Useful for Color.FromArgb(int).
        /// </summary>
        [JsonProperty]
        public int argb_color;

        /// <summary>
        /// The unique ID.
        /// </summary>
        [JsonProperty(PropertyName ="uid")]
        public int uId;

        /// <summary>
        /// The team ID. When a player splits, the team ID is shared among the cubes. The player's original cube will
        /// have the unique ID equal the team ID.
        /// </summary>
        [JsonProperty(PropertyName = "team_id")]
        public int teamId;

        /// <summary>
        /// A condition to define the cube as food.
        /// </summary>
        [JsonProperty]
        public bool food;

        /// <summary>
        /// The name given the cube.
        /// </summary>
        [JsonProperty]
        public string Name;

        /// <summary>
        /// The mass of the cube.
        /// </summary>
        [JsonProperty]
        public int Mass;

        private int mSteps;     // momentum steps
        private int mx, my;     // momentum in x and y
        public DateTime mergeAfter;    // time to merge  


        /// <summary>
        /// A getter for the size of the cube. Size is the width and height of the square the cube can be drawn as.
        /// </summary>
        public double Size { get { return Math.Pow(Mass, 0.5); } } //math.sqrt(mass)

        /// <summary>
        /// The x coordinate location of the left edge of the cube.
        /// </summary>
        public double LeftEdge { get { return X - Size / 2; } }

        /// <summary>
        /// The x coordinate location of the right edge of the cube.
        /// </summary>
        public double RightEdge { get { return X + Size / 2; } }

        /// <summary>
        /// The y coordinate location of the top edge of the cube.
        /// </summary>
        public double TopEdge { get { return Y - Size / 2; } }

        /// <summary>
        /// The y coordinate location of the bottom edge of the cube.
        /// </summary>
        public double BottomEdge { get { return Y + Size / 2; } }

        /// <summary>
        /// A constructor to fill all parameters of the cube. This is most useful for deserializing a JSON.
        /// </summary>
        /// <param name="loc_x">The x location.</param>
        /// <param name="loc_y">The y location.</param>
        /// <param name="argb_color">An integer that can be fed into Color.FromArgb(int).</param>
        /// <param name="uid">A unique ID.</param>
        /// <param name="team_id">A team ID.</param>
        /// <param name="food">A boolean to state if the cube is food.</param>
        /// <param name="Name">The name given to the cube.</param>
        /// <param name="Mass">The size of the cube.</param>
        [JsonConstructorAttribute]
        public Cube(double loc_x, double loc_y, int argb_color, int uid, int team_id, bool food, string Name, int Mass)
        {
            this.X = loc_x;
            this.Y = loc_y;
            this.argb_color = argb_color;
            this.uId = uid;
            this.teamId = team_id;
            this.food = food;
            this.Name = Name;
            this.Mass = Mass;
        }

        /// <summary>
        /// An Equals method for checking cube equality.
        /// Cubes are equal if the unique ID's are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.uId == ((Cube)obj).uId;
        }

        /// <summary>
        /// A GetHashCode method to allow cubes to efficiently be used in a HashSet.
        /// The hash code is the unique ID.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return uId;
        }

        /// <summary>
        /// A ToString method to more easily read debug data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Food: " + food + ", Name: " + Name + ", ID: " + uId + ", Team: " + teamId;
        }


        /// <summary>
        /// Applies the momentum to this cube
        /// </summary>
        public void ApplyMomentum()
        {
            if(!(mSteps < 0))
            {
                mSteps -= 1;
                X += mx;
                Y += my;
            }
        }

        /// <summary>
        /// Sets the momentum in x and y direction with given steps to apply
        /// </summary>
        public void SetMomentum(int mmx, int mmy, int steps)
        {
            this.mx = mmx;
            this.my = mmy;
            this.mSteps = steps;
        }

        /// <summary>
        /// Returns true if this cube contains <paramref name="c"/> inside it.
        /// </summary>
        public bool Contains(Cube c)
        {
            return LeftEdge < c.LeftEdge && RightEdge > c.RightEdge && TopEdge < c.TopEdge && BottomEdge > c.BottomEdge; 
        }

    }
}
