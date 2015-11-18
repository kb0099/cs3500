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
        private long playerId;
        private bool dead;
        private bool detectMouse;
        private PreservedState _ps;
        private System.Diagnostics.Stopwatch frameWatch = new System.Diagnostics.Stopwatch();
        private int frameCount;

        public GameForm()
        {
            InitializeComponent();
            // a way to have GamePanel double buffer (DoubleBuffered is a private method of Panel)
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
                    this.serverNameLabel.Text = this.serverTextBox.Text;
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
            // TODO DEBUG
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
                        world.AddCube(cube);
                        i++;
                    }

                }
                catch
                {
                }

                this.dead = world.PlayerDeath();
                // TODO debug purpose
                //System.IO.File.WriteAllText("testfile.txt",                     "# of converted cubes = " + success + "\n" + receivedData.ToString());
                ps.receivedData.Clear();

                if (success > 0) GamePanel.Invalidate();

                // need to put back the last unparsed json string
                if ((success < jsonCubes.Length) && (jsonCubes.Length > 1))
                    receivedData.Append(jsonCubes[jsonCubes.Length - 1]);
            }
            if (this.dead)
            {
                // an approach would be to have the message box ask the user to restart or end game; 
                // if clicked yes, hide GamePanel and show ConnectionPanel (which can start the connection process again); otherwise, close the program
                switch(MessageBox.Show("Your cube Died! Restart the Game?", "Cube Died", MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        socket.Close();
                        _ps = null;
                        this.dead = false;
                        Network.ConnectToServer(OnConnectedToServer, serverTextBox.Text);
                        return;
                    default:
                        this.ConnectionPanel.Show();
                        this.GamePanel.Hide();
                        frameWatch.Stop();
                        return;
                }
            }

            // Ready to receive more data!
            Network.WantMoreData(ps);

            // TODO FOR_DEBUG
            // this.Invoke( (Action)(() => { move(); }));
            // System.Threading.Timer t = new System.Threading.Timer(GetData, null, 0, 400);
        }

        /// <summary>
        /// the percentage the player's cube size should be in comparison to the panel's smallest dimension.
        /// </summary>
        private static double percentPanel = .2;

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
                double pLeft, pTop, pSizeX, pSizeY;
                world.GetPlayerCubesParameters(out pLeft, out pTop, out pSizeX, out pSizeY);
                double pMaxSize = Math.Max(pSizeX, pSizeY); // the largest size the cubes take up (e.g. Max(farthestRight-farthestLeft,farthestBottom-farthestTop))

                // find values used for later calculations
                double panelMinSize = Math.Min(GamePanel.Width, GamePanel.Height); // the minimum dimension size of the game panel

                // calculate the parameters that affect the scale and location of rendering (so player cubes are in center and magnified to a scale)
                double multiplier = (panelMinSize * percentPanel) / pMaxSize; // how many times the world is magnified; calculate in relation to player size and GamePanel minSize(lesser of width or height) (finalSize = p.Size*multiplier = GamePanel.MinSize*A%; multiplier = (GamePanel.MinSize*A%)/p.Size; A < 100%)
                double offsetX = (pLeft + pSizeX / 2) * multiplier - GamePanel.Width / 2; // how many units to subtract in the x-axis to center the player; calculate in relation to player x location, GamePanel width, and multiplier (OffsetX = p.CenterX*multiplier - GamePanel.Width/2)
                double offsetY = (pTop + pSizeY / 2) * multiplier - GamePanel.Height / 2; // how many units to subtract in the y-axis to center the player; calculate in relation to player y location, GamePanel height, and multiplier (OffsetY = p.CenterY*multiplier - GamePanel.Height/2)

                // loop through world's cubes, render each
                SolidBrush brush = new SolidBrush(Color.White);
                SolidBrush fontBrush = new SolidBrush(Color.Black);
                int cSize;
                int left;
                int top;
                int numberPlayers = 0;
                foreach (Cube c in world.GetCubes())
                {
                    // set brush color as given by cube
                    brush.Color = Color.FromArgb(c.argb_color);
                    // calculate size and location based on cube data in relation to multiplier and offsets
                    cSize = (int)Math.Ceiling(c.Size * multiplier); // this will be the height and width of the square in rendering
                    left = (int)Math.Ceiling(c.LeftEdge * multiplier - offsetX); // this will be the left location in relation to the GamePanel's left edge
                    top = (int)Math.Ceiling(c.TopEdge * multiplier - offsetY); // this will be the top location in relation to the GamePanel's top edge
                    // render cube
                    e.Graphics.FillRectangle(brush, new Rectangle(left, top, cSize, cSize));
                    // if the cube is not food, render name
                    if (!c.food)
                    {
                        // Set brush color to be contrasted enough to cube color to be readable
                        if (brush.Color.GetBrightness() > 0.6) fontBrush.Color = Color.Black;
                        else fontBrush.Color = Color.LightGray;
                        // draw the text
                        // TODO how to use Graphics.DrawString() to center text? are we fine if the text isn't centered?
                        // e.Graphics.DrawString(c.Name, GamePanel.Font, fontBrush, new PointF(left, top));
                        Rectangle rect1 = new Rectangle(left, top, cSize, cSize);

                        using (Font font1 = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point))
                        {

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            // Draw the text and the surrounding rectangle.
                            e.Graphics.DrawString(c.Name, font1, Brushes.Blue, rect1, stringFormat);
                            e.Graphics.DrawRectangle(Pens.Black, rect1);
                        }
                        numberPlayers++;
                    }
                    playersLabel.Text = numberPlayers.ToString();
                }

                // display cube info
                foodsLabel.Text = (world.NumberCubes() - numberPlayers).ToString();
                double playerMass = 0;
                foreach (Cube c in world.GetPlayerCubes())
                {
                    playerMass += c.Mass;
                }
                massLabel.Text = playerMass.ToString();
            }


            if (detectMouse) move();
            ++this.frameCount;

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
        }

        /// <summary>
        /// This method is run when the GamePanel changes size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Resize(object sender, EventArgs e)
        {
            // ensure panel repaints when window resizes
            GamePanel.Invalidate();
        }

        /// <summary>
        /// This method is called to tell the server to move the player
        /// </summary>
        private void move()
        {
            if (this.world == null) return;
            double left, top, sizeX, sizeY;
            lock (this.world)
            {
                if (world == null || world.PlayerDeath()) return;
                world.GetPlayerCubesParameters(out left, out top, out sizeX, out sizeY);
            }
            Point point = GamePanel.PointToClient(Control.MousePosition);
            double multiplier = (Math.Min(GamePanel.Width, GamePanel.Height) * percentPanel) / Math.Max(sizeX, sizeY); // how many times the world is magnified; calculate in relation to player size and GamePanel minSize(lesser of width or height) (finalSize = p.Size*multiplier = GamePanel.MinSize*A%; multiplier = (GamePanel.MinSize*A%)/p.Size; A < 100%)
            double offsetX = (left + sizeX / 2) * multiplier - GamePanel.Width / 2; // how many units to subtract in the x-axis to center the player; calculate in relation to player x location, GamePanel width, and multiplier (OffsetX = p.CenterX*multiplier - GamePanel.Width/2)
            double offsetY = (top + sizeY / 2) * multiplier - GamePanel.Height / 2; // how many units to subtract in the y-axis to center the player; calculate in relation to player y location, GamePanel height, and multiplier (OffsetY = p.CenterY*multiplier - GamePanel.Height/2)
            double moveX = (point.X + offsetX) / multiplier;
            double moveY = (point.Y + offsetY) / multiplier;
            string msg = "(move, " + (int)moveX + ", " + (int)moveY + ")\n";
            Network.Send(this.socket, msg);
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

        private void GameForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                Network.Send(socket, "(split, " + this.PointToClient(Control.MousePosition).X + ", " + this.PointToClient(Control.MousePosition).Y + ")\n");
            }
        }
    }
}
