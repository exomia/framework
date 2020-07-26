using System;

namespace Exomia.Framework.Example.JumpAndRun
{
    class Program
    {
        private static void Main(string[] args)
        {
            using(Game.Game g = new JumpAndRunGame())
            {
                g.Run();
            }
        }
    }
}
