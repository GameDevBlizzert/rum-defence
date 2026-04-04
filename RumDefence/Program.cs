using System;

namespace RumDefence
{
    public static class Program
    {
        static void Main()
        {
            using (var game = new RumGame())
                game.Run();
        }
    }
}