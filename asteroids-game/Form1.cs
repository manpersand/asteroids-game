using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Keys = System.Windows.Forms.Keys;
using Point = System.Drawing.Point;

namespace asteroids_game
{
    /// <summary>
    /// A serializable custom data structure to store player names and scores
    /// </summary>
    [Serializable]
    public struct HighScore { public string Name { get; set; } public int Score { get; set; } }

    /// <summary>
    /// An enumeration representin different states of the game, to manage the inputs accordingly.
    /// </summary>
    public enum GameState { Running, Paused, StartScreen, InfoScreen, NamePrompt }

    public partial class Form1 : Form
    {
        const int MAXLIVES = 3;  //number of lives available at start of a new game
        const int LIFESCORE = 10000;  //a new life is awarded every time user adds this number to their score
        const string HIGHSCOREFILE = "HighScores.bin";  //name of the high scores file
        const int MAXSPAWNTIME = 5000;  //maximum time in ms between spawns of the large asteroids

        GameState _gameState;  //keeps track of which state the game is in currently
        int _currentScore = 0; //current score in the game
        int _bonusScore = LIFESCORE;  //keeps track of the next score number where a life will be awarded
        Stopwatch _autoFireSW = new Stopwatch();  //stop watch used to repeat fire, if input is held 
        Ship _ship; //a reference to the spaceship currently in use

        //sound player for extra ship sound
        SoundPlayer _extraShip = new SoundPlayer(Properties.Resources.extraShip);

        List<Asteroid> _asteroids = new List<Asteroid>();  //collection of all asteroids
        Stack<Ship> _stackOfShips = new Stack<Ship>();  //a queue of ships
        Dictionary<string, string> _dicControlsKB = new Dictionary<string, string>();  //contains all the controls instructions for keyboard
        Dictionary<string, string> _dicControlsXbox = new Dictionary<string, string>();  //contains all the controls instructions for xbox controller

        //these bools keep track of the previous states for some buttons
        //in order to perform their respective action only once
        bool _prevFireState = false;
        bool _prevHSState = false;
        bool _prevOKState = false;
        bool _prevUpState = false;
        bool _prevDownState = false;
        bool _prevCancelState = false;

        List<HighScore> _highScores = new List<HighScore>(); //keeps track of all the usernames and scores
        BindingSource _bs = new BindingSource(); //binding source for the data grid view to display high scores

        Timer _tmrRender = new Timer(); //main render timer for the game rendering and collision checking
        Timer _tmrAddAsteroids = new Timer();  //timer for adding new asteroids as game progresses
        Timer _tmrControls = new Timer();  //timer to handle the inputs according to the game state

        //collection of buttons used for menu navigation using the Input class
        List<Button> _menuButtons = new List<Button>();
        int _buttonIndex = 0;

        //double buffering objects
        BufferedGraphicsContext _bgc = new BufferedGraphicsContext();
        BufferedGraphics _bg;

        public Form1()
        {
            InitializeComponent();
            //controls setup
            Text = "Asteroids by Manpreet Sandhu";
            _tmrRender.Interval = 10;
            _tmrControls.Interval = 10;
            _tmrAddAsteroids.Interval = MAXSPAWNTIME;  //starting spawn interval for asteroids
            _dgvHighScores.DataSource = _bs;
            _bs.DataSource = _highScores;
            _dgvHighScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _tmrControls.Start(); //controls timer starts as soon as the program is run
            _gameState = GameState.StartScreen; //game starts at start screen

            //load extra ship sound
            _extraShip.LoadAsync();

            //events
            _tmrRender.Tick += tmrRender_Tick;
            _tmrAddAsteroids.Tick += tmrAdd_Tick;
            _tmrControls.Tick += tmrControls_Tick;
            KeyDown += Input.KeyDownHandler;
            KeyUp += Input.KeyUpHandler;
            Shown += Form1_Shown;
            Load += Form1_Load;
            _btnHighScores.GotFocus += MenuButton_GotFocus;
            _btnNewGame.GotFocus += MenuButton_GotFocus;
            _btnControls.GotFocus += MenuButton_GotFocus;
            _btnHome.GotFocus += MenuButton_GotFocus;
            _btnHighScores.LostFocus += MenuButton_LostFocus; ;
            _btnNewGame.LostFocus += MenuButton_LostFocus;
            _btnControls.LostFocus += MenuButton_LostFocus;
            _btnHome.LostFocus += MenuButton_LostFocus;
            _btnControls.MouseHover += MenuButton_Hover;
            _btnNewGame.MouseHover += MenuButton_Hover;
            _btnHighScores.MouseHover += MenuButton_Hover;
            _btnHome.MouseHover += MenuButton_Hover;
            _btnNewGame.Click += NewGame_Click;
            _btnHighScores.Click += HighScores_Click;
            _btnControls.Click += Controls_Click;
            _btnOK.Click += OK_Click;
            _btnCancel.Click += Cancel_Click;
            _btnHome.Click += Home_Click;
            _txtPlayerName.KeyDown += PlayerName_KeyDown;

            //populate the keyboard controls collection
            _dicControlsKB["↑ or W"] = "Thrust";
            _dicControlsKB["← or A"] = "Rotate Counter Clockwise";
            _dicControlsKB["→ or D"] = "Rotate Clockwise";
            _dicControlsKB["SPACE"] = "Fire";
            _dicControlsKB["SHIFT"] = "Hyper Space";
            _dicControlsKB["P"] = "Pause Game";
            _dicControlsKB["ENTER"] = "Resume Game";
            _dicControlsKB["ESC"] = "Cancel Game";

            //populate the xbox controls collection
            _dicControlsXbox["LT"] = "Thrust";
            _dicControlsXbox["RT"] = "Fire";
            _dicControlsXbox["LS Left or DPad Left"] = "Rotate Counter Clockwise";
            _dicControlsXbox["LS Right or DPad Right"] = "Rotate Clockwise";
            _dicControlsXbox["Y"] = "Hyper Space";
            _dicControlsXbox["START"] = "Pause Game";
            _dicControlsXbox["A"] = "Resume Game/OK";
            _dicControlsXbox["B"] = "Cancel Game";

            //add all menu buttons to a collection to be used for navigation
            _menuButtons.Add(_btnNewGame);
            _menuButtons.Add(_btnHighScores);
            _menuButtons.Add(_btnControls);
        }

        /// <summary>
        /// Loads the previous high scores from a .bin file (if exists) into a collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //clear variables and list view before new load
            _highScores.Clear();

            //load the previous scores into the high scores collection
            if (File.Exists(HIGHSCOREFILE))
            {
                //try-catch for the file-read operation
                try
                {
                    //create a file stream and binary formatter
                    FileStream fs = new FileStream(HIGHSCOREFILE, FileMode.Open, FileAccess.Read);
                    BinaryFormatter bf = new BinaryFormatter();

                    //deserialize the file into the list
                    _highScores = (List<HighScore>)bf.Deserialize(fs);
                    fs.Close(); //close the file stream
                }
                catch (Exception ex)
                {
                    //show error message in the form title bar
                    Text = "Unable to load high scores file : " + ex.Message;
                }
            }
        }

        /// <summary>
        /// Form Shown event, just calls a method that loads the start screen of the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            StartScreen();
        }

        /// <summary>
        /// Tick event for the Controls timer manages all the control according to the inputs from the Input class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrControls_Tick(object sender, EventArgs e)
        {
            //perform actions according to game state and inputs
            switch (_gameState)
            {
                case GameState.Running:
                    if (Input.Pause)
                    {
                        StopGame();
                        _gameState = GameState.Paused;
                        //obtain the graphics reference the form again
                        Graphics gr = CreateGraphics();
                        //show the pause message according to input device
                        string message = (Input.GamepadConnected) ? "Game paused\nPress 'A' to Resume\n Press 'B' to cancel the game" : "Game paused\nPress 'ENTER' to Resume\n Press 'ESC' to cancel the game";
                        TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                        TextRenderer.DrawText(gr, message, new Font("Impact", 20, FontStyle.Bold),
                         ClientRectangle, Color.WhiteSmoke, flags);
                    }

                    //rotate the ship according to the input from the class
                    if (Input.Left)
                        _ship.Rotation = Rotation.CounterClockwise;
                    else if (Input.Right)
                        _ship.Rotation = Rotation.Clockwise;
                    else
                        _ship.Rotation = Rotation.None;

                    //turn on or off the thruster according to the input
                    if (Input.Thrust)
                        _ship.ThrustOn();
                    else
                        _ship.ThrustOff();

                    //only need to fire one bullet at a time
                    //so keep track of the previous fire state 
                    //and only fire when changes from false to true
                    if (Input.Fire && !_prevFireState)
                    {
                        _autoFireSW.Start();
                        _ship.Fire();
                        _prevFireState = Input.Fire;
                    }  //if the fire input state is true for longer than 200 ms then fire again
                    else if (Input.Fire && _autoFireSW.ElapsedMilliseconds > 200)
                    {
                        _ship.Fire();
                        _autoFireSW.Restart();
                    } //fire input state went from true to false, stop firing and reset stopwatch
                    else if (_prevFireState && !Input.Fire)
                    {
                        _prevFireState = Input.Fire;
                        _autoFireSW.Reset();
                    }

                    //same solution as above
                    if (Input.HyperSpace && !_prevHSState)
                    {
                        _ship.HyperSpace(ClientRectangle.Size);
                        _prevHSState = Input.HyperSpace;
                    }
                    else
                        _prevHSState = Input.HyperSpace;
                    break;

                case GameState.Paused:
                    if (Input.OK)
                    {
                        //restart game and change game state
                        StartGame();
                        _gameState = GameState.Running;
                    }

                    if (Input.Cancel)
                        GameOver("Game Cancelled");
                    break;

                case GameState.StartScreen:
                    //on up/down inputs decrement/increment the selected button index
                    //while keeping it between 0 and count of buttons
                    if (Input.GamepadConnected)
                    {
                        //use previous state of inputs in order to 
                        //perform the respective action once per transition from false to true
                        if (Input.Up && !_prevUpState)
                        {
                            _buttonIndex = (--_buttonIndex + _menuButtons.Count) % _menuButtons.Count;
                            _prevUpState = Input.Up;
                        }
                        else
                            _prevUpState = Input.Up;

                        if (Input.Down && !_prevDownState)
                        {
                            _buttonIndex = ++_buttonIndex % _menuButtons.Count;
                            _prevDownState = Input.Down;
                        }
                        else
                            _prevDownState = Input.Down;

                        //give the focus to the button at current index
                        _menuButtons[_buttonIndex].Focus();

                        if (Input.OK && !_prevOKState)
                        {
                            _menuButtons[_buttonIndex].PerformClick();
                            _prevOKState = Input.OK;
                        }
                        else
                            _prevOKState = Input.OK;
                    }
                    break;

                case GameState.InfoScreen:  //info screen is highscores and controls screen
                    if (Input.OK && !_prevOKState)
                    {
                        _btnHome.PerformClick();
                        _prevOKState = Input.OK;
                    }
                    else
                        _prevOKState = Input.OK;

                    //there is only one button on this 
                    //screen so give it the focus on either up or down input
                    if (Input.Up || Input.Down)
                        _btnHome.Focus();
                    break;

                case GameState.NamePrompt:
                    //operate the ok and cancel buttons on the 
                    //name prompt screen
                    //previous input state used to act only on
                    //transition from false to true
                    if (Input.OK && !_prevOKState)
                    {
                        _btnOK.PerformClick();
                        _prevOKState = Input.OK;
                    }
                    else
                        _prevOKState = Input.OK;
                    if (Input.Cancel && !_prevCancelState)
                    {
                        _btnCancel.PerformClick();
                        _prevCancelState = Input.Cancel;
                    }
                    else
                        _prevCancelState = Input.Cancel;
                    break;
            }
        }

        /// <summary>
        /// Tick event for the asteroids adding timer, the interval starts at 5s and 
        /// after adding each asteroid the interval for the timer decreases by 
        /// 100 ms until it reaches 1s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrAdd_Tick(object sender, EventArgs e)
        {
            //add a new asteroid every tick of the add timer
            _asteroids.Add(new LargeAsteroid(ClientRectangle));

            //reduce the add timer interval to increase difficulty as game progresses
            _tmrAddAsteroids.Interval -= (_tmrAddAsteroids.Interval > 1000) ? 100 : 0;
        }

        /// <summary>
        /// Tick event for the Render timer, it renders everything to the screen and checks 
        /// for collisions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrRender_Tick(object sender, EventArgs e)
        {
            Graphics gr = CreateGraphics(); //obtain reference to the form's graphics object
            //game-over check
            if (!_ship.IsMarkedForRemoval)
            {
                //virtual surface to draw on (back-buffer)
                _bg = _bgc.Allocate(gr, ClientRectangle);
                _bg.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                //clear the back buffer
                _bg.Graphics.Clear(Color.Black);

                //move the ship, bullets and the asteroids
                _ship.Tick(ClientRectangle.Size);
                _ship.Bullets.ForEach(b => b.Tick(ClientRectangle.Size));
                _asteroids.ForEach(a => a.Tick(ClientRectangle.Size));

                //check for intersection between ship and asteroids
                foreach (Asteroid a in _asteroids.ToList())
                {
                    //no need to check for intersection if the large asteroid is still fading in
                    //as a fading asteroid cannot destroy the ship
                    if (a is LargeAsteroid && (a as LargeAsteroid).IsFading)
                        continue; //skip to the next iteration

                    //create new regions from the graphics path of the ship and each asteroid
                    Region shipReg = new Region(_ship.GetPath());
                    Region astrReg = new Region(a.GetPath());
                    //intersect the two regions
                    shipReg.Intersect(astrReg);
                    //check the intersection 
                    if (!shipReg.IsEmpty(gr))
                    {
                        //if region not empty, kill the ship and asteroid
                        _ship.IsMarkedForRemoval = true;
                        _asteroids.AddRange(a.Die());
                        //send full vibration signal
                        Input.Vibrate(1);
                    }
                }

                //check for intersection between bullets and asteroids
                foreach (Bullet b in _ship.Bullets)
                {
                    foreach (Asteroid a in _asteroids.ToList())
                    {
                        //create new regions from the graphics path of each bullet and each asteroid
                        Region bulletReg = new Region(b.GetPath());
                        Region astrReg = new Region(a.GetPath());

                        //intersect both regions to check each bullet agains every asteroid
                        bulletReg.Intersect(astrReg);

                        if (!bulletReg.IsEmpty(gr))
                        {
                            //if region not empty then kill the bullet and asteroid
                            b.IsMarkedForRemoval = true;
                            _asteroids.AddRange(a.Die());

                            //award the score according to the type of 
                            //asteroid destroyed
                            //also send the vibration signal according to the 
                            //size of the asteroid destriyed
                            switch (a.GetType().Name)
                            {
                                case "LargeAsteroid":
                                    _currentScore += 100;
                                    Input.Vibrate(1);
                                    break;
                                case "MediumAsteroid":
                                    _currentScore += 200;
                                    Input.Vibrate((float)0.75);
                                    break;
                                case "SmallAsteroid":
                                    _currentScore += 300;
                                    Input.Vibrate((float)0.50);
                                    break;
                            }
                        }
                    }
                }

                //remvoe all asteroids and bullets marked for removal
                _asteroids.RemoveAll(a => a.IsMarkedForRemoval);
                _ship.Bullets.RemoveAll(b => b.IsMarkedForRemoval);

                //render the ship, bullets and asteroids
                _ship.Render(_bg.Graphics);
                _ship.Bullets.ForEach(b => b.Render(_bg.Graphics));
                _asteroids.ForEach(a => a.Render(_bg.Graphics));

                //show lives and current score
                _ship.ShowLives(_bg.Graphics, _stackOfShips.Count);
                TextRenderer.DrawText(_bg.Graphics, _currentScore.ToString("D6"), new Font("Impact", 25),
                                    new Point(ClientRectangle.X, ClientRectangle.Y), Color.Snow);

                //award a new life every time user adds the required score
                if (_currentScore >= _bonusScore)
                {
                    _extraShip.Play();  //play the sound for extra ship
                    _stackOfShips.Push(new Ship(ClientSize));
                    _bonusScore += LIFESCORE;  //increment the bonus score check to next level
                }

                //flip the back buffer to the front
                _bg.Render();
            }
            else if (_stackOfShips.Count > 0)  //see if there is still any ships left
            {
                _ship = _stackOfShips.Pop(); //grab the new ship from the stack
                //increase the add timer interval again to allow the player to get back in the groove when ship dies
                //also ensuring it does not go higher than the maximum time between spawns
                _tmrAddAsteroids.Interval = (MAXSPAWNTIME - _tmrAddAsteroids.Interval > 1000) ? _tmrAddAsteroids.Interval + 1000 : MAXSPAWNTIME;
            }
            else
            {
                //no ships left, game-over!
                StopGame();
                GameOver("Game Over");
            }
        }

        /// <summary>
        /// Changes the focus to the button that the mouse hovers over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_Hover(object sender, EventArgs e)
        {
            //change the curretly in focus button to a different color
            Button b = (Button)sender;
            b.Focus();
        }

        /// <summary>
        /// Changes the back color and fore color of the button that gets focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_GotFocus(object sender, EventArgs e)
        {
            //change the curretly in focus button to a different color
            Button b = (Button)sender;
            b.ForeColor = Color.FromArgb(233, 23, 73);
            b.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Changes the fore color back to original when a menu button looses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_LostFocus(object sender, EventArgs e)
        {
            //change the color of the button that lost focus
            Button b = (Button)sender;
            b.ForeColor = Color.FromArgb(249, 234, 25);
        }

        /// <summary>
        /// Shows the high scores if user selects cancel on the name prompt screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            ShowHighScores();
        }

        /// <summary>
        /// Performs a click on the OK button if the user presses enter while the text box has focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _btnOK.PerformClick();  //perform a click in the OK button
                e.SuppressKeyPress = true;  //suppress the key press from being passed to the control
            }
        }

        /// <summary>
        /// Takes the game back to the start screen from the High score display or the controls display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, EventArgs e)
        {
            StartScreen();
        }

        /// <summary>
        /// Saves the user's score and name entered in the textbox, and then shows high score list.
        /// Also, fills in a default name if user clicks OK without entering anything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, EventArgs e)
        {
            //save the current score using the player name entered, and then show the high score list
            //if the player name is empty when OK is pressed then replace the textbox text with a default name
            _txtPlayerName.Text = (_txtPlayerName.Text.Trim() == "") ? "PlayerUnknown" : _txtPlayerName.Text;
            SaveScore(_txtPlayerName.Text);
            ShowHighScores(_txtPlayerName.Text);
        }

        /// <summary>
        /// Main menu button, Shows the controls for the game according to the input device.
        /// If xbox controller is connected it shows xbox controls otherwise keyboard controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Controls_Click(object sender, EventArgs e)
        {
            //change game state to info screen
            _gameState = GameState.InfoScreen;

            //hide the background image
            BackgroundImage = null;

            //hide menu buttons and show home button
            HideMenu();
            _btnHome.Visible = true;
            _btnHome.Enabled = true;
            _bs.DataSource = (Input.GamepadConnected) ? _dicControlsXbox : _dicControlsKB;  //show the controls according to the controller type
            _dgvHighScores.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;// .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvHighScores.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _dgvHighScores.Visible = true;
            _lblControls.Show(); //show the controls header label

            //set the focus to the home button
            _btnHome.Focus();
        }

        /// <summary>
        /// Shows  the high score list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighScores_Click(object sender, EventArgs e)
        {
            //hide the background image
            BackgroundImage = null;
            ShowHighScores();
        }

        /// <summary>
        /// Start a new game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGame_Click(object sender, EventArgs e)
        {
            //hide the buttons panel and disable all buttons
            HideMenu();

            Input.Reset(); //reset all inputs for a new game

            //hide the background image
            BackgroundImage = null;

            //reset score and spawn timer interval
            _currentScore = 0;
            _tmrAddAsteroids.Interval = MAXSPAWNTIME;

            //add maximum number of ships allowed per game, to the stack
            for (int i = 0; i < MAXLIVES; i++)
            {
                //initialize each ship using the form's client rectangle
                _stackOfShips.Push(new Ship(ClientSize));
            }

            //grab the first ship for the game 
            _ship = _stackOfShips.Pop();

            //add 3 starting asteroids
            _asteroids.Add(new LargeAsteroid(ClientRectangle));
            _asteroids.Add(new LargeAsteroid(ClientRectangle));
            _asteroids.Add(new LargeAsteroid(ClientRectangle));

            //start the game and change the game state
            StartGame();
            _gameState = GameState.Running;

            //give the focus to the form Activate() or this.Focus() did not work!
            _btnNewGame.GetContainerControl().ActivateControl(this);
        }

        /// <summary>
        /// This method stops both the Render and the Asteroids Add timers.
        /// </summary>
        private void StopGame()
        {
            _tmrAddAsteroids.Stop();
            _tmrRender.Stop();
        }

        /// <summary>
        /// This method starts both the Render and the Asteroids Add timers.
        /// </summary>
        private void StartGame()
        {
            _tmrAddAsteroids.Start();
            _tmrRender.Start();
        }

        /// <summary>
        /// Hides all main menu controls.
        /// </summary>
        private void HideMenu()
        {
            //hide the buttons panel and disable all buttons
            _pnlMenu.Visible = false;
            this.Focus();
            foreach (Control c in _pnlMenu.Controls)
            {
                if (c is Button)
                {
                    (c as Button).Enabled = false;
                    (c as Button).Visible = false;
                }
            }
        }

        /// <summary>
        /// Loads the start screen for the game, while hiding the controls that are not required.
        /// </summary>
        private void StartScreen()
        {
            //change game state to start screen
            _gameState = GameState.StartScreen;

            //show the buttons panel and enable all menu buttons
            //in case returning from a different menu
            _pnlMenu.Visible = true;
            foreach (Control c in _pnlMenu.Controls)
            {
                if (c is Button)
                {
                    (c as Button).Enabled = true;
                    (c as Button).Visible = true;
                }
            }

            //give the focus to the new game button
            _btnNewGame.Focus();

            //hide the menu option header labels
            _lblHighScore.Hide();
            _lblControls.Hide();

            //hide data grid view, name text box , ok and cancel button and home button
            _dgvHighScores.Visible = false;
            _txtPlayerName.Visible = false;
            _btnOK.Visible = false;
            _btnCancel.Visible = false;
            _btnHome.Visible = false;
            _btnHome.Enabled = false;

            //show the background image for start screen
            BackgroundImage = Properties.Resources.asteroids_bg;

            //key preview true for gameplay as 
            //all keys need to go to the form key down and key up
            KeyPreview = true;
        }

        /// <summary>
        /// Performs the steps after a game is over or cancelled, prompts user to enter their name
        /// if the current score needs to be saved otherwise just shows previous high scores.
        /// </summary>
        /// <param name="message">Message to be displayed to show if game was over or cancelled</param>
        private void GameOver(string message)
        {
            //clear the game running flag, asteroids collection
            //and the pressed keys collection
            _asteroids.Clear();
            _stackOfShips.Clear(); //clear he ship stack as the may have been cancelled

            //obtain the graphics reference the form again
            Graphics gr = CreateGraphics();
            //show the game over message before clearing everything
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(gr, message, new Font("Impact", 60),
             ClientRectangle, Color.Maroon, flags);

            //wait \, then clear screen
            System.Threading.Thread.Sleep(1000); 
            gr.Clear(Color.Black);

            //if the current score is greater than 0 and the lowest score in the list
            //or if there are less than 10 high scores saved,
            //then display the name input text box and OK button
            if (_currentScore > 0 && (_highScores.Count < 10 || _highScores.Last().Score < _currentScore))
            {
                KeyPreview = false; //to allow the textbox to receive keys
                _txtPlayerName.Visible = true;
                _btnOK.Visible = true;
                _btnCancel.Visible = true;
                TextRenderer.DrawText(gr, "Enter Your Name", new Font("Impact", 20),ClientRectangle, Color.Red, flags);
                _txtPlayerName.Clear();
                _txtPlayerName.Focus();
                _gameState = GameState.NamePrompt; //set the game state to name prompt
            }
            else //otherwise just show previous high scores
                ShowHighScores();
        }

        /// <summary>
        /// Adds the current score and passed in player name to the high scores collection, sorts the collection in descending order
        /// of scores, then removes the last entry if the count is greater than 10 and then finally serializes the collection
        /// into the .bin file.
        /// </summary>
        /// <param name="playerName">Name of the player to be added to the list</param>
        private void SaveScore(string playerName)
        {
            //add the current player name and score the list
            _highScores.Add(new HighScore() { Name = playerName, Score = _currentScore });

            //sort the list by scores highest to lowest
            //_highScores = _highScores.OrderByDescending(hs => hs.Score).ToList();
            _highScores.Sort((hs1, hs2) => hs2.Score.CompareTo(hs1.Score));

            //only keep 10 scores
            if (_highScores.Count > 10)
                _highScores.Remove(_highScores.Last());

            //save the list to a file
            //try-catch block for save operation
            try
            {
                //create a file stream and binary formatter
                FileStream fs = new FileStream(HIGHSCOREFILE, FileMode.Create, FileAccess.Write);
                BinaryFormatter bf = new BinaryFormatter();

                //serialize the list into the file
                bf.Serialize(fs, _highScores);
                fs.Close();
            }
            catch (Exception ex)
            {
                //show error message in the form title bar
                Text = "Unable to save high scores to the file : " + ex.Message;
            }
        }

        /// <summary>
        /// Shows the high scores in the data grid view, also highlights the last player, if a user just finished a game.
        /// </summary>
        /// <param name="lastPlayer">name of the user that last entered high scores default is ""</param>
        private void ShowHighScores(string lastPlayer = "")
        {
            //change game state to info screen
            _gameState = GameState.InfoScreen;

            //ensure that menu buttons are hidden (in case the high scores button is clicked directly)
            HideMenu();
            //hide textbox and buttons
            _txtPlayerName.Visible = false;
            _btnOK.Visible = false;
            _btnCancel.Visible = false;
            _lblHighScore.Show();

            //populate the data grid view with high scores data
            _bs.DataSource = null;
            _bs.DataSource = _highScores;
            _dgvHighScores.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _dgvHighScores.Visible = true;

            //highlight the high scores of the last player if exists
            if (_currentScore > 0 && lastPlayer != "")
            {
                foreach (DataGridViewRow row in _dgvHighScores.Rows)
                {
                    if (row.Cells[0].Value.ToString() == lastPlayer && (int)row.Cells[1].Value == _currentScore)
                    {
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(233, 23, 73);
                        row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(233, 23, 73);
                    }
                }
            }

            _btnHome.Visible = true;
            _btnHome.Enabled = true;
            //set the focus to the home button
            _btnHome.Focus();
        }
    }
}

