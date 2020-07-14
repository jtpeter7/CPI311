using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab06
{
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;

        SpriteFont font; 

        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;
        Random random;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Transform> transforms;

        Camera camera;
        Transform cameraTransform;

        int numberCollisions;

        //***Lab7: Multi Thread
        int lastSecondCollisions = 0;
        bool haveThreadRunning = false;
        //**********************************

        public Lab07()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += Vector3.Right * 2 * i; //avoid overlapping each sphere **CHANGE LATER**
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3(
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity =
                   direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 2.5f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);
            }

            //************** Lab 7 shit
            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        } 

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("font");
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();
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

            if (InputManager.IsKeyPressed(Keys.Space))
                AddSphere();

            InputManager.Update();
            time.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Transform transform in transforms)
                model.Draw(transform.World, camera.View, camera.Projection);

            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                           new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }


            //*** Lab 07: Print out Collision Num
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Collision" +lastSecondCollisions, Vector2.Zero, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //**** Lab 7 : Multi Thread Method
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
            transform.LocalPosition += Vector3.Right * 3 * (float)random.NextDouble();
            Rigidbody sphere = new Rigidbody();
            sphere.Transform = transform;
            sphere.Mass = 1;
            sphere.Acceleration = Vector3.Down * 9.81f;

            Vector3 direction = new Vector3(
                (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();

            sphere.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = transform.LocalScale.Y;
            sphereCollider.Transform = transform;
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(sphere);
        }


    }
}
