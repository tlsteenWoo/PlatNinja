using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectQuickInput
{
    public struct WorldState
    {
        public Vector2 Gravity;
        public float pixelsPerMeter;
    }
    /// <summary>
    /// Collision event where Entity A checked for collision with Entity B.
    /// Then, returned a positive contact C.
    /// </summary>
    public class EntityCollisionEventArgs : EventArgs
    {
        public Entity a, b;
        public Contact c;

        public EntityCollisionEventArgs(Entity A, Entity B, Contact C)
        {
            a = A;
            b = B;
            c = C;
        }
    }
    public static class World
    {
        //scales the physical world into the visual world to improve precision
        public static EventHandler<EntityCollisionEventArgs> onEntityColliding;
        public static GraphicsDevice graphics;
        public static SpriteBatch spritebatch;
        public static Level curLevel;
        public static WorldState i;

        /// <summary>
        /// Initialize so we can perform calculations on gravity q
        /// </summary>
        public static void Initialize()
        {
            i = new WorldState();
            i.pixelsPerMeter = 10000;
            i.Gravity = Scale(new Vector2(0, 5000));
        }

        public static void CollisionProcess(float dt)
        {
            Entity a, b;
            for (int i = 0; i < curLevel.entities.Count; i++)
            {
                a = curLevel.entities[i];
                    for (int j = i + 1; j < curLevel.entities.Count; j++)
                    {
                        b = curLevel.entities[j];
                        if (a.state.dontCollide || b.state.dontCollide)
                            continue;
                        Contact c;
                        //todo: randomly do a or b first, then do the other
                        if (a.state.doCollideResponse)
                            c = a.DynamicCollide(b);
                        else if (b.state.doCollideResponse)
                            c = b.DynamicCollide(a);
                    }
            }
        }

        /// <summary>
        /// Creates a matrix used for rendering the physical world
        /// </summary>
        /// <returns>A spritebatch matrix</returns>
        public static Matrix GetRenderMatrix()
        {
            return Matrix.CreateScale(i.pixelsPerMeter);
        }

        /// <summary>
        /// Returns the scaled screen width
        /// </summary>
        /// <returns>Screen Width / Scale</returns>
        public static float GetWorldWidth()
        {
            return Scale(graphics.Viewport.Width);
        }

        /// <summary>
        /// Returns the scaled screen height
        /// </summary>
        /// <returns>Screen Height / Scale</returns>
        public static float GetWorldHeight()
        {
            return Scale(graphics.Viewport.Height);
        }

        /// <summary>
        /// converts the component from pixels to world units
        /// </summary>
        /// <param name="ComponentInPixels">The value in pixels to convert, typically x, y, width, or height</param>
        /// <returns>Component / Scale</returns>
        public static float Scale(float ComponentInPixels)
        {
            return ComponentInPixels / World.i.pixelsPerMeter;
        }

        /// <summary>
        /// Converts the vector from pixels to world units
        /// </summary>
        /// <param name="VectorInPixels">The vector in pixels to convert, typically position or size</param>
        /// <returns>Vector / Scale</returns>
        public static Vector2 Scale(Vector2 VectorInPixels)
        {
            return VectorInPixels / World.i.pixelsPerMeter;
        }

        /// <summary>
        /// Converts a vector from pixels to world units
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 ToWorld(Vector2 vectorPx)
        {
            return vectorPx / i.pixelsPerMeter;
        }
    }
}
