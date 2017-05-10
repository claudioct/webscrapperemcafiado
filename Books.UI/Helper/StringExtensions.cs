using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.UI.Helper
{
    public static class StringExtensions
    {

        public static string RemoveDiacritics(this string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool Contains(this string search, IEnumerable<string> termos)
        {
            foreach (var termo in termos)
            {
                if (search.Contains(termo) == false)
                {
                    return false;
                }
            }
            return true;

        }
    }
}
