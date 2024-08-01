using Xunit;

namespace Omnius.Core.Testkit;

public class StableTestSkipAttribute : FactAttribute
{
#if STABLE_TEST
    public StableTestSkipAttribute()
    {
        Skip = "stable test";
    }
#endif
}
