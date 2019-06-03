using System;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Network
{
    partial class OmniAddress
    {
        public string[] Parse()
        {
            return this.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
