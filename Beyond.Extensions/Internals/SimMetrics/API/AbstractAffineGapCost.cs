﻿namespace Beyond.Extensions.Internals.SimMetrics.API;

internal abstract class AbstractAffineGapCost : IAffineGapCost
{
    public abstract double MaxCost { get; }

    public abstract double MinCost { get; }

    public abstract string ShortDescriptionString { get; }

    public abstract double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap);
}