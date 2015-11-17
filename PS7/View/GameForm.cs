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
using System.Reflection;

namespace AgCubio
{
    public partial class GameForm : Form
    {
        // field declarations
        private World world;
        private System.Net.Sockets.Socket socket;
        private long playerId;      // todo: need to change to team
        private bool dead;
        private bool detectMouse;
        private PreservedState _ps;
        private System.Diagnostics.Stopwatch frameWatch = new System.Diagnostics.Stopwatch();
        private int frameCount;
        private System.Threading.Timer timer;

        public GameForm()
        {
            InitializeComponent();
            // a possible way to have GamePanel double buffer
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, GamePanel, new object[] { true });
        }

        /// <summary>
        /// The method run when the "Connect" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // Create and connect to socket
            Network.ConnectToServer(OnConnectedToServer, serverTextBox.Text);
        }

        private void OnConnectedToServer(PreservedState pso)
        {
            // check the connection
            if (pso.errorMsg != null)
            {
                this.Invoke((Action)(() =>
                {
                    MessageBox.Show("Cannot connect to the server at: " + serverTextBox.Text + "\nPlease, make sure your server is reachable.\n" + pso.errorMsg);
                }));
                pso.errorMsg = null;    //reset
                return;
            }

            // if all goes well
            // change panels displayed; the connect panel can be visible first, then when the connection works it is hidden and the game panel is shown (which will start painting)
            this.Invoke(
                (Action)(() =>
                {
                    this.ConnectionPanel.Hide();
                    this.GamePanel.Show();
                    frameWatch.Start();
                }));
            _ps = pso;
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


        /// <summary>
        /// This method is used to handle the first message back from the server, which will be the player's cube JSON.
        /// </summary>
        /// <param name="ps"></param>
        private void HandleFirstMessageFromServer(PreservedState ps)
        {
            // grab the { ... } part except the new line
            var playerStr = ps.receivedData.ToString().Substring(0, ps.receivedData.Length - 1);
            var cube = JsonConvert.DeserializeObject<Cube>(playerStr);
            world = new World(1000, 1000, cube.uId);
            lock (this.world)
            {
                world.AddCube(cube);
            }
            playerId = cube.uId;
            GamePanel.Invalidate();

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
            if (ps.errorMsg != null)
            {
                this.Invoke((Action)(() => { MessageBox.Show("Unexpected error while receiveing from server" + ps.errorMsg); }));
                ps.errorMsg = null;
            }
            //System.IO.File.AppendAllText("last.txt", ps.receivedData.ToString());
            //MessageBox.Show("receiving!");

            StringBuilder receivedData = ps.receivedData;
            if (receivedData.Length < 1)
                return;
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
                        if (!(line.StartsWith("{") && line.EndsWith("}")))
                        {
                            // if this broken line is not the last line:
                            if (i != jsonCubes.Length - 1)
                            {
                                success++; // there is no way to recover what was server saying, so, discard it as a success
                                i++;
                                continue;
                            }
                            else
                                break;
                        }

                        Cube cube = JsonConvert.DeserializeObject<Cube>(jsonCubes[i]);
                        success++;

                        //if (cube.food)
                        //{
                        //    // food is eaten remove it.
                        //    if (cube.Mass == 0.0)
                        //        this.world.foodCubes.Remove(cube.uId);
                        //    else
                        //        this.world.foodCubes[cube.uId] = cube;
                        //}
                        //else if (cube.Mass == 0.0)
                        //{
                        //    // if some player cube dies remove it.
                        //    this.world.playerCubes.Remove(cube.uId);
                        //    // indicate the death if the current player cube died!
                        //    if (cube.uId == this.playerId)
                        //        this.dead = true;
                        //}
                        //else
                        //{
                        //    // if it is not food, it must be a player cube!
                        //    this.world.playerCubes[cube.uId] = cube;
                        //}
                        world.AddCube(cube);
                        i++;
                    }

                }
                catch
                {
                }

                this.dead = world.PlayerDeath();
                // debug purpose
                //System.IO.File.WriteAllText("testfile.txt",                     "# of converted cubes = " + success + "\n" + receivedData.ToString());
                ps.receivedData.Clear();

                if (success > 0) GamePanel.Invalidate();

                // need to put back the last unparsed json string
                if ((success < jsonCubes.Length) && (jsonCubes.Length > 1))
                    receivedData.Append(jsonCubes[jsonCubes.Length - 1]);
            }
            if (this.dead)
            {
                MessageBox.Show("Player Died! Restart the Game.");
                this.dead = false;
                return;
            }

            //this.Invoke((Action)(() => { this.Update(); }));

            // Ready to receive more data!
            Network.WantMoreData(ps);

            // FOR_DEBUG
            // this.Invoke( (Action)(() => { move(); }));
            // System.Threading.Timer t = new System.Threading.Timer(GetData, null, 0, 400);
        }

        /// <summary>
        /// the percentage the player's cube size should be in comparison to the panel's smallest dimension.
        /// </summary>
        private static double percentPanel = 0.3;

        /// <summary>
        /// The paint method run to paint the game content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            //MessageBox.Show(this.world.foodCubes.Count + "");
            if (dead) return;
            if (this.world == null) return;
            lock (this.world)
            {
                // calculate rough dimensions for player cubes
                double pX, pY, pSizeX, pSizeY;
                world.GetPlayerCubesParameters(out pX, out pY, out pSizeX, out pSizeY);
                double pMaxSize = Math.Max(pSizeX, pSizeY); // the largest size the cubes take up (e.g. Max(farthestRight-farthestLeft,farthestBottom-farthestTop))

                // find values used for later calculations
                double panelMinSize = Math.Min(GamePanel.Width, GamePanel.Height); // the minimum dimension size of the game panel

                // calculate the parameters that affect the scale and location of rendering (so player cubes are in center and magnified to a scale)
                double multiplier = (panelMinSize * percentPanel) / pMaxSize; // how many times the world is magnified; calculate in relation to player size and GamePanel minSize(lesser of width or height) (finalSize = p.Size*multiplier = GamePanel.MinSize*A%; multiplier = (GamePanel.MinSize*A%)/p.Size; A < 100%)
                double offsetX = (pX + pSizeX / 2) * multiplier - GamePanel.Width / 2; // how many units to subtract in the x-axis to center the player; calculate in relation to player x location, GamePanel width, and multiplier (OffsetX = p.CenterX*multiplier - GamePanel.Width/2)
                double offsetY = (pY + pSizeY / 2) * multiplier - GamePanel.Height / 2; // how many units to subtract in the y-axis to center the player; calculate in relation to player y location, GamePanel height, and multiplier (OffsetY = p.CenterY*multiplier - GamePanel.Height/2)

                // loop through world's cubes, render each
                SolidBrush brush = new SolidBrush(Color.White);
                SolidBrush black = new SolidBrush(Color.Black);
                int cSize;
                int x;
                int y;
                int numberPlayers = 0;
                foreach (Cube c in world.GetCubes())
                {
                    // set brush color as given by cube
                    brush.Color = Color.FromArgb(c.argb_color);
                    // calculate size and location based on cube data in relation to multiplier and offsets
                    cSize = (int)Math.Ceiling(c.Size * multiplier); // this will be the height and width of the square in rendering
                    x = (int)Math.Ceiling(c.X * multiplier - offsetX); // this will be the x location in relation to the GamePanel's left edge
                    y = (int)Math.Ceiling(c.Y * multiplier - offsetY); // this will be the y location in relation to the GamePanel's top edge
                    // render cube
                    e.Graphics.FillRectangle(brush, new Rectangle(x, y, cSize, cSize));
                    // if the cube is not food, render name
                    if (!c.food)
                    {
                        // TODO how to use Graphics.DrawString() to center text?
                        e.Graphics.DrawString(c.Name, GamePanel.Font, black, new PointF(x, y));
                        numberPlayers++;
                    }
                }

                // display cube info
                playersLabel.Text = numberPlayers.ToString();
                foodsLabel.Text = (world.NumberCubes() - numberPlayers).ToString();
                double playerMass = 0;
                foreach (Cube c in world.GetPlayerCubes())
                {
                    playerMass += c.Mass;
                }
                massLabel.Text = playerMass.ToString();
            }


            //move();
            ++this.frameCount;

            //this.Update();

            TimeSpan elapsed = this.frameWatch.Elapsed;
            if (elapsed.Seconds > 0)
            {
                int fps = (this.frameCount / elapsed.Seconds);
                this.fpsLabel.Text = String.Empty + fps;
                this.fpsLabel.Invalidate();
            }
            // reset frameCount/watch around every 10 seconds
            if (elapsed.Seconds > 9)
            {
                this.frameWatch.Restart();
                this.frameCount = 0;
            }

            //lock (this.world)
            //{
            //    foreach (Cube current in this.world.foodCubes.Values)
            //    {
            //        int num2 = Math.Max(current.Width, 5);
            //        SolidBrush brush = new SolidBrush(Color.FromArgb(current.argb_color));
            //        e.Graphics.FillRectangle(brush, (float)current.X, (float)current.Y, (float)num2, (float)num2);
            //    }
            //    foreach (Cube current2 in this.world.playerCubes.Values)
            //    {
            //        Font font = new Font("Arial", 16f);
            //        int num3 = current2.Width / 2;
            //        SolidBrush brush = new SolidBrush(Color.FromArgb(current2.argb_color));
            //        e.Graphics.FillRectangle(brush, (float)current2.X - (float)num3, (float)current2.Y - (float)num3, (float)current2.Width, (float)current2.Width);
            //        brush = new SolidBrush(Color.Yellow);
            //        SizeF sizeF = e.Graphics.MeasureString(current2.Name, font);
            //        e.Graphics.DrawString(current2.Name, font, brush, (float)current2.X - sizeF.Width / 2f, (float)current2.Y - sizeF.Height / 2f);
            //    }
            //}
        }

        /// <summary>
        /// This method is run when the GamePanel changes size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Resize(object sender, EventArgs e)
        {
            // TODO temp way to test panel refresh in relation to resize
            // this.Invalidate();
            GamePanel.Invalidate();
            //this.Update();
        }

        /// <summary>
        /// This method is called to tell the server to move the player
        /// </summary>
        private void move()
        {
            if (this.world == null) return;
            double x, y, sizeX, sizeY;
            lock (this.world)
            {
                if (world == null || world.PlayerDeath()) return;
                world.GetPlayerCubesParameters(out x, out y, out sizeX, out sizeY);
            }
            // TODO need to figure out more accurate information on how to move in coordinates of the world, may need values from rendering
            Point point = GamePanel.PointToClient(Control.MousePosition);
            double delta = Math.Min(GamePanel.Width, GamePanel.Height) * 0.05; // this is used to create a dead zone if the cursor is near the center
            if ((point.X > GamePanel.Width / 2 - delta && point.X < GamePanel.Width / 2 + delta) && (point.Y > GamePanel.Height / 2 - delta && point.Y < GamePanel.Height / 2 + delta)) return; // the cursor was in the dead zone
            double offset = 5; // a way to offset the movement from the cube's current coordinates
            double moveX;
            if (point.X < GamePanel.Width / 2) moveX = x - offset;
            else if (point.X > GamePanel.Width / 2) moveX = x + offset;
            else moveX = x;
            double moveY;
            if (point.Y < GamePanel.Height / 2) moveY = x - offset;
            else if (point.Y > GamePanel.Height / 2) moveY = x + offset;
            else moveY = y;
            string msg = "(move, " + (int)moveX + ", " + (int)moveY + ")\n";
            Network.Send(this.socket, msg);
            // FOR_DEBUG
            serverNameLabel.Text = msg;
        }

        /// <summary>
        /// The method run when the mouse moves. This should handle moving the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            // if the mouse should be detected, continue
            if (detectMouse && !dead)
            {
                // call move
                move();
            }
        }

        /// <summary>
        /// The method run when the mouse enters the range of the GamePanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_MouseEnter(object sender, EventArgs e)
        {
            // set detectMouse to start allowing the player to move
            detectMouse = true;
        }

        /// <summary>
        /// The method run when the mouse leaves the range of the GamePanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_MouseLeave(object sender, EventArgs e)
        {
            // set detectMouse to stop allowing the player to move
            detectMouse = false;
        }
    }
}
