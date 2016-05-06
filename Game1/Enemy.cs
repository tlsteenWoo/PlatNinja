using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public class Enemy : Entity
    {
        bool isDead;
        public bool disable;
        public float elapsedAttackTime, attackTimeTarget = 1;

        public Enemy(Vector2 Position)
            : base(Position, World.Scale(30f), World.Scale(30f), Color.White, Resources.GetTexture("GribuGrabu"))
        {
            state.doGravity = true;
            state.doCollideResponse = true;
            state.m_mass = 5;
        }

        //basic enemy should flick off the screen
        public virtual void Die(bool Left)
        {
            state.doCollideResponse = false;
            state.dontCollide = true;
            state.velocity.X = World.Scale(150);
            if (Left)
                state.velocity.X = -state.velocity.X;
            isDead = true;

        }
        public virtual void Dying()
        {
            state.rotation += 1;
            if (state.position.Y > World.graphics.Viewport.Height)
            {
                World.curLevel.RemoveEntity(this);
            }
        }
        //incapacitates the enemy
        public virtual void Disable()
        {
            disable = true;
            state.texture = Resources.GetTexture("GribuGrabu_Squish");
        }

        /// <summary>
        /// Should provide the following functionality
        /// Handle death
        /// handle attack time
        /// </summary>
        protected override void OnPreUpdate(float dt)
        {
            base.OnPreUpdate(dt);
            elapsedAttackTime += dt;
            float distance = World.Scale(100), attackStrength = World.Scale(200);
            if (elapsedAttackTime > attackTimeTarget)
            {
                Vector2 player = World.curLevel.p.GetCenterWorld();
                Vector2 enemy = GetCenterWorld();
                Vector2 dif = player - enemy;
                if(dif.Length() < distance)
                {
                    JumpForward(Math.Sign(dif.X) * attackStrength, World.Scale(-500));
                    elapsedAttackTime = 0;
                }
            }
            if (isDead)
            {
                Dying();
            }
        }

        public override void Draw()
        {
            //only flip when not 0, that way it retains the direction when the player stops
            if (state.velocity.X < 0) state.effect = SpriteEffects.FlipHorizontally;
            else if (state.velocity.X > 0) state.effect = SpriteEffects.None;
            base.Draw();
        }
    }
}
