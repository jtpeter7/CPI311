using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using CPI311.GameEngine;
using System;
using System.Collections.Generic;

namespace lab08
{
    public class lab08 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont Font;
        Effect effect;
        Texture2D texture;
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        Model cube;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;
        Camera camera, topDownCamera;

        public lab08()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();

            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            time.Initialize();
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");
            Font = Content.Load<SpriteFont>("Font");
            gunSound = Content.Load<SoundEffect>("Gun");


            ScreenManager.Setup(true, 1920, 1080);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);


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

            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());
            foreach (Collider collider in colliders)
            {
                collider.Transform.Rotate(Vector3.Up, time.ElapsedGameTime);
                collider.Transform.Rotate(Vector3.Right, time.ElapsedGameTime);
                collider.Transform.Rotate(Vector3.Forward, time.ElapsedGameTime);

                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Blue.ToVector3();
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (cube.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Red.ToVector3();
                }
            }

                if (InputManager.IsKeyPressed(Keys.Space))
                {
                    soundInstance = gunSound.CreateInstance();
                    soundInstance.IsLooped = false;
                    soundInstance.Volume = 0.2f;
                    soundInstance.Pitch = 0.2f;
                    soundInstance.Play();
                }

                base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;

                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["Shininess"].SetValue(20f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in cube.Meshes)
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                            }
                    }
                }


                base.Draw(gameTime);
            }
        }
    }
}
