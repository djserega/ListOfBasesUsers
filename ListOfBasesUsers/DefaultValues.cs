using System;

namespace ListOfBasesUsers
{

    internal static class DefaultValues
    {

        #region internal properties

        internal static readonly string defaultCulture = "ru-UA"; // uk-UA, ru-RU

        internal static readonly string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string pathDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static readonly string pathUsers = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        internal static readonly string nameIbases = "ibases.v8i";

        internal static readonly string postfixIbases = @"\1c\1cestart\";
        internal static readonly string postfixCache = @"\1c\1cv8\";
        internal static readonly string postfixFileIbases = $"{postfixIbases}{nameIbases}";

        internal static readonly string nameCurrentUser = Environment.UserName;
        internal static readonly string nameCurrentDomainUser = Environment.UserDomainName;

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
