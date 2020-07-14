using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab02
{
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        spiralMover spiral;

        public Lab02()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            time.Initialize();
            spiralMover.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D temp = Content.Load<Texture2D>("Square");
            spiral = new spiralMover(temp, new Vector2(300, 250), 150);

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            time.Update(gameTime);
            InputManager.Update();
            spiral.Update();
            if (InputManager.IsKeyDown(Keys.Left))
                spiral.Radius -= 5;

            if (InputManager.IsKeyDown(Keys.Right))
                spiral.Radius += 5;

            if (InputManager.IsKeyDown(Keys.Up))
                spiral.speed += (float)0.1;

            if (InputManager.IsKeyDown(Keys.Down))
                spiral.speed -= (float) 0.1;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spiral.Draw(spriteBatch); //CPI311.GameEngine's Sprite
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
