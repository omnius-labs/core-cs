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

        public static OmniAddress Combine(params OmniAddress[] omniAddresses)
        {
            var sb = new StringBuilder();
            sb.Append('/');

            foreach (var omniAddress in omniAddresses)
            {
                sb.Append(omniAddress.Value.Trim('/'));
                sb.Append('/');
            }

            return new OmniAddress(sb.ToString());
        }

        public static implicit operator string(OmniAddress omniAddress)
        {
            return omniAddress.Value;
        }

        public static implicit operator OmniAddress(string text)
        {
            return new OmniAddress(text);
        }
    }
}
