using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AgCubio
{
    public partial class GameForm : Form
    {
        // field declarations
        private World world;
        private System.Net.Sockets.Socket socket;
        private long playerId;      // todo: need to change to team
        private bool dead;

        public GameForm()
        {
            world = new World(800, 600);
            InitializeComponent();
        }

        /// <summary>
        /// The method run when the "Connect" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // TODO a test on being able to change panels displayed; the connect panel can be visible first, then when the connection works it is hidden and the game panel is shown (which will start painting)
            this.ConnectionPanel.Hide();
            this.GamePanel.Show();

            // Create and connect to socket
            Network.ConnectToServer((pso) =>
            {
                socket = pso.clientSocket;
                // first get the player cube
                // 1. send name
                Network.Send(socket, nameTextBox.Text);

                // 2. receive data
                // 2.a. Indicate the first time call back funciton to handle player
                pso.callback = HandleFirstMessageFromServer;
                // 2.b. First time call to receive data (server will send the player which is handled by above callback)

                Network.WantMoreData(pso);
            }
            , serverTextBox.Text);


            //TempWorldSetup();
        }


        private void HandleFirstMessageFromServer(PreservedState ps)
        {
            // grabe the { ... } part except the new line
            var playerStr = ps.receivedData.ToString().Substring(0, ps.receivedData.Length - 1); 
                var cube = JsonConvert.DeserializeObject<Cube>(playerStr);
                world.playerCubes[cube.uId] = cube;
                playerId = cube.uId;
                this.Invalidate();
    
                // remove the cube data from the state
                ps.receivedData.Clear();

                // after the first player message, server is going to send cubes separated by a new line
                ps.callback = ProcessReceivedData;
                //MessageBox.Show("Success in cubing the player!" + JsonConvert.SerializeObject(cube));

                // make the second call to receive data explicitly
                // ProcessReceivedData will be called now after data is received.
                Network.WantMoreData(ps);
        }

        /// <summary>
        /// This method is a callback method when data is received.
        /// It parses the received data into a meaningful Json string.
        /// Then, it tries to convert that to a cube object.
        /// It removes that parsed part of the string from ps.receivedData.
        /// </summary>
        /// <param name="ps">The preserved state object</param>
        private void ProcessReceivedData(PreservedState ps)
        {
            System.IO.File.AppendAllText("last.txt", ps.receivedData.ToString());
            //MessageBox.Show("receiving!");

            StringBuilder receivedData = ps.receivedData;
            // need to make the world thread safe
            lock (this.world)
            {
                // split the data by new line
                string[] jsonCubes = receivedData.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // number of successful conversions to Cube object
                int success = 0;
                int i = 0;
                try
                {
                    while (i < jsonCubes.Length)
                    {
                        // try to onverting to cube object
                        string line = jsonCubes[i];

                        // check it is not of the form: ${.*}^
                        if( !(line.StartsWith("{") && line.EndsWith("}")))
                        {
                            // if this broken line is not the last line:
                            if (i != jsonCubes.Length - 1)
                            {
                                success++; // there is no way to recover what was server saying, so, discard it as a success
                                i++;
                                continue;
                            }
                        }
                        
                        Cube cube = JsonConvert.DeserializeObject<Cube>(jsonCubes[i]);                        
                        success++;

                        if (cube.food)
                        {
                            // food is eaten remove it.
                            if (cube.Mass == 0.0)
                                this.world.foodCubes.Remove(cube.uId);
                            else
                                this.world.foodCubes[cube.uId] = cube;
                        }
                        else if (cube.Mass == 0.0)
                        {
                            // if some player cube dies remove it.
                            this.world.playerCubes.Remove(cube.uId);
                            // indicate the death if the current player cube died!
                            if (cube.uId == this.playerId)
                                this.dead = true;
                        }
                        else
                        {
                            // if it is not food, it must be a player cube!
                            this.world.playerCubes[cube.uId] = cube;
                        }
                        i++;
                    }

                }
                catch {                         
                }

                // debug purpose
                //System.IO.File.WriteAllText("testfile.txt",                     "# of converted cubes = " + success + "\n" + receivedData.ToString());
                receivedData.Clear();

                if (success > 0)        this.Invoke( (Action)(() => {
                    this.Invalidate();
                }) );   
                
                // need to put back the last unparsed json string
                if (success < jsonCubes.Length)
                    receivedData.Append(jsonCubes[success - 1]);
            }
            if (this.dead)
                base.Invoke((Action)(() => { MessageBox.Show("your cube died!"); }));

            // Ready to receive more data!
            Network.WantMoreData(ps);
        }

        ///// <summary>
        ///// Temporary way to setup world for testing.
        ///// </summary>
        //private void TempWorldSetup()
        //{
        //    // temp way to setup world
        //    HashSet<Cube> cubes = new HashSet<Cube>();
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":926.0,\"loc_y\":682.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":1000.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":928.0,\"loc_y\":714.0,\"argb_color\":-65536,\"uid\":2571,\"team_id\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":900.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":920.0,\"loc_y\":673.0,\"argb_color\":-9834450,\"uid\":1571,\"team_id\":1571,\"food\":false,\"Name\":\"Bill Gates\",\"Mass\":500.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":116.0,\"loc_y\":350.0,\"argb_color\":-8243084,\"uid\":5002,\"team_id\":5002,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":193.0,\"loc_y\":523.0,\"argb_color\":-4759773,\"uid\":5075,\"team_id\":5075,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":267.0,\"loc_y\":55.0,\"argb_color\":-7502725,\"uid\":2,\"team_id\":2,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":998.0,\"loc_y\":580.0,\"argb_color\":-16481514,\"uid\":3,\"team_id\":3,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":69.0,\"loc_y\":895.0,\"argb_color\":-5905052,\"uid\":4,\"team_id\":4,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":387.0,\"loc_y\":506.0,\"argb_color\":-2505812,\"uid\":5,\"team_id\":5,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":687.0,\"loc_y\":152.0,\"argb_color\":-9834450,\"uid\":6,\"team_id\":6,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":395.0,\"loc_y\":561.0,\"argb_color\":-2210515,\"uid\":7,\"team_id\":7,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":585.0,\"loc_y\":222.0,\"argb_color\":-11930702,\"uid\":8,\"team_id\":8,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":49.0,\"loc_y\":614.0,\"argb_color\":-4232190,\"uid\":9,\"team_id\":9,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":809.0,\"loc_y\":452.0,\"argb_color\":-9234755,\"uid\":10,\"team_id\":10,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":286.0,\"loc_y\":666.0,\"argb_color\":-11083980,\"uid\":11,\"team_id\":11,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":252.0,\"loc_y\":869.0,\"argb_color\":-8317209,\"uid\":12,\"team_id\":12,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":748.0,\"loc_y\":364.0,\"argb_color\":-1845167,\"uid\":13,\"team_id\":13,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":185.0,\"loc_y\":406.0,\"argb_color\":-2364104,\"uid\":14,\"team_id\":14,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":324.0,\"loc_y\":62.0,\"argb_color\":-13328918,\"uid\":5015,\"team_id\":5015,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":962.0,\"loc_y\":884.0,\"argb_color\":-12198033,\"uid\":16,\"team_id\":16,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":64.0,\"loc_y\":392.0,\"argb_color\":-15736963,\"uid\":5056,\"team_id\":5056,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":280.0,\"loc_y\":662.0,\"argb_color\":-14308540,\"uid\":18,\"team_id\":18,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":663.0,\"loc_y\":549.0,\"argb_color\":-4577953,\"uid\":19,\"team_id\":19,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":475.0,\"loc_y\":742.0,\"argb_color\":-10962961,\"uid\":20,\"team_id\":20,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":279.0,\"loc_y\":458.0,\"argb_color\":-7381092,\"uid\":21,\"team_id\":21,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":360.0,\"loc_y\":823.0,\"argb_color\":-2848730,\"uid\":5098,\"team_id\":5098,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":881.0,\"loc_y\":629.0,\"argb_color\":-6724733,\"uid\":23,\"team_id\":23,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":510.0,\"loc_y\":561.0,\"argb_color\":-6326708,\"uid\":24,\"team_id\":24,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":201.0,\"loc_y\":913.0,\"argb_color\":-7373343,\"uid\":5046,\"team_id\":5046,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":630.0,\"loc_y\":359.0,\"argb_color\":-3829330,\"uid\":26,\"team_id\":26,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":459.0,\"loc_y\":579.0,\"argb_color\":-9519582,\"uid\":5367,\"team_id\":5367,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":822.0,\"loc_y\":981.0,\"argb_color\":-16113991,\"uid\":28,\"team_id\":28,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":806.0,\"loc_y\":172.0,\"argb_color\":-10185411,\"uid\":29,\"team_id\":29,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":844.0,\"loc_y\":40.0,\"argb_color\":-11329073,\"uid\":5055,\"team_id\":5055,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":957.0,\"loc_y\":848.0,\"argb_color\":-7554557,\"uid\":31,\"team_id\":31,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":391.0,\"loc_y\":490.0,\"argb_color\":-9442438,\"uid\":32,\"team_id\":32,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":594.0,\"loc_y\":869.0,\"argb_color\":-10116250,\"uid\":33,\"team_id\":33,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":367.0,\"loc_y\":669.0,\"argb_color\":-6626356,\"uid\":34,\"team_id\":34,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":140.0,\"loc_y\":347.0,\"argb_color\":-8316193,\"uid\":35,\"team_id\":35,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":885.0,\"loc_y\":634.0,\"argb_color\":-15560314,\"uid\":36,\"team_id\":36,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
        //    world = new World(1000, 1000, 5571);
        //    foreach (Cube c in cubes)
        //    {
        //        world.AddCube(c);
        //    }
        //    // useful magnification: 2.4
        //    // useful offsetX: 1400
        //    // useful offsetY: 1200
        //}

        /// <summary>
        /// The paint method run to paint the game content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            //MessageBox.Show(this.world.foodCubes.Count + "");

            //    lock (this.world)
            //    {
            //        // calculate rough dimensions for player cubes
            //        IEnumerable<Cube> pc = world.playerCubes.Values;
            //        double pX = world.Width; // the farthest left edge of the player cubes
            //        double pY = world.Height; // the farthest top edge of the player cubes
            //        double farthestRight = 0, farthestBottom = 0; // helpers to find farthest right and bottom edges
            //        double size; // helper to save a cube's size
            //        foreach (Cube c in pc)
            //        {
            //            // find minimum X
            //            if (c.X < pX) pX = c.X;
            //            // find minimum Y
            //            if (c.Y < pY) pY = c.Y;
            //            size = c.Size;
            //            // find maximum farthestRight (X+Size)
            //            if (c.X + size > farthestRight) farthestRight = c.X + size;
            //            // find maximum farthestBottom (Y+Size)
            //            if (c.Y + size > farthestBottom) farthestBottom = c.Y + size;
            //        }
            //        double pSizeX = farthestRight - pX;
            //        double pSizeY = farthestBottom - pY;
            //        double pMaxSize = Math.Max(pSizeX, pSizeY); // the largest size the cubes take up (e.g. Max(farthestRight-farthestLeft,farthestBottom-farthestTop))

            //        // find values used for later calculations
            //        double percentPanel = 0.2; // the percentage the player's cube size should be in comparison to the panel size
            //        double panelMinSize = (GamePanel.Width > GamePanel.Height) ? GamePanel.Height : GamePanel.Width; // the minimum dimension size of the game panel

            //        // calculate the parameters that affect the scale and location of rendering (so player cubes are in center and magnified to a scale)
            //        double multiplier = (panelMinSize * percentPanel) / pMaxSize; // how many times the world is magnified; calculate in relation to player size and GamePanel minSize(lesser of width or height) (finalSize = p.Size*multiplier = GamePanel.MinSize*A%; multiplier = (GamePanel.MinSize*A%)/p.Size; A < 100%)
            //        double offsetX = (pX + pSizeX / 2) * multiplier - GamePanel.Width / 2; // how many units to subtract in the x-axis to center the player; calculate in relation to player x location, GamePanel width, and multiplier (OffsetX = p.CenterX*multiplier - GamePanel.Width/2)
            //        double offsetY = (pY + pSizeY / 2) * multiplier - GamePanel.Height / 2; // how many units to subtract in the y-axis to center the player; calculate in relation to player y location, GamePanel height, and multiplier (OffsetY = p.CenterY*multiplier - GamePanel.Height/2)

            //        // loop through world's cubes, render each
            //        SolidBrush brush = new SolidBrush(Color.White);
            //        SolidBrush black = new SolidBrush(Color.Black);
            //        int cSize;
            //        int x;
            //        int y;
            //        foreach (Cube c in world.foodCubes.Values)
            //        {
            //            // set brush color as given by cube
            //            brush.Color = Color.FromArgb(c.argb_color);
            //            // calculate size and location based on cube data in relation to multiplier and offsets
            //            cSize = (int)Math.Ceiling(c.Size * multiplier); // this will be the height and width of the square in rendering
            //            x = (int)Math.Ceiling(c.X * multiplier - offsetX); // this will be the x location in relation to the GamePanel's left edge
            //            y = (int)Math.Ceiling(c.Y * multiplier - offsetY); // this will be the y location in relation to the GamePanel's top edge
            //                                                               // render cube
            //            e.Graphics.FillRectangle(brush, new Rectangle(x, y, cSize, cSize));
            //            // if the cube is not food, render name
            //            if (!c.food)
            //            {
            //                // TODO how to use Graphics.DrawString() to center text?
            //                e.Graphics.DrawString(c.Name, GamePanel.Font, black, new PointF(x, y));
            //            }
            //        }
            //    }

            //    // send move request
            //   move();


            lock (this.world)
            {

                foreach (Cube current in this.world.foodCubes.Values)
                {
                    int num2 = Math.Max(current.Width, 5);
                    SolidBrush brush = new SolidBrush(Color.FromArgb(current.argb_color));
                    e.Graphics.FillRectangle(brush, (float)current.X, (float)current.Y, (float)num2, (float)num2);
                }
                foreach (Cube current2 in this.world.playerCubes.Values)
                {
                    Font font = new Font("Arial", 16f);
                    int num3 = current2.Width / 2;
                    SolidBrush brush = new SolidBrush(Color.FromArgb(current2.argb_color));
                    e.Graphics.FillRectangle(brush, (float)current2.X - (float)num3, (float)current2.Y - (float)num3, (float)current2.Width, (float)current2.Width);
                    brush = new SolidBrush(Color.Yellow);
                    SizeF sizeF = e.Graphics.MeasureString(current2.Name, font);
                    e.Graphics.DrawString(current2.Name, font, brush, (float)current2.X - sizeF.Width / 2f, (float)current2.Y - sizeF.Height / 2f);

                }
            }
            //this.Update();

            this.Invalidate();
        }

        private void GamePanel_Resize(object sender, EventArgs e)
        {
            // TODO temp way to test panel refresh in relation to resize
            // this.Invalidate();
            GamePanel.Invalidate();
         }


        private void move()
        {
            Cube cube;
            if (!this.world.playerCubes.TryGetValue(this.playerId, out cube))
                return;
            Network.Send(this.socket, "move," + this.PointToClient(Control.MousePosition).X + ", " + this.PointToClient(Control.MousePosition).Y + "\n");
        }
    }
}
