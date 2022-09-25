using System.Text.RegularExpressions;

namespace Omnius.Core;

public class LogicalStringComparer : IComparer<string>
{
    public static LogicalStringComparer Instance = new LogicalStringComparer();

    private static Regex _regex = new Regex("([0-9]+)", RegexOptions.IgnoreCase);

    public int Compare(string? x, string? y)
    {
        x ??= string.Empty;
        y ??= string.Empty;

        // 比較結果
        int ret = 0;

        // 英数字毎に分割
        var xs = _regex.Split(x).Where(n => n.Length > 0).ToArray();
        var ys = _regex.Split(y).Where(n => n.Length > 0).ToArray();

        int length = Math.Min(xs.Length, ys.Length);

        for (int i = 0; i < length; i++)
        {
            bool xf = long.TryParse(xs[i], out var xi);
            bool yf = long.TryParse(ys[i], out var yi);

            // 数値の比較
            if (xf && yf)
            {
                ret = xi.CompareTo(yi);
                if (ret != 0) return ret;
            }
            // 文字列の比較
            else if (!xf && !yf)
            {
                ret = xs[i].CompareTo(ys[i]);
                if (ret != 0) return ret;
            }
            else
            {
                ret = xf.CompareTo(yf);
                if (ret != 0) return ret;
            }
        }

        return x.CompareTo(y);
    }
}
