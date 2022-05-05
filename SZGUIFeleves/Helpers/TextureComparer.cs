using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Helpers
{
    public class TextureComparer : IComparer<string>
    {
        public static bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

        public int Compare(string s1, string s2)
        {
            const int S1GreaterThanS2 = 1;
            const int S2GreaterThanS1 = -1;

            s1 = Path.GetFileNameWithoutExtension(s1);
            s2 = Path.GetFileNameWithoutExtension(s2);

            s1 = s1.Substring(1, s1.Length - 2);
            s2 = s2.Substring(1, s2.Length - 2);

            var IsNumeric1 = IsNumeric(s1);
            var IsNumeric2 = IsNumeric(s2);

            if (IsNumeric1 && IsNumeric2)
            {
                var i1 = Convert.ToInt32(s1);
                var i2 = Convert.ToInt32(s2);

                if (i1 > i2)
                {
                    return S1GreaterThanS2;
                }

                if (i1 < i2)
                {
                    return S2GreaterThanS1;
                }

                return 0;
            }

            if (IsNumeric1)
            {
                return S2GreaterThanS1;
            }

            if (IsNumeric2)
            {
                return S1GreaterThanS2;
            }

            return string.Compare(s1, s2, true, CultureInfo.InvariantCulture);
        }
    }
}
