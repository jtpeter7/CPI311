using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Lab11
{
    public class Lab11 : Game
    {
        class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D texture;
        Color background = Color.Blue;

        Button exitButton;
        List<GUIElement> guiElements;

        SpriteFont font;

        Dictionary<String, Scene> scenes;
        Scene currentScene;

        public Lab11()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            guiElements = new List<GUIElement>();
        }

        protected override void Initialize()
        {
            time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            scenes = new Dictionary<string, Scene>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("font");

            GUIGroup group = new GUIGroup();

            exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            exitButton.Action += ExitGame;
            group.Children.Add(exitButton);

            CheckBox optionBox = new CheckBox();
            optionBox.Text = "Full Screen";
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(50, 75, 300, 20);
            optionBox.Action += MakeFullScreen;
            group.Children.Add(optionBox);

            CheckBox playBox = new CheckBox();
            playBox.Text = "Play!";
            playBox.Texture = texture;
            playBox.Box = texture;
            playBox.Bounds = new Rectangle(50, 100, 300, 20);
            playBox.Action += GoToPlay;
            group.Children.Add(playBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            
            currentScene = scenes["Menu"];
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
            currentScene.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);
            currentScene.Draw();
            base.Draw(gameTime);
        }

        void ExitGame(GUIElement element)
        {
            background = (background == Color.White ? Color.Blue : Color.White);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);

            spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back", 
                Vector2.Zero, Color.Black);
            spriteBatch.End();

        }

        void MakeFullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.IsFullScreen, ScreenManager.Width + 1, ScreenManager.Height + 1);
        }
        void GoToPlay(GUIElement element)
        {
            currentScene = scenes["Play"];
        }
    }
}
