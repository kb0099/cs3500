using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class GameForm : Form
    {
        private System.Drawing.SolidBrush brush;

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
        }

        /// <summary>
        /// The paint method run to paint the game content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            // TODO a test on painting in panel
            Color color = Color.FromArgb(255, 0, 255);
            brush = new System.Drawing.SolidBrush(color);
            e.Graphics.FillRectangle(brush, new Rectangle(20, 50, 500, 480));
        }
    }
}
