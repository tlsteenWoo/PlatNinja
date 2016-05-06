using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectQuickInput
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatNinja : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //pixel created at runtime, used for drawing rectangles
        public static Texture2D pixelRUNTIME;
        //gives us the ability to add and manipulate entities
        EditorBackend editor;
        //exposes the programs active state globally
        public static bool isActive;

        public PlatNinja()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //enable the cursor to help with editing
            IsMouseVisible = true;
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
            World.Initialize();
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
            //expose the graphics globally
            World.spritebatch = spriteBatch;
            World.graphics = GraphicsDevice;

            //create a 1x1 pixel
            pixelRUNTIME = new Texture2D(GraphicsDevice, 1, 1);
            pixelRUNTIME.SetData<Color>(new Color[] { Color.White });
            //Load resources for later use
            Resources.AddFont(Content.Load<SpriteFont>("Default"), "Default");
            //Is static pixelRuntime redundant if it can be accessed through resources?
            Resources.AddTexture(pixelRUNTIME, "Pixel");
            //Load Rojo's different stances
            Resources.AddTexture(Content.Load<Texture2D>("Idle"), "Idle");
            Resources.AddTexture(Content.Load<Texture2D>("Crouch"), "Crouch");
            Resources.AddTexture(Content.Load<Texture2D>("Run"), "Run");
            Resources.AddTexture(Content.Load<Texture2D>("Jump"), "Jump");
            //Load enemies
            Resources.AddTexture(Content.Load<Texture2D>("GribuGrabu"), "GribuGrabu");
            Resources.AddTexture(Content.Load<Texture2D>("GribuGrabu_Squish"), "GribuGrabu_Squish");
            //Load props
            Resources.AddTexture(Content.Load<Texture2D>("Ice"), "Ice");
            Resources.AddTexture(Content.Load<Texture2D>("Block"), "Block");
            Resources.AddTexture(Content.Load<Texture2D>("StuccoHouse"), "House");
            Resources.AddTexture(Content.Load<Texture2D>("StuccoHouse_Tall"), "House_Tall");
            Resources.AddTexture(Content.Load<Texture2D>("Cruiser"), "Car");
            World.curLevel = new Level();
            LoadLevel();
            editor = new EditorBackend();
        }
        void LoadLevel()
        {
            World.curLevel.AWholeNewWorld();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //Resources.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //set IsActive so we can know it globally
            isActive = IsActive;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update(dt);
            //Reload the startup level
            if (Input.IsKeyRelease(Keys.Tab))
                LoadLevel();
            //Perform any custom update logic
            if (World.curLevel.updateLogic != null)
                World.curLevel.updateLogic();

            int initialCount = World.curLevel.entities.Count;
            for(int i = 0; i < World.curLevel.entities.Count; i++)
            {
                World.curLevel.entities[i].Update(dt);
                int dif = World.curLevel.entities.Count - initialCount;
                i -= dif;
            }
            World.CollisionProcess(dt);
            if (Input.IsKeyDown(Keys.Down))
            {
                World.curLevel.p.Crouch();
                World.curLevel.p.Slide();
            }
            else if(Input.i.IsKeyRelease(Keys.Down))
            {
                World.curLevel.p.UnCrouch();
                World.curLevel.p.Unslide();
            }
            float moveSpeed = World.Scale(100);
                World.curLevel.p.Move(MovementType.VELOCITYSET, moveSpeed,
                    false, Input.IsKeyDown(Keys.Right),
                    false, Input.IsKeyDown(Keys.Left));
            float bound = World.Scale(200);
            float jump = World.Scale(750), bigJump = World.Scale(900);
            if (Input.RecentKeySequence(true, Keys.Down, Keys.Up, Keys.Space))
                World.curLevel.p.AirStall();
            else if (Input.RecentKeySequence(true, Keys.Down, Keys.Left, Keys.Down, Keys.Left, Keys.Space))
                World.curLevel.p.JumpForward(-bound * 3, -bigJump * 0.9f);
            else if (Input.RecentKeySequence(true, Keys.Down, Keys.Right, Keys.Down, Keys.Right, Keys.Space))
                World.curLevel.p.JumpForward(bound * 3, -bigJump * 0.9f);
            else if (Input.RecentKeySequence(true, Keys.Down, Keys.Left, Keys.Space))
                World.curLevel.p.JumpForward(-bound, -bigJump);
            else if (Input.RecentKeySequence(true, Keys.Down, Keys.Right, Keys.Space))
                World.curLevel.p.JumpForward(bound, -bigJump);
            else if (Input.RecentKeySequence(true, Keys.Down, Keys.Space))
                World.curLevel.p.Jump(-bigJump);
            else if (Input.IsKeyPress(Keys.Space))
            {
                World.curLevel.p.Jump(-jump);
            }
            if (World.curLevel.p.state.position.X < 0) World.curLevel.p.state.position.X = World.GetWorldWidth();
            else if (World.curLevel.p.state.position.X > World.GetWorldWidth()) World.curLevel.p.state.position.X = 0;

            editor.Update();

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.BackToFront,null,null,null,null,null,World.GetRenderMatrix());

            foreach (Entity thing in World.curLevel.entities)
            {
                thing.Draw();
            }
            spriteBatch.DrawString(Resources.GetFont("Default"), "X: " + World.curLevel.p.state.position.X + ", Y: " + World.curLevel.p.state.position.Y, new Vector2(0.05f, 0), Color.Red, 0, Vector2.Zero, 1 / World.i.pixelsPerMeter, SpriteEffects.None, 0);
            for (int i = 0; i < Input.sequence.Count; i++)
            {
                spriteBatch.DrawString(Resources.GetFont("Default"), Input.sequence[i] + " : ", new Vector2(0.5f, 0.5f + 0.5f * i), Color.Red);
            }
            spriteBatch.End();
            editor.Draw();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
