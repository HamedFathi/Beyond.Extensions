﻿namespace Beyond.Extensions.Internals.SimMetrics.API;

public interface IAffineGapCost
{
    double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap);

    double MaxCost { get; }

    double MinCost { get; }

    string ShortDescriptionString { get; }
}