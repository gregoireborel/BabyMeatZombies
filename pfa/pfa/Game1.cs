using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace pfa
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager   graphics;
        SpriteBatch             spriteBatch;
        AnimationSprite[]       AnimSprite = new AnimationSprite[4];
        int                     direction;
        int                     nbZombies;
        bool                    walk;
        Rectangle[]             srcRecSprite = new Rectangle[4];
        Texture2D               fullPlayerSprite;
        Zombie lol;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.AnimSprite[0] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "down",
                FrameRate = 10,
                FrameSize = new Point(24, 32),
                Loop = true,
                NbFrames = new Point(3, 1)
            });
            srcRecSprite[0] = new Rectangle(24, 64, 24, 32);
            this.AnimSprite[1] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "left",
                FrameRate = 10,
                FrameSize = new Point(24, 32),
                Loop = true,
                NbFrames = new Point(3, 1)
            });
            srcRecSprite[1] = new Rectangle(24, 96, 24, 32);
            this.AnimSprite[2] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "up",
                FrameRate = 10,
                FrameSize = new Point(24, 32),
                Loop = true,
                NbFrames = new Point(3, 1)
            });
            srcRecSprite[2] = new Rectangle(24, 0, 24, 32);
            this.AnimSprite[3] = new AnimationSprite(this, new AnimationDefinition()
            {
                AssetName = "right",
                FrameRate = 10,
                FrameSize = new Point(24, 32),
                Loop = true,
                NbFrames = new Point(3, 1)
            });
            srcRecSprite[3] = new Rectangle(24, 32, 24, 32);

            for (int i = 0; i < 4; i++)
                this.AnimSprite[i].Initialize();

            direction = 0;
            walk = false;
            nbZombies = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            for (int i = 0; i < 4; i++)
                this.AnimSprite[i].LoadContent(spriteBatch);
            fullPlayerSprite = Content.Load<Texture2D>("fullPlayerSprite");
            for (int i = 0; i < nbZombies; i++)
                ;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState   kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Up) || kbState.IsKeyDown(Keys.Down) || kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.Right))
                walk = true;
            else
                walk = false;
            if (kbState.IsKeyDown(Keys.Up))
                direction = 2;
            if (kbState.IsKeyDown(Keys.Down))
                direction = 0;
            if (kbState.IsKeyDown(Keys.Left))
                direction = 1;
            if (kbState.IsKeyDown(Keys.Right))
                direction = 3;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (walk == true)
                this.AnimSprite[direction].Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            if (walk == true)
                this.AnimSprite[direction].Draw(gameTime, true);
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(fullPlayerSprite, new Rectangle(0, 0, 24, 32), srcRecSprite[direction], Color.White);
                spriteBatch.End();
            }
          base.Draw(gameTime);
        }
    }
}
