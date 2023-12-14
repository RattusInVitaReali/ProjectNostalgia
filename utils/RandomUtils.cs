using System;

namespace ProjectNostalgia.utils;

public static class RandomUtils
{
    
    private const int Seed = 0;

    // ReSharper disable once InconsistentNaming
    public static Random RNG = new(Seed);

}