using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Media;
using System.IO;
using WMPLib;
using System.Text.RegularExpressions;

namespace memory
{
    public partial class Form1 : Form
    {
        public List<Bitmap> Fronts = new List<Bitmap>(); // the image the card should have when clicked
        public List<Bitmap> Images = new List<Bitmap>();
        public List<Bitmap> RealImage = new List<Bitmap>()
        { Properties.Resources.ball,
        Properties.Resources.boat,
        Properties.Resources.bunny,
        Properties.Resources.car,
        Properties.Resources.chesnut,
        Properties.Resources.cow,
        Properties.Resources.dogOrSmth,
        Properties.Resources.dolphin,
        Properties.Resources.flower,
        Properties.Resources.leaf,
        Properties.Resources.lion,
        Properties.Resources.luck,
        Properties.Resources.moon,
        Properties.Resources.pussy,
        Properties.Resources.rainstopper,
        Properties.Resources.rat,
        Properties.Resources.rock,
        Properties.Resources.snail,
        Properties.Resources.sonic,
        Properties.Resources.strawberry,
        Properties.Resources.sun,
        //21
        };


        public List<string> SplitScores = new List<string>();
        static Random Rand = new Random();
        public DateTime localDate = DateTime.Now;
        static Random rnd = new Random();
        public int id; // id of a button so button10 becomes 10
        public int ClickCount; //if it  is the first or second image that you click
        public object FirstButton; // the first button that was clicked
        public bool disableclick = true; // disables all buttons, used when game is pauzed or when you are clicking buttons
        public int Player1Score;
        public int Player2Score;
        public bool GameRunning = false;
        public bool GameFinished = false;
        bool cheating = false;
        public int Flip; //the amount of cards that have been flipped
        public int turns; // the amount of turns
        public bool Player1;  //set to true if it is the turn for player one to play
        public string Mode;
        public string Highscores;
        int count;
        SoundPlayer sound = new SoundPlayer(Properties.Resources.flopp);
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
            AddImages();
            AddImages();
            Extra(false);
            bs.DataSource = listView1;
            Highscore(false);
            ClearBoard(true);
            for (int a = 0; a < 42; a++)
            {
                string objec = "button" + a;
                Button butt = (Button)Controls.Find(objec, true)[0];
                butt.Visible = false;
            }
        }
        public void SoundPlayer(Stream zound) 
        {
            sound.Stream = zound;
            sound.Play();
        }
        public void AddImages() //adds all the images of the RealImage list to the Images list twice, because there are 2 of each image
        {
            count = RealImage.Count;
            for (int i = 0; i < count; i++)
            {
                Images.Add(RealImage[i]);

            }
        }
        string ProcessText(string s, string delim, string replaceWith)
        {
            return Regex.Replace(s, @"" + delim + "+", replaceWith);
        }
        public void Highscore(bool save) 
        {
            if (Directory.Exists("C:\\HighScores\\"))
            {
                if (File.Exists("C:\\HighScores\\Score.txt")) //code goes here
                {
                    if (save) 
                    {
                        txtbxName.Text = ProcessText(txtbxName.Text, ",","");
                        txtbxName.Text = ProcessText(txtbxName.Text, "-", "");
                        Highscores = Highscores + txtbxName.Text+"-"+CardsFlipped.Text + "-"+ lblTime.Text + "-"+Mode+",";
                        File.WriteAllText("C:\\HighScores\\Score.txt", Highscores);
                        txtbxName.Text = "";
                    }
                    if (!save) //if you want to retrive
                    {
                        listView1.Items.Clear();
                        Highscores = File.ReadAllText("C:\\HighScores\\Score.txt");
                        for (int q = 0; q < Highscores.Split(',').Count(); q++) 
                        {
                            string SubItem = Highscores.Split(',')[q];
                            if (SubItem.Contains('-')) 
                            {
                                ListViewItem item = new ListViewItem();
                                string bolz = SubItem.Split('-')[0];
                                string bolzz = SubItem.Split('-')[1];
                                string bolzzz = SubItem.Split('-')[2];
                                string bolzzzz = SubItem.Split('-')[3];
                                item.Text = bolz;
                                item.SubItems.Add(bolzz);
                                item.SubItems.Add(bolzzz);
                                item.SubItems.Add(bolzzzz);
                                listView1.Items.Add(item);
                                //item.Remove();
                            }
                        }
                        
                    }
                }
                else //if file does not exist make the file
                {
                    File.WriteAllText("C:\\HighScores\\Score.txt", "");
                    Highscore(save);
                }
            }
            else //if filepath does not exist? then make it
            {
                Directory.CreateDirectory("C:\\HighScores\\");
                Highscore(save);
            }
        }
        private async void button_Click(object sender, EventArgs e)
        {
            if (!disableclick)
            {
                switch (ClickCount)
                {
                    case 0: //First button to be clicked
                        SoundPlayer(Properties.Resources.CardFlip);
                        id = int.Parse((sender as Button).Name.Replace("button", ""));
                        (sender as Button).BackgroundImage = Fronts[id]; //gets the corresponding image
                        (sender as Button).Enabled = false;
                        ClickCount = 1;
                        Flip++;
                        FirstButton = sender;
                        break;
                    case 1: // second button to be clicked then copare the images
                        disableclick = true;
                        id = int.Parse((sender as Button).Name.Replace("button", ""));
                        (sender as Button).BackgroundImage = Fronts[id];
                        (sender as Button).Enabled = false;
                        ClickCount = 0;
                        Flip++;
                        if ((sender as Button).BackgroundImage == (FirstButton as Button).BackgroundImage) // if the 2 images match
                        {
                            SoundPlayer(Properties.Resources.correct);
                            (sender as Button).Text = "";
                            (FirstButton as Button).Text = "";
                            if (Player1) //increase the score of the player that got it correct
                            {
                                Player1Score++;
                            }
                            else
                            {
                                Player2Score++;
                            }
                            await Task.Delay(300);
                            (FirstButton as Button).Visible = false;
                            (sender as Button).Visible = false;
                            await Task.Delay(200);
                            disableclick = false; // enables buttons again
                        }
                        else //if wrong, then wait and reset
                        {
                            SoundPlayer(Properties.Resources.Incorrect);
                            await Task.Delay(500);
                            disableclick = false;
                            (FirstButton as Button).Enabled = true;
                            (sender as Button).Enabled = true;
                            if (!cheating) 
                            {
                                (FirstButton as Button).BackgroundImage = Properties.Resources.empty;
                                (sender as Button).BackgroundImage = Properties.Resources.empty;
                            }
                            
                        }
                        if (chkbxMultiplayer.Checked == true) //if you are playing multiplayer
                        {
                            Player1 = !Player1;
                            turns++;
                        }
                        disableclick = false;
                        break;
                }
            }
        }
        private void Update_Tick(object sender, EventArgs e)
        {
            DateTime NowDate = DateTime.Now;
            TimeSpan delta = NowDate.Subtract(localDate);
            lblTime.Text = delta.ToString(@"mm\:ss");
            CardsFlipped.Text = Flip.ToString();
            if (Player1) //highlight the current player
            {
                Player2Text.BackColor = System.Drawing.Color.Transparent;
                Player1Text.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                Player1Text.BackColor = System.Drawing.Color.Transparent;
                Player2Text.BackColor = System.Drawing.Color.Green;
            }
            P1Score.Text = Player1Score.ToString();
            P2Score.Text = Player2Score.ToString();
            lblTurns.Text = turns.ToString();
            if (Player1Score + Player2Score == 21 && GameRunning) 
            {
                GameFinished = true;
                StartStop();
            }
            if (cheating) 
            {
                Mode = "Cheat";
            }
            lblMode.Text = Mode;
        }
        public async void ClearBoard(bool vis) //get every button and reset it(pretty obvious ngl)
        {
            for (int a = 0; a < 42; a++) 
            {
                await Task.Delay(2);
                string objec = "button" + a;
                Button butt = (Button)Controls.Find(objec, true)[0];
                butt.BackgroundImage = Properties.Resources.empty;
                butt.Text = "";
                butt.Enabled = true;
                if (vis) 
                {
                    butt.Visible = true;
                }
            }
        }
        public void Extra(bool Open) 
        {
            if (Open == true) 
            {
                this.MaximumSize = new Size(610, 470);
                this.MinimumSize = new Size(610, 470);
                       this.Size = new Size(610, 470);
                bttnExtra.Text = "Options <";
            }
            if (Open == false)
            {
                this.MaximumSize = new Size(400, 470);
                this.MinimumSize = new Size(400, 470);
                       this.Size = new Size(400, 470);
                bttnExtra.Text = "Options >";
            }
        }
        private async void ShowCards() 
        {
            for (int a = 0; a < 42; a++)
            {
                await Task.Delay(1);
                string objec = "button" + a;
                Button butt = (Button)Controls.Find(objec, true)[0];
                butt.BackgroundImage = Fronts[a];
            }
        }
        private async void Start() //resets all the variables and starts the game (also quite obvious)
        {
            Extra(false);
            lblScoreSubmitText.Visible = false;
            txtbxName.Visible = false;
            lblName.Visible = false;
            bttnSubmit.Visible = false;
            cheating = false;
            lblWinner.Visible = false;
            GameRunning = true;
            GameFinished = false;
            ClearBoard(true);
            Images.Clear();
            AddImages();
            AddImages();
            Fronts.Clear();
            turns = 0;
            Flip = 0;
            for (int i = 0; i < 42; i++)
            {

                int r = Rand.Next(Images.Count);

                Fronts.Add(Images[r]); //binds a random image to this button
                Images.RemoveAt(r);
            }
            Player1Score = 0;
            Player2Score = 0;
            ClickCount = 0;
            localDate = DateTime.Now;
            Player1 = true;
            await Task.Delay(100);
            if (chkbxEasy.Checked) // if easy mode is enabled
            {
                ShowCards();
                await Task.Delay(3000);
                ClearBoard(true);
            }
            disableclick = false;
            bttnHint.Text = "Hints: 5";
            Mode = "Normal";
            if (chkbxEasy.Checked) 
            {
                Mode = "Easy";
            }
        }
        private void bttnStartStop_Click(object sender, EventArgs e)
        {
            StartStop();
        }
        private void StartStop() 
        {
            if (bttnStartStop.Text == "Start")
            {
                chkbxMultiplayer.Visible = false;
                chkbxEasy.Visible = false;
                lblEasyModeText.Visible = false;
                tmrUpdate.Enabled = true;
                bttnStartStop.Text = "Stop";
                bttnStartStop.BackColor = System.Drawing.Color.Crimson;
                GameRunning = true;
                Start();
            }
            else
            {
                disableclick = true;
                ShowCards();
                chkbxMultiplayer.Visible = true;
                chkbxEasy.Visible = true;
                lblEasyModeText.Visible = true;
                tmrUpdate.Enabled = false;
                bttnStartStop.Text = "Start";
                bttnStartStop.BackColor = System.Drawing.Color.MediumSeaGreen;
                GameRunning = false;
            }
            if (GameFinished) 
            {
                SoundPlayer sound = new SoundPlayer(Properties.Resources.Finish);
                sound.Load();
                sound.Play();
                if (chkbxMultiplayer.Checked == true) // if you finished the game via multiplayer
                {
                    lblWinner.Text = "Player 1 has won!";
                    if (Player2Score > Player1Score)
                    {
                        lblWinner.Text = "Player 2 has won!";
                    }
                    lblWinner.Visible = true;
                }
                else //if you finished in single player
                {
                    lblWinner.Text = "You have won!";
                    lblWinner.Visible = true;
                    bttnSubmit.Visible = true;
                    lblName.Visible = true;
                    lblScoreSubmitText.Visible = true;
                    txtbxName.Visible = true;
                    Extra(true);
                }
            }
        }
        private void chkbxMultiplayerChanged(object sender, EventArgs e)
        {
            if (chkbxMultiplayer.Checked == true)
            {
                P2Score.Visible = true;
                Player2Text.Visible = true;
                lblTurns.Visible = true;
                lblTurnsText.Visible = true;
            }
            else
            {
                P2Score.Visible = false;
                Player2Text.Visible = false;
                lblTurns.Visible = false;
                lblTurnsText.Visible = false;
            }
        }

        public async void chet() 
        {
            cheating = true;
            for (int a = 0; a < 42; a++)
            {
                await Task.Delay(5);
                string objec = "button" + a;
                Button butt = (Button)Controls.Find(objec, true)[0];
                butt.BackgroundImage = Fronts[a];
                butt.Text = "X";
                butt.Enabled = true;
            }
        }

        private void lblTime_Click(object sender, EventArgs e)
        {
            if (lblTime.Text.Contains("5")) 
            {
                chet();
            }
        }

        private void bttnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtbxName.Text)) 
            {
                bttnSubmit.Visible = false;
                txtbxName.Visible = false;
                lblName.Visible = false;
                Highscore(true);
                Highscore(false);
            }
        }

        private async void bttnHint_Click(object sender, EventArgs e)
        {
            int hints = int.Parse(bttnHint.Text.Replace("Hints: ", ""));
            if (hints > 0 && ClickCount == 0 && GameRunning == true)
            {
                disableclick = true;
                hints--;
                bttnHint.Text = "Hints: " + hints;
                ShowCards();
                await Task.Delay(2000);
                ClearBoard(false);
                disableclick = false;
            }
        }

        private void chkbxEasy_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbxEasy.Checked)
            {
                bttnHint.Visible = true;
                lblMode.Text = "Easy";
            }
            else 
            {
                bttnHint.Visible = false;
                lblMode.Text = "Normal";
            }
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }

        private void chkbxTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkbxTop.Checked;
        }

        private void bttnExtra_Click(object sender, EventArgs e)
        {
            switch (bttnExtra.Text)
            {
                case ("Options <"):
                    Extra(false);
                    bttnExtra.Text = "Options >";
                    break;
                case ("Options >"):
                    Extra(true);
                    bttnExtra.Text = "Options <";
                    break;
            }
        }
    }
} 
// haha under 505 lines of code ( ͡° ͜ʖ ͡°)