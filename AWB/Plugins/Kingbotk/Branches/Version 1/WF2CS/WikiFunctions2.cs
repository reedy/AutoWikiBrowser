public static class WikiFunctions2
{
    /// <summary>
    /// WikiFunctions2.dll version
    /// </summary>
    //INSTANT C# NOTE: These were formerly VB static local variables:
    private static System.Version Version_rtn = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

    public static System.Version Version
    {
        get
        {
            //INSTANT C# NOTE: VB local static variable moved to class level
            //			Static rtn As System.Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
            return Version_rtn;
        }
    }
}