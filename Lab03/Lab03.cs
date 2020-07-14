using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model torusModel;
        Matrix torusWorld;
        Matrix view;
        Matrix projection;

        Vector3 torusPosition;
        float TorusScale;
        Vector3 torusRotation;
        Vector3 cameraPosition;

        public Lab03()
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            torusModel = Content.Load<Model>("Torus");
            torusPosition = new Vector3(0, 0, 0);
            TorusScale = 1.0f;
            torusRotation = Vector3.Zero;
            cameraPosition = Vector3.Backward*5;
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
            // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //   Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.D))
            {
                cameraPosition += Vector3.Right * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                cameraPosition += Vector3.Left * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.W))
            {
                cameraPosition += Vector3.Up * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.S))
            {
                cameraPosition += Vector3.Down * Time.ElapsedGameTime;
            }

            //Angles
            if (InputManager.IsKeyDown(Keys.Right))
            {
                torusPosition += Vector3.Right * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                torusPosition += Vector3.Left * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                torusPosition += Vector3.Up * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                torusPosition += Vector3.Down * Time.ElapsedGameTime;
            }



            torusWorld = Matrix.CreateScale(TorusScale) *
                Matrix.CreateFromYawPitchRoll(0, 0, 0) *
                Matrix.CreateTranslation(torusPosition);
            view = Matrix.CreateLookAt(cameraPosition,new Vector3(0,0,0)+Vector3.Forward, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);


            base.Update(gameTime);
        }
        


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            torusModel.Draw(torusWorld, view, projection);

            base.Draw(gameTime);
        }
    }
}
