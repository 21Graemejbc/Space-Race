using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Media;

/// <summary>
/// GRAEME COOK
/// MARCH 23, 2021
/// ICS3U
/// 
/// A two player game where the players race to the top of the screen. First to three points wins!
/// </summary>

namespace SpaceRacce
{
    public partial class Form1 : Form
    {
        #region init
        //Players
        const int playerSpeed = 5;
        const int playerSide = 16;

        //Player1
        const int player1X = 250;
        int player1Y;
        int player1Score = 0;

        //Player2
        const int player2X = 450;
        int player2Y;
        int player2Score = 0;

        //Rocks
        List<int> rightMoversXList = new List<int>(new int[] { });
        List<int> rightMoversYList = new List<int>(new int[] { });

        List<int> leftMoversXList = new List<int>(new int[] { });
        List<int> leftMoversYList = new List<int>(new int[] { });

        const int rockSpeed = 10;

        const int rockX = 10;
        const int rockY = 5;

        //General
        string gameState = "startup";
        const int safeZone = 450;
        string winner = "";
        const int spawnPercent = 14;

        //Key presses
        bool wDown = false;
        bool sDown = false;
        bool upDown = false;
        bool downDown = false;

        //Pens & Brushes
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //Random Number Generator
        Random randGen = new Random();

        //Soundplayers
        SoundPlayer collisionPlayer = new SoundPlayer();
        SoundPlayer winPlayer = new SoundPlayer();
        SoundPlayer gameOverPlayer = new SoundPlayer();
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region keys
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.Space:
                    if (gameState == "startup" || gameState == "over") { startGame(); }
                    break;
                case Keys.Escape:
                    if (gameState == "startup" || gameState == "over") { Application.Exit(); }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.Up:
                    upDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
            }
        }
        #endregion

        private void fps_Tick(object sender, EventArgs e)
        {
            #region movement
            //Player One
            if (wDown == true && player1Y > 0) { player1Y -= playerSpeed; }
            if (sDown == true && player1Y < this.Height - playerSide) { player1Y += playerSpeed; }

            //Player Two
            if (upDown == true && player2Y > 0) { player2Y -= playerSpeed; }
            if (downDown == true && player2Y < this.Height - playerSide) { player2Y += playerSpeed; }

            //Left-moving rocks
            for (int i = 0; i < leftMoversXList.Count(); i++) { leftMoversXList[i] -= rockSpeed; }

            //Right-moving rocks
            for (int i = 0; i < rightMoversXList.Count(); i++) { rightMoversXList[i] += rockSpeed; }
            #endregion

            #region rockSpawner
            int leftVal = randGen.Next(1, 101);
            int rightVal = randGen.Next(1, 101);

            if (rightVal < spawnPercent)
            {
                rightMoversXList.Add(10);
                rightMoversYList.Add(randGen.Next(0, safeZone));
            }

            if (leftVal < spawnPercent)
            {
                leftMoversXList.Add(690);
                leftMoversYList.Add(randGen.Next(0, safeZone));
            }
            #endregion

            #region rockKiller
            for (int i = 0; i < leftMoversXList.Count(); i++)
            {
                if (leftMoversXList[i] < 0)
                {
                    leftMoversXList.RemoveAt(i);
                    leftMoversYList.RemoveAt(i);
                }
            }

            for (int i = 0; i < rightMoversXList.Count(); i++)
            {
                if (rightMoversXList[i] > 700)
                {
                    rightMoversXList.RemoveAt(i);
                    rightMoversYList.RemoveAt(i);
                }
            }
            #endregion

            #region scoring
            if (player1Y <= 0)
            {
                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";

                player1Y = 475;

                winPlayer.Play();
            }

            if (player2Y <= 0)
            {
                player2Score++;
                player2ScoreLabel.Text = $"{player2Score}";

                player2Y = 475;

                winPlayer.Play();
            }
            #endregion

            #region winCondition
            if (player1Score == 3)
            {
                fps.Enabled = false;
                winner = "Player 1";
                gameState = "over";
                gameOverPlayer.Play();
            }
            else if (player2Score == 3)
            {
                fps.Enabled = false;
                winner = "Player 2";
                gameState = "over";
                gameOverPlayer.Play();
            }
            #endregion

            #region collisions
            Rectangle player1Box = new Rectangle(player1X, player1Y, playerSide, playerSide);
            Rectangle player2Box = new Rectangle(player2X, player2Y, playerSide, playerSide);

            for (int i = 0; i < rightMoversXList.Count(); i++)
            {
                Rectangle rightRockBox = new Rectangle(rightMoversXList[i], rightMoversYList[i], rockX, rockY);

                if (player1Box.IntersectsWith(rightRockBox)) { player1Y = 475; collisionPlayer.Play(); }
                else if (player2Box.IntersectsWith(rightRockBox)) { player2Y = 475; collisionPlayer.Play(); }
            }

            for (int i = 0; i < leftMoversXList.Count(); i++)
            {
                Rectangle leftRockBox = new Rectangle(leftMoversXList[i], leftMoversYList[i], rockX, rockY);

                if (player1Box.IntersectsWith(leftRockBox)) { player1Y = 475; collisionPlayer.Play(); }
                else if (player2Box.IntersectsWith(leftRockBox)) { player2Y = 475; collisionPlayer.Play(); }
            }
            #endregion

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            #region screenDrawing
            if (gameState == "startup")
            {
                titleLabel.Text = "SPACE RACE";
                subTitleLabel.Text = "Start - Space | Exit - Escape";
            }
            else if (gameState == "running")
            {
                //Draw player 1
                e.Graphics.FillRectangle(whiteBrush, player1X, player1Y, playerSide, playerSide);

                //Draw player 2
                e.Graphics.FillRectangle(whiteBrush, player2X, player2Y, playerSide, playerSide);

                //Draw left-moving rocks
                for (int i = 0; i < leftMoversXList.Count(); i++) { e.Graphics.FillRectangle(whiteBrush, leftMoversXList[i], leftMoversYList[i], rockX, rockY); }

                //Draw right-moving rocks
                for (int i = 0; i < rightMoversXList.Count(); i++) { e.Graphics.FillRectangle(whiteBrush, rightMoversXList[i], rightMoversYList[i], rockX, rockY); }
            }
            else if (gameState == "over")
            {
                titleLabel.Text = $"{winner} won!!";
                subTitleLabel.Text = "Play again - Space | Exit - Escape";
            }
            #endregion
        }

        public void startGame()
        {
            #region gameInit
            titleLabel.Text = null;
            subTitleLabel.Text = null;

            fps.Enabled = true;
            gameState = "running";

            leftMoversXList.Clear();
            leftMoversYList.Clear();

            rightMoversXList.Clear();
            rightMoversYList.Clear();

            player1Y = 475;
            player2Y = 475;

            player1Score = 0;
            player2Score = 0;

            player1ScoreLabel.Text = "0";
            player2ScoreLabel.Text = "0";

            collisionPlayer = new SoundPlayer(SpaceRac.Properties.Resources.Collision);
            winPlayer       = new SoundPlayer(SpaceRac.Properties.Resources.Win);
            gameOverPlayer = new SoundPlayer(SpaceRac.Properties.Resources.Game_Over);

            #endregion
        }
    }
}
