using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Tile_Engine
{
    class TileMap
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int MapWidth = 100;
        public int MapHeight = 100;
        public int index, valTmp;
        public string line, nbTmp;
        public int[][] _colideMap = new int[500][];

        /// <summary>
        /// Charge un fichier map.txt en tant que map
        /// </summary>
        public TileMap()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                _colideMap[y] = new int[500];
                for (int x = 0; x < MapWidth; x++)
                {
                    _colideMap[y][x] = 0;
                    thisRow.Columns.Add(new MapCell(0));
                }
                Rows.Add(thisRow);
            }
            /*Recuperation de la map dans un fichier txt*/
            TextReader readFile = File.OpenText("Content/map.txt");

            for (int x = 0; x < MapWidth && (line = readFile.ReadLine()) != null; x++)
            {
                _colideMap[x] = new int[50];
                for (int y = 0; y < MapHeight && line.Length > 0; y++)
                {
                    index = line.IndexOf(",");
                    nbTmp = line.Substring(0, index);
                    Int32.TryParse(nbTmp, out valTmp);
                    Rows[x].Columns[y].TileID = valTmp;
                    if (valTmp > 0)
                        if (valTmp <= 6 || valTmp == 35 || valTmp == 49 || valTmp == 59 || valTmp == 84 || valTmp == 85 || valTmp == 86 || valTmp == 56 || valTmp == 66 || valTmp == 76 || valTmp == 54)
                            _colideMap[x][y] = 0;
                        else
                            _colideMap[x][y] = 1;
                    line = line.Remove(0, index + 1);
                }
            }
        }

        public int[][] getColideMap()
        {
            return _colideMap;
        }
    }

    class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }
}
