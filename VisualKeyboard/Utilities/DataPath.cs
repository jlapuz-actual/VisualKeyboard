using System;

namespace VisualKeyboard.Utilities
{
    public static class DataPath
    {
        private static string appDataPath;
        private static string userAppDataPath;

        public static string AppDataPath
        {
            get => appDataPath ??= Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData );
        }
        public static string UserAppDataPath
        {
            get => userAppDataPath ??= Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
        }
    }
}
