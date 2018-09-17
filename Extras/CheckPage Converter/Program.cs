using WikiFunctions;

namespace CheckPage_Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkpage =
                Tools.GetHTML(
                    "https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage&action=raw");


        }
    }
}
