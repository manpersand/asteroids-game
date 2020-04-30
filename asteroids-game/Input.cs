using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = System.Windows.Forms.Keys;

namespace asteroids_game
{
    /// <summary>
    /// Represents input layer of the game.
    /// </summary>
    public static class Input
    {
        static private GamePadState _gps; //current state of the cuntroller
        static private float _vibLevel; //vibration level 

        //each property represents a corresponding input
        static public bool Right { get; private set; }
        static public bool Left { get; private set; }
        static public bool Up { get; private set; }
        static public bool Down { get; private set; }
        static public bool OK { get; private set; }
        static public bool Thrust { get; private set; }
        static public bool Fire { get; private set; }
        static public bool HyperSpace { get; private set; }
        static public bool Pause { get; private set; }
        static public bool Cancel { get; private set; }

        //used in the main game layer to do some UI enhancements depending on the controller connected
        static public bool GamepadConnected { get; private set; }

        /// <summary>
        /// Starts a new thread to poll the controller.
        /// </summary>
        static Input()
        {
            System.Threading.Thread t = new System.Threading.Thread(PollController);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// KeyDown event handler method that accepts the keyboard events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void KeyDownHandler(object sender, KeyEventArgs e)
        {
            //ignore keyboard if xbox controller is connected
            if (!_gps.IsConnected)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                    case Keys.Left:
                        Left = true;
                        break;
                    case Keys.D:
                    case Keys.Right:
                        Right = true;
                        break;
                    case Keys.W:
                    case Keys.Up:
                        Thrust = true;
                        Up = true;
                        break;
                    case Keys.ShiftKey:
                        HyperSpace = true;
                        break;
                    case Keys.Space:
                        Fire = true;
                        break;
                    case Keys.P:
                        Pause = true;
                        break;
                    case Keys.Enter:
                        OK = true;
                        break;
                    case Keys.Escape:
                        Cancel = true;
                        break;
                }
            }
            //suppress the key press to avoid the windows beep sound
            e.SuppressKeyPress = true;  
        }

        /// <summary>
        /// KeyUp event handler method that accepts the keyboard events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            //ignore keyboard if xbox controller is connected
            if (!_gps.IsConnected)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                    case Keys.Left:
                        Left = false;
                        break;
                    case Keys.D:
                    case Keys.Right:
                        Right = false;
                        break;
                    case Keys.W:
                    case Keys.Up:
                        Thrust = false;
                        Up = false;
                        break;
                    case Keys.ShiftKey:
                        HyperSpace = false;
                        break;
                    case Keys.Space:
                        Fire = false;
                        break;
                    case Keys.P:
                        Pause = false;
                        break;
                    case Keys.Escape:
                        Cancel = false;
                        break;
                    case Keys.Enter:
                        OK = false;
                        break;
                }
            }
            //suppress the key press to avoid the windows beep sound
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// A thread method that polls the Xbox Controller for its state.
        /// </summary>
        static private void PollController()
        {
            while (true)
            {
                //get the first controller connected to the system
                _gps = GamePad.GetState(PlayerIndex.One);
                GamepadConnected = _gps.IsConnected;

                if (_gps.IsConnected)  //only change inputs if controller is connected
                {
                    //set the directional inputs according to left thumbstick or dpad
                    Right = _gps.ThumbSticks.Left.X > 0 || _gps.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    Left = _gps.ThumbSticks.Left.X < 0 || _gps.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    Up = _gps.ThumbSticks.Left.Y > 0 || _gps.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    Down = _gps.ThumbSticks.Left.Y < 0 || _gps.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

                    //set the hyperspace, thrust and fire inputs according to Y button and triggers
                    HyperSpace = _gps.IsButtonDown(Buttons.Y);
                    Thrust = _gps.IsButtonDown(Buttons.LeftTrigger);
                    Fire = _gps.IsButtonDown(Buttons.RightTrigger);

                    //set pause, OK and Cancel inputs according to Start, A and B buttons respectively
                    Pause = _gps.IsButtonDown(Buttons.Start);
                    OK = _gps.IsButtonDown(Buttons.A);
                    Cancel = _gps.IsButtonDown(Buttons.B);
                }
            }
        }

        /// <summary>
        /// Resets all inputs for a new game.
        /// </summary>
        static public void Reset()
        {
            Right = false;
            Left = false;
            Up = false;
            Down = false;
            OK = false;
            Thrust = false;
            Fire = false;
            HyperSpace = false;
            Pause = false;
            Cancel = false;
        }

        /// <summary>
        /// If xbox controller is connected, this method sends vibration feedback to the controlller.
        /// It calls a asynchronous method to turn on vibration and turn it off after a delay.
        /// </summary>
        /// <param name="level">The level of the vibration between 0 and 1</param>
        static async public void Vibrate(float level)
        {
            _vibLevel = level;
            if (_gps.IsConnected)
                await Task.Run(new Action(VibrateAsync));
        }

        /// <summary>
        /// This method is used by the Vibrate() method to send vibration to the controller asynchronously.
        /// </summary>
        static private void VibrateAsync()
        {
            //set a vibration level and clear it after a delay
            GamePad.SetVibration(PlayerIndex.One, _vibLevel, _vibLevel);
            System.Threading.Thread.Sleep(1000);
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }
    }
}
