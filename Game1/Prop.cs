using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public class Prop : Entity
    {
        /// <summary>
        /// Creates some prop in the world based on the texture width and height
        /// </summary>
        /// <param name="Position">Position in world space</param>
        /// <param name="Texture">Texture to based size on</param>
        /// <param name="Mass">Defaults to 0.0f for a static object, specify a number greater than 0 for dynamics</param>
        public Prop(Vector2 Position, Texture2D Texture, float Mass = 0.0f)
            : base(Position, Texture.Width / World.i.pixelsPerMeter, Texture.Height / World.i.pixelsPerMeter, Color.White, Texture)
        {
            if (Mass != 0)
            {
                state.doGravity = true;
                state.doCollideResponse = true;
            }
            state.m_mass = Mass;
        }
    }
}
