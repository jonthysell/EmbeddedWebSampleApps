// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Windows;

namespace EmbeddedWebSampleApps.Common;

public static class SizeHelpers
{
    public static bool TryParse(string s, out Size result)
    {
        try
        {
            result = Parse(s);
            return true;
        }
        catch { }

        result = default;
        return false;
    }

    public static Size Parse(string s)
    {
        var split = s.Split('x');
        return new Size(double.Parse(split[0]), double.Parse(split[1]));
    }
}