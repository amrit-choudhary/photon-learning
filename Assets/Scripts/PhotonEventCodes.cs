using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonServerDemo
{
    public static class PhotonEventCodes
    {
        public static readonly byte DamageEventCode = 1;
        public static readonly byte HealEventCode = 2;
        public static readonly byte InitPlayerEventCode = 3;
        public static readonly byte HealthUpdateEventCode = 4;
        public static readonly byte TimerUpdateEventCode = 5;
    }
}
