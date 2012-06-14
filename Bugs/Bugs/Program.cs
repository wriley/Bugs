using System;

namespace Bugs
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Bugs game = new Bugs())
            {
                game.Run();
            }
        }
    }
#endif
}

