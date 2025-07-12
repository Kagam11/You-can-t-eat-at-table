using System.Collections.Generic;

namespace YouCantEatAtTable
{
    public static class Utils
    {
        public static Dictionary<string, int> Titles => new Dictionary<string, int>
        {
            { "Freeholder", 0 },
            { "Novice", 1 },
            { "Acolyte", 2 },
            { "Knight", 3 },
            { "Praetor", 4 },
            { "Baron", 5 },
            { "Archon", 6 },
            { "Dominus", 7 },
            { "Consul", 8 },
            { "Stellarch", 9 },
            { "Emperor", 10 },
        };
    }
}
