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
using Model;

namespace View
{
    public partial class GameForm : Form
    {
        private World world;

        public GameForm()
        {
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

            TempWorldSetup();
        }

        /// <summary>
        /// Temporary way to setup world for testing.
        /// </summary>
        private void TempWorldSetup()
        {
            // temp way to setup world
            HashSet<Cube> cubes = new HashSet<Cube>();
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":926.0,\"loc_y\":682.0,\"argb_color\":-65536,\"uid\":5571,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":1000.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":920.0,\"loc_y\":673.0,\"argb_color\":-9834450,\"uid\":1571,\"food\":false,\"Name\":\"Bill Gates\",\"Mass\":500.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":116.0,\"loc_y\":350.0,\"argb_color\":-8243084,\"uid\":5002,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":193.0,\"loc_y\":523.0,\"argb_color\":-4759773,\"uid\":5075,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":267.0,\"loc_y\":55.0,\"argb_color\":-7502725,\"uid\":2,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":998.0,\"loc_y\":580.0,\"argb_color\":-16481514,\"uid\":3,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":69.0,\"loc_y\":895.0,\"argb_color\":-5905052,\"uid\":4,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":387.0,\"loc_y\":506.0,\"argb_color\":-2505812,\"uid\":5,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":687.0,\"loc_y\":152.0,\"argb_color\":-9834450,\"uid\":6,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":395.0,\"loc_y\":561.0,\"argb_color\":-2210515,\"uid\":7,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":585.0,\"loc_y\":222.0,\"argb_color\":-11930702,\"uid\":8,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":49.0,\"loc_y\":614.0,\"argb_color\":-4232190,\"uid\":9,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":809.0,\"loc_y\":452.0,\"argb_color\":-9234755,\"uid\":10,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":286.0,\"loc_y\":666.0,\"argb_color\":-11083980,\"uid\":11,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":252.0,\"loc_y\":869.0,\"argb_color\":-8317209,\"uid\":12,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":748.0,\"loc_y\":364.0,\"argb_color\":-1845167,\"uid\":13,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":185.0,\"loc_y\":406.0,\"argb_color\":-2364104,\"uid\":14,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":324.0,\"loc_y\":62.0,\"argb_color\":-13328918,\"uid\":5015,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":962.0,\"loc_y\":884.0,\"argb_color\":-12198033,\"uid\":16,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":64.0,\"loc_y\":392.0,\"argb_color\":-15736963,\"uid\":5056,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":280.0,\"loc_y\":662.0,\"argb_color\":-14308540,\"uid\":18,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":663.0,\"loc_y\":549.0,\"argb_color\":-4577953,\"uid\":19,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":475.0,\"loc_y\":742.0,\"argb_color\":-10962961,\"uid\":20,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":279.0,\"loc_y\":458.0,\"argb_color\":-7381092,\"uid\":21,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":360.0,\"loc_y\":823.0,\"argb_color\":-2848730,\"uid\":5098,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":881.0,\"loc_y\":629.0,\"argb_color\":-6724733,\"uid\":23,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":510.0,\"loc_y\":561.0,\"argb_color\":-6326708,\"uid\":24,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":201.0,\"loc_y\":913.0,\"argb_color\":-7373343,\"uid\":5046,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":630.0,\"loc_y\":359.0,\"argb_color\":-3829330,\"uid\":26,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":459.0,\"loc_y\":579.0,\"argb_color\":-9519582,\"uid\":5367,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":822.0,\"loc_y\":981.0,\"argb_color\":-16113991,\"uid\":28,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":806.0,\"loc_y\":172.0,\"argb_color\":-10185411,\"uid\":29,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":844.0,\"loc_y\":40.0,\"argb_color\":-11329073,\"uid\":5055,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":957.0,\"loc_y\":848.0,\"argb_color\":-7554557,\"uid\":31,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":391.0,\"loc_y\":490.0,\"argb_color\":-9442438,\"uid\":32,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":594.0,\"loc_y\":869.0,\"argb_color\":-10116250,\"uid\":33,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":367.0,\"loc_y\":669.0,\"argb_color\":-6626356,\"uid\":34,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":140.0,\"loc_y\":347.0,\"argb_color\":-8316193,\"uid\":35,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            cubes.Add(JsonConvert.DeserializeObject<Cube>("{\"loc_x\":885.0,\"loc_y\":634.0,\"argb_color\":-15560314,\"uid\":36,\"food\":true,\"Name\":\"\",\"Mass\":1.0}"));
            world = new World(1000, 1000, 5571);
            world.SetCubes(cubes);
            // useful magnification: 2.4
            // useful offsetX: 1400
            // useful offsetY: 1200
        }

        /// <summary>
        /// The paint method run to paint the game content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            // calculate the parameters that affect the scale and location of rendering (so player is in center and magnified to a scale)
            Cube p = world.GetPlayerCube();
            double percentPanel = 0.4; // the percentage the player's cube size should be in comparison to the panel size
            double panelMinSize = (GamePanel.Width > GamePanel.Height) ? GamePanel.Height : GamePanel.Width; // the minimum dimension size of the game panel
            double multiplier = (panelMinSize*percentPanel)/p.Size; // how many times the world is magnified; calculate in relation to player size and GamePanel minSize(lesser of width or height) (finalSize = p.Size*multiplier = GamePanel.MinSize*A%; multiplier = (GamePanel.MinSize*A%)/p.Size; A < 100%)
            double offsetX = p.CenterX * multiplier - GamePanel.Width / 2; // how many units to subtract in the x-axis to center the player; calculate in relation to player x location, GamePanel width, and multiplier (OffsetX = p.CenterX*multiplier - GamePanel.Width/2)
            double offsetY = p.CenterY * multiplier - GamePanel.Height / 2; // how many units to subtract in the y-axis to center the player; calculate in relation to player y location, GamePanel height, and multiplier (OffsetY = p.CenterY*multiplier - GamePanel.Height/2)

            // loop through world's cubes, render each
            SolidBrush brush = new SolidBrush(Color.White);
            SolidBrush black = new SolidBrush(Color.Black);
            int size;
            int x;
            int y;
            foreach (Cube c in world.GetCubes())
            {
                // set brush color as given by cube
                brush.Color = Color.FromArgb(c.argb_color);
                // calculate size and location based on cube data in relation to multiplier and offsets
                size = (int)Math.Ceiling(c.Size * multiplier); // this will be the height and width of the square in rendering
                x = (int)Math.Ceiling(c.X * multiplier - offsetX); // this will be the x location in relation to the GamePanel's left edge
                y = (int)Math.Ceiling(c.Y * multiplier - offsetY); // this will be the y location in relation to the GamePanel's top edge
                // render cube
                e.Graphics.FillRectangle(brush, new Rectangle(x, y, size, size));
                // if the cube is not food, render name
                if (!c.food)
                {
                    // TODO how to use Graphics.DrawString() to center text?
                    e.Graphics.DrawString(c.Name, GamePanel.Font, black, new PointF(x, y));
                }
            }
        }

        private void GamePanel_Resize(object sender, EventArgs e)
        {
            // TODO temp way to test panel refresh in relation to resize
            GamePanel.Refresh();
        }
    }
}
