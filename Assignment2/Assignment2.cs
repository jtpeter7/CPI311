using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        float solR = 5;
        float mercuryR = 2;
        float earthR = 3;
        float lunaR = 1;
        Model solarSystem;
        Model sol;
        Model mercury;
        Model earth;
        Model luna;
        Transform solarSystemTransform;
        Transform solTransform;
        Transform mercuryTransform;
        Transform earthTransform;
        Transform lunaTransform;
        Transform cameraTransform;
        Camera camera;

        public Assignment2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            solarSystem = Content.Load<Model>("solarSystem");
            sol = Content.Load<Model>("sol");
            mercury = Content.Load<Model>("mercury");
            earth = Content.Load<Model>("earth");
            luna = Content.Load<Model>("luna");
            solarSystemTransform = new Transform();
            solTransform = new Transform(); 
            mercuryTransform = new Transform(); mercuryTransform.LocalPosition += Vector3.Right*50;
            earthTransform = new Transform(); earthTransform.LocalPosition += Vector3.Left*75;
            lunaTransform = new Transform(); lunaTransform.LocalPosition += Vector3.Up*25;
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 100;
            camera = new Camera();
            camera.Transform = cameraTransform;
            mercuryTransform.Parent = solTransform;
            earthTransform.Parent = solTransform;
            lunaTransform.Parent = earthTransform;
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            solTransform.Rotate(Vector3.Forward, Time.ElapsedGameTime);
            earthTransform.Rotate(Vector3.Backward, Time.ElapsedGameTime*2);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            solarSystem.Draw(solarSystemTransform.World, camera.View, camera.Projection);
            sol.Draw(solTransform.World, camera.View, camera.Projection);
            mercury.Draw(mercuryTransform.World, camera.View, camera.Projection);
            earth.Draw(earthTransform.World, camera.View, camera.Projection);
            luna.Draw(lunaTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
