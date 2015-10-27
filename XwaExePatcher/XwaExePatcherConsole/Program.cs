using System;
using JeremyAnsel.Xwa.ExePatcher;

namespace XwaExePatcherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                bool writeNewValues = true;

                if (args != null && args.Length >= 1 && args[0] == "restore")
                {
                    writeNewValues = false;
                }

                Console.WriteLine("Xwa Exe Patcher");

                var patcher = Patcher.Read(@"patcher.xml");

                foreach (var patch in patcher.Patches)
                {
                    Console.Write(" - ");
                    Console.WriteLine(patch.Name);
                }

                Console.WriteLine(writeNewValues ? "Writing new values..." : "Restoring original values...");

                patcher.Apply(@"XWINGALLIANCE.EXE", writeNewValues);

                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
