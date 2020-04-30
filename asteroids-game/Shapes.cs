using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace asteroids_game
{
    /// <summary>
    /// enumerations used for rotation direction of the ship.
    /// </summary>
    public enum Rotation { None, Clockwise, CounterClockwise };

    /// <summary>
    /// An abstract class used as a base for all shapes, as all shapes have a lot of similar begaviours. 
    /// </summary>
    abstract class BaseShape
    {
        protected static Random _rng = new Random();  //static random object shared by all derived instances

        protected float _radius;  //radius size for the shape
        protected float _rotation; //rotation of the shape
        protected float _rotationIncrement;  //rotation increment for the shape (controls speed of rotation)
        protected Color _color;  //color of the shape
        protected PointF _location;  //location of the shape
        protected float _xVel;  //velocity increment in the x-direction
        protected float _yVel;  //velocity increment in the y-direction
        protected GraphicsPath _model;  //graphics path model of the shape
        public bool IsMarkedForRemoval { get; set; }  //auto property to flag if a shape is to be removed

        public BaseShape()
        {
            //default ctor, nothing to do here really, as the system already initializes all memebers 
            //to their default values, which is what is needed at this point
        }

        /// <summary>
        /// Produces the transformed graphics path of the cloned model
        /// of each shape.
        /// </summary>
        /// <returns>A fully transformed graphics path</returns>
        public GraphicsPath GetPath()
        {
            //create a new transform matrix and add the translation and rotation
            Matrix transforms = new Matrix();
            transforms.Translate(_location.X, _location.Y);
            transforms.Rotate(_rotation);
            //clone the Bullet model and perform the transforms to it
            GraphicsPath gp = (GraphicsPath)_model.Clone();
            gp.Transform(transforms);
            return gp;
        }
    
        /// <summary>
        /// Fills the graphics path returned by GetPath() with a provided color on the Graphics
        /// reference that is passed in.
        /// </summary>
        /// <param name="gr">Graphics area reference</param>
        virtual public void Render(Graphics gr)
        {
            gr.FillPath(new SolidBrush(_color), GetPath());
        }

        /// <summary>
        /// Moves the shape around the screen and wraps it if hits the edge of the screen.
        /// </summary>
        /// <param name="size">Size of the client area (screen size)</param>
        virtual public void Tick(Size size)
        {
            //provide wrapping for each edge of the screen
            if ((_location.X + _xVel) > size.Width)
                _location = new PointF(0, _location.Y);
            if ((_location.X + _xVel) < 0)
                _location = new PointF(size.Width, _location.Y);
            if ((_location.Y + _yVel) > size.Height)
                _location = new PointF(_location.X, 0);
            if ((_location.Y + _yVel) < 0)
                _location = new PointF(_location.X, size.Height);

            ////move the bullet by adding x and y offsets to the current location
            _location = new PointF(_location.X + _xVel, _location.Y + _yVel);
        }
    }

    /// <summary>
    /// Represents the spaceship.
    /// </summary>
    class Ship : BaseShape
    {
        private const float MAXSPEED = 5;  //maximum ship speed during a thrust

        private GraphicsPath _thrustModel;  //graphics path for the thrust animation of the ship
        private bool _thrustOn;
        private float _thrustSpeed; //offset for the speed of thrust, used a variable to show accelerating thrust
        public List<Bullet> Bullets { get; set; }  //collection of bullets for the ship (maximum 8)
        public Rotation Rotation { get; set; }  //rotation of the ship ccw or cw

        static SoundPlayer _fireSound; //sound player for the fire sound of the ship
        static SoundPlayer _thrustSound; //sound player for the fire sound of the ship


        /// <summary>
        /// Initializes the sound players and preloads them.
        /// </summary>
        static Ship()
        {
            _fireSound = new SoundPlayer(Properties.Resources.fire);
            _fireSound.LoadAsync();

            _thrustSound = new SoundPlayer(Properties.Resources.thrust);
            _thrustSound.LoadAsync();
        }

        /// <summary>
        /// Initializes various members of the class, some according to the passed in screen size.
        /// </summary>
        /// <param name="screen">size of the screen</param>
        public Ship(Size screen)
        {
            _location = new PointF(screen.Width / 2, screen.Height / 2);
            _radius = 15; //radius fixed for ship size
            _color = Color.Yellow;  //fixed color for the ship
            _model = MakeShip().Ship;  //create the model of the ship
            _thrustModel = MakeShip().Thrust; //create the thrust model of the ship
            _rotationIncrement = 5;  //set rotation speed for the ship
            Bullets = new List<Bullet>();
            _thrustOn = false;
            _thrustSpeed = 0;  //initial thrust speed
        }

        /// <summary>
        /// Makes the graphics path model of the ship. Also makes an additional graphics model for the thrust
        /// model, to be used for the thrust animation.
        /// </summary>
        /// <returns>A tuple containing graphics path of the ship model and thrust model</returns>
        private (GraphicsPath Ship,GraphicsPath Thrust) MakeShip()
        {
            List<PointF> shipPoints = new List<PointF>();  //collection of points to be genereated for the grpahics path of the ship
            GraphicsPath gpShip = new GraphicsPath(); //graphics path to be returned for the ship

            List<PointF> thrustPoints = new List<PointF>();  //collection of points to be genereated for the grpahics path of the thrust effect
            GraphicsPath gpThrust = new GraphicsPath(); //graphics path to be assigned to the thrust model

            //make points for a custom isosceles triangle for the ship
            double angle = -1 * Math.PI / 2;  //start at -90 degrees to make the ship pointing towards the top of the screen
            shipPoints.Add(new PointF((float)Math.Cos(angle) * _radius, (float)Math.Sin(angle) * _radius));

            //add an angle of 140 degrees
            //first point at full radius and second point at half radius for the cut at the back of the ship
            angle += Math.PI * 7 / 9;
            shipPoints.Add(new PointF((float)Math.Cos(angle) * _radius, (float)Math.Sin(angle) * _radius));
            shipPoints.Add(new PointF((float)Math.Cos(angle) * _radius / 2, (float)Math.Sin(angle) * _radius / 2));
            //add a point for the thrust model at the same Y coord as the last ship point, but lower X coord
            thrustPoints.Add(new PointF(shipPoints.Last().X - _radius / 10, shipPoints.Last().Y));
            //add another 80 degrees (90 + 140 + 80)
            //repeat above process but this time add the half radius point first for continuity in the path
            angle += Math.PI * 4 / 9;
            shipPoints.Add(new PointF((float)Math.Cos(angle) * _radius / 2, (float)Math.Sin(angle) * _radius / 2));
            //add another point for the thrust model at the same Y coord as the last ship point, but slightly bigger X coord
            thrustPoints.Add(new PointF(shipPoints.Last().X + _radius / 10, shipPoints.Last().Y));
            shipPoints.Add(new PointF((float)Math.Cos(angle) * _radius, (float)Math.Sin(angle) * _radius));
            //add the final thrust point at the end of the ship, in the middle. 
            //X is 0 as it is centered and Y is incremented to make it stick out a little bit
            thrustPoints.Add(new PointF(0, shipPoints.Last().Y + 5));

            //create a graphics path figure from the points for the ship and thrust effect
            gpShip.StartFigure();
            gpShip.AddLines(shipPoints.ToArray());
            gpShip.CloseFigure();
            gpThrust.StartFigure();
            gpThrust.AddLines(thrustPoints.ToArray());
            gpThrust.CloseFigure();

            //return both grpahics paths as a tuple
            return (gpShip, gpThrust);
        }

        /// <summary>
        /// Overridden to add Rotation.
        /// </summary>
        /// <param name="size">size of the screen</param>
        public override void Tick(Size size)
        {
            //call base Tick() for movements
            base.Tick(size);

            //perform rotation according to the direction set to
            //the property
            switch (Rotation)
            {
                case Rotation.CounterClockwise:
                    _rotation -= _rotationIncrement;
                    //decelerate the thrust speed while rotating
                    //but do not let it go below zero, as the ship will change direction
                    _thrustSpeed -= (_thrustSpeed < 0) ? 0 : (float)0.1;
                    break;
                case Rotation.Clockwise:
                    _rotation += _rotationIncrement;
                    //decelerate the thrust speed while rotating
                    //but do not let it go below zero, as the ship will change direction
                    _thrustSpeed -= (_thrustSpeed < 0) ? 0 : (float)0.1;
                    break;
            }
        }

        /// <summary>
        /// Makes the ship move in the direction that it is currently pointing.
        /// </summary>
        public void ThrustOn()
        {
            //play the thrust sound on a loop
            //use the flag to prevent playing more than once
            if (!_thrustOn)
                _thrustSound.PlayLooping();
            //set the thrust flag on to show animation
            _thrustOn = true;
            //set x and y offsets for the ship according to the rotation of the ship
            //as the thrust will always make the ship move in the forward direction
            _xVel = (float)(Math.Sin(_rotation * Math.PI / 180) * _thrustSpeed);
            _yVel = -1 * (float)(Math.Cos(_rotation * Math.PI / 180) * _thrustSpeed);

            //increment the thrust speed to show acceleration
            if (_thrustSpeed < MAXSPEED)
                _thrustSpeed += (float)0.1;
        }

        /// <summary>
        /// Slowly decelerates the ship until it stops completely.
        /// Ship keeps moving in the same direction it was when the thrust was on last.
        /// </summary>
        public void ThrustOff()
        {
            //stop playing the thrust sound
            if (_thrustOn)
                _thrustSound.Stop();

            //clear the thrust flag for animation, and reset the thrust speed
            _thrustOn = false;
            _thrustSpeed = 0;

            //set the x and y offsets to 99% of previous values until it becomes zero and stops
            //it gives the floating effect
            _xVel *= (float)0.99;
            _yVel *= ((float)0.99);
        }

        /// <summary>
        /// Fires a bullet. Only allows a maximum of 8 on the screen at a time.
        /// </summary>
        public void Fire()
        {
            //only add a bullet if currently less then 8 on the screen 
            if (Bullets.Count < 8)
            {
                //play the fire sound and add a bullet
                _fireSound.Play();
                Bullets.Add(new Bullet(_location, _rotation, _radius));
            }
        }

        /// <summary>
        /// A feature from the original games. Simply respawns the ship at a random new location.
        /// Use is risky as the ship could spawn on top of or close to an asteroid.
        /// </summary>
        /// <param name="size"> size of the screen</param>
        public void HyperSpace(Size size)
        {
            //create a random new location for the ship while keeping it in bounds of the screen
            _location = new PointF(_rng.Next(size.Width), _rng.Next(size.Height));
        }

        /// <summary>
        /// Produces the transformed graphics path of the cloned model
        /// of ship thrust animation. Uses the same location and rotation as the ship.
        /// This part of the ship is not used in colision checking as it is not technically
        /// a part of the ship, it is supposed to be fire and smoke ;)
        /// </summary>
        /// <returns></returns>
        private GraphicsPath GetThrustPath()
        {
            //create a new transform matrix and add the translation and rotation
            Matrix transforms = new Matrix();
            transforms.Translate(_location.X, _location.Y);
            transforms.Rotate(_rotation);

            //clone the Rock model and perform the transforms to it
            GraphicsPath gp = (GraphicsPath)_thrustModel.Clone();
            gp.Transform(transforms);
            return gp;
        }

        /// <summary>
        /// Override of the base Render, adds the rendering for the thrust.
        /// </summary>
        /// <param name="gr">Graphics object reference</param>
        public override void Render(Graphics gr)
        {
            base.Render(gr);
            if (_thrustOn)
                gr.FillPath(new TextureBrush(Properties.Resources.flame), GetThrustPath());
        }

        /// <summary>
        /// Shows the current number of ship lives available in the top left corner of the screen.
        /// </summary>
        /// <param name="gr">Graphics object refrence</param>
        /// <param name="count">Number of lives</param>
        public void ShowLives(Graphics gr, int count)
        {
            //loop and create a path for each life
            for (int i = 0; i < count; i++)
            {
                //clone the ship model and apply transforms to show
                //them in a row in the top left corner
                GraphicsPath gp = (GraphicsPath)_model.Clone();
                Matrix transforms = new Matrix();
                transforms.Translate(i * 30 + 20, 60);
                gp.Transform(transforms);

                //fill each path on the passed in graphics object
                gr.FillPath(new SolidBrush(_color), gp);
            }
        }
    }

    /// <summary>
    /// Represents the bullet fired by the ship.
    /// </summary>
    class Bullet : BaseShape
    {
        private const float BULLETSPEED = 10;  //speed of the bullet
        public float _travel = 100;  //maximum allowed distance of travel for bullets

        /// <summary>
        /// Creates a new bullet using the location, rotation and radius of the ship.
        /// </summary>
        /// <param name="location">location of the ship</param>
        /// <param name="rotation">rotation of the ship</param>
        /// <param name="radius">radius of the ship</param>
        public Bullet(PointF location, float rotation, float radius)
        {
            //use the same location, rotation and radius as the ship
            _location = location;
            _rotation = rotation;
            _radius = radius;
            _color = Color.White;
            _model = MakeBullet();

            //calculate x and y offsets for movements
            //according to the rotation of the ship
            //have to flip the sign of the y offset due to initial rotation applied to the ship
            _xVel = (float)(Math.Sin(rotation * Math.PI / 180) * BULLETSPEED);
            _yVel = -1 * (float)(Math.Cos(rotation * Math.PI / 180) * BULLETSPEED);
        }

        /// <summary>
        /// Creates a graphics path for the bullet. 
        /// </summary>
        /// <returns></returns>
        private GraphicsPath MakeBullet()
        {
            //add a centered ellipse representing the bullet
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(new RectangleF((float)-1.5, (float)1.5, 3, 3));
            return gp;
        }

        /// <summary>
        /// Override for the Tick() to add a finite distance of travel for bullets.
        /// </summary>
        /// <param name="size"></param>
        public override  void Tick(Size size)
        {
            //call base Tick for bullet movements
            base.Tick(size);

            //decrement the travel distance counter for the bullet and
            //set the flag accordingly for it to die
            IsMarkedForRemoval = (_travel-- == 0);
        }
    }

    /// <summary>
    /// An abstract base class representing an asteroid derived from base Shape class. 
    /// </summary>
    abstract class Asteroid : BaseShape
    {

        /// <summary>
        /// Custom ctor initializes the base Asteroid class
        /// </summary>
        public Asteroid()
        {
            _rotation = 0;  //set rotation to 0
            _rotationIncrement = (float)_rng.NextDouble() * 4 - 2;  //set the increment to a random value between -2 and +2
            _xVel = (float)(_rng.NextDouble() * 2 - 1);  //set the x-axis increment to a randome value between -1 to +1
            _yVel = (float)(_rng.NextDouble() * 2 - 1);  //set the y-axis increment to a randome value between -1 to +1
        }

        /// <summary>
        /// Move the shape according to the current speed values while keeping it in bounds
        /// </summary>
        /// <param name="size">Size of the drawing area</param>
        public override void Tick(Size size)
        {
            base.Tick(size);

            //increment the rotations
            _rotation += _rotationIncrement;
        }

        /// <summary>
        /// Abstract method to kill the asteroids, and spawn medium or small asteroids.
        /// </summary>
        /// <returns></returns>
        abstract public List<Asteroid> Die();

        /// <summary>
        /// Makes a graphics path based on a radius, number of vertices and a variance for the radii
        /// </summary>
        /// <param name="radius">Radius for the path</param>
        /// <param name="vertexCount">Number of vertices </param>
        /// <param name="variance">radius variance</param>
        /// <returns></returns>
        protected static GraphicsPath MakePolyPath(float radius, int vertexCount, float variance)
        {
            List<PointF> points = new List<PointF>();  //collection of points to be genereated
            GraphicsPath gp = new GraphicsPath(); //graphics path to be returned

            //loop around n number of vertices
            for (int i = 0; i < vertexCount; i++)
            {
                double angle = (Math.PI * 2 / vertexCount) * i;  //angle of rotation according to the number of vertices
                float x = (float)Math.Cos(angle) * (radius - (float)_rng.NextDouble() * radius * variance);
                float y = (float)Math.Sin(angle) * (radius - (float)_rng.NextDouble() * radius * variance);

                //create points at each vertex and add to the list
                points.Add(new PointF(x, y));
            }

            //create a graphics path figure from the points
            gp.StartFigure();
            gp.AddLines(points.ToArray());
            gp.CloseFigure();

            //return the new graphics path
            return gp;
        }
    }

    /// <summary>
    /// Represents the large asteroid.
    /// </summary>
    class LargeAsteroid : Asteroid
    {
        static SoundPlayer _bang; //sound player for large bang sound
        static System.Diagnostics.Stopwatch _fadeInSW; //used to keep track of the fade time

        private long _spawnTime;  //keeps track of the time in ms when the asteroids was spawned/instantiated
        public bool IsFading { get { return (_fadeInSW.ElapsedMilliseconds - _spawnTime < 3000); } } //flags if the Asteroid is fading or not

        /// <summary>
        /// Initializes the sounds and preloads them, also initializes the stop watch used for fade-in.
        /// </summary>
        static LargeAsteroid()
        {
            _bang = new SoundPlayer(Properties.Resources.bangLarge);
            _bang.LoadAsync();
            _fadeInSW = new System.Diagnostics.Stopwatch();
            _fadeInSW.Start();
        }

        /// <summary>
        /// Creates a new Large asteroid at a random location on the screen.
        /// </summary>
        /// <param name="screen"></param>
        public LargeAsteroid(Rectangle screen)
        {
            //set a random location according to screen size
            _location = new PointF(_rng.Next(screen.Width), _rng.Next(screen.Height)); 
            //set the radius of the asteroids according to screen size and the multiplier passed from the ctor
            _radius = screen.Width / 20; //radius for large asteroid, scaled according to screen size
            _model = MakePolyPath(_radius, 15, (float)0.5);  //15 vertices for large asteroids 50% variance
            _color = Color.SlateGray; 
            _spawnTime = _fadeInSW.ElapsedMilliseconds;  //save the current elpased ms as the spawn time for this asteroid
        }

        /// <summary>
        /// Marks current instance for removal and creates two medium asteroids.
        /// </summary>
        /// <returns>A collection of MediumAsteroids</returns>
        public override List<Asteroid> Die()
        {
            List<Asteroid> retVal = new List<Asteroid>();
            retVal.Add(new MediumAsteroid(_radius, _location, _xVel, _yVel));
            retVal.Add(new MediumAsteroid(_radius, _location, _xVel, _yVel));
            IsMarkedForRemoval = true;  //kill the large asteroid
            _bang.Play();
            return retVal; //return the medium asteroids
        }

        /// <summary>
        /// Override of the base render to incorporate fading in of the new asteroids.
        /// </summary>
        /// <param name="gr">Graphics object where the asteroids is to be rendered.</param>
        public override void Render(Graphics gr)
        {
            if (IsFading)
                gr.FillPath(new SolidBrush(Color.FromArgb(100, _color)), GetPath());
            else
                base.Render(gr);
        }
    }

    /// <summary>
    /// Represents the Medium sized asteroids.
    /// </summary>
    class MediumAsteroid : Asteroid
    {
        static SoundPlayer _bang;  //sound player for medium bang sound

        /// <summary>
        /// pre-loads the sound.
        /// </summary>
        static MediumAsteroid()
        {
            _bang = new SoundPlayer(Properties.Resources.bangMedium);
            _bang.LoadAsync();
        }

        /// <summary>
        /// Creates a new medium asteroid using the large asteroid characteristics.
        /// </summary>
        /// <param name="radius">radius of the parent asteroid</param>
        /// <param name="location">location of the parent asteroid</param>
        /// <param name="xVel">X-Velocity of the parent asteroid</param>
        /// <param name="yVel">Y-Velocity of the parent asteroid</param>
        public MediumAsteroid(float radius, PointF location, float xVel, float yVel) 
        {
            _radius = (float)0.60 * radius;  //60% radius for medium asteroid
            _location = location;  //use the same location as the parent
            //make the smaller asteroid go in the same general direction
            //by ensuring they have the same sign - or +
            //multiply by 1.25 to make them move away from each other slightly faster
            //because they spawn out of a blast
            _xVel = (xVel < 0) ? Math.Abs(_xVel) * (float)-1.25 : Math.Abs(_xVel) * (float)1.25;
            _yVel = (yVel < 0) ? Math.Abs(_yVel) * (float)-1.25 : Math.Abs(_yVel) * (float)1.25;
            _model = MakePolyPath(_radius, 11, (float)0.5); //11 vertices for medium asteroid, 40% variance
            _color = Color.SlateGray; 
        }

        /// <summary>
        /// Marks the medium asteroid for removal and creates 3 small ones.
        /// </summary>
        /// <returns>a collection of SmallAsteroid</returns>
        public override List<Asteroid> Die()
        {
            List<Asteroid> retVal = new List<Asteroid>();
            //create three smaller asteroids
            retVal.Add(new SmallAsteroid(_radius, _location, _xVel, _yVel));
            retVal.Add(new SmallAsteroid(_radius, _location, _xVel, _yVel));
            retVal.Add(new SmallAsteroid(_radius, _location, _xVel, _yVel));
            IsMarkedForRemoval = true;  //kill the large asteroid
            _bang.Play();
            return retVal; //return the medium asteroids
        }
    }

    /// <summary>
    /// Represents a small sized asteroid.
    /// </summary>
    class SmallAsteroid : Asteroid
    {
        static SoundPlayer _bang;  //sound player for small bang sound

        /// <summary>
        /// pre-loads the sound.
        /// </summary>
        static SmallAsteroid()
        {
            _bang = new SoundPlayer(Properties.Resources.bangSmall);
            _bang.LoadAsync();
        }

        /// <summary>
        /// Creates a new small asteroid using the medium asteroid characteristics.
        /// </summary>
        /// <param name="radius">radius of the parent asteroid</param>
        /// <param name="location">location of the parent asteroid</param>
        /// <param name="xVel">X-Velocity of the parent asteroid</param>
        /// <param name="yVel">Y-Velocity of the parent asteroid</param>
        public SmallAsteroid(float radius, PointF location, float xVel, float yVel)
        {
            _radius = (float)0.40 * radius;  //40% radius for medium asteroid
            _location = location;
            //make the smaller asteroid go in the same general direction
            //by ensuring they have the same sign - or +
            //multiply by 1.5 make them move away from each other faster
            //as they spawn out of a blast
            _xVel = (xVel < 0) ? Math.Abs(_xVel) * (float)-1.5 : Math.Abs(_xVel) * (float)1.5;
            _yVel = (yVel < 0) ? Math.Abs(_yVel) * (float)-1.5 : Math.Abs(_yVel) * (float)1.5;
            _model = MakePolyPath(_radius, 6, (float)0.5); //6 vertices for medium asteroid, 50% variance
            _color = Color.LightGray;
        }

        /// <summary>
        /// Marks the small asteroid for removed. And returns an empty collection of Asteroid
        /// just to meet the method signature requirements, small asteroid does not spawn new asteroids.
        /// </summary>
        /// <returns>an empty collection of Asteroids</returns>
        public override List<Asteroid> Die()
        {
            IsMarkedForRemoval = true;
            _bang.Play();
            return new List<Asteroid>();  //return empty collection
        }
    }
}
