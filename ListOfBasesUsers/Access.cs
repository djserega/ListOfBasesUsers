using System;
using System.Diagnostics;
using System.IO;

namespace ListOfBasesUsers
{
    internal static class Access
    {

        internal static bool TryGetAccess(string path)
        {

            try
            {
                //return new DirFile(_path).OpenDirectory(true);
                new DirFile(path).OpenDirectory();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                Dialog.ShowMessage($"Каталог не существует: \n{path}.");
            }
            catch (UnauthorizedAccessException)
            {
                string textMessage = $"Не удалось изменить права доступа к каталогу: \n{path}.";
                Dialog.ShowMessage(textMessage);
            }

            catch (Exception ex)
            {
                Dialog.ShowMessage("Произошла непредвиденная ошибка.");
                Dialog.ShowMessage(ex.Message);
            }
            
            return false;

        }

    }
}
