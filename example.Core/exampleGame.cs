using System;
using System.Collections.Generic;
using System.Globalization;
using example.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace example.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings,
    /// and platform-specific configurations.
    /// </summary>
    public class exampleGame : Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;
        private TextureArray textureArray;
        private Effect effect;
        private Model model;

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings,
        /// initializes services like settings and leaderboard managers, and sets up the
        /// screen manager for screen transitions.
        /// </summary>
        public exampleGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;

            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);

            Content.RootDirectory = "Content";

            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations =
                DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            effect = Content.Load<Effect>("effect");
            model = Content.Load<Model>("hexagon");
            var texture1 = Content.Load<Texture2D>("square");

            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var elements = part.VertexBuffer.VertexDeclaration.GetVertexElements();
                    foreach (var e in elements)
                        System.Diagnostics.Debug.WriteLine(
                            $"{e.VertexElementUsage} {e.UsageIndex} {e.VertexElementFormat}"
                        );
                }
            }

            textureArray = new TextureArray(GraphicsDevice, texture1.Width, texture1.Height, 3);
            textureArray.Add(0, texture1);
            textureArray.Add(1, texture1);
            textureArray.Add(2, texture1);

            base.LoadContent();
        }

        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            var keyboardState = Keyboard.GetState();
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || keyboardState.IsKeyDown(Keys.Escape)
            )
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect
                .Parameters["Projection"]
                ?.SetValue(
                    Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.PiOver4,
                        (float)Window.ClientBounds.Width / (float)Window.ClientBounds.Height,
                        1,
                        3000
                    )
                );
            Vector3 cameraPosition = new Vector3(0, 0, 25);
            Vector3 lookAt = Vector3.Zero;
            Vector3 up = Vector3.Up;
            var view = Matrix.CreateLookAt(cameraPosition, lookAt, up);

            effect.Parameters["View"]?.SetValue(view);
            effect.Parameters["World"]?.SetValue(Matrix.Identity);
            effect.Parameters["Textures"]?.SetValue(textureArray);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // Swap in your custom effect
                    part.Effect = effect;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
