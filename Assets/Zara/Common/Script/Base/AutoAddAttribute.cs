using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zara.Common.ExBase
{
    public class AutoAddAttribute : AutoAttribute
    {
        public override sealed bool ShouldAddAutomatilcally => true;
    }
}