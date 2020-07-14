using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Effect effect;

        Model cube;
        Model sphere;
        Model torus;

        Player player;
        Agent agent;

        int timeSpent; //total time spent in game
        int aliensLeft; //amount of aliens left to catch



        public Assignment4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            cube = Content.Load<Model>("Box2");
            sphere = Content.Load<Model>("Sphere");
            torus = Content.Load<Model>("Torus");



            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH2"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN2");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Up * 50;
            camera.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2);

            player = new Player(terrain, Content, GraphicsDevice, camera, Light.Current);
            player.Transform.LocalPosition = new Vector3(0, 0, 0);
            player.Transform.LocalScale = new Vector3(10, 10, 10);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            time.Update(gameTime);

            player.Update();
            //Exit & Camera Controls
            if (InputManager.IsKeyDown(Keys.Escape))
                Exit();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.HotPink);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

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

            spriteBatch.Begin();

            player.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}