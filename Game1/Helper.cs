using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public static class Helper
    {
        public static Random Random = new Random();
        public static float Max(params float[] Numbers)
        {
            float max = float.NegativeInfinity;
            for (int i = 0; i < Numbers.Length; i++)
            {
                max = Math.Max(max, Numbers[i]);
            }
            return max;
        }
    }
}
