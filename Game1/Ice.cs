using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public class Ice : Entity
    {
        public Ice(Vector2 Position, float Width, float Height)
            : base(Position, Width, Height, Color.White, Resources.GetTexture("Ice"))
        {
            state.m_friction.setAll(0);
        }

        protected override void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            //base.onColliding(E, C);
            if(C.contactType == ContactType.GROUND && Math.Abs(E.state.velocity.X) > 0.01f)
                E.state.noMove = true;
        }
    }
}
