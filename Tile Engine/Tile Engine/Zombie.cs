using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Tile_Engine
{
    class Zombie
    {
        public Texture2D _fullZombieSprite;
        public int _zombieDirection = 0;
        public bool _walk = false;
        public bool _moveHoriz, _moveRight, _pause;
        public int _walkExpire = 50;
        public int _posX, _posY;
        public float _timeSpan = 2.0f;
        public bool _escape = false;
        public bool _seePreySound = false;
        public int _randomDirection = -1;
        public RNGCryptoServiceProvider _rand = new RNGCryptoServiceProvider();

        public Zombie()
        {
        }

        public Texture2D getTexture()
        {
            return (this._fullZombieSprite);
        }

        public bool isTimerEnabled(GameTime gameTime, ref int[][] _colideMap, ref Player player, int width, int height)
        {
            if (this._escape == true)
            {
                this._timeSpan -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this._timeSpan <= 0) // If time's up, reset the timer to 2 seconds
                {
                    this._timeSpan = 2.0f;
                    this._escape = false;
                    this._randomDirection = -1;
                    return false;
                }
                else // If timer still enabled, continue escaping
                {
                    if (this._randomDirection == -1)
                        return (this.continueEscaping(ref _colideMap, ref player, width, height));
                    else
                        return (this.mustEscape(ref _colideMap, ref player, width, height)); // Return true
                }
            }
            return false;
        }

        public int isEnlightened(ref int[][] _colideMap, ref Player player)
        {
            int xA, yA, xB, yB;

            if (player._direction == 0) // down
            {
                xA = player._posXpix - 39; yA = player._posYpix + 108;
                xB = player._posXpix + 39; yB = player._posYpix + 108;
                if ((this._posX >= xA && this._posX <= xB) && (this._posY >= (player._posYpix + 15) && this._posY <= yA))
                {
                    for (int tmp = player._posY; tmp < (this._posY / 16); tmp++)
                        if (_colideMap[tmp / 16][player._posX / 16] == 1)
                            return -1;
                    return 1;
                }
            }
            else if (player._direction == 1) // left
            {
                xA = player._posXpix - 118; yA = player._posYpix - 39;
                xB = player._posXpix - 118; yB = player._posYpix + 39;
                if ((this._posY >= yA && this._posY <= yB) && (this._posX >= xA && this._posX <= (player._posXpix - 15)))
                {
                    for (int tmp = player._posX; tmp > (this._posX / 16); tmp--)
                        if (_colideMap[player._posY][tmp / 16] == 1)
                            return -1;
                    return 1;
                }
            }
            else if (player._direction == 2) // up
            {
                xA = player._posXpix - 39; yA = player._posYpix - 128;
                xB = player._posXpix + 39; yB = player._posYpix - 128;
                if ((this._posX >= xA && this._posX <= xB) && (this._posY >= yA && this._posY <= (player._posYpix - 15)))
                {
                   for (int tmp = player._posY; tmp > (this._posY / 16); tmp--)
                        if (_colideMap[tmp / 16][player._posX / 16] == 1)
                            return -1;
                    return 1;
                }
            }
            else if (player._direction == 3) // right
            {
                xA = player._posXpix + 118; yA = player._posYpix - 39;
                xB = player._posXpix + 118; yB = player._posYpix + 39;
                if ((this._posY >= yA && this._posY <= yB) && (this._posX >= (player._posXpix + 15) && this._posX <= xA))
                {
                    for (int tmp = player._posX; tmp < (this._posX / 16); tmp++)
                        if (_colideMap[player._posY][tmp / 16] == 1)
                            return -1;
                    return 1;
                }
            }
            return 0;
        }



        public int seePrey(ref int[][] _colideMap, ref Player player)
        {
            int aX = (_posX - player._posXpix) * (_posX - player._posXpix);
            int aY = (_posY - player._posYpix) * (_posY - player._posYpix);
            int distance = (int)Math.Sqrt(aX + aY);
            if (distance < 100)
            {
                if (this._seePreySound == false)
                {
                    this._seePreySound = true;
                    return 1;
                }
                return 0;
            }
            this._seePreySound = false;
            return -1;
        }

        public bool continueEscaping(ref int[][] _colideMap, ref Player player, int width, int height)
        {
            this._randomDirection = -1;
            int playerPos = -1;
            if (player._direction == 0) // down
                playerPos = 2;
            else if (player._direction == 1) // left
                playerPos = 3;
            else if (player._direction == 2) // up 
                playerPos = 0;
            else if (player._direction == 3) // right
                playerPos = 1;
            Random  rand = new Random();
            do
            {
                this._randomDirection = rand.Next(0, 3);
            }   while (this._randomDirection == playerPos);

            _colideMap[this._posY / 16][this._posX / 16] = 0;
            if (this._randomDirection == 0) // down
                this._posY += checkCollision(0, (_posY < height && _colideMap[(_posY + 1) / 16][_posX / 16] == 0));
            else if (this._randomDirection == 1) // left
                this._posX -= checkCollision(1, (_posX > 16 && _colideMap[_posY / 16][(_posX - 1) / 16] == 0));
            else if (this._randomDirection == 2) // up
                this._posY -= checkCollision(2, (_posY > 16 && _colideMap[(_posY - 1) / 16][_posX / 16] == 0));
            else if (this._randomDirection == 3) // right
                this._posX += checkCollision(3, (_posX < width && _colideMap[_posY / 16][(_posX + 1) / 16] == 0));
            _colideMap[this._posY / 16][this._posX / 16] = 3;
            return true;
        }

        public bool mustEscape(ref int[][] _colideMap, ref Player player, int width, int height)
        {
            _colideMap[this._posY / 16][this._posX / 16] = 0;
            if (this._randomDirection == 0) // down
                this._posY += checkCollision(0, (_posY < height && _colideMap[(_posY + 1) / 16][_posX / 16] == 0));
            else if (this._randomDirection == 1) // left
                this._posX -= checkCollision(1, (_posX > 16 && _colideMap[_posY / 16][(_posX - 1) / 16] == 0));
            else if (this._randomDirection == 2) // up
                this._posY -= checkCollision(2, (_posY > 16 && _colideMap[(_posY - 1) / 16][_posX / 16] == 0));
            else if (this._randomDirection == 3) // right
                this._posX += checkCollision(3, (_posX < width && _colideMap[_posY / 16][(_posX + 1) / 16] == 0));
            _colideMap[this._posY / 16][this._posX / 16] = 3;
            return true;
        }

        public bool chasePlayer(ref int[][] _colideMap, ref Player player, int width, int height)
        {
            this._moveHoriz = false;
            this._moveRight = false;

            #region IA ALGO
            if (player._posXpix > _posX) // joueur a droite du zombie
            {
                if (player._posYpix < _posY) // joueur en haut a droite du zombie
                {
                    if ((Math.Pow(player._posXpix, 2) + Math.Pow(_posX, 2)) <= (Math.Pow(player._posYpix, 2) + Math.Pow(_posY, 2)))
                    {
                        this._moveHoriz = true;
                        this._moveRight = true;
                    }
                    else
                    {
                        this._moveHoriz = false;
                        this._moveRight = false;
                    }
                }
                else if (player._posYpix == _posY)
                {
                    this._moveHoriz = true;
                    this._moveRight = true;
                }
                else // joueur en bas a droite du zombie
                {
                    if ((Math.Pow(player._posXpix, 2) + Math.Pow(_posX, 2)) <= (Math.Pow(player._posYpix, 2) + Math.Pow(_posY, 2)))
                    {
                        this._moveHoriz = true;
                        this._moveRight = true;
                    }
                    else
                    {
                        this._moveHoriz = false;
                        this._moveRight = true;
                    }
                }
            }
            else if (player._posXpix == _posX)
            {
                if (player._posYpix < _posY)    // joueur en haut
                {
                    this._moveHoriz = false;
                    this._moveRight = false;
                }
                else
                {
                    this._moveHoriz = false;
                    this._moveRight = true;
                }
            }
            else // joueur a gauche du zombie
            {
                if (player._posYpix < _posY) // joueur en haut a gauche du zombie
                {
                    if ((Math.Pow(player._posXpix, 2) + Math.Pow(_posX, 2)) <= (Math.Pow(player._posYpix, 2) + Math.Pow(_posY, 2)))
                    {
                        this._moveHoriz = true;
                        this._moveRight = false;
                    }
                    else
                    {
                        this._moveHoriz = false;
                        this._moveRight = false;
                    }
                }
                else if (player._posYpix == _posY)
                {
                    this._moveHoriz = true;
                    this._moveRight = false;
                }
                else // joueur en bas a gauche du zombie
                {
                    if ((Math.Pow(player._posXpix, 2) + Math.Pow(_posX, 2)) <= (Math.Pow(player._posYpix, 2) + Math.Pow(_posY, 2)))
                    {
                        this._moveHoriz = true;
                        this._moveRight = false;
                    }
                    else
                    {
                        this._moveHoriz = false;
                        this._moveRight = true;
                    }
                }
            }
            #endregion
            _colideMap[_posY / 16][_posX / 16] = 0;
            if (this._moveHoriz)
            {
                if (this._moveRight) // right
                    this._posX += checkCollision(3, (_posX < width && _colideMap[_posY / 16][(_posX + 1) / 16] == 0));
                else // left
                    this._posX -= checkCollision(1, (_posX > 16 && _colideMap[_posY / 16][(_posX - 1) / 16] == 0));
            }
            else
            {
                if (this._moveRight) // down                       
                    this._posY += checkCollision(0, (_posY < height && _colideMap[(_posY + 1) / 16][_posX / 16] == 0));
                else // up
                    this._posY -= checkCollision(2, (_posY > 16 && _colideMap[(_posY - 1) / 16][_posX / 16] == 0));
            }

            // If zombie touches player
            if (_colideMap[_posY / 16][(_posX + 1) / 16] == 2 || _colideMap[_posY / 16][(_posX - 1) / 16] == 2 ||
                 _colideMap[(_posY + 1) / 16][_posX / 16] == 2 || _colideMap[(_posY - 1) / 16][_posX / 16] == 2)
            {
                player.isHit();
                return true;
            }
            _colideMap[_posY / 16][_posX / 16] = 3;
            return false;
        }

        protected int checkCollision(int new_direction, bool condition)
        {
            _zombieDirection = new_direction;
            if (condition)
                return 1;
            else
            {
                this._moveRight = (this._moveRight ? false : true);
                return 0;
            }
        }

        public bool randMove(ref int[][] _colideMap, int _height, int _width, ref Player _player)
        {
            if (!this._walk)
            {
                byte[] randomNumber = new byte[4];
                this._rand.GetBytes(randomNumber);
                /* Random for horizontal or vertical movement and its direction */
                this._moveHoriz = ((randomNumber[0] % 2 == 0) ? true : false);
                this._moveRight = ((randomNumber[1] % 2 == 0) ? true : false);
                this._pause = (randomNumber[2] % 3 == 0) ? true : false;
                this._walk = true;
                this._walkExpire = randomNumber[3] % 50 + 1;
            }

            _colideMap[_posY / 16][_posX / 16] = 0;

            if (!this._pause)
            {
                if (this._moveHoriz)
                {
                    if (this._moveRight)
                        this._posX += checkCollision(3, (_posX < _width && _colideMap[_posY / 16][(_posX + 1) / 16] == 0));
                    else
                        this._posX -= checkCollision(1, (_posX > 16 && _colideMap[_posY / 16][(_posX / 16) - 1] == 0));
                }
                else
                {
                    if (this._moveRight)
                        this._posY += checkCollision(0, (_posY < _height && _colideMap[(_posY + 1) / 16][_posX / 16] == 0));
                    else
                        this._posY -= checkCollision(2, (_posY > 16 && _colideMap[_posY / 16 - 1][_posX / 16] == 0));
                }
            }
            _colideMap[_posY / 16][_posX / 16] = 3;
            this._walkExpire -= 1;
            if (this._walkExpire == 0)
                this._walk = false;
            return true;
        }
    }
}
