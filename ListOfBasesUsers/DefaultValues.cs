using System;

namespace ListOfBasesUsers
{

    internal static class DefaultValues
    {

        #region internal properties

        internal static string defaultCulture = "ru-UA"; // uk-UA, ru-RU

        internal static string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static string pathDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static string pathUsers = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        internal static string nameIbases = "ibases.v8i";

        internal static string postfixIbases = @"\1c\1cestart\";
        internal static string postfixCache = @"\1c\1cv8\";
        internal static string postfixFileIbases = $"{postfixIbases}{nameIbases}";

        internal static string nameCurrentUser = Environment.UserName;
        internal static string nameCurrentDomainUser = Environment.UserDomainName;

        #endregion

        static DefaultValues()
        {

            string[] pathUsersSplit = pathUsers.Split('\\');

            pathUsers = $"{pathUsersSplit[0]}\\{pathUsersSplit[1]}\\";

        }

        #region internal methods

        internal static string GetPathAppData(string userName) => pathAppData.Replace($"\\{nameCurrentUser}\\", $"\\{userName}\\");

        internal static string GetPathDataLocal(string userName) => pathDataLocal.Replace($"\\{nameCurrentUser}\\", $"\\{userName}\\");

        internal static string GetPathUserDir(string userName) => pathUsers + userName;

        internal static string GetNameFileIbases(string userName) => $"{GetPathAppData(userName)}\\{postfixFileIbases}";

        internal static string GetNameDirCacheLocal(string userName) => $"{GetPathDataLocal(userName)}\\{postfixCache}";

        internal static string GetNameDirCacheAppData(string userName) => $"{GetPathAppData(userName)}\\{postfixCache}";

        #endregion

    }

}
