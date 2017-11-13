using System;
using System.Diagnostics;
using System.IO;

namespace ListOfBasesUsers
{
    internal class Access
    {

        private string _path;

        public Access(string path)
        {
            _path = path;
        }

        internal bool TryGetAccess()
        {

            try
            {
                //return new DirFile(_path).OpenDirectory(true);
                new DirFile(_path).OpenDirectory();
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                Dialog.ShowMessage($"Каталог не существует: \n{_path}.");
            }
            catch (UnauthorizedAccessException)
            {
                string textMessage = $"Не удалось изменить права доступа к каталогу: \n{_path}.";
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
