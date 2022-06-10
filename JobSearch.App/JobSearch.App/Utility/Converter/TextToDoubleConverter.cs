using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobSearch.App.Utility.Converter
{
    internal class TextToDoubleConverter
    {
        public static double ToDouble(string value)
        {
            if(value != null)
            {
                value = RemoveExtraText(value);
                return double.Parse(value);
            }
            return 0;
        }
      
        private static string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}