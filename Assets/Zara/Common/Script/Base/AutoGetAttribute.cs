using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zara.Common.ExBase
{
    public class AutoGetAttribute : AutoAttribute
    {
        public override sealed bool ShouldAddAutomatilcally => false;
    }
}