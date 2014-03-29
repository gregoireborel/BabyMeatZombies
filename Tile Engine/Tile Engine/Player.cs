using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    class Player
    {
        public Texture2D    _fullPlayerSprite;
        public int          _direction = 0;
        public bool         _walk;
        public int          _posX, _posY;
        public int          _posXpix, _posYpix;
        public bool         _alive;
        public int          _hp;
        public int          _nbKeys;

        public              Player()
        {
            this._walk = false;
            this._alive = true;
            this._hp = 100;
            this._nbKeys = 0;
        }

        public Texture2D    getTexture()
        {
            return (this._fullPlayerSprite);
        }

        public bool         isWalking()
        {
            return this._walk;
        }
        
        public bool         isAlive()
        {
            return (this._alive);
        }

        public void getKey(int _keyPosX, int _keyPosY, Level _level)
        {
            int i = 0;

            _nbKeys += 1;
            while (i != _level._nbKeyDraw)
            {
                if (_level._keyTab[i]._posX == _keyPosX && _level._keyTab[i]._posY == _keyPosY)
                {
                    _level._keyTab[i]._posX = -1;
                    _level._keyTab[i]._posY = -1;
                }
                i++;
            }
            _level._nbKey -= 1;
        }

        public bool isHit()
        {
            int hit = 4000;

            this._alive = false;
            if (this._hp <= hit)
            {
                this._alive = false;
                this._hp = 0;
                return false;
            }
            else
            {
                this._hp -= hit;
                return true;
            }
        }

        public void newGame()
        {
            this._walk = false;
            this._alive = true;
            this._hp = 100;
            this._nbKeys = 0;
            this._direction = 0;
        }
    }
}
