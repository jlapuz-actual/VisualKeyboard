namespace VisualKeyboard.Utilities
{
    using System.Diagnostics;
    using System.IO;
    class FileOps
    {
        public FileOps()
        {

        }
        public void Write(string toBeWritten, string destinationFile)
        {
            try
            {
                using (var writer = new StreamWriter(destinationFile))
                {
                    writer.Write(toBeWritten);
                    writer.Close();
                    Debug.WriteLine("file written");
                    writer.Dispose();
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }
        public string Load(string originFile)
        {
            string output;
            using (var reader = new StreamReader(originFile))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }
        public void OpenFileDialog()
        {

        }
    }
}
