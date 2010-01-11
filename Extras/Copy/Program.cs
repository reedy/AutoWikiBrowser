using System.IO;

namespace Copy
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 2 && File.Exists(args[0]))
                {
                    File.Copy(args[0], args[1], true);
                    return 0;
                }
                return 1;
            }
            catch
            {
                return 1;
            }
        }
    }
}
