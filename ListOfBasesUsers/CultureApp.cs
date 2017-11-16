using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListOfBasesUsers
{
    internal class CultureApp
    {
        internal CultureInfo GetCulture()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;

            try
            {
                cultureInfo = new CultureInfo(DefaultValues.defaultCulture);
            }
            catch (Exception ex)
            {
#if DEBUG
                Dialog.ShowMessage(ex.Message);
#endif
            }
                return cultureInfo;
        }

        internal string GetFormatData()
        {
            string cultureDateTime;
            try
            {
                //string cultureDateTime = new CultureInfo("uk-UA").DateTimeFormat.FullDateTimePattern;
                cultureDateTime = new CultureInfo(DefaultValues.defaultCulture).DateTimeFormat.FullDateTimePattern;
                cultureDateTime = cultureDateTime.Replace("dddd, ", "d");
            }
            catch (Exception)
            {
                cultureDateTime = "dd MMMM yyyy HH:mm:ss";
            }

            return cultureDateTime;
        }

    }
}
