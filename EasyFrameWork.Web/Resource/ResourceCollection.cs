﻿using Easy.Web.Resource.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Web.Resource
{
    public class ResourceCollection : List<ResourceEntity>
    {
        public bool Required { get; set; }
        public ResourcePosition Position { get; set; }
    }
}
