using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace endless_runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Dispatcher.
        private readonly DispatcherTimer gameTimer = new DispatcherTimer();

        // Hit boxes for all objects.
        private Rect playerHitBox;
        private Rect groundHitBox;
        private Rect obstacleHitBox;

        // Sprite variable, used for swapping sprites of the player.
        private readonly ImageBrush playerSprite = new ImageBrush();
        private readonly ImageBrush backgroundSprite = new ImageBrush();
        private readonly ImageBrush obstacleSprite = new ImageBrush();

        // Randomly generate height and distance of obstacles.
        private readonly Random rand = new Random();

        // Default variables.
        private bool gameover;
        private bool jumping;
        private double spriteInt;
        private int backgroundSpeed;
        private int force;
        private int nextObstacle;
        private int obstacleMovement;
        private int score;
        private int speed;
        private readonly int backgroundWidth = 1250;
        private readonly int obstacleMaxHeight = 290;
        private readonly int obstacleMinHeight = 275;
        private readonly int obstacleStartX = 280;
        private readonly int obstacleStartY = 950;
        private readonly int playerStartX = 100;
        private readonly int playerStartY = 150;

        public MainWindow()
        {
            InitializeComponent();
            _ = MainCanvas.Focus();

            // Assign the game engine event to the game timer tick.
            gameTimer.Tick += GameEngine;

            // Set game timer interval to 20 ms.
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            // Set background sprite image.
            backgroundSprite.ImageSource = new BitmapImage(new Uri(Paths.BackgroundImage, UriKind.Relative));
            Background1.Fill = backgroundSprite;
            Background2.Fill = backgroundSprite;

            // Start.
            StartGame();
        }

        /// <summary>
        /// Space: Character jumps.
        /// Enter: When gameover is true, start new game.
        /// If the game is over and the enter key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !jumping && Canvas.GetTop(Player) > 250)
            {
                jumping = true;
                force = 15;
                speed = -12;
                playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage02, UriKind.Relative));
            }

            if (e.Key == Key.Enter && gameover)
            {
                backgroundSpeed = 3;
                obstacleMovement = 12;
                nextObstacle = 950;

                StartGame();
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        private void StartGame()
        {
            // Background location.
            Canvas.SetLeft(Background1, 0);
            Canvas.SetLeft(Background2, backgroundWidth);

            // Player starting x, y coordinate.
            Canvas.SetLeft(Player, playerStartX);
            Canvas.SetTop(Player, playerStartY);

            // Obstacle starting x, y coordinate.
            Canvas.SetLeft(Obstacle, obstacleStartY);
            Canvas.SetTop(Obstacle, obstacleStartX);

            // Set sprite for player.
            RunSprite(1);

            // Set obstacle sprite.
            obstacleSprite.ImageSource = new BitmapImage(new Uri(Paths.ObstacleImage, UriKind.Relative));
            Obstacle.Fill = obstacleSprite;

            // Starting values.
            backgroundSpeed = 3;
            force = 5;
            gameover = false;
            jumping = false;
            nextObstacle = 950;
            obstacleMovement = 12;
            score = 0;
            speed = 5;
            ScoreText.Content = $"Score: {score}";

            // Start the game timer.
            gameTimer.Start();
        }

        /// <summary>
        /// Change the sprite of the player.
        /// </summary>
        /// <param name="i">Used to change the images of the player.</param>
        private void RunSprite(double i)
        {
            switch (i)
            {
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage01, UriKind.Relative));
                    break;

                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage02, UriKind.Relative));
                    break;

                case 3:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage03, UriKind.Relative));
                    break;

                case 4:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage04, UriKind.Relative));
                    break;

                case 5:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage05, UriKind.Relative));
                    break;

                case 6:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage06, UriKind.Relative));
                    break;

                case 7:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage07, UriKind.Relative));
                    break;

                case 8:
                    playerSprite.ImageSource = new BitmapImage(new Uri(Paths.RunnerImage08, UriKind.Relative));
                    break;

                default:
                    break;
            }

            Player.Fill = playerSprite;
        }

        /// <summary>
        /// Main logic of the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEngine(object sender, EventArgs e)
        {
            // Move the player character down using the speed integer.
            Canvas.SetTop(Player, Canvas.GetTop(Player) + speed);

            // Move the background pixels to the left with each tick.
            Canvas.SetLeft(Background1, Canvas.GetLeft(Background1) - backgroundSpeed);
            Canvas.SetLeft(Background2, Canvas.GetLeft(Background2) - backgroundSpeed);

            // Move obstacle rectangle to the left pixels per tick.
            Canvas.SetLeft(Obstacle, Canvas.GetLeft(Obstacle) - obstacleMovement);

            // Link the score text label to the score integer.
            ScoreText.Content = $"Score: {score}";

            // Assign the player hit box to the player, ground hit box to the ground, rectangle and obstacle hit box to the obstacle rectangle.
            playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);
            groundHitBox = new Rect(Canvas.GetLeft(Ground), Canvas.GetTop(Ground), Ground.Width, Ground.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(Obstacle), Canvas.GetTop(Obstacle), Obstacle.Width, Obstacle.Height);

            // If the player hits the ground.
            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                Canvas.SetTop(Player, Canvas.GetTop(Ground) - Player.Height);
                speed = 0;
                spriteInt++;
                jumping = false;

                // Reset the sprite if it goes above 8.
                if (spriteInt > 8)
                {
                    spriteInt = 1;
                }

                // Pass sprite int to the sprite function.
                RunSprite(spriteInt);
            }

            // Intersection with the box.
            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameover = true;
                gameTimer.Stop();
            }

            // Player jumping.
            if (jumping)
            {
                speed = -9;
                force--;
            }
            else
            {
                speed = 12;
            }

            if (force < 0)
            {
                jumping = false;
            }

            /*********************
             * Parallax scrolling.
             *********************/

            // If the first background goes below a certain point, position 
            // the first background behind the second background.
            if (Canvas.GetLeft(Background1) < -backgroundWidth)
            {
                Canvas.SetLeft(Background1, Canvas.GetLeft(Background2) + Background2.Width);
            }

            // If the second background goes below a certain point, position 
            // the second background behind the first background.
            if (Canvas.GetLeft(Background2) < -backgroundWidth)
            {
                Canvas.SetLeft(Background2, Canvas.GetLeft(Background1) + Background1.Width);
            }

            // If the obstacle goes beyond -50, set the left position of the obstacle to 950 pixels.
            // Randomly set the top position of the obstacle from the array create earlier. This will
            // randomly pick a position from the array so it won't be the same each time it comes
            // around the screen. Also adds 1 to the score tally.
            if (Canvas.GetLeft(Obstacle) < -50)
            {
                // Distance to next obstacle: Previous obstacle plus random number between 1-100 pixels.
                // Height of next obstacle: Random number between the min and max height in pixels.
                Canvas.SetLeft(Obstacle, nextObstacle + rand.Next(1, 100));
                Canvas.SetTop(Obstacle, rand.Next(obstacleMinHeight, obstacleMaxHeight));

                score++;
                backgroundSpeed++;
                obstacleMovement++;
            }

            // If gameover, draw a yellow border around the obstacle. Draw a red border around
            // the player and show a message.
            if (gameover)
            {
                Obstacle.Stroke = Brushes.Yellow;
                Obstacle.StrokeThickness = 1;

                Player.Stroke = Brushes.Red;
                Player.StrokeThickness = 1;

                ScoreText.Content += " - Press enter to try again.";
            }
            // Else reset the border thickness.
            else
            {
                Player.StrokeThickness = 0;
                Obstacle.StrokeThickness = 0;
            }
        }
    }
}
