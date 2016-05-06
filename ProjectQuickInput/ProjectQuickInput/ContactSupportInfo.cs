using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectQuickInput
{
    public enum SupportType
    {
        NONE,STATIC,DYNAMIC
    }
    public struct ContactSupportInfo
    {
        SupportType left;
        SupportType right;
        SupportType top;
        SupportType bottom;

        public void applyContactType(ContactType a_contactType, bool a_entityIsA, bool a_supportIsStatic)
        {
            SupportType type = SupportType.STATIC;
            if(!a_supportIsStatic)
                type= SupportType.DYNAMIC;
            if (!a_entityIsA)
            {
                a_contactType = (ContactType)(-(int)a_contactType);
            }
            switch (a_contactType)
            {
                case ContactType.CEILING:
                        top = type;
                    break;
                case ContactType.WALL_LEFT:
                    left = type;
                    break;
                case ContactType.WALL_RIGHT:
                    right = type;
                    break;
                case ContactType.GROUND:
                    bottom = type;
                    break;
            }
        }
    }
}
