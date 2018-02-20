using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Zara.Common.Utility
{
    public class IdGenerator
    {
        int latestId = 0;

        public int Get()
        {
            latestId++;
            latestId %= int.MaxValue;
            return latestId;
        }
    }
}