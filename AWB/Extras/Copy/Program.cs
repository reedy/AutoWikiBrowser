using System.IO;

namespace Copy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && File.Exists(args[0]))
            {
                File.Copy(args[0], args[1], true);
            }
        }
    }
}
