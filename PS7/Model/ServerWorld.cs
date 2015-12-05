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
        // TODO: define units in seconds to help in determining use in equations
        /// <summary>
        /// The rate of updates per second the world should do.
        /// </summary>
        public int HeartbeatsPerSecond { get; private set; }

        // TODO: figure out units
        /// <summary>
        /// The rate at which cubes can move at their smallest mass.
        /// </summary>
        public int TopSpeed { get; private set; }

        // TODO: figure out units
        /// <summary>
        /// The rate at which cubes can move at their largest mass.
        /// </summary>
        public int LowSpeed { get; private set; }

        // TODO: figure out units
        /// <summary>
        /// The rate at which cubes lose mass. Units are the amount of mass lost per heartbeat. This applies to
        /// smaller cubes, and scales up for larger cubes.
        /// </summary>
        public double AttritionRate { get; private set; }

        /// <summary>
        /// The default mass of food.
        /// </summary>
        public int FoodValue { get; private set; }

        /// <summary>
        /// The initial mass of players.
        /// </summary>
        public int PlayerStartMass { get; private set; }

        /// <summary>
        /// The maximum amount of food for the world. When world has less food, 1 food should be added per heartbeat.
        /// </summary>
        public int MaxFood { get; private set; }

        /// <summary>
        /// The mass that should not allow splitting when below this value.
        /// </summary>
        public int MinimumSplitMass { get; private set; }

        /// <summary>
        /// The distance a cube can be "thrown" when split.
        /// </summary>
        public int MaximumSplitDistance { get; private set; }

        /// <summary>
        /// The amount of cubes a player can split to at most.
        /// </summary>
        public int MaximumSplits { get; private set; }

        /// <summary>
        /// The distance cubes must be to have the larger eat the smaller.
        /// This value is a percentage of how much of the smaller cube is overlapped by the larger cube in an axis.
        /// </summary>
        public double AbsorbDistanceDelta { get; private set; }

        /// <summary>
        /// A random number generator used for random locations of cubes.
        /// </summary>
        private Random r;

        /// <summary>
        /// A dictionary that represents all the foods on the server. The keys are unique id's, and the values are
        /// the cubes.
        /// </summary>
        public Dictionary<int, Cube> foodCubes;

        /// <summary>
        /// A dictionary that represents all the player cubes on the server. The keys are the unique id's, and the
        /// values are the cubes.
        /// </summary>
        public Dictionary<int, Cube> playerCubes;

        /// <summary>
        /// A dictionary that represents all the splitted cubes that belong to a single team/player. The keys are
        /// the team id's, and the values are the collection of cubes belonging to the team.
        /// </summary>
        public Dictionary<int, List<Cube>> teamCubes;

        /// <summary>
        /// Represents a temporary instance of mergedCubes.
        /// </summary>
        public List<Cube> mergedCubes = new List<Cube>();

        /// <summary>
        /// Represents the minimum # of seconds the cube needs to wait before it can merge.
        /// </summary>
        public int MinTimeToMerge { get { return 4; } }

        /// <summary>
        /// A dictionary that represents momentums of cubes. Used during splitting. It acts as a multiplier for the
        /// speed of a cube.
        /// </summary>
        //private Dictionary<int, int> cubeMomentum;

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
            this.AttritionRate = double.Parse(p.Element("attrition_rate").Value);
            this.FoodValue = int.Parse(p.Element("food_value").Value);
            this.PlayerStartMass = int.Parse(p.Element("player_start_mass").Value);
            this.MaxFood = int.Parse(p.Element("max_food").Value);
            this.MinimumSplitMass = int.Parse(p.Element("min_split_mass").Value);
            this.MaximumSplitDistance = int.Parse(p.Element("max_split_dist").Value);
            this.MaximumSplits = int.Parse(p.Element("max_splits").Value);
            this.AbsorbDistanceDelta = double.Parse(p.Element("absorb_constant").Value);

            this.r = new Random();
            this.foodCubes = new Dictionary<int, Cube>(this.MaxFood); // initializing capacity will cut down on times the object will have to resize
            this.playerCubes = new Dictionary<int, Cube>(10); // assuming an initial capacity of 10 will work here
            this.teamCubes = new Dictionary<int, List<Cube>>(10); // assuming an initial capacity of 10 will work here
        }

        /// <summary>
        /// A constant array of colors that are used to set player cube colors.
        /// </summary>
        private static readonly Color[] PLAYER_COLORS = {
            Color.Red, Color.Blue, Color.Black, Color.Violet, Color.LightPink, Color.Yellow, Color.Orange, Color.Pink
        };

        /// <summary>
        /// A counter to help iterate through colors available when setting player cube color.
        /// </summary>
        private static int nextColor = 0;

        /// <summary>
        /// A counter to help track available unique id's.
        /// </summary>
        private static int nextUID = -1;

        /// <summary>
        /// A constant to set the color of virus cubes.
        /// </summary>
        private static readonly Color VIRUS_COLOR = Color.Green;

        /// <summary>
        /// Returns next player color.
        /// </summary>
        /// <returns>Next available color.</returns>
        private static int NextPlayerColor()
        {
            return PLAYER_COLORS[(nextColor++) % (PLAYER_COLORS.Length)].ToArgb();
        }

        /// <summary>
        /// Returns the next unique id.
        /// </summary>
        /// <returns></returns>
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
            lock (this)
            {
                if (this.foodCubes.Count >= this.MaxFood)
                    return false;
                food = new Cube(r.Next() % this.Width, r.Next() % this.Height, Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)).ToArgb(), NextUID(), 0, true, "", FoodValue);
                foodCubes[food.uId] = food;
            }
            return true;
        }

        /// <summary>
        /// Moves the cube towards the given point and not to that point.
        /// The cube could be either player or food or virus.
        /// </summary>
        /// <param name="cId">The cube id</param>
        /// <param name="toX">Towards X co-ordinate</param>
        /// <param name="toY">Towards Y co-oprdinate</param>
        public void MoveCube(int cId, int toX, int toY)
        {
            // iterate through cubes with a team id or cube id like the input
            double h, speed, finalX, finalY;
            Cube c;
            if (!playerCubes.TryGetValue(cId, out c)) return;

            // calculate length of vector from cube point to destination point
            h = Math.Sqrt(Math.Pow(toX - c.X, 2) + Math.Pow(toY - c.Y, 2));
            // calculate speed for cube
            speed = TopSpeed - c.Mass / PlayerStartMass - 1; // when at minimum mass, TopSpeed applies; larger mass decreases speed
            if (speed < LowSpeed) speed = LowSpeed;
            // calculate final point, which overall has unit vector dimensions multiply with the speed, then add to the current location
            finalX = (toX - c.X) / h * speed + c.X;
            finalY = (toY - c.Y) / h * speed + c.Y;
            c.X = finalX;
            c.Y = finalY;
            // handle world edges
            HandleWorldEdges(cId);
        }

        /// <summary>
        /// Restricts the cubes to move outside the world boundary.
        /// </summary>
        /// <param name="cId">The id of the cube</param>
        private void HandleWorldEdges(int cId)
        {
            Cube c = playerCubes[cId];
            int delta = (int)c.Size / 2 - r.Next(10, 20);
            if (c.RightEdge > Width + delta) c.X = Width - delta;
            if (c.LeftEdge < -delta) c.X = delta;
            if (c.BottomEdge > Height + delta) c.Y = Height - delta;
            if (c.TopEdge < -delta) c.Y = delta;
        }

        /// <summary>
        /// Moves the splitted cubes towards the given point and not to that point.
        /// </summary>
        /// <param name="tid">The common team id</param>
        /// <param name="toX">Towards X co-ordinate</param>
        /// <param name="toY">Towards Y co-oprdinate</param>
        public void MoveTeam(int tid, int toX, int toY)
        {
            List<Cube> team;
            if (teamCubes.TryGetValue(tid, out team))
            {
                foreach (Cube current in team)
                {
                    current.ApplyMomentum();
                    MoveCube(current.uId, toX, toY);
                }
                HandleTeamOverlap(team);
                return;
            }
            else
            {
                Cube single;
                if (playerCubes.TryGetValue(tid, out single))
                {
                    MoveCube(single.uId, toX, toY);
                }
            }
        }

        private void HandleTeamOverlap(List<Cube> team)
        {
        }

        /// <summary>
        /// Handles eating of food cubes by the world player cubes.
        /// Side effect: Removes the food cubes which have been eaten from the world.
        /// </summary>
        /// <returns>All the cubes that are eaten.</returns>
        public IEnumerable<Cube> EatFoods()
        {
            // initialize the output
            List<Cube> output = new List<Cube>();
            lock (this)
            {
                // iterate through players
                foreach (Cube c in playerCubes.Values)
                {
                    // iterate through food
                    foreach (Cube f in foodCubes.Values)
                    {
                        // if IsAbsorbable() is true, add food cube to output and manage consumption
                        if (IsAbsorbable(c, f))
                        {
                            output.Add(f);
                            // consumption involves removing the food and adding to the cube's mass, then setting food to 0 mass to kill it

                            Console.WriteLine(c.Mass + ", " + f.Mass);                        // debug
                            c.Mass += f.Mass;
                            f.Mass = 0;
                        }
                    }
                    // removal of cubes must be done here to avoid foreach exception of changing IEnumerable
                    foreach (Cube dead in output)
                    {
                        foodCubes.Remove(dead.uId);
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Handles players eating other players. Also, removes dead players from world.
        /// </summary>
        /// <returns>All the players those have been eaten. If a cube heading a team is eaten, it will be in here too.</returns>
        public IEnumerable<Cube> EatPlayers()
        {
            // initialize the output
            List<Cube> output = new List<Cube>();
            lock (this)
            {
                try
                {
                    // Format the player cubes into a sorted list by mass
                    List<Cube> sorted = new List<Cube>(playerCubes.Values);
                    sorted.Sort(Comparer);
                    // TODO: iteration style will affect game behavior; a special case of large can absorb medium, medium can absorb small, large can absorb small, but in what order can they absorb?
                    Cube a, b;
                    double tempX, tempY;
                    int tempMass;
                    // iterate from second to smallest cube to largest cube
                    for (int i = 1; i < sorted.Count; i++)
                    {
                        a = sorted[i];
                        // iterate through all cubes smaller than i, in order of smallest to largest
                        for (int j = 0; j < i; j++)
                        {
                            b = sorted[j];
                            // if IsAbsorbable() is true, add smaller cube to output and manage consumtion
                            if (IsAbsorbable(a, b))
                            {
                                // if the consumed cube was head of a team, find another team cube to swap the cube roles
                                if (b.uId == b.teamId)
                                {
                                    foreach (Cube swap in playerCubes.Values)
                                    {
                                        if (swap.teamId == b.teamId && swap.uId != b.uId) // if there is no other cube of the same team, then nothing special is done
                                        {
                                            // swap cubes by changing coordinates and mass between the cubes, adding head cube to output, and changing b to point to the swapped cube
                                            tempX = b.X;
                                            tempY = b.Y;
                                            tempMass = b.Mass;
                                            b.X = swap.X;
                                            b.Y = swap.Y;
                                            b.Mass = swap.Mass;
                                            swap.X = tempX;
                                            swap.Y = tempY;
                                            swap.Mass = tempMass;
                                            output.Add(b);
                                            b = swap;
                                            break;
                                        }
                                    }
                                }
                                output.Add(b);
                                // consumption involves removing the smaller cube (from dictionary and list) and adding to the larger cube's mass, then setting player to 0 mass to kill it
                                playerCubes.Remove(b.uId);
                                sorted.RemoveAt(j);
                                a.Mass += b.Mass;
                                b.Mass = 0;
                                // The change in a's mass could ruin the sorting order of the list, so re-sort
                                sorted.Sort(Comparer); // if the cube mass did ruin sorting, it will jump up higher on the list, so the order of cubes below it should not be affected
                                                       // since the list was modified, i, j, and a need to be modified to have loop consistency
                                                       // to fix it so the for loops are consistent:
                                                       //      j is set to -1 (in case of re-sorting changing a) to have next loop start from beginning of list; a's increased size or changed cube could affect all smaller cubes, so need to start over
                                                       //      i is decremented in relation to the removal in the list shifting cubes to the left
                                                       //      a is re-obtained (in case of previous re-sorting changing a)
                                j = -1;
                                i--;
                                a = sorted[i];
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                // add merged players as eaten players
                if (mergedCubes.Count > 0)
                {
                    output.AddRange(mergedCubes);
                    mergedCubes.Clear();
                }
            }
            return output;
        }

        /// <summary>
        /// A helper method to determine if the smaller cube can be absorbed by the larger cube due to proximity to
        /// each other.
        /// </summary>
        /// <param name="c1">The first cube.</param>
        /// <param name="c2">The second cube.</param>
        /// <returns></returns>
        private bool IsAbsorbable(Cube c1, Cube c2)
        {
            // the cube sizes must be determined
            Cube large, small;
            if (c1.Mass == c2.Mass || (c1.teamId == c2.teamId && c1.teamId != 0))
            {
                return false; // masses are too close to be absorbable, or they are of the same team
            }
            else if (c1.Mass > c2.Mass)
            {
                large = c1;
                small = c2;
            }
            else
            {
                large = c2;
                small = c1;
            }
            // determine max difference for absorbtion
            double maxD = 0.5 * large.Size + small.Size * (0.5 - AbsorbDistanceDelta); // max diffference = 1/2 * large size - small size * absorb percentage + 1/2 * small size
            // determine differences in axes
            double dx = Math.Abs(large.X - small.X);
            double dy = Math.Abs(large.Y - small.Y);
            // if the differences are in range, they are absorbable
            if (maxD >= dx && maxD >= dy) return true;
            else return false;
        }

        /// <summary>
        /// Splits the cube honoring the requirements.
        /// Side effect: Should add the splitted cubes to the  teamCubes.
        /// </summary>
        /// <param name="cId">Cube id to split</param>
        /// <param name="toX">Split towards X</param>
        /// <param name="toY">Split towards Y</param>
        public void SplitCube(int cId, int toX, int toY)
        {
            double h;
            List<Cube> currentTeam = null;
            List<Cube> newTeam = new List<Cube>();
            lock (this)
            {
                if (!teamCubes.TryGetValue(cId, out currentTeam))
                {
                    int tid = playerCubes[cId].teamId;
                    if (!teamCubes.TryGetValue(tid, out currentTeam))
                    {
                        playerCubes[cId].teamId = cId;
                        currentTeam = new List<Cube> { playerCubes[cId] };
                    }
                    else
                    {
                        cId = tid;  // cId will work as team id going forward
                    }
                }
                foreach (Cube c in currentTeam)
                {
                    newTeam.Add(c);
                    if (c.Mass < MinimumSplitMass || currentTeam.Count > MaximumSplits) return;

                    // set cube to be half its mass
                    c.Mass = c.Mass / 2;
                    h = Math.Sqrt(Math.Pow(toX - c.X, 2) + Math.Pow(toY - c.Y, 2));
                    Cube split = new Cube(c.X, c.Y, c.argb_color, NextUID(), c.teamId, false, c.Name, c.Mass);
                    split.mergeAfter = DateTime.Now.AddSeconds(MinTimeToMerge);
                    split.SetMomentum((int)((toX - c.X) / h * split.Mass / 100), (int)((toY - c.Y) / h * split.Mass / 100), HeartbeatsPerSecond);

                    newTeam.Add(split);
                    playerCubes[split.uId] = split;
                }
            }
            if (newTeam.Count > 1)
            {
                lock (this)
                {
                    teamCubes[cId] = newTeam;
                }
                TryMerge(cId);
            }
        }

        private void TryMerge(int cId)
        {
            Task.Run(async () =>
            {
                await Task.Delay(MinTimeToMerge * 1000);
                MergeTeam(cId);
            });
        }

        /// <summary>
        /// Private helper to merge back the splitted cubes.
        /// </summary>
        /// <param name="tid">Id of the team</param>
        private void MergeTeam(int tid)
        {
            List<Cube> splits;
            teamCubes.TryGetValue(tid, out splits);
            if (splits == null) return;
            splits = new List<Cube>(splits);    // new enumeration
            lock (this)
            {
                foreach (Cube s in splits)
                {
                    if (s.mergeAfter < DateTime.Now && s.uId != tid) // exclude the main cube
                    {
                        playerCubes[tid].Mass += s.Mass;
                        playerCubes.Remove(s.uId);
                        teamCubes[tid].Remove(s);      // changes the enumeration using another instance
                        s.Mass = 0;
                        mergedCubes.Add(s);
                    }
                }
                if (teamCubes[tid].Count == 1) teamCubes.Remove(tid);
                else TryMerge(tid);
            }
        }

        /// <summary>
        /// A method to apply attrition (or lose mass) to all player cubes. Larger cubes should lose mass faster than
        /// smaller cubes. There should be a minimum mass where attrition will not apply.
        /// This method is only allowed for server operation; it will throw an exception if the world was not
        /// constructed for server use.
        /// </summary>
        public void ApplyAttrition()
        {
            // iterate through player cubes
            foreach (Cube c in playerCubes.Values)
            {
                // calculate the amount of mass to remove in relation to a minimum mass (PlayerStartMass for now), then remove that mass
                // if the cube is under minimum mass, do nothing to it
                if (c.Mass <= PlayerStartMass)
                {
                    continue;
                }
                // remove mass based on AttritionRate and ratio of player mass to PlayerStartMass
                c.Mass = (int)(c.Mass - Math.Sqrt(c.Mass) / AttritionRate); //AttritionRate * (c.Mass / PlayerStartMass));
            }
        }

        private const int MAX_VIRUS_COUNT = 4;
        public List<Cube> viruses = new List<Cube>();
        /// <summary>
        /// Adds the virus feature
        /// </summary>
        public void HandleViruses()
        {
            lock (viruses)
            {
                if (viruses.Count < MAX_VIRUS_COUNT)
                {
                    if (r.Next(20) > 12)
                    {
                        Cube c;
                        lock (this)
                        {
                            c = new Cube(r.Next(Width),
                            r.Next(Height),
                            VIRUS_COLOR.ToArgb(),
                            NextUID(),
                            0,
                            false,
                            "",
                            r.Next(100, 500));
                            // add the virus
                            viruses.Add(c);
                        }
                    }
                }

            }

            List<Cube> temp = new List<Cube>();
            lock (this)
            {
                foreach (Cube v in viruses)
                {
                    foreach (Cube p in playerCubes.Values.ToList())
                    {
                        if (p.Mass > 600)
                        {
                            if (IsAbsorbable(p, v))
                            {
                                SplitCube(p.uId, r.Next(Width), r.Next(Height));    // cube gets exploded
                                temp.Add(v);        // virus gets destroyed
                            }
                        }
                    }
                }
                foreach (Cube v in temp)
                {
                    v.Mass = 0;
                    viruses.Remove(v);
                    mergedCubes.Add(v);
                }
            }
        }

    }
}
