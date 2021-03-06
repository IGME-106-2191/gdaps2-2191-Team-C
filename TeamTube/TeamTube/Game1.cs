﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTube
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
  
    public class Game1 : Game
    {
        public static int screenWidth;
        public static int screenHeight;

        //Camera
        Camera camera;

        //item textures and rectangles
        Texture2D hpPotTexture;
        Rectangle potOneRect;
        Rectangle potTwoRect;
        Rectangle potThreeRect;
        HealthPotion potOne;
        HealthPotion potTwo;
        HealthPotion potThree;
        List<Item> items = new List<Item>();
        Vector2 potions;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //temporary wall and floor texture
        Texture2D wallTexture;
        Texture2D floorTexture;
        Texture2D entranceTexture;
        Texture2D exitTexture;

        //gamestate textures
        Texture2D pauseMenu;

        //basic font
        SpriteFont font;

        //temp player texture
        Texture2D playerTexture;
        Rectangle playerRectangle;
        Texture2D enemyTexture;
        Rectangle enemyRectangle;
        Rectangle enemyRectangle2;

        CharacterController characterController;
        Player player;
        Enemy enemy;
        Enemy enemy2;

        //we need a tile Controller
        TileController tileController;
        //entrance and exit tiles
        Point entrance1;
        Point exit1;

        //GameStatecontroller
        GameStateController menuController;

        //menuStack and Menunodes
        MenuNode menuRoot;
        MenuStack menuStack;

        //menuTexture
        Texture2D playerMenuTexture;

        //Enum items
        GameState gState;
        MenuState mState;
        ItemState iState;
        KeyboardState kbState;
        public KeyboardState previousKbState;

        //check if items have been picked up
       
        //Attack selection
        Vector2 exitVector;
        Vector2 attackVector;
        Vector2 strongVector;
        Vector2 itemVector;
        Vector2 bombVector;
        Vector2 potionVector;
        Rectangle selectionRect;
        Texture2D selectionBGTxt;
        SpriteFont selectionText;

        //shader time
        public static Texture2D lightMask;
        public static Effect effect1;
        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        //menu images
        Rectangle backGround;
        Texture2D mainMenu;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1200;
            graphics.PreferredBackBufferWidth = 1600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //window dimentions
            
            // TODO: Add your initialization logic here
            gState = new GameState();
            gState = GameState.mainMenu;
            mState = new MenuState();
            iState = new ItemState();
            kbState = Keyboard.GetState();
            previousKbState = Keyboard.GetState();
           // bombActive = false;
          //  potionActive = false;
            playerRectangle = new Rectangle(new Point(96, 96), new Point(32, 32));
            enemyRectangle = new Rectangle(new Point(32*5, 32*5), new Point(32, 32));
            enemyRectangle2 = new Rectangle(new Point(32 * 12, 32 * 4), new Point(32, 32));
            potOneRect = new Rectangle(new Point(128+8, 128+8), new Point(16, 16));
            potTwoRect = new Rectangle(new Point(160+8, 488), new Point(16, 16));
            potThreeRect = new Rectangle(new Point(520, 128+8), new Point(16, 16));
            exitVector = new Vector2(20, 70);
            itemVector = new Vector2(20, 60);
            attackVector = new Vector2(20, 50);
            strongVector = new Vector2(20, 40);
            bombVector = new Vector2(20, 40);
            potionVector = new Vector2(20, 50);
            potions = new Vector2(screenWidth/2 + 200, screenHeight/2);
            selectionRect = new Rectangle(20, 30, 200, 70);
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            camera = new Camera();
            //GameState controller
            menuController = new GameStateController();
            backGround = new Rectangle(0, 0, 1600, 1200);
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

            //load temporary wall and floor assets
            wallTexture = Content.Load<Texture2D>("Wall_tile");
            floorTexture = Content.Load<Texture2D>("Ground_tile");
            entranceTexture = Content.Load<Texture2D>("Entrance_tile");
            exitTexture = Content.Load<Texture2D>("exit");
            //load spritefont
            font = Content.Load<SpriteFont>("File");
            //Move select textures
            //selectionText = Content.Load<SpriteFont>("AttackFont");
            selectionBGTxt=Content.Load<Texture2D>("SelectionBG");
            //load temp player texture
            playerTexture = Content.Load<Texture2D>("mayor_anim");
            enemyTexture = Content.Load<Texture2D>("slime_idle");
            //item textures
            hpPotTexture = Content.Load<Texture2D>("HealthPotion.png");
            //instantiate Tile Controller
            tileController = new TileController(26,26);
            //create first level with filepath 
            tileController.CreateLevel1("..\\..\\..\\..\\Levels\\Oomph.txt");
            //player rectangle is set to the find rectangle point
            entrance1 = tileController.FindEntrance(1);
            exit1 = tileController.FindExit(1);
            playerRectangle.Location = new Point(entrance1.X * 32, entrance1.Y * 32);

            //gamestate textures
            pauseMenu = Content.Load<Texture2D>("pause screen");

            //player menu texture
            playerMenuTexture = Content.Load<Texture2D>("MenuDrop");

            //menu nodes and menu stack
            //why on earth did I use a tree here
            menuRoot = new MenuNode("menu", MenuItem.root);
            menuRoot.Add(new MenuNode("items", MenuItem.menu));
            menuRoot.Add(new MenuNode("attacks", MenuItem.menu));
            menuRoot.Add(new MenuNode("back", MenuItem.back));
            //add items here
            menuRoot.MenuItems.ElementAt<MenuNode>(0).Add(new MenuNode("use health potion", MenuItem.item));
            menuRoot.MenuItems.ElementAt<MenuNode>(0).Add(new MenuNode("back", MenuItem.back));
            //add attacks here
            menuRoot.MenuItems.ElementAt<MenuNode>(1).Add(new MenuNode("WEAK ATTACK", MenuItem.attack));
            menuRoot.MenuItems.ElementAt<MenuNode>(1).Add(new MenuNode("back", MenuItem.back));

            characterController = new CharacterController(26, 26);
            player = new Player(characterController, tileController, 10, playerRectangle, playerTexture);
            enemy = new Enemy(characterController, tileController, 10, enemyRectangle, enemyTexture, player);
            enemy2 = new Enemy(characterController, tileController, 10, enemyRectangle2, enemyTexture, player);

            potOne = new HealthPotion(10, potOneRect, hpPotTexture);
            potTwo = new HealthPotion(10, potTwoRect, hpPotTexture);
            potThree = new HealthPotion(10, potThreeRect, hpPotTexture);
            items.Add(potOne);
            items.Add(potTwo);
            items.Add(potThree);
            mainMenu = Content.Load<Texture2D>("Title screen");
            //loading shader stuff
            effect1 = Content.Load<Effect>("lighteffect");
            lightMask = Content.Load<Texture2D>("lightmask");
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            Random rng = new Random();
            
            foreach (Item i in items)
            {
                i.Populate(tileController, 1, rng);
            }

            //menu stack
            menuStack = new MenuStack(menuRoot, player);
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
        /// private bool SingleKeyPress(Keys key)//Checks if enter has been pressed for more than one frame
        //single key press is renamed to previous keyboard state
        public bool SingleKeyPress(Keys key)
        {
            //checks to see if the key is pressed now, but not pressed before
            if(kbState.IsKeyDown(key) && previousKbState.IsKeyUp(key))
            {
                return true;
            }
            return false;
        }
        protected override void Update(GameTime gameTime)
        {
            previousKbState = kbState;
            kbState = Keyboard.GetState();

            //this menu logic is commented out
            #region menu logic
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (gState == GameState.mainMenu)
            {
                if (kbState.IsKeyDown(Keys.Enter))
                {
                    gState = GameState.gamePlay;
                }
            }
            if (gState == GameState.gamePlay)
            {
                if (kbState.IsKeyDown(Keys.Space))
                {
                    gState = GameState.pauseMenu;
                }
                if (kbState.IsKeyDown(Keys.F))
                {
                    gState = GameState.moveSelect;
                }
            }
            if (gState == GameState.pauseMenu)
            {
                if (kbState.IsKeyDown(Keys.Enter))
                {
                   gState=GameState.gamePlay;//Returns to Gameplay state if enter is pressed
                }
                else if (kbState.IsKeyDown(Keys.X))
                {
                    gState = GameState.mainMenu;//Returns to menu if x is pressed
                }
            }
            if (gState == GameState.moveSelect)
            {//Exiting the menu will be allowed once an attack method has been implemented
                exitVector = new Vector2(20, 70);//Scrolling through move options
                
                
                if (mState == MenuState.exit)//Allows scrolling through the menu
                {
                    if (SingleKeyPress(Keys.Up))
                    {
                        mState = MenuState.item;
                    }
                    else if (SingleKeyPress(Keys.Down))
                    {
                        mState = MenuState.strongAttack;
                    }
                    else if (kbState.IsKeyDown(Keys.Enter))
                    {
                        gState = GameState.gamePlay;
                    }
                }
                else if (mState == MenuState.item)
                {
                    if (SingleKeyPress(Keys.Up))
                    {
                        mState = MenuState.attack;
                    }
                    else if (SingleKeyPress(Keys.Down))
                    {
                        mState = MenuState.exit;
                    }
                    else if (SingleKeyPress(Keys.Enter))
                    {
                        gState = GameState.itemSelect;
                    }
                }
                else if (mState == MenuState.attack)
                {
                    if (SingleKeyPress(Keys.Up))
                    {
                        mState = MenuState.strongAttack;
                    }
                    else if (SingleKeyPress(Keys.Down))
                    {
                        mState = MenuState.item;
                    }
                }
                else if (mState == MenuState.strongAttack)
                {
                    if (SingleKeyPress(Keys.Up))
                    {
                        mState = MenuState.exit;
                    }
                    else if (SingleKeyPress(Keys.Down))
                    {
                        mState = MenuState.attack;
                    }
                }
            
            }
            if (gState == GameState.itemSelect)//Item logic
            {
                if (iState == ItemState.bomb)
                {
                    if (potionActive == true)//What is selected if potion is or isn't activated and down is pressed
                    {
                        //coordinate change
                        if (SingleKeyPress(Keys.Down))
                        {
                            iState = ItemState.potion;
                        }
                    }
                    else
                    {
                        if (SingleKeyPress(Keys.Down))
                        {
                            iState = ItemState.exit;
                        }
                    }
                    if (SingleKeyPress(Keys.Up))//What happens when up is pressed, whethere or not potion is activated
                    {
                        iState = ItemState.exit;
                    }
                    //will put coordinates after assets have been added in.
                }
                else if (iState==ItemState.potion)
                {
                    if (SingleKeyPress(Keys.Enter))//If potion is used, heals and gets rid of potion
                    {
                        player.Health += 5;
                        potionActive = false;
                        gState = GameState.gamePlay;
                    }
                    if (bombActive == true)//What is selected if bomb is or isn't activated and up is pressed
                    {
                        if (SingleKeyPress(Keys.Up))
                        {
                            iState = ItemState.bomb;
                        }
                    }
                    else
                    {
                        if (SingleKeyPress(Keys.Up))
                        {
                            iState = ItemState.exit;
                        }
                    }
                    if (SingleKeyPress(Keys.Down))//What happens when down is pressed, whethere or not bomb is activated
                    {
                        iState = ItemState.exit;
                    }
                }
                else if (iState == ItemState.exit)
                {
                    if (bombActive == true && potionActive == true)//if both items are active
                    {
                        if (SingleKeyPress(Keys.Up))
                        {
                            iState = ItemState.potion;
                        }
                        else if (SingleKeyPress(Keys.Down))
                        {
                            iState = ItemState.bomb;
                        }
                    }
                    else if (bombActive == true && potionActive != true)//if only bomb is active
                    {
                        if (SingleKeyPress(Keys.Up) || SingleKeyPress(Keys.Down))
                        {
                            iState = ItemState.bomb;
                        }
                    }
                    else if (bombActive != true && potionActive == true)//if only potion is active
                    {
                        if (SingleKeyPress(Keys.Up) || SingleKeyPress(Keys.Down))
                        {
                            iState = ItemState.potion;
                        }
                    }
                    if (SingleKeyPress(Keys.Enter))//if enter is pressed, go back to move select
                    {
                        gState = GameState.moveSelect;
                    }
                }
                if (bombActive == true && potionActive == true)
                {
                    bombVector = new Vector2(20, 40);
                    potionVector = new Vector2(20, 50);
                    exitVector = new Vector2(20, 60);
                }
                else if (bombActive == true && potionActive != true)
                {
                    bombVector = new Vector2(20, 40);
                    exitVector = new Vector2(20, 50);
                }
                else if (bombActive != true && potionActive == true)
                {
                    potionVector = new Vector2(20, 40);
                    exitVector = new Vector2(20, 50);
                }
                else
                {
                    exitVector = new Vector2(20, 40);
                }
            }

            // TODO: Add your update logic here
            */
            #endregion

            switch (gState)
            {
                case GameState.gamePlay:

                    //gamestate logic
                    //if the menu is open, don't update the player or the enemies
                    if(!menuStack.update(kbState, previousKbState))
                    {
                        player.Update(kbState, previousKbState);
                        enemy.Update(kbState);
                        enemy2.Update(kbState);
                    }
                    foreach(Item item in items)
                    {
                        item.Update(player);
                    }
                    camera.Follow(player);
                    if (player.dead)
                        gState = GameState.gameOver;
                    break;
                case GameState.mainMenu:
                    //menu state logic

                    break;
                case GameState.pauseMenu:
                    //pause menu logic

                    break;
                case GameState.gameOver:
                    //game over logic

                    break;
                case GameState.winState:
                    //winstate logic

                    break;
            }
            //characterController.TakeTurns(kbState);

            //update gamestate using menu controller
            gState = menuController.GameStateUpdate(kbState, previousKbState, gState, player, exit1);

            base.Update(gameTime);
        }

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //shader drawing stuff
            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, transformMatrix: camera.Transform);
            //draw light mask where there should be torches etc...
            spriteBatch.Draw(lightMask, new Rectangle(new Point(player.PlayerRectangle.X - lightMask.Width, player.PlayerRectangle.Y - lightMask.Height), new Point(lightMask.Width * 2,lightMask.Height * 2)) , Color.White);
            //spriteBatch.Draw(lightMask, new Vector2(X, Y), Color.White);

            spriteBatch.End();


            //begin
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.Transparent);
            
            
            if (gState == GameState.gamePlay)//only transform stuff if in the gamestate
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: camera.Transform);
            }
            //if not in gamestate, do not transform
            else
            {
                spriteBatch.Begin();
            }

            //commented out draw logic
            #region draw logic
            /*
            if (gState == GameState.moveSelect || gState == GameState.itemSelect)
            {
                spriteBatch.Draw(selectionBGTxt, selectionRect, Color.White);
            }
            if (gState == GameState.moveSelect)//What shows up in the attack selection
            {
                if (mState == MenuState.exit)//changes colors if selected
                {
                    spriteBatch.DrawString(selectionText, "Exit", exitVector, Color.Blue);
                }
                else
                {
                    spriteBatch.DrawString(selectionText, "Exit", exitVector, Color.White);
                }
                if (mState == MenuState.attack)
                {
                    spriteBatch.DrawString(selectionText, "Attack(not functional)", attackVector, Color.Blue);
                }
                else
                {
                    spriteBatch.DrawString(selectionText, "Attack(not functional)", attackVector, Color.White);
                }
                if (mState == MenuState.strongAttack)
                {
                    spriteBatch.DrawString(selectionText, "Strong Attack(not functional)", strongVector, Color.Blue);
                }
                else
                {
                    spriteBatch.DrawString(selectionText, "Strong Attack(not functional)", strongVector, Color.White);
                }
                if (mState == MenuState.item)
                {
                    spriteBatch.DrawString(selectionText, "Items", itemVector, Color.Blue);
                }
                else
                {
                    spriteBatch.DrawString(selectionText, "Items", itemVector, Color.White);
                }
            }
            else if (gState == GameState.itemSelect)
            {
                if (iState == ItemState.exit)//changes colors if selected
                {
                    spriteBatch.DrawString(selectionText, "Exit", exitVector, Color.Blue);
                }
                else
                {
                    spriteBatch.DrawString(selectionText, "Exit", exitVector, Color.White);
                }
                if (bombActive == true)
                {
                    if (iState == ItemState.bomb)
                    {
                        spriteBatch.DrawString(selectionText, "Bomb(not functional)", bombVector, Color.Blue);
                    }
                    else
                    {
                        spriteBatch.DrawString(selectionText, "Bomb(not functional)", bombVector, Color.White);
                    }
                }
                if (potionActive == true)
                {
                    if (iState == ItemState.potion)
                    {
                        spriteBatch.DrawString(selectionText, "Potion", potionVector, Color.Blue);
                    }
                    else
                    {
                        spriteBatch.DrawString(selectionText, "Potion)", potionVector, Color.White);
                    }
                }
            }
            //end
            */
            #endregion
            //draw state logic
            switch (gState)
            {
                case GameState.gamePlay:
                    //gameplay draw logic
                    tileController.DrawLevel(spriteBatch, wallTexture, floorTexture, entranceTexture, exitTexture, 1);
                    player.Draw(spriteBatch);
                    enemy.Draw(spriteBatch);
                    enemy2.Draw(spriteBatch);
                    foreach(Item item in items)
                    {
                        item.Draw(spriteBatch);
                    }
                    
                    break;
                case GameState.mainMenu:
                    //draw a prompt
                    spriteBatch.DrawString(font, "Press enter to start", Vector2.Zero, Color.White);
                    spriteBatch.Draw(mainMenu, backGround, Color.White);
                    break;
                case GameState.pauseMenu:
                    //draw the pause menu
                    spriteBatch.Draw(pauseMenu, Vector2.Zero, Color.White);
                    break;
                case GameState.winState:
                    break;
                case GameState.gameOver:
                    player.Draw(spriteBatch);
                    spriteBatch.Draw(mainMenu, backGround, Color.White);
                    break;
            }
            spriteBatch.End();

            //splicing lights onto game
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            effect1.Parameters["lightMask"].SetValue(lightsTarget);
            effect1.CurrentTechnique.Passes[effect1.CurrentTechnique.Passes.Count-1].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            switch (gState)
            {
                case GameState.gamePlay:
                    //gameplay draw logic
                    //only if
                    spriteBatch.DrawString(font, "Potions: " + player.ItemsHeld.Count, potions, Color.White);
                    spriteBatch.DrawString(font, "Health: " + player.Health, new Vector2(potions.X, potions.Y + 40), Color.White);
                    //menu
                    menuStack.Draw(spriteBatch, playerMenuTexture, font);
                    break;
                case GameState.mainMenu:
                    //draw a prompt
                    spriteBatch.Draw(mainMenu, backGround, Color.White);
                    spriteBatch.DrawString(font, "Press enter to start", Vector2.Zero, Color.White);
                    break;
                case GameState.pauseMenu:
                    break;
                case GameState.winState:
                    break;
                case GameState.gameOver:
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
