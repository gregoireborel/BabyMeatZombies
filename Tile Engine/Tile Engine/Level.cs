using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    class Level
    {
        /* Constantes */
        public int Y_MAX = 38;                                      // Map maximum width
        public int X_MAX = 50;                                      // Map maximum length

        /* Map */
        public int[][] _colideMap;                                  // Collision map
        public Texture2D _bg;                                       // Level background

        /* Keys */
        public Key[] _keyTab;                                       // Keys array
        public int _nbKey;                                          // Number of keys
        public int _nbKeyDraw;                                      // Number of keys for Draw function

        /* Doors */
        public Door[] _doorTab;                                     // Door array
        public int _nbDoor;                                         // Number of doors
        public int _nbDoorDraw;                                     // Number of doors for Draw function

        /* Level information */
        public int _lvl;                                            // Number of current level
        public Vector2 _spawn;                                      // Spawning level
        public int _nbZombies;                                      // Number of zombies

        public Level(int lvl, ContentManager content)
        {
            _lvl = lvl;
            loadMap(ref content);
        }

        private void loadMap(ref ContentManager content)
        {
            String line, nbTmp;
            int index, valTmp;
            int i = 0, j = 0;

            /* We set the background */
            _bg = content.Load<Texture2D>("Map/map_" + _lvl);

            /* We get the level informations*/
            TextReader readData = File.OpenText("Content/Map/data_" + _lvl + ".data");

            /* Number of keys */
            line = readData.ReadLine();
            line = line.Remove(0, 4);
            _nbKey = Convert.ToInt32(line);

            /* Number of doors */
            _nbDoor = _nbKey;

            /* Number of zombies */
            line = readData.ReadLine();
            line = line.Remove(0, 8);
            _nbZombies = Convert.ToInt32(line);

            /* Spawn */
            line = readData.ReadLine();
            line = line.Remove(0, 2);
            int xPos = Convert.ToInt32(line);
            line = readData.ReadLine();
            line = line.Remove(0, 2);
            int yPos = Convert.ToInt32(line);
            _spawn = new Vector2(xPos, yPos);

            /* We load the collision map */
            TextReader readCol = File.OpenText("Content/Map/col_" + _lvl + ".col");

            if (_nbKey > 0)
            {
                _keyTab = new Key[_nbKey];
                _doorTab = new Door[_nbDoor];
            }

            _colideMap = new int[Y_MAX][];
            for (int y = 0; y < Y_MAX && (line = readCol.ReadLine()) != null; ++y)
            {
                _colideMap[y] = new int[X_MAX];
                for (int x = 0; x < X_MAX && line.Length > 0; ++x)
                {
                    index = line.IndexOf(",");
                    nbTmp = line.Substring(0, index);
                    Int32.TryParse(nbTmp, out valTmp);
                    _colideMap[y][x] = valTmp;
                    if (valTmp == 4 && _nbKey > 0)
                    {
                        _keyTab[i] = new Key();
                        _keyTab[i]._posX = x;
                        _keyTab[i]._posY = y;
                        i++;
                    }
                    if (valTmp == 5 && _nbDoor > 0)
                    {
                        _doorTab[j] = new Door();
                        _doorTab[j]._posX = x;
                        _doorTab[j]._posY = y;
                        j++;
                    }
                    line = line.Remove(0, index + 1);
                }
            }

            _nbKeyDraw = _nbKey;
            _nbDoorDraw = _nbDoor;
            readCol.Close();
            readData.Close();
        }
    }
}
