using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectQuickInput
{
    public enum STANCE_OPTION { STAND, CROUCH, RISE, FALL, PENCIL }
    public struct StanceModification
    {
        Vector2 m_sizeOffset;
        Vector2 m_positionOffset;
        public Vector2 p_sizeOffset
        {
            get { return m_sizeOffset; }
            set { m_sizeOffset = value; }
        }
        public Vector2 p_positionOffset
        {
            get { return m_positionOffset; }
            set { m_positionOffset = value; }
        }
    }
    public class Stance
    {
        STANCE_OPTION m_desiredStance;
        STANCE_OPTION m_currentStance;
        Dictionary<STANCE_OPTION, StanceModification> m_stances;

        public Stance()
        {
            m_stances = new Dictionary<STANCE_OPTION, StanceModification>();
            m_stances.Add(STANCE_OPTION.STAND, new StanceModification());
            m_stances.Add(STANCE_OPTION.CROUCH, new StanceModification());
            m_stances.Add(STANCE_OPTION.RISE, new StanceModification());
            m_stances.Add(STANCE_OPTION.FALL, new StanceModification());
            m_stances.Add(STANCE_OPTION.PENCIL, new StanceModification());
        }

        public StanceModification getCurrentStanceModification()
        {
            return m_stances[m_currentStance];
        }
        public STANCE_OPTION getCurrentStanceOption()
        {
            return m_currentStance;
        }
        public void setStanceOption(STANCE_OPTION a_option, StanceModification a_modification)
        {
            m_stances[a_option] = a_modification;
        }
        public bool changeStance(STANCE_OPTION a_option)
        {
            if (m_currentStance != a_option)
            {
                m_desiredStance = a_option;
                return true;
            }
            return false;
        }
        public void update(Entity a_entity)
        {
            if (m_desiredStance != m_currentStance)
            {
                //reverse last stance change
                StanceModification old = getCurrentStanceModification();
                a_entity.state.position -= old.p_positionOffset;
                a_entity.state.size -= old.p_sizeOffset;

                //update to new stance
                m_currentStance = m_desiredStance;

                //apply new stance change
                StanceModification current = getCurrentStanceModification();
                a_entity.state.position += current.p_positionOffset;
                a_entity.state.size += current.p_sizeOffset;
            }
        }
    }
}
