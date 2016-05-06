using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    public struct Contact
    {
        public ContactType contactType;
        public bool isContacting;
        public float penetration;
        public Vector2 contactPoint, contactSurfaceTowardsA;
    }
}
