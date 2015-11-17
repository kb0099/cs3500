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
        /// The x location. It is assumed to be the left edge of the cube.
        /// </summary>
        [JsonProperty(PropertyName = "loc_x")]
        public double X;

        /// <summary>
        /// The y location. It is assumed to be the top edge of the cube.
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
        [JsonProperty]
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
        public double Mass;

        /// <summary>
        /// A getter for the size of the cube. Size is the width and height of the square the cube can be drawn as.
        /// </summary>
        public double Size { get { return Math.Sqrt(Mass); } }

        /// <summary>
        /// The x coordinate location of the center.
        /// </summary>
        public double CenterX { get { return X + Size / 2; } }

        /// <summary>
        /// The y coordinate location of the center.
        /// </summary>
        public double CenterY { get { return Y + Size / 2; } }

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
        public Cube(double loc_x, double loc_y, int argb_color, int uid, int team_id, bool food, string Name, double Mass)
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
    }
}
