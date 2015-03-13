using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    enum MessageType
    {
        InputState1,
        InputState2,
        InputState3,
        InputState4,
        InputAction1,
        InputAction2,
        InputAction3,
        InputAction4
    }

    class Message
    {
        public uint messageID = 0;
        public uint receiver = 0;

        /// <summary>
        /// Compare Message ID as a bitmask with a generic value
        /// </summary>
        public bool ValueFound<T>(T value)
        {
            int compareBit = (1 << Convert.ToInt32(value));

            if ((messageID & compareBit) == compareBit)
                return true;

            return false;
        }
    }
}
