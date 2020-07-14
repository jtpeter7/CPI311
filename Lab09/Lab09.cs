using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Lab09
{
    public class Lab09 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;
        Model enemy;

        TimeSpan totalTime;
        int index = 0;

        Random random = new Random();
        Camera camera;
        int size = 30; // 10x10 grid
        Transform cameraTransform;

        public Lab09()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            search = new AStarSearch(size, size); // size of grid 

            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            search.Search(); // A search is made here.

            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("Box2");
            enemy = Content.Load<Model>("Torus");

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(15, 30, 15);
            camera.Transform = cameraTransform;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
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

            if(InputManager.IsKeyPressed(Keys.Space))
                {
                    search = new AStarSearch(size, size);
                foreach (AStarNode node in search.Nodes)
                    if (random.NextDouble() < 0.2)
                        search.Nodes[random.Next(size), random.Next(size)].Passable = false;

                    search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)];
                    search.Start.Passable = true;
                    search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)];
                    search.End.Passable = true;

                    search.Search(); 

                    path = new List<Vector3>();
                    AStarNode current = search.End;
                    while (current != null)
                    {
                     path.Insert(0, current.Position);
                        current = current.Parent;
                    }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                       Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) *
                     Matrix.CreateTranslation(position), camera.View, camera.Projection);

            totalTime += gameTime.ElapsedGameTime;

            if (totalTime > TimeSpan.FromSeconds(1))
            {
                totalTime = totalTime.Subtract(TimeSpan.FromSeconds(1));
                index++;
                index = Math.Min(search.Nodes.Length - 2, index);
            }

            Vector3 pos1 = path[index];
            Vector3 pos2 = path[index+1];
            Vector3 diff = pos2 - pos1;

            (enemy.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.HotPink.ToVector3();
            enemy.Draw(Matrix.CreateScale(0.2f, 0.2f, 0.2f) *
                    Matrix.CreateTranslation(pos1 + diff), camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
