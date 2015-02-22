using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    enum MessageType
    {
        Player1State = 0,
        Player1Action
    }

    class Message
    {
        public uint messageID = 0;
        public uint data = 0;

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
