using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Tile_Engine
{
    class ZombiesManager
    {
        int _nbZombies;                                             // Number of zombies
        public Zombie[] _zombieTab;                                 // Array containing the zombies
        AnimationSprite[,] _zombiesSprite;                          // Zombie sprites
        Rectangle[,] _zombieSrcRecSprite;                           // Search for the relevant part of the zombie sprite
        Random rndNumbers = new Random();                           // Must be declared to make random position for zombies
        public bool _soundEffect = true;
        SoundEffect[] _growlTab;

        public ZombiesManager(ref Level level, Game game, ref SpriteBatch spriteBatch, ContentManager content, ref SoundEffect[] growlTab, ref Player player)
        {
            _nbZombies = level._nbZombies;
            _zombieTab = new Zombie[_nbZombies];
            _growlTab = growlTab;
            #region INIT
            this._zombiesSprite = new AnimationSprite[this._nbZombies, 4];
            this._zombieSrcRecSprite = new Rectangle[this._nbZombies, 4];

            int tmpX = 0, tmpY = 0;

            // cree des zombies aleatoirement sur la map
            for (int i = 0; i < this._nbZombies; i++)
            {
                this._zombieTab[i] = new Zombie();
                for (int count = 0; count < 10000; count++)
                {
                    tmpX = rndNumbers.Next(800 / 16);
                    tmpY = rndNumbers.Next(600 / 16);
                    int aX = ((tmpX * 16) - player._posXpix) * ((tmpX * 16) - player._posXpix);
                    int aY = ((tmpY * 16) - player._posYpix) * ((tmpY * 16) - player._posYpix);
                    int distance = (int)Math.Sqrt(aX + aY);
                    if (distance > 110)
                    {
                        if (level._colideMap[tmpY][tmpX] == 0)
                        {
                            this._zombieTab[i]._posX = tmpX * 16;
                            this._zombieTab[i]._posY = tmpY * 16;
                            level._colideMap[tmpY][tmpX] = 3;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < this._nbZombies; i++)
            {
                this._zombiesSprite[i, 0] = new AnimationSprite(game, new AnimationDefinition()
                {
                    AssetName = "Zombie/Animations/Walk/zombie_down",
                    FrameRate = 10,
                    FrameSize = new Point(32, 48),
                    Loop = true,
                    NbFrames = new Point(4, 1)
                });
                _zombieSrcRecSprite[i, 0] = new Rectangle(0, 0, 32, 48);
                this._zombiesSprite[i, 1] = new AnimationSprite(game, new AnimationDefinition()
                {
                    AssetName = "Zombie/Animations/Walk/zombie_left",
                    FrameRate = 10,
                    FrameSize = new Point(32, 48),
                    Loop = true,
                    NbFrames = new Point(3, 1)
                });
                _zombieSrcRecSprite[i, 1] = new Rectangle(0, 48, 32, 48);
                this._zombiesSprite[i, 2] = new AnimationSprite(game, new AnimationDefinition()
                {
                    AssetName = "Zombie/Animations/Walk/zombie_up",
                    FrameRate = 10,
                    FrameSize = new Point(26, 48),
                    Loop = true,
                    NbFrames = new Point(4, 1)
                });
                _zombieSrcRecSprite[i, 2] = new Rectangle(0, 96, 32, 48);
                this._zombiesSprite[i, 3] = new AnimationSprite(game, new AnimationDefinition()
                {
                    AssetName = "Zombie/Animations/Walk/zombie_right",
                    FrameRate = 10,
                    FrameSize = new Point(32, 48),
                    Loop = true,
                    NbFrames = new Point(3, 1)
                });
                _zombieSrcRecSprite[i, 3] = new Rectangle(0, 144, 32, 48);
            }

            for (int i = 0; i < this._nbZombies; i++)
                for (int j = 0; j < 4; j++)
                    this._zombiesSprite[i, j].Initialize();
            #endregion

            #region LOAD_CONTENT

            for (int i = 0; i < this._nbZombies; i++)
                for (int j = 0; j < 4; j++)
                    this._zombiesSprite[i, j].LoadContent(spriteBatch);

            for (int i = 0; i < this._nbZombies; i++)
                _zombieTab[i]._fullZombieSprite = content.Load<Texture2D>("Zombie/Static/zombie_static");

            #endregion

        }

        public void update(ref Level level, ref Player player, GameTime gameTime)
        {
            for (int i = 0; i < this._nbZombies; i++)
            {
                if (this._zombieTab.ElementAt(i).isTimerEnabled(gameTime, ref level._colideMap, ref player, 800, 600) == false)
                {
                    int state = -1;
                    if ((state = this._zombieTab.ElementAt(i).isEnlightened(ref level._colideMap, ref player)) == 1) // If zombie is enlightened
                    {
                        this._zombieTab.ElementAt(i)._escape = true;
                        this._zombieTab.ElementAt(i)._walk = true;
                        this._zombieTab.ElementAt(i)._pause = false;
                    }
                    else // If zombie is chasing player or making a random move
                    {
                        if (state == -1)
                            this._zombieTab.ElementAt(i).randMove(ref level._colideMap, 800, 600, ref player);
                        else
                        {
                            int stateWithSound = -1;
                            stateWithSound = this._zombieTab.ElementAt(i).seePrey(ref level._colideMap, ref player);
                            if (stateWithSound == 0)
                            {
                                if (this._zombieTab.ElementAt(i).chasePlayer(ref level._colideMap, ref player, 800, 600) == true)
                                    _growlTab[3].Play();
                            }
                            else if (stateWithSound == 1)
                            {
                                if (_soundEffect == true)
                                {
                                    Random rand = new Random();
                                    int soundNb = rand.Next(0, 2);
                                    if (soundNb == 0)
                                        _growlTab.ElementAt(0).Play();
                                    else if (soundNb == 1)
                                        _growlTab.ElementAt(1).Play();
                                    else if (soundNb == 2)
                                        _growlTab.ElementAt(2).Play();
                                }
                                if (this._zombieTab.ElementAt(i).chasePlayer(ref level._colideMap, ref player, 800, 600) == true)
                                    _growlTab[3].Play();
                            }
                            else
                                this._zombieTab.ElementAt(i).randMove(ref level._colideMap, 800, 600, ref player);
                        }
                    }
                }
            }

            for (int i = 0; i < this._nbZombies; i++)
                if (this._zombieTab.ElementAt(i)._walk == true)
                    this._zombiesSprite[i, this._zombieTab.ElementAt(i)._zombieDirection].Update(gameTime);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /* We display the zombies */
            for (int i = 0; i < this._nbZombies; i++)
            {
                if (this._zombieTab.ElementAt(i)._walk == true && this._zombieTab.ElementAt(i)._pause == false)
                    this._zombiesSprite[i, this._zombieTab[i]._zombieDirection].Draw(gameTime, true, this._zombieTab[i]._posX - 16, this._zombieTab[i]._posY - 48);
                else
                    spriteBatch.Draw(this._zombieTab[i]._fullZombieSprite, new Rectangle(this._zombieTab[i]._posX - 16, this._zombieTab[i]._posY - 48, 32, 48), _zombieSrcRecSprite[i, this._zombieTab[i]._zombieDirection], Color.White);

            }
        }
    }
}
