using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace Tile_Engine
{
    class Save
    {
        /* Constantes */
        public int Y_MAX = 38;                                      // Map maximum width
        public int X_MAX = 50;                                      // Map maximum length

        /* Map */
        public int _lvl;                                            // Number of current level
        public int[][] _colideMap;                                  // Collision map

        /* Keys */
        public Key[] _keyTab;                                       // Keys array
        public int _keyLvl;                                         // Level key

        /* Doors */
        public Door[] _doorTab;                                     // Doors array

        /* Zombies */
        public int _nbZombies;                                      // Number of zombies
        public int[][] _posZombies;                                 // Position of zombies

        /* Joueur */
        public int _nbKey;                                          // Player's number of keys
        public int _posX, _posY;                                    // Player's position

        /* save.data :
          - Numero LVL
          - Nombre de clefs joueur
          - Nombre Zombies
          - Position joueur
          - Positions chaque zombies
        */

        /* Save the current game */
        public void save(ref Level level, ref ZombiesManager zombiesManager, ref Player player)
        {
            /* We save the collision map */
            String[] colMap = new String[level.Y_MAX];
            for (int y = 0; y < level.Y_MAX; ++y)
            {
                colMap[y] = "";
                for (int x = 0; x < level.X_MAX; ++x)
                    colMap[y] += level._colideMap[y][x] + ",";
            }
            File.WriteAllLines("Content/Save/save.col", colMap);

            /* We save all data */
            String[] dataMap = new String[level._nbZombies + 4];
            dataMap[0] = "" + level._lvl;
            dataMap[1] = "" + player._nbKeys;
            dataMap[2] = "" + level._nbZombies;
            dataMap[3] = "" + player._posX + ";" + player._posY;
            for (int i = 4; i < (level._nbZombies + 4); ++i)
                dataMap[i] = "" + zombiesManager._zombieTab[i - 4]._posX + ";" + zombiesManager._zombieTab[i - 4]._posY;
            File.WriteAllLines("Content/Save/save.data", dataMap);
        }

        /* Loads savegame */
        public bool load()
        {
            String[] subline;
            String line = "";
            String nbTmp;
            _keyLvl = 0;
            int index, valTmp;

            /* Loads all data by reading a file */
            TextReader readData = File.OpenText("Content/Save/save.data");

            /* We check if the file is not empty, otherwise we return false */
            line = readData.ReadLine();
            if (line.Equals("@"))
            {
                readData.Close();
                return false;
            }

            /* We stock all data */
            _lvl = Convert.ToInt32(line);                       // Level
            line = readData.ReadLine();
            _nbKey = Convert.ToInt32(line);                     // Player's number of keys
            line = readData.ReadLine();
            _nbZombies = Convert.ToInt32(line);                 // Number of zombies
            line = readData.ReadLine();
            subline = line.Split(';');
            _posX = Convert.ToInt32(subline[0]);                // Player's posX
            _posY = Convert.ToInt32(subline[1]);                // Player's posY
            _posZombies = new int[_nbZombies][];
            for (int i = 0; i < _nbZombies; ++i)                // Position of zombies
            {
                line = readData.ReadLine();
                _posZombies[i] = new int[2];
                subline = line.Split(';');
                _posZombies[i][0] = Convert.ToInt32(subline[0]);
                _posZombies[i][1] = Convert.ToInt32(subline[1]);
            }

            /* We load collision map */
            TextReader readCol = File.OpenText("Content/Save/save.col");

            /* We check if the file is not empty, otherwise we return false */
            if (line.Equals("@"))
            {
                readData.Close();
                readCol.Close();
                return false;
            }

            /* We load collision map */
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
                    line = line.Remove(0, index + 1);
                }
            }

            /* We count the number of keys and doors remaining in the level */
            for (int y = 0; y < Y_MAX; ++y)
            {
                for (int x = 0; x < X_MAX; ++x)
                {
                    if (_colideMap[y][x] == 4)
                        _keyLvl++;
                }
            }

            /* We fill the arrays of keys and doors */
            _keyTab = new Key[_keyLvl];
            _doorTab = new Door[_keyLvl + _nbKey];
            int temp1 = 0; int temp2 = 0;
            for (int y = 0; y < Y_MAX; ++y)
            {
                for (int x = 0; x < X_MAX; ++x)
                {
                    if (_colideMap[y][x] == 4)
                    {
                        _keyTab[temp1] = new Key();
                        _keyTab[temp1]._posX = x;
                        _keyTab[temp1]._posY = y;
                        temp1++;
                    }
                    if (_colideMap[y][x] == 5)
                    {
                        _doorTab[temp2] = new Door();
                        _doorTab[temp2]._posX = x;
                        _doorTab[temp2]._posY = y;
                        temp2++;
                    }
                }
            }
            readData.Close();
            readCol.Close();
            return true;
        }
    }
}
