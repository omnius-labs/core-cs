using Xunit;

namespace Core.UnitTestToolkit;

public class StableTestSkipAttribute : FactAttribute
{
#if STABLE_TEST
    public StableTestSkipAttribute()
    {
        Skip = "stable test";
    }
#endif
}
