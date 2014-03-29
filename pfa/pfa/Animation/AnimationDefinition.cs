using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pfa
{
    class AnimationDefinition
    {
        public string   AssetName   { get; set; }
        public Point    FrameSize   { get; set; }
        public Point    NbFrames    { get; set; }
        public int      FrameRate   { get; set; }
        public bool     Loop        { get; set; }
    }
}
