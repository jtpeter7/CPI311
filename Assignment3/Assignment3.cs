using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CPI311.GameEngine;
using System;
using System.Threading;

namespace Assignment3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont spriteFont;
        Model model;
        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;

        Random random;
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        Camera camera;
        Transform cameraTransform;
        float SpeedBuggy = 1;

        //Lab07: Multi Thread
        int lastSecondCollisions = 0;
        int numberCollisions = 0;
        bool haveThreadRunning = false;

        public Assignment3()
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

            Time.Initialize();
            InputManager.Initialize();


            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            for (int i = 0; i < 2; i++)
            {
                AddSphere();
            }
            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

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
            model = Content.Load<Model>("Sphere");
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            spriteFont = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            //// TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();
            //Speed of animation
            for (int j = 0; j < rigidbodies.Count; j++)
            {
                rigidbodies[j].GameSpeed = SpeedBuggy;
            }

            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                        numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                           * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }

            if (InputManager.IsKeyPressed(Keys.Left))
            {
                SpeedBuggy = SpeedBuggy / 1.1f;
            }
            if (InputManager.IsKeyPressed(Keys.Right))
            {
                SpeedBuggy = SpeedBuggy * 1.1f;
            }

            if (InputManager.IsKeyPressed(Keys.Up))
            {
                AddSphere();
            }


            if (InputManager.IsKeyPressed(Keys.Down))
            {
                rigidbodies.RemoveAt(0);
                transforms.RemoveAt(0);
                colliders.RemoveAt(0);
            }

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                AddSphere();
            }

            Time.Update(gameTime);
            InputManager.Update();

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Transform transform in transforms)
            {
                model.Draw(transform.World, camera.View, camera.Projection);
            }

            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                           new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }

            //for (int i = 0; i < renderers.Count; i++) renderers[i].Draw();

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, "Collision: " + lastSecondCollisions, Vector2.Zero, Color.Black);
            spriteBatch.DrawString(spriteFont, "GameSpeed: " + SpeedBuggy, new Vector2(0,15), Color.Black);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.LocalPosition += Vector3.Right * ((float)random.NextDouble() - .5f);
            Rigidbody sphere = new Rigidbody();
            sphere.Transform = transform;
            sphere.Mass = .5f + (float)random.NextDouble();
            sphere.Acceleration = Vector3.Down * 9.81f;

            // other parameters 
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            sphere.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = transform.LocalScale.Y;
            sphereCollider.Transform = transform;
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(sphere);

            /*
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content,
                            GraphicsDevice, ..., "SimpleShading", 20f, texture);
            renderers.Add(renderer);
            */

        }


    }
}
