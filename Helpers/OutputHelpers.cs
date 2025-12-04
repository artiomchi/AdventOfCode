using static System.Console;

namespace AoC.Helpers;

public static class OutputHelpers
{
    public static T Dump<T>(this T instance, string? title = null)
    {
        if (title != null)
        {
            Write("{0}: ", title);
        }
        WriteLine(instance);
        return instance;
    }

    public static T DumpAndAssert<T>(this T instance, string title, params T[] acceptedValues)
    {
        Dump(instance, title);
        if (!acceptedValues.Contains(instance))
        {
            Error.WriteLine("Expected value to be one of " + string.Join(", ", acceptedValues) + " but got " + instance);
        }
        return instance;
    }

    static string ToTimeString(this TimeSpan time, int targetWidth = 20)
    {
        if (time.TotalMinutes > 1)
        {
            return time.ToString();
        }
        if (time.TotalSeconds >= 1)
        {
            return SmartRound(time.TotalSeconds, "s", targetWidth);
        }
        if (time.TotalMilliseconds >= 1)
        {
            return SmartRound(time.TotalMilliseconds, "ms", targetWidth);
        }
        if (time.TotalMicroseconds >= 1)
        {
            return SmartRound(time.TotalMicroseconds, "μs", targetWidth);
        }
        return SmartRound(time.TotalNanoseconds, "ns", targetWidth);

        static string SmartRound(double value, string suffix, int targetWidth)
        {
            var digits = (int)Math.Log10(value) + 1;
            return Math.Round(value, Math.Clamp(targetWidth - suffix.Length - digits - 1, 0, 3)) + suffix;
        }
    }

    public static void PrintTimings(params TimeSpan[] parts)
    {
        if (parts.Length == 0)
            return;

        const int MinBarLength = 50;
        var barLength = Math.Max(MinBarLength - parts.Length + 1, parts.Length * 5);

        var total = new TimeSpan(parts.Sum(t => t.Ticks));
        var totalStr = total.ToTimeString();
        var widths = parts.Select(t => (int)(barLength * t.Ticks / total.Ticks)).ToArray();
        foreach (var (index, part) in widths.Index().OrderByDescending(x => x.Item).Take(Math.Max(barLength - widths.Sum(), 0)))
        {
            widths[index] = widths[index] + 1;
        }
        var partsStr = parts.Index()
            .Select(x =>
            {
                var time = x.Item;
                var width = widths[x.Index];
                var timeStr = time.ToTimeString(width);
                var padding = Math.Max(0, width - timeStr.Length);
                return (time, timeStr, width, padding);
            })
            .ToArray();
        barLength = partsStr.Sum(x => x.width) + parts.Length - 1;

        var overshot = partsStr.Sum(x => x.timeStr.Length + x.padding) + parts.Length - 1 - barLength;
        while (overshot > 0)
        {
            var positions = partsStr
                .Aggregate(
                    new List<(int index, int shift)>(),
                    (x, i) =>
                    {
                        var position = partsStr.Take(x.Count).Sum(x => x.timeStr.Length + x.padding) + x.Count;
                        var expected = partsStr.Take(x.Count).Sum(x => x.width) + x.Count;
                        x.Add((x.Count, expected - position));
                        return x;
                    });

            var mostShifted = positions.OrderByDescending(x => Math.Abs(x.shift)).First();
            var leftWithPadding = partsStr.Index().Take(mostShifted.index).Reverse().FirstOrDefault(x => x.Item.padding > 0);
            var rightWithPadding = partsStr.Index().Skip(mostShifted.index).FirstOrDefault(x => x.Item.padding > 0);
            var shift = leftWithPadding.Item.padding > 0 && rightWithPadding.Item.padding > 0
                ? mostShifted.shift > 0
                    ? rightWithPadding
                    : leftWithPadding
                : leftWithPadding.Item.padding > 0
                ? leftWithPadding
                : rightWithPadding;

            if (shift.Item.padding == 0)
                break;

            var removing = overshot > 3 && shift.Item.padding > 1 ? 2 : 1;
            overshot -= removing;
            partsStr[shift.Index] = (shift.Item.time, shift.Item.timeStr, shift.Item.width, shift.Item.padding - removing);
        }

        WriteLine(new string('_', barLength + 2));
        WriteLine('|' + string.Join(' ', partsStr.Select(s => new string(' ', s.padding / 2) + s.timeStr + new string(' ', s.padding - s.padding / 2))) + '|');
        WriteLine('|' + string.Join('|', partsStr.Select(s => new string('-', s.width))) + '|');
        WriteLine(new string(' ', (barLength + 2 - totalStr.Length) / 2) + totalStr);
    }
}
