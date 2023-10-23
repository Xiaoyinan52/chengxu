﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireTestProgram
{
    public interface IGroupable
    {
        Guid ID { get; }
        Guid ParentID { get; set; }
        bool IsGroup { get; set; }
    }
}
