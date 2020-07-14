using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using CPI311.GameEngine;

namespace Assignment1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D Explorer;
        Random random = new Random(160);
        AnimatedSprite AnimateTheGuy;
        //Sprite theSpriteGuy;
        ProgressBar firstBar;
        ProgressBar secondBar;
        Sprite bonus;

        public Assignment1()
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Explorer = Content.Load<Texture2D>("explorer");
            bonus = Content.Load<Sprite>("Bonus");
            AnimateTheGuy = new AnimatedSprite(Explorer, 1);


            spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W)) {
                Explorer.Position += 5;
            }
            if (InputManager.IsKeyDown(Keys.S)) {

            }
            if (InputManager.IsKeyDown(Keys.A)) {

            }
            if (InputManager.IsKeyDown(Keys.D)) {

            }

            //Update animation right here

            //if ((AnimateTheGuy.Position – bonus.Position).Length() < 32){
            //    firstBar.Value += 10;
            //    bonus.Position = new Vector2(random.Next(-160,160), random.Next(-160, 160));
            //}



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            AnimateTheGuy.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
