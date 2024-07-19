using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniApp.Helpers
{
    public static class Helper
    {
        public static bool StartsWithACapitalLetter(this string s)
        => !string.IsNullOrEmpty(s) && Char.IsUpper(s[0]);

        public static bool IsValid(this string s)
        => !string.IsNullOrEmpty(s) && !s.Contains(" ") && s.Length >= 3;

        public static bool ValidClassroomName(this string s)
        => !string.IsNullOrEmpty(s) && 
            s.Length == 5 && 
            char.IsUpper(s[0]) && 
            char.IsUpper(s[1]) && 
            char.IsDigit(s[2]) && 
            char.IsDigit(s[3]) && 
            char.IsDigit(s[4]);

    }
}
