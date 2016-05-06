using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    //assigned numbers so that a contact types opposite is also its negative
    public enum ContactType
    {
        GROUND=1, WALL_LEFT=2, WALL_RIGHT=-2, CEILING=-1
    }

    public struct AABBFrictionModel
    {
        float m_left;
        float m_right;
        float m_up;
        float m_down;

        public float p_left
        {
            get { return m_left; }
            set { m_left = value; }
        }
        public float p_right
        {
            get { return m_right; }
            set { m_right = value; }
        }
        public float p_up
        {
            get { return m_up; }
            set { m_up = value; }
        }
        public float p_down
        {
            get { return m_down; }
            set { m_down = value; }
        }

        public void setAll(float a_value)
        {
            setAll(a_value, a_value, a_value, a_value);
        }
        public void setAll(float a_left, float a_right, float a_up, float a_down)
        {
            m_left = a_left;
            m_right = a_right;
            m_up = a_up;
            m_down = a_down;
        }
    }

    public struct AABB
    {
        Vector2 min, max;


        public AABB(Vector2 Min, Vector2 Max)
        {
            min = Min;
            max = Max;
        }

        public Contact Collide(AABB other)
        {
            Contact info = new Contact();
            Vector2 d1 = other.max - min, d2 = max - other.min;

            if (d1.X <= 0 || d2.X <= 0 || d1.Y <= 0 || d2.Y <= 0)
            {
                info.isContacting = false;

                return info;
            }
            info.isContacting = true;
            if (d1.X > d1.Y) d1.X = 0;
            else d1.Y = 0;
            if (d2.X > d2.Y) d2.X = 0;
            else d2.Y = 0;
            Vector2 resolve = d1;
            if (d1.LengthSquared() > d2.LengthSquared())
                resolve = -d2;
            info.contactSurfaceTowardsA = resolve;
            info.penetration = info.contactSurfaceTowardsA.Length();
            if (info.penetration != 0)
            {
                info.contactSurfaceTowardsA /= info.penetration;
                float rightdot = Vector2.Dot(info.contactSurfaceTowardsA, Vector2.UnitX);
                float contactAngle = MathHelper.ToDegrees((float)Math.Acos(rightdot)) *
                    Math.Sign(Vector2.Dot(info.contactSurfaceTowardsA, Vector2.UnitY));
                contactAngle += 180;
                //contactAngle *= Math.Sign
                //can this be considered ground?
                if (contactAngle > 45 && contactAngle < 135)
                    info.contactType = ContactType.GROUND;
                else if (contactAngle < 225)
                    info.contactType = ContactType.WALL_LEFT;
                else if(contactAngle < 315)
                    info.contactType = ContactType.CEILING;
                else
                    info.contactType = ContactType.WALL_RIGHT;
            }
            return info;
        }

        public bool Contains(Vector2 Point)
        {
            if (Point.X < min.X) return false;
            if (Point.X > max.X) return false;
            if (Point.Y < min.Y) return false;
            if (Point.Y > max.Y) return false;
            return true;
        }
    }
}
