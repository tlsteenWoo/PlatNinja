using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public enum MovementType
    {
        POSITION, VELOCITYSET, VELOCITYADD, FORCE
    }
    /// <summary>
    /// Contains the physics, collision, and render information for entities.
    /// Performs the newtonian physics update.
    /// Contains flags for behaviour.
    /// Contains states about entity.
    /// This struct contains perhaps too much information, consider separating out to physicsState, renderState, collsionState
    /// Encapsulate them in 1 entityState if need be.
    /// </summary>
    public struct EntityState
    {
        //toggle variables
        public bool doTileTexture, doGravity, doCollideResponse, dontCollide, doFrictionless;
        //state variables
        public bool inAir, noMove, noJump;
        //physics variables
        public float rotation;
        public float m_mass;
        public Vector2 position, velocity, force;
        //collision variables
        public Vector2 size;
        public AABBFrictionModel m_friction;
        //render variables (separate these from this struct?)
        public Color color;
        public SpriteEffects effect;
        public Texture2D texture; //not advisable to have a reference inside a struct

        public float p_inverseMass
        {
            get
            {
                if (m_mass == 0)
                {
                    return 0;
                }
                return 1 / m_mass;
            }
        }

        public void UpdatePhysics(float elapsedSeconds)
        {
            //integrate force
            velocity += force * p_inverseMass * elapsedSeconds;
            //clear force
            force.X = force.Y = 0;
            //integrate velocity
            position += velocity * elapsedSeconds;
        }
    }
    public class Entity : ICloneable
    {
        public EntityState state, stateOld;

        public Entity(Vector2 Position, float Width, float Height, Color Color, Texture2D Texture)
        {
            state.position = Position;
            state.size = new Vector2( Width, Height );
            state.color = Color;
            state.texture = Texture;
            state.m_friction.setAll(0,0,1,1);
        }

        //used to avoid precision error, doesnt work
        public AABB GetRelativeAABB(Vector2 Origin)
        {
            //get the normal min and max
            Vector2 min = state.position;
            Vector2 max = state.position + state.size;
            //push them towards the origin
            return new AABB(min - Origin, max - Origin);
        }
        public AABB GetAABB()
        {
            //return a truly bounding box (no padding)
            return GetAABB(0, 0, 0, 0);
        }
        public AABB GetAABB(float PadRight, float PadLeft, float PadUp, float PadDown)
        {
            return new AABB(state.position - new Vector2(PadLeft, PadUp), state.position + state.size + new Vector2(PadLeft + PadRight, PadUp + PadDown));
        }
        /// <summary>
        /// does not work, meant for sweep collisions
        /// </summary>
        /// <returns></returns>
        public AABB Get4dAABB()
        {
            //position based
            //float left = Math.Max(0, state.position.X - stateOld.position.X);
            //float right = Math.Max(0, stateOld.position.X - state.position.X);
            //float top = Math.Max(0, state.position.Y - stateOld.position.Y);
            //float down = Math.Max(0, stateOld.position.Y - state.position.Y);
            float left = 0;
            float fakeDeltaTime = 0.016f;
            if (state.velocity.X > 0)
                left = state.velocity.X * fakeDeltaTime;
            float right = 0;
            if (state.velocity.X < 0)
                right = -state.velocity.X * fakeDeltaTime;
            float top = 0;
            if (state.velocity.Y > 0)
                top = state.velocity.Y * fakeDeltaTime;
            float down = 0;
            if (state.velocity.Y < 0)
                down = -state.velocity.Y * fakeDeltaTime;
            return GetAABB(right, left, top, down);
        }
        public Vector2 GetCenterLocal()
        {
            return new Vector2(state.size.X / 2, state.size.Y / 2);
        }
        public Vector2 GetCenterWorld()
        {
            return state.position + GetCenterLocal();
        }

        /// <summary>
        /// Calls preUpdate, then updates the state (physics), then calls postUpdate
        /// </summary>
        /// <param name="elapsedSeconds">Delta time (in seconds)</param>
        public void Update(float elapsedSeconds)
        {
            OnPreUpdate(elapsedSeconds);
            //save the state
            stateOld = state;
            //reset the state
            state.inAir = true;
            state.noMove = false;
            state.UpdatePhysics(elapsedSeconds);
            if (state.doGravity == true)
                state.force += World.i.Gravity * state.m_mass;
            OnPostUpdate(elapsedSeconds);
        }

        /// <summary>
        /// Apply changes before the current and previous state are modified
        /// </summary>
        /// <param name="deltaTime">Elapsed Seconds</param>
        protected virtual void OnPreUpdate(float deltaTime)
        {

        }
        /// <summary>
        /// Apply change after the current and previous state were modified
        /// </summary>
        /// <param name="deltaTime">Elapsed Seconds</param>
        protected virtual void OnPostUpdate(float deltaTime)
        {

        }
        /// <summary>
        /// Perform a collision test, if these Entities collide, perform collision response
        /// </summary>
        /// <param name="entityB">The other entity</param>
        /// <returns>A contact containing the results of the collision detection, but no information of collision response</returns>
        public Contact DynamicCollide(Entity entityB)
        {
            AABB a = GetRelativeAABB(state.position), b = entityB.GetRelativeAABB(state.position);
            Contact contact = a.Collide(b);
            if (contact.isContacting)
            {
                //if (E.state.doCollideResponse)//handling 2 dynamic bodies
                //{
                Vector2 relativeVelocity = state.velocity - entityB.state.velocity;
                float dot = Vector2.Dot(relativeVelocity, contact.contactSurfaceTowardsA);
                Vector2 normalVelocityA = dot * contact.contactSurfaceTowardsA;
                //Calculate Impulse...
                Vector2 impulse = Vector2.Zero;
                float combinedInverseMass = (state.p_inverseMass + entityB.state.p_inverseMass);
                float restitution = 0;
                //Only create an impulse if these objects move towards eachother
                //get the vector from entity A to entity B and normalize
                Vector2 fromAtoB = entityB.GetCenterWorld() - this.GetCenterWorld();
                fromAtoB.Normalize();
                //get the dot product of velocity A
                float velocityDotA = Vector2.Dot(state.velocity, fromAtoB);
                //get the dot product of velocity B * -1
                float velocityDotB = -Vector2.Dot(entityB.state.velocity, fromAtoB);
                //if the dotA is greater than 0 it is moving towards B
                bool aTowardsB = velocityDotA > 0;
                //if the dotB is greater than 0 it is moving towards A
                bool bTowardsA = velocityDotB > 0;
                //if both are moving towards eachother, do the calculation
                bool calculateImpulse = false;
                if (aTowardsB && bTowardsA) calculateImpulse = true;
                //if one is moving towards the other, do the calculation if it is greater than the other
                else if (aTowardsB && velocityDotA > velocityDotB) calculateImpulse = true;
                else if (bTowardsA && velocityDotB > velocityDotA) calculateImpulse = true;
                if (calculateImpulse)
                {
                    impulse = (-(1 + restitution) * normalVelocityA) / combinedInverseMass; //... / inverse masses added together
                }
                //Calculate Friction...
                //create the cross product of the normal towards A
                Vector2 tangent = new Vector2(contact.contactSurfaceTowardsA.Y, -contact.contactSurfaceTowardsA.X);
                float tangentDot = Vector2.Dot(relativeVelocity, tangent);
                Vector2 frictionVelocity = tangentDot * tangent;
                float friction1 = 0;
                float friction2 = 0;
                //Obtain friction for the side these entities are touching
                switch (contact.contactType)
                {
                    case ContactType.WALL_LEFT:
                            friction1 = state.m_friction.p_left;
                            friction2 = entityB.state.m_friction.p_right;
                            break;
                    case ContactType.WALL_RIGHT:
                            friction1 = state.m_friction.p_right;
                            friction2 = entityB.state.m_friction.p_left;
                        break;
                    case ContactType.GROUND:
                        friction1 = state.m_friction.p_down;
                        friction2 = entityB.state.m_friction.p_up;
                        break;
                    case ContactType.CEILING:
                        friction1 = state.m_friction.p_up;
                        friction2 = entityB.state.m_friction.p_down;
                        break;
                }
                //multiplicative friction
                //todo: Create an option for different friction types
                float frictionCoefficient = friction1 * friction2;
                //same impulse calculation as above but with tangent/friction variables
                Vector2 frictionImpulse = ((-frictionCoefficient) * frictionVelocity) / combinedInverseMass;
                //apply some position based separation to prevent objects sinking into eachother through gravity/constant force
                Vector2 positionChange = ( contact.contactSurfaceTowardsA * contact.penetration );
                //if either entity is static, simply push the other one
                if (state.m_mass == 0)
                {
                    entityB.state.position -= positionChange;
                }
                else if (entityB.state.m_mass == 0)
                {
                    state.position += positionChange;
                }
                else //uh-oh
                {
                    //if entities are moving into eachother apply separation based on momentum
                    if (calculateImpulse)
                    {
                        //find the stronger entity, determine momentum along the collision axis
                        float strengthA = -Vector2.Dot(state.velocity * state.m_mass, contact.contactSurfaceTowardsA);
                        float strengthB = Vector2.Dot(entityB.state.velocity * entityB.state.m_mass, contact.contactSurfaceTowardsA);
                        float momentumA = Math.Max(0, strengthA);
                        float momentumB = Math.Max(0, strengthB);
                        float total = momentumA + momentumB;
                        //convert to percentages
                        if (momentumA != 0)
                        {
                            momentumA = momentumA / total;
                        }
                        if (momentumB != 0)
                        {
                            momentumB = momentumB / total;
                        }
                        //apply the momentum percentage so that weaker objects wont be sunk by stronger ones
                        state.position += (positionChange * momentumA);
                        entityB.state.position -= (positionChange * momentumB);
                    }
                    //otherwise separate bsed on mass (if they arent reacting to eachother then the smaller one should probably push away)
                    else 
                    {
                        Vector2 positionResolution = (positionChange / combinedInverseMass);
                        state.position += positionResolution * state.p_inverseMass;
                        entityB.state.position -= positionResolution * entityB.state.p_inverseMass;
                    }
                }

                //apply restitution impulse and friction impulse
                state.velocity += (impulse * state.p_inverseMass) +(frictionImpulse * state.p_inverseMass);
                entityB.state.velocity -= (impulse * entityB.state.p_inverseMass) +(frictionImpulse * entityB.state.p_inverseMass);

                //todo: apply supporting force based on the impulse

                //set our entityState flags
                if (contact.contactType == ContactType.GROUND)
                    state.inAir = false;
                if (contact.contactType == ContactType.CEILING)
                    entityB.state.inAir = false;
                //callbacks
                onColliding(entityB, contact, true);
                entityB.onColliding(this, contact, false);
                //global callback
                if (World.onEntityColliding != null)
                    World.onEntityColliding(this, new EntityCollisionEventArgs(this, entityB, contact));
            }
            return contact;
        }

        /// <summary>
        /// Typical Usage
        /// Do something TO entity 'E'
        /// NOT: Do something TO entity 'this'
        /// </summary>
        /// <param name="E">The other entity</param>
        protected virtual void onColliding(Entity E, Contact C, bool thisIsAlphaDog)
        {
            /*if (C.contactType == ContactType.GROUND)
            {
                if(!E.state.doFrictionless)
                    E.state.velocity.X = 0;
            }*/
        }

        public void Move(MovementType Movem, float Speed, bool UP, bool RIGHT, bool DOWN, bool LEFT)
        {
            if (state.inAir || state.noMove) return;
            Vector2 translate = new Vector2(
                ((RIGHT ? 1 : 0) + (LEFT ? -1 : 0)) * Speed,
                ((UP ? -1 : 0) + (DOWN ? 1 : 0)) * Speed);
            switch(Movem)
            {
                case MovementType.FORCE:
                    state.force += translate;
                    break;
                case MovementType.VELOCITYADD:
                    state.velocity += translate;
                    break;
                case MovementType.VELOCITYSET:
                    if(UP || DOWN)
                        state.velocity.Y = translate.Y;
                    if(RIGHT || LEFT)
                        state.velocity.X = translate.X;
                    break;
                case MovementType.POSITION:
                    state.position += translate;
                    break;
            }
        }

        public virtual bool Jump(float Strength)
        {
            if (CanJump())
            {
                state.velocity.Y = Strength;
                return true;
            }
            return false;
        }

        public virtual bool JumpForward(float Forward, float Strength)
        {
            if (CanJump())
            {
                state.velocity = new Vector2(Forward, Strength);
                return true;
            }
            return false;
        }

        public virtual bool AirStall()
        {
            if (state.inAir)
            {
                state.velocity.X = 0;
                return true;
            }
            return false;
        }

        public virtual bool CanJump()
        {
            return !state.inAir;
        }

        public virtual void Draw()
        {
            int tw = state.texture.Width, th = state.texture.Height;
            Vector2 scale = new Vector2(state.size.X / (float)tw, state.size.Y / (float)th);
            Vector2 origin = new Vector2((float)state.texture.Width / 2, (float)state.texture.Height / 2);
            if (state.doTileTexture)
            {
                //BROKEN
                World.spritebatch.Draw(state.texture, state.position+origin*scale, new Rectangle(0,0,tw,th), state.color, state.rotation, origin,
                    scale, state.effect, 0);
            }
            else
            {
                World.spritebatch.Draw(state.texture, state.position + origin * scale, null, state.color, state.rotation, origin,
                    scale, state.effect, 0);
            }
        }

        public Object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}