﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Cube
    {
        /// <summary>
        /// The x location.
        /// </summary>
        [JsonProperty(PropertyName = "loc_x")]
        public double X;

        /// <summary>
        /// The y location.
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
        public int uid;

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
        /// A constructor to fill all parameters of the cube. This is most useful for deserializing a JSON.
        /// </summary>
        /// <param name="loc_x">The x location.</param>
        /// <param name="loc_y">The y location.</param>
        /// <param name="argb_color">An integer that can be fed into Color.FromArgb(int).</param>
        /// <param name="uid">A unique ID.</param>
        /// <param name="food">A boolean to state if the cube is food.</param>
        /// <param name="Name">The name given to the cube.</param>
        /// <param name="Mass">The size of the cube.</param>
        [JsonConstructorAttribute]
        public Cube(double loc_x, double loc_y, int argb_color, int uid, bool food, string Name, double Mass)
        {
            this.X = loc_x;
            this.Y = loc_y;
            this.argb_color = argb_color;
            this.uid = uid;
            this.food = food;
            this.Name = Name;
            this.Mass = Mass;
        }
    }
}