using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    public class FallingPlatform : Entity
    {
        public FallingPlatform(Vector2 Position, float Width, float Height)
            : base(Position, Width, Height, Color.Red, Resources.GetTexture("Pixel"))
        {
        }

        protected override void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            base.onColliding(E, C, thisIsAlphaDog);
            if (C.contactType == ContactType.GROUND && (E is Player || E is Enemy))
            {
                state.doGravity = true;
            }
        }
    }
}
