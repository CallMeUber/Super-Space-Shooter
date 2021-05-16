using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Super_Space_Shooter
{
    /// <summary>
    /// This is the main type for your game.
    /// 
    ///             CONTROLS:   X TO SHOOT 
    ///                         A TO ACCELERATE
    ///                         B TO DECCELERATE/REVERSE
    ///                         Y TO PAUSE
    /// 
    ///             THE GAME SUPPORTS UP TO 4 PLAYERS!!!
    ///             
    ///             THE ENEMIES GET FASTER EVERY 100 POINTS!!
    ///             
    ///             GOODLUCK!!!
    /// 
    /// </summary>
    public class Game1 : Game
    {
        Rectangle viewPortRect;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int screenwidth;
        int screenheight;

        GamePadState[] gamepad;
        GamePadState oldmenuinput;

        //Menu Variables

        Texture2D menuscreen;
        SpriteFont menufont;
        Color menucolor;
        Color optioncolor;
        Color exitcolor;
        Song bgm;

        //Option Variables
        Texture2D arrowsprite;
        int BGMvolume;
        int SFXvolume;
        Color bgmoption = Color.White;
        Color sfxoption;
        int soundfix;

        //Game Variables
        SoundEffect shootsfx;
        Texture2D gameBG;
        Vector2 playerpos;
        Vector2 playerspeed;
        Texture2D healthbar;
        int healthbarwidth;
        SpriteFont endfont;
        SoundEffect enemydeath;
        bool pause;
        Vector2 playerorigin;
        int[] score;
        GamePadState menuinput;

        //Earth Variables
        Texture2D earthsprite;
        Rectangle earthrec;
        int earthhealth;                /// <summary>
        /// ////////// PUT EARHT HEALTHJ
        /// </summary>


        //Player Class variable
        List<PlayerClass> playerlist;
        PlayerClass player;

        Texture2D playersprite;

        //Bullets Class variables
        List<Projectile> Bullets;
        List<Projectile> Deletebul;
        Texture2D bulletsprite;

        //Enemy Variables
        Texture2D enemysprite1;
        float enemyspeed;
        int enemysize;
        EnemyClass enemy;
        List<EnemyClass> Enemylist;
        List<EnemyClass> Deleteenemy;

        //debug variables
        Texture2D rectest;

        private enum Screen
        {
            menu = 1,
            gamescreen = 2,
            option
        }

        private enum MenuItem
        {
            menuItem = 1,
            GameItem = 2,
            Exit = 3
        }


        Screen currentScreen = Screen.menu;
        MenuItem currentItem = MenuItem.menuItem;

        //textureData for pixel perfect collision
        Color[] shipTextureData;
        Color[] enemyTextureData;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screenwidth = 800;
            screenheight = 600;
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;
            graphics.ApplyChanges();

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            viewPortRect = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            BGMvolume = 5;
            SFXvolume = 5;
            soundfix = 1;

            playerorigin = new Vector2(70, 150);

            score = new int[4];

            gamepad = new GamePadState[4];

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuscreen = Content.Load<Texture2D>("MenuScreen");
            menufont = Content.Load<SpriteFont>("MenuFont");
            bgm = Content.Load<Song>("BackgroundMusic");
            arrowsprite = Content.Load<Texture2D>("Arrow");
            shootsfx = Content.Load<SoundEffect>("ShootSFX");
            gameBG = Content.Load<Texture2D>("background");
            playersprite = Content.Load<Texture2D>("Player Sprite");
            bulletsprite = Content.Load<Texture2D>("Bullet Sprite");
            enemysprite1 = Content.Load<Texture2D>("Alien Sprite");
            healthbar = Content.Load<Texture2D>("Health Bar");
            enemydeath = Content.Load<SoundEffect>("Enemy death");
            endfont = Content.Load<SpriteFont>("Endfont");
            earthsprite = Content.Load<Texture2D>("Earth");

            // TODO: use this.Content to load your game content here

            //textureData for ship
            shipTextureData = new Color[playersprite.Width * playersprite.Height];
            playersprite.GetData(shipTextureData);

            enemyTextureData = new Color[enemysprite1.Width * enemysprite1.Height];
            enemysprite1.GetData(enemyTextureData);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            PlayBGM();
            menuinput = GamePad.GetState(0);
            SoundFix();
            if (currentScreen == Screen.menu)
            {
                UpdateMenuItem();
                InitializeGameVar();
            }
            GameUpdate();
            oldmenuinput = menuinput;


            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }


        public static bool IntersectPixels(
                           Matrix transformA, int widthA, int heightA, Color[] dataA,
                           Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            switch ((int)currentScreen)
            {
                case 1:
                    DrawMenuScreen();
                    break;
                case 2: DrawGameScreen(); break;
                case 3: DrawOptionScreen(); break;
            }
            spriteBatch.End();
            //      GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void InitializeGameVar()
        {
            Enemylist = new List<EnemyClass>();
            Bullets = new List<Projectile>();
            Deletebul = new List<Projectile>();
            Deleteenemy = new List<EnemyClass>();
            playerpos = new Vector2(screenwidth / 2, screenheight / 2);
            playerspeed = new Vector2(0, 0);
            healthbarwidth = screenwidth;
            earthhealth = screenwidth;
            score = new int[] { 0, 0, 0, 0 };
            enemyspeed = 0.5f;
            enemysize = 100;

            playerlist = new List<PlayerClass>();
            for(int count = 0; count <4; count++)
            {
                if (GamePad.GetState(count).IsConnected)
                {
                    player = new PlayerClass(playersprite, new Vector2(screenwidth / 5 * (count+1), screenheight / 2), 50f, count);
                    playerlist.Add(player);
                }
            }                                  
        }


        #region Draw Methods
        private void DrawMenuScreen()
        {
            spriteBatch.Draw(menuscreen, new Rectangle(0, 0, screenwidth, screenheight), Color.White);
            spriteBatch.DrawString(menufont, "START", new Vector2(35f, 400f), menucolor);
            spriteBatch.DrawString(menufont, "GAME", new Vector2(40f, 490f), menucolor);
            spriteBatch.DrawString(menufont, "OPTIONS", new Vector2(280f, 440f), optioncolor);
            spriteBatch.DrawString(menufont, "EXIT", new Vector2(610f, 440f), exitcolor);
        }
        private void DrawGameScreen()
        {
                if (earthhealth > 1)
                {
                    spriteBatch.Draw(gameBG, new Rectangle(0, 0, screenwidth, screenheight), Color.White);
                    DrawEarth();
                    foreach (PlayerClass player in playerlist)
                    {
                        player.Draw(spriteBatch, healthbar);
                    }
                    spriteBatch.DrawString(endfont, "" + score[0], new Vector2(0), Color.White);
                    if (Bullets != null)
                    {
                        foreach (Projectile item in Bullets)
                            item.Draw(spriteBatch);
                    }
                    if (Enemylist != null)
                    {
                        foreach (EnemyClass item in Enemylist)
                            item.Draw(spriteBatch);
                    }
                    if (pause == true)
                        spriteBatch.DrawString(menufont, "PAUSED", new Vector2(screenwidth / 2-120, screenheight / 2-40), Color.Black);
                }
                else
                {
                    DrawGameOver();
                }
                   
        }

        private void DrawOptionScreen()
        {
            spriteBatch.Draw(menuscreen, new Rectangle(0, 0, screenwidth, screenheight), Color.White);
            spriteBatch.DrawString(menufont, "BGM", new Vector2(100, 370), Color.White);
            spriteBatch.DrawString(menufont, "SFX", new Vector2(120, 450), Color.White);
            spriteBatch.DrawString(menufont, "" + BGMvolume, new Vector2(500, 370), bgmoption);
            spriteBatch.DrawString(menufont, "" + SFXvolume, new Vector2(500, 450), sfxoption);
            spriteBatch.Draw(arrowsprite, new Vector2(600, 370), bgmoption);
            spriteBatch.Draw(arrowsprite, new Vector2(350, 370), null, bgmoption, 0f, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(arrowsprite, new Vector2(600, 450), sfxoption);
            spriteBatch.Draw(arrowsprite, new Vector2(350, 450), null, sfxoption, 0f, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0);
        }
        private void DrawGameOver()
        {
            spriteBatch.DrawString(endfont, "Game Over!", new Vector2(200, 200), Color.White);
            spriteBatch.DrawString(endfont, "Press Start to Continue", new Vector2(10, 250), Color.White);
        }

        private void DrawEarth()
        {
            spriteBatch.Draw(earthsprite, earthrec, Color.White);
            spriteBatch.Draw(healthbar, new Rectangle(0, screenheight-10, earthhealth, 10), Color.White);
        }
        #endregion

        #region Control Methods


        private void PlayerMovement()
        {
            foreach(PlayerClass player in playerlist)
            {
                player.Update(screenwidth, screenheight);
            }
        }

        private void PlayerShoot()
        {
            foreach (PlayerClass player in playerlist)
            {
                player.Shoot(shootsfx, bulletsprite);         
            }                               
        }

        #endregion

        #region Menu Items

        private void UpdateMenuItem()
        {
            if (menuinput.DPad.Right == ButtonState.Pressed && oldmenuinput.DPad.Right == ButtonState.Released)
            {
                if (currentItem == MenuItem.Exit)
                    currentItem = MenuItem.menuItem;
                else
                    currentItem++;

            }

            if (menuinput.DPad.Left == ButtonState.Pressed && oldmenuinput.DPad.Left == ButtonState.Released)
            {
                if (currentItem == MenuItem.menuItem)
                    currentItem = MenuItem.Exit;
                else
                    currentItem--;
            }

            switch ((int)currentItem)
            {
                case 1:
                    menucolor = Color.Yellow;
                    optioncolor = Color.White;
                    exitcolor = Color.White;
                    if (menuinput.Buttons.Start == ButtonState.Pressed && oldmenuinput.Buttons.Start == ButtonState.Released)
                        currentScreen = Screen.gamescreen;
                    break;
                case 2:
                    menucolor = Color.White;
                    optioncolor = Color.Yellow;
                    exitcolor = Color.White;
                    if (menuinput.Buttons.Start == ButtonState.Pressed && oldmenuinput.Buttons.Start == ButtonState.Released)
                        currentScreen = Screen.option;
                    break;
                case 3:
                    menucolor = Color.White;
                    optioncolor = Color.White;
                    exitcolor = Color.Yellow;
                    if (menuinput.Buttons.Start == ButtonState.Pressed && oldmenuinput.Buttons.Start == ButtonState.Released)
                        Exit();
                    break;
            }
        }
        private void PlayBGM()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Volume = BGMvolume * 0.1f;
                MediaPlayer.Play(bgm);
                MediaPlayer.IsRepeating = true;
            }
        }
        private void SoundFix()
        {
            if (currentScreen == Screen.option)
            {
                if (soundfix == 1)
                {
                    bgmoption = Color.Yellow;
                    sfxoption = Color.Gray;

                    if (menuinput.DPad.Right == ButtonState.Pressed && oldmenuinput.DPad.Right == ButtonState.Released && BGMvolume < 10)
                    {
                        BGMvolume += 1;
                    }
                    if (menuinput.DPad.Left == ButtonState.Pressed && oldmenuinput.DPad.Left == ButtonState.Released && BGMvolume > 0)
                    {
                        BGMvolume -= 1;
                    }
                    if (menuinput.DPad.Down == ButtonState.Pressed && oldmenuinput.DPad.Down == ButtonState.Released)
                    {
                        soundfix = 2;
                    }
                }
                if (soundfix == 2)
                {
                    bgmoption = Color.Gray;
                    sfxoption = Color.Yellow;
                    if (menuinput.DPad.Right == ButtonState.Pressed && oldmenuinput.DPad.Right == ButtonState.Released && SFXvolume < 10)
                    {
                        SFXvolume += 1;
                    }
                    if (menuinput.DPad.Left == ButtonState.Pressed && oldmenuinput.DPad.Left == ButtonState.Released && SFXvolume > 0)
                    {
                        SFXvolume -= 1;
                    }
                    if (menuinput.DPad.Up == ButtonState.Pressed && oldmenuinput.DPad.Up == ButtonState.Released)
                    {
                        soundfix = 1;
                    }
                }
                MediaPlayer.Volume = BGMvolume * 0.1f;
                if (menuinput.Buttons.Back == ButtonState.Pressed)
                    currentScreen = Screen.menu;
            }
        }
        private void GameUpdate()
        {
            if (currentScreen == Screen.gamescreen)
            {
                if (!pause)
                {
                    SoundEffect.MasterVolume = SFXvolume * 0.1f;
                    UpdateEarth();
                    PlayerMovement();
                    PlayerShoot();
                    BulletUpdate();
                    EnemyUpdate();
                    CollisionUpdate();
                    UpdateLevel();
                }
                if ((int)currentScreen == 2)
                {
                    MediaPlayer.Stop();
                }
                if (menuinput.Buttons.Y == ButtonState.Pressed && oldmenuinput.Buttons.Y == ButtonState.Released)
                    pause = !pause;
            }
            if (menuinput.Buttons.Start == ButtonState.Pressed && oldmenuinput.Buttons.Start == ButtonState.Released && earthhealth < 1)
                currentScreen = Screen.menu;
        }
        private void UpdateLevel()
        {
            if (score[0] / 100 >= 1)
            {
                enemyspeed = (score[0] / 100) * 0.2f + 0.5f;
                if (score[0] < 800)
                    enemysize = 100 - (10 * (score[0] / 100));
            }
        }

        #endregion

        #region Update Entities

        private void UpdateEarth()
        {
            float allplayhealth = 0;
            earthrec = new Rectangle(screenwidth / 2 - 40, screenheight / 2 - 40, earthsprite.Width, earthsprite.Height);
            foreach (EnemyClass enemy in Enemylist)
            {
                if (PixelCollision(earthrec, enemy.EnemyRec, earthsprite, enemy._enemysprite))
                    earthhealth--;
            }
            foreach (PlayerClass player in playerlist)
            {
                if (player.Health > allplayhealth)
                    allplayhealth = player.Health;
            }
            if (allplayhealth == 0)
                earthhealth = 0;
        }
        private void BulletUpdate()
        {
            if (Bullets != null)
            {
                foreach (Projectile item in Bullets)
                {
                    item.UpdateBullet();
                }
                foreach (Projectile item2 in Bullets)
                {
                    if (item2.RemoveItem())
                        Deletebul.Add(item2);
                }
                foreach (Projectile item3 in Deletebul)
                {
                    Bullets.Remove(item3);
                }
            }
        }
        private void EnemyUpdate()
        {
            Random random = new Random();
            Vector2 randompos = new Vector2();
            int edgeofscreen = random.Next(1, 5);
            switch (edgeofscreen)
            {
                case 1:
                    randompos = new Vector2(0, random.Next(screenheight));
                    break;
                case 2:
                    randompos = new Vector2(random.Next(screenwidth), 0);
                    break;
                case 3:
                    randompos = new Vector2(screenwidth, random.Next(screenheight));
                    break;
                case 4:
                    randompos = new Vector2(random.Next(screenwidth), screenheight);
                    break;
            }
            int intrandomy = random.Next(600);
            int randomtime = random.Next(50);
            Vector2 enemyorient;
            if (randomtime < 2)
            {
                enemy = new EnemyClass(enemysprite1, randompos);
                Enemylist.Add(enemy);
            }
            foreach (EnemyClass item in Enemylist)
            {
                enemyorient = new Vector2(screenwidth / 2 - item.EnemyRec.Center.X, screenheight / 2 - item.EnemyRec.Center.Y);
                enemyorient.Normalize();
                item.Update(enemyorient, enemyspeed, item._enemysprite.Width);
            }
        }

        #endregion

        #region Collision Methods
        private void CollisionUpdate() 
        {
            foreach(PlayerClass player in playerlist)
            {
                Matrix playerTransform =
                Matrix.CreateTranslation(new Vector3(-new Vector2(70, 150), 0.0f)) *
                Matrix.CreateScale(0.3f) *
                Matrix.CreateRotationZ(player.Rotation) *
                Matrix.CreateTranslation(new Vector3(player.Position, 0.0f));

                player.TransformedRectangle = CalculateBoundingRectangle(new Rectangle(0, 0, playersprite.Width, playersprite.Height), playerTransform);

                foreach (EnemyClass item in Enemylist)
                {
                    Matrix enemyTransform =
                    Matrix.CreateTranslation(new Vector3(-item._enemyorigin, 0.0f)) *
                    // Matrix.CreateScale(block.Scale) *  would go here
                    Matrix.CreateRotationZ(0) *
                    Matrix.CreateTranslation(new Vector3(item.Position, 0.0f));

                    Rectangle newenemyrec = CalculateBoundingRectangle(new Rectangle(0, 0, item._enemysprite.Width, item._enemysprite.Height), enemyTransform);

                    if (newenemyrec.Intersects(player.TransformedRectangle))
                    {

                        if (IntersectPixels(playerTransform, player.Rectangle.Width, player.Rectangle.Height, shipTextureData, enemyTransform, item.EnemyRec.Width, item.EnemyRec.Height, enemyTextureData))
                        {
                            player.Health -= 0.3f;
                        }
                    }

                    foreach (Projectile item2 in player.BulletList)
                    {
                        if (PixelCollision(item2.CollisionRec, item.EnemyRec, item2._projectileSprite, item._enemysprite))
                        {
                            Deleteenemy.Add(item);
                            player.DeleteBulletList.Add(item2);
                            enemydeath.Play();
                            score[0] += 5;
                        }
                    }
                }            
            }
            foreach (EnemyClass item3 in Deleteenemy)
            {
                Enemylist.Remove(item3);              
            }
        }      
        private bool PixelCollision(Rectangle rec1, Rectangle rec2, Texture2D tex1, Texture2D tex2) //wrong pixel collision due to rotation of texture!!
        {

            if (rec1.Intersects(rec2))
            {
            
                int top, bottom, left, right;
                Color rec1color, rec2color;
                Color[] Texturerec1, Texturerec2;
                if (rec1.Intersects(rec2))
                {
                    top = Math.Max(rec1.Top, rec2.Top);
                    bottom = Math.Min(rec1.Bottom, rec2.Bottom);
                    left = Math.Max(rec1.Left, rec2.Left);
                    right = Math.Min(rec1.Right, rec2.Right);

                    Texturerec1 = new Color[tex1.Height * tex1.Width];
                    tex1.GetData(Texturerec1);

                    Texturerec2 = new Color[tex2.Height * tex2.Width];
                    tex2.GetData(Texturerec2);

                    for (int x = top; x < bottom; x++)
                    {
                        for (int y = left; y < right; y++)
                        {
                            rec1color = Texturerec1[(x - rec1.Top) * tex1.Width + (y - rec1.Left)];
                            rec2color = Texturerec2[(x - rec2.Top) * tex2.Width + (y - rec2.Left)];

                            if (rec1color.A != 0 && rec2color.A != 0)
                                return true;
                        }
                    }
                }
                return false;
            }
            
            else return false;
        }

#endregion

    }
}
