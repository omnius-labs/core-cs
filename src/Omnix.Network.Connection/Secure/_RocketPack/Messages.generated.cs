using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Secure
{
    public enum OmniSecureConnectionType : byte
    {
        Connect = 0,
        Accept = 1,
    }

    public enum OmniSecureConnectionVersion : byte
    {
        Version1 = 1,
    }

}
