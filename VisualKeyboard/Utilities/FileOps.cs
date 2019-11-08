namespace VisualKeyboard.Utilities
{
    using System.Diagnostics;
    using System.IO;
    static class FileOps
    {

        public static void Write( string toBeWritten, string destinationFile )
        {
            try
            {
                using var writer = new StreamWriter( destinationFile );
                writer.Write( toBeWritten );
                writer.Close();
                Debug.WriteLine( "file written" );
                writer.Dispose();
            }
            catch ( System.Exception e )
            {
                Debug.WriteLine( e.Message );
                throw;
            }
        }
        public static string Load( string originFile )
        {
            string output;
            try
            {
                using var reader = new StreamReader( originFile );
                output = reader.ReadToEnd();
            }
            catch ( System.Exception )
            {
                throw;
            }
            return output;
        }
        public static void OpenFileDialog()
        {

        }
    }
}
