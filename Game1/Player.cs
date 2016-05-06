using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public enum PlayerForm { STAND, CROUCH, PENCIL }
    public class Player : Entity
    {
        Stance m_stance;
        SpriteSheet idle;

        public Player(Vector2 Position)
            : base(Position, World.Scale(30), World.Scale(30), Color.White, Resources.GetTexture("Pixel"))
        {
            m_stance = new Stance();
            state.doCollideResponse = true;
            state.doGravity = true;
            state.m_mass = 10;
            idle = new SpriteSheet(Resources.GetTexture("Idle"), 30, 30, 3);
            StanceModification crouchModification = new StanceModification();
            float twoThirds = 2f/3f;
            float crouchSquash = state.size.Y * twoThirds;
            crouchModification.p_positionOffset = new Vector2(0, crouchSquash);
            crouchModification.p_sizeOffset = new Vector2(0, -crouchSquash);
            m_stance.setStanceOption(STANCE_OPTION.CROUCH, crouchModification);
            StanceModification pencilModification = new StanceModification();
            float oneThird = 1f / 3f;
            float oneSixth = 1f / 6f;
            float pencilSqueeze = oneSixth * state.size.X;
            float pencilStretch = oneThird * state.size.Y;
            pencilModification.p_positionOffset = new Vector2(pencilSqueeze, -pencilStretch);
            pencilModification.p_sizeOffset = new Vector2(-(pencilSqueeze*2), pencilStretch);
            m_stance.setStanceOption(STANCE_OPTION.PENCIL, pencilModification);
        }

        /// <summary>
        /// should provide the following functionality
        /// switch into pencil form
        /// detach from enemy in grasp
        /// </summary>
        public override bool Jump(float Strength)
        {
            if (base.Jump(Strength))
            {
                Pencil();
                return true;
            }
            return false;
        }

        /// <summary>
        /// should provide the following functionality
        /// detach from enemy in grasp
        /// </summary>
        public override bool JumpForward(float Forward, float Strength)
        {
            if (base.JumpForward(Forward, Strength))
            {
                return true;
            }
            return false;
        }
        public override bool CanJump()
        {
            return base.CanJump();
        }

        public bool Slide()
        {
            if (state.m_friction.p_down != 0)
            {
                state.m_friction.p_down = 0;
                return true;
            }
            return false;
        }
        public bool Unslide()
        {
            if (state.m_friction.p_down == 0)
            {
                state.m_friction.p_down = 1;
                return true;
            }
            return false;
        }
        public bool Pencil()
        {
            return m_stance.changeStance(STANCE_OPTION.PENCIL);
        }
        public bool Crouch()
        {
            return m_stance.changeStance(STANCE_OPTION.CROUCH);
        }
        public bool UnCrouch()
        {
            return m_stance.changeStance(STANCE_OPTION.STAND);
        }
        /// <summary>
        /// should contain following functionality
        /// slide player when on wall
        /// Modify player size and position based on desired form
        /// Attach player to the enemy in grasp
        /// Update animations
        /// </summary>
        /// <param name="dt"></param>
        protected override void OnPreUpdate(float dt)
        {
            base.OnPreUpdate(dt);
            m_stance.update(this);
            //UPDATE ANIMATIONS
            idle.Update(dt);
        }
        protected override void OnPostUpdate(float dt)
        {
            base.OnPostUpdate(dt);
        }
        /// <summary>
        /// Should the following functionality
        /// Attach player to enemy head when he jumps on them
        /// </summary>
        protected override void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            base.onColliding(E, C, thisIsAlphaDog);
            //Change to standing stance when Rojo lands, unless he is crouching (we assume a crouch is intentional)
            if (!state.inAir && stateOld.inAir && m_stance.getCurrentStanceOption() != STANCE_OPTION.CROUCH)
            {
                m_stance.changeStance(STANCE_OPTION.STAND);
            }
        }

        public override void Draw()
        {
            //base.Draw(sb);
            //only flip when not 0, that way it retains the direction when the player stops
            if (state.velocity.X < 0) state.effect = SpriteEffects.FlipHorizontally;
            else if (state.velocity.X > 0) state.effect = SpriteEffects.None;
            if (m_stance.getCurrentStanceOption() == STANCE_OPTION.STAND)
            {
                if (Math.Abs(state.velocity.X) > 0.001f)
                {
                    World.spritebatch.Draw(Resources.GetTexture("Run"), state.position, null, state.color, state.rotation, Vector2.Zero, 1 / World.i.pixelsPerMeter, state.effect, 0);
                }
                else
                {
                    idle.Draw(state.position, state.color, state.effect, 1 / World.i.pixelsPerMeter);
                }
            }
            else if (m_stance.getCurrentStanceOption() == STANCE_OPTION.CROUCH)
                World.spritebatch.Draw(Resources.GetTexture("Crouch"), state.position - m_stance.getCurrentStanceModification().p_positionOffset, null, state.color, state.rotation, Vector2.Zero, 1 / World.i.pixelsPerMeter, state.effect, 0);
            else if (m_stance.getCurrentStanceOption() == STANCE_OPTION.PENCIL)
            {
                World.spritebatch.Draw(Resources.GetTexture("Jump"), state.position - m_stance.getCurrentStanceModification().p_positionOffset, null, state.color, state.rotation, Vector2.Zero, 1 / World.i.pixelsPerMeter, state.effect, 0);
            }
            //World.spritebatch.DrawString(Resources.GetFont("Default"), state.on
        }
    }
}