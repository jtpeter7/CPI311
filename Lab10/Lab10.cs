using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab10
{
    public class Lab10 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Effect effect;


        public Lab10()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //*** MonoGame3.6 *******************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //***********************************************

        }

        protected override void Initialize()
        {
            time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            terrain = new TerrainRenderer(
                Content.Load<Texture2D>("Heightmap"),
                Vector2.One * 100, Vector2.One * 200);

            terrain.NormalMap = Content.Load<Texture2D>("Normalmap");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;
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

            if (InputManager.IsKeyDown(Keys.W)) 
            {
                camera.Transform.LocalPosition += camera.Transform.Forward * time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.S)) 
            {
                camera.Transform.LocalPosition += camera.Transform.Backward * time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.A)) 
            {
                camera.Transform.Rotate(Vector3.Up, time.ElapsedGameTime);
            }
            if (InputManager.IsKeyDown(Keys.D)) 
            {
                camera.Transform.Rotate(Vector3.Down, time.ElapsedGameTime);
            }

            camera.Transform.LocalPosition = new Vector3(camera.Transform.LocalPosition.X, terrain.GetAltitude(camera.Transform.LocalPosition), camera.Transform.LocalPosition.Z);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(camera.Transform.Position + Vector3.Up * 10);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
