using System.Windows;

namespace ListOfBasesUsers
{
    internal static class Dialog
    {
        internal static bool DialogQuestion(string textQuestion)
        {
            return MessageBox.Show(textQuestion, "Вопрос", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        internal static void ShowMessage(string textMessage)
        {
            MessageBox.Show(textMessage);
        }
    }
}
