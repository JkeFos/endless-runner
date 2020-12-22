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
        // Instance of dispatcher timer.
        private readonly DispatcherTimer gameTimer = new DispatcherTimer();

        // Rect class instances called player hit box, ground hit box, obstacle hit box.
        private Rect playerHitBox;
        private Rect groundHitBox;
        private Rect obstacleHitBox;

        // Jumping status, default is false.
        private bool jumping;

        // Force integer, default is 20.
        private int force = 20;

        // Speed integer, default is 5.
        private int speed = 5;

        // Random class instance.
        private readonly Random rand = new Random();

        // Game over status, default is false.
        private bool gameover;

        // Used to swap sprites for the player.
        private double spriteInt;

        // Sprite variable, used for swapping sprites of the player.
        private readonly ImageBrush playerSprite = new ImageBrush();
        private readonly ImageBrush backgroundSprite = new ImageBrush();
        private readonly ImageBrush obstacleSprite = new ImageBrush();

        // Array to change the obstacle position on screen.
        // Random element is selected for the height after the first one.
        private readonly int[] obstaclePosition = { 275, 280, 285, 290, 290 };

        // Score, default is 0.
        private int score;

        // Speed at which the game moves. After every jump, speed increases.
        // Default speeds are 3, 12.
        private int backgroundSpeed = 3;
        private int playerSpeed = 12;

        // Gap between the last and next obstacle.
        private int nextObstacle = 950;

        public MainWindow()
        {
            InitializeComponent();
            _ = MainCanvas.Focus();

            // Assign the game engine event to the game timer tick.
            gameTimer.Tick += GameEngine;

            // Set game timer interval to 20 ms.
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            // Set back sprite image.
            // 'pack://' - Current app package.
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
                playerSpeed = 12;
                nextObstacle = 950;

                StartGame();
            }
        }

        private void StartGame()
        {
            // Background location.
            Canvas.SetLeft(Background1, 0);
            Canvas.SetLeft(Background2, 1262);

            // Player starting x, y coordinate.
            Canvas.SetLeft(Player, 110);
            Canvas.SetTop(Player, 140);

            // Obstacle starting x, y coordinate.
            Canvas.SetLeft(Obstacle, 950);
            Canvas.SetTop(Obstacle, 280);

            // Set sprite for player.
            RunSprite(1);

            // Set obstacle sprite.
            obstacleSprite.ImageSource = new BitmapImage(new Uri(Paths.ObstacleImage, UriKind.Relative));
            Obstacle.Fill = obstacleSprite;

            // Starting values.
            jumping = false;
            gameover = false;
            score = 0;
            ScoreText.Content = "Score: " + score;

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

        private void GameEngine(object sender, EventArgs e)
        {
            // Move the player character down using the speed integer.
            Canvas.SetTop(Player, Canvas.GetTop(Player) + speed);

            // Move the background 3 pixels to the left with each tick.
            Canvas.SetLeft(Background1, Canvas.GetLeft(Background1) - backgroundSpeed);
            Canvas.SetLeft(Background2, Canvas.GetLeft(Background2) - backgroundSpeed);

            // Move obstacle rectangle to the left 12 pixels per tick.
            Canvas.SetLeft(Obstacle, Canvas.GetLeft(Obstacle) - playerSpeed);

            // Link the score text label to the score integer.
            ScoreText.Content = "Score: " + score;

            // Assign the player hit box to the player, ground hit box to the ground, rectangle and obstacle hit box to the obstacle rectangle.
            playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);
            groundHitBox = new Rect(Canvas.GetLeft(Ground), Canvas.GetTop(Ground), Ground.Width, Ground.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(Obstacle), Canvas.GetTop(Obstacle), Obstacle.Width, Obstacle.Height);

            // If the player hits the ground.
            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 0;
                Canvas.SetTop(Player, Canvas.GetTop(Ground) - Player.Height);
                jumping = false;
                spriteInt += 1;

                // Reset the sprite if it goes above 8.
                if (spriteInt > 8)
                {
                    spriteInt = 1;
                }

                // Pass sprite int to the sprite function.
                RunSprite(spriteInt);
            }

            // If the player hits the obstacle.
            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameover = true;
                gameTimer.Stop();
            }

            // If jumping.
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
            if (Canvas.GetLeft(Background1) < -1262)
            {
                Canvas.SetLeft(Background1, Canvas.GetLeft(Background2) + Background2.Width);
            }

            // If the second background goes below a certain point, position 
            // the second background behind the first background.
            if (Canvas.GetLeft(Background2) < -1262)
            {
                Canvas.SetLeft(Background2, Canvas.GetLeft(Background1) + Background1.Width);
            }

            // If the obstacle goes beyond -50, set the left position of the obstacle to 950 pixels.
            // Randomly set the top position of the obstacle from the array create earlier. This will
            // randomly pick a position from the array so it won't be the same each time it comes
            // around the screen. Also adds 1 to the score tally.
            if (Canvas.GetLeft(Obstacle) < -50)
            {
                Canvas.SetLeft(Obstacle, nextObstacle + rand.Next(1, 100));
                Canvas.SetTop(Obstacle, obstaclePosition[rand.Next(0, obstaclePosition.Length)]);

                score++;
                backgroundSpeed++;
                playerSpeed++;
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
