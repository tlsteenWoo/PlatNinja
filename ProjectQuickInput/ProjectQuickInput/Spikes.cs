using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    public class Spikes : Entity
    {
        public Spikes(Vector2 Position, float Width, float Height) :
            base(Position, Width, Height, Color.White, Resources.GetTexture("Pixel"))
        {
        }

        protected override void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            base.onColliding(E, C,thisIsAlphaDog);
            E.state.position = new Vector2(200, 400);
        }
    }
}
