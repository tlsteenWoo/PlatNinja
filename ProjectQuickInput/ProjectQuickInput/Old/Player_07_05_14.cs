using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput.Old
{
    /*
    public enum PlayerForm { STAND, CROUCH, PENCIL }
    public class Player : Entity
    {
        public PlayerForm currentForm;
        public PlayerForm desiredForm;
        readonly Vector2 standSize = new Vector2(30, 30);
        readonly Vector2 crouchSize = new Vector2(30, 10);
        readonly Vector2 pencilSize = new Vector2(15, 40);
        Vector2 modify;
        public bool isCrouching, isSprinting;
        Entity wallSlideTarget;
        Enemy enemyInGrasp;
        SpriteSheet idle;

        public Player(Vector2 Position)
            : base(Position, 30, 30, Color.White, Resources.GetTexture("Pixel"))
        {
            state.size = standSize;
            state.doCollideResponse = true;
            state.doGravity = true;
            state.m_mass = 1;
            idle = new SpriteSheet(Resources.GetTexture("Idle"), 30, 30, 3);
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
                Detach(state.effect == SpriteEffects.None);
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
                Detach(Forward < 0);
                return true;
            }
            return false;
        }
        public override bool CanJump()
        {
            return base.CanJump() || wallSlideTarget != null;
        }

        public bool Slide()
        {
            if (state.m_friction != 0)
            {
                state.m_friction = 0;
                return true;
            }
            return false;
        }
        public bool Unslide()
        {
            if (state.m_friction == 0)
            {
                state.m_friction = 1;
                return true;
            }
            return false;
        }
        public bool Sprint()
        {
            if (!isSprinting)
            {
                isSprinting = true;
                return true;
            }
            return false;
        }
        public bool Unsprint()
        {
            if (isSprinting)
            {
                isSprinting = false;
                return true;
            }
            return false;
        }
        public bool Pencil()
        {
            if (desiredForm != PlayerForm.PENCIL)
            {
                desiredForm = PlayerForm.PENCIL;
                return true;
            }
            return false;
        }
        public bool Crouch()
        {
            if (!isCrouching)
            {
                isCrouching = true;
                desiredForm = PlayerForm.CROUCH;
                return true;
            }
            return false;
        }
        public bool UnCrouch()
        {
            if (isCrouching)
            {
                isCrouching = false;
                desiredForm = PlayerForm.STAND;
                return true;
            }
            return false;
        }
        //attach to an enemy
        public bool Attach(Enemy e)
        {
            if (enemyInGrasp == null)
            {
                enemyInGrasp = e;
                state.doCollideResponse = false;
                state.doGravity = false;
                state.dontCollide = true;
                enemyInGrasp.Disable();
                return true;
            }
            return false;
        }
        //detach from the enemy in grasp and kill it
        public bool Detach(bool left)
        {
            if (wallSlideTarget != null)
                wallSlideTarget = null;
            if (enemyInGrasp != null)
            {
                enemyInGrasp.Die(left);
                state.doCollideResponse = true;
                state.doGravity = true;
                state.dontCollide = false;
                enemyInGrasp = null;
                return true;
            }
            return false;
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
            float wallSlideLimit = 50;
            if (wallSlideTarget != null)
            {
                Contact c = GetAABB(10,10,0,0).Collide(wallSlideTarget.GetAABB());
                if (!state.inAir || !c.isContacting)
                    wallSlideTarget = null;
                else
                {
                    if (state.velocity.Y > wallSlideLimit)
                        state.velocity.Y = wallSlideLimit;
                }
            }
            //UPDATE FORM
            if (currentForm != desiredForm)
            {
                state.position -= modify;
                modify = Vector2.Zero;
                switch (desiredForm)
                {
                    case PlayerForm.STAND:
                        state.size = standSize;
                        break;
                    case PlayerForm.CROUCH:
                        modify.Y = (standSize.Y - crouchSize.Y);
                        state.size = crouchSize;
                        break;
                    case PlayerForm.PENCIL:
                        state.size = pencilSize;
                        Vector2 dif = standSize - pencilSize;
                        modify.X = dif.X / 2;
                        modify.Y = dif.Y;
                        break;
                }
                state.position += modify;
                currentForm = desiredForm;
            }
            else
                if (state.inAir == false && !isCrouching && currentForm != PlayerForm.STAND)
                {
                    desiredForm = PlayerForm.STAND;
                }
            //STEADY ENEMY ATTACHMENT
            if (enemyInGrasp != null)
            {
                state.position = enemyInGrasp.state.position;
                state.position.Y -= (enemyInGrasp.state.size.Y);
                state.force = Vector2.Zero;
                state.velocity = Vector2.Zero;
            }
            //UPDATE ANIMATIONS
            idle.Update(dt);
        }
        protected override void OnPostUpdate(float dt)
        {
            base.OnPostUpdate(dt);
            if (enemyInGrasp != null)
            {
                state.inAir = false;
            }
        }
        /// <summary>
        /// Should the following functionality
        /// Attach player to enemy head when he jumps on them
        /// </summary>
        protected override void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            base.onColliding(E, C, thisIsAlphaDog);
            if (E is Enemy && C.contactType == ContactType.GROUND && enemyInGrasp == null && stateOld.inAir == true)
            {
                Attach(E as Enemy);
            }
            if (!(E is Enemy || E is Player || E is Spikes) && C.contactType == ContactType.WALL_LEFT || C.contactType == ContactType.WALL_RIGHT)
            {
                wallSlideTarget = E;
            }
        }

        public override void Draw()
        {
            //base.Draw(sb);
            //only flip when not 0, that way it retains the direction when the player stops
            if (state.velocity.X < 0) state.effect = SpriteEffects.FlipHorizontally;
            else if (state.velocity.X > 0) state.effect = SpriteEffects.None;
            if (currentForm == PlayerForm.STAND && enemyInGrasp == null)
            {
                if (Math.Abs(state.velocity.X) > 0.001f)
                {
                    World.spritebatch.Draw(Resources.GetTexture("Run"), state.position, null, state.color, state.rotation, Vector2.Zero, 1, state.effect, 0);
                }
                else
                {
                    idle.Draw(state.position, state.color, state.effect);
                }
            }
            else if (currentForm == PlayerForm.CROUCH || enemyInGrasp != null)
                World.spritebatch.Draw(Resources.GetTexture("Crouch"), state.position - modify, null, state.color, state.rotation, Vector2.Zero, 1, state.effect, 0);
            else if (currentForm == PlayerForm.PENCIL)
            {
                World.spritebatch.Draw(Resources.GetTexture("Jump"), state.position - modify, null, state.color, state.rotation, Vector2.Zero, 1, state.effect, 0);
            }
            //World.spritebatch.DrawString(Resources.GetFont("Default"), state.on
        }
    }
     */
}