using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs_Modbus_Maitre
{
    public class Locomotive
    {
        const int LOCO_DIR_FRONT = 0;
        const int LOCO_DIR_REVERSE = 1;
        public String m_sName;
        public int m_iAddress;
        public int m_iSpeed;
        public int m_iNewSpeed;
        public int m_iDirection;
        public bool[] m_FunctionsArray = new bool[12];
        public String[] m_sFunctionNameArray = new String[12];
        public bool m_bESD;

        public Locomotive()
        {
            m_sName = "No name";
            m_iAddress = 1;
            m_iSpeed = 0;
            m_iNewSpeed = 0;
            m_iDirection = LOCO_DIR_FRONT;
            for (int i = 0; i < 12; i++)
            {
                m_FunctionsArray[i] = false;
            }
            m_bESD = false;
        }

    } // class Locomotive
}
