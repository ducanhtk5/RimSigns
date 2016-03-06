using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace CMS.Common.Helpers
{
        public class StringHelpers
        {
            public string ToSeoUrl(string url)
            {
                // make the url lowercase
                string encodedUrl = (url ?? "").ToLower();

                // replace & with and
                encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

                // remove characters
                encodedUrl = encodedUrl.Replace("'", "");

                // remove invalid characters
                encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

                // remove duplicates
                encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

                // trim leading & trailing characters
                encodedUrl = encodedUrl.Trim('-');

                return encodedUrl;
            }


            private readonly string[] VietnameseSigns = new string[]

    {

        "aAeEoOuUiIdDyY",

        "áàạảãâấầậẩẫăắằặẳẵ",

        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

        "éèẹẻẽêếềệểễ",

        "ÉÈẸẺẼÊẾỀỆỂỄ",

        "óòọỏõôốồộổỗơớờợởỡ",

        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

        "úùụủũưứừựửữ",

        "ÚÙỤỦŨƯỨỪỰỬỮ",

        "íìịỉĩ",

        "ÍÌỊỈĨ",

        "đ",

        "Đ",

        "ýỳỵỷỹ",

        "ÝỲỴỶỸ"

    };



            public string RemoveSign4VietnameseString(string str)
            {

                //Tiến hành thay thế , lọc bỏ dấu cho chuỗi

                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }

                str = ToSeoUrl(str);

                return str;

            }

            public string Cutstr(string strInput, int numChar)
            {
                HtmlRemoval htmlremoveta = new HtmlRemoval();
                strInput = htmlremoveta.StripTagsRegex(strInput);
                string strReturn = strInput.Replace("\r\n\t", string.Empty);
                if (strReturn.Length <= numChar)
                    strReturn = strInput;
                else
                {
                    strReturn = strReturn.Substring(0, numChar) + "...";
                }
                return strReturn;
            }

            public static string ConvertListToString(object input_object)
            {
                string rs = "";
                if (input_object != null)
                {
                    List<string> input_list = (List<string>)input_object;

                    for (int i = 0; i < input_list.Count(); i++)
                    {
                        rs += input_list[i];
                        if (i < input_list.IndexOf(input_list.Last()))
                        {
                            rs += "|";
                        }
                    }
                }
                return rs;
            }

            public static object ConvertStringToList(string input_string)
            {
                input_string = input_string.Replace(",", "|");
                string[] lst = input_string.Split('|');
                List<string> rs = new List<string>();
                foreach (var item in lst)
                {
                    rs.Add(item);
                }
                return rs;
            }
        }

        public class HtmlRemoval
        {
            public string StripHtml(string html, bool allowHarmlessTags)
            {
                return string.IsNullOrEmpty(html) ? string.Empty : System.Text.RegularExpressions.Regex.Replace(html, allowHarmlessTags ? "" : "<[^>]*>", string.Empty);
            }

            /// <summary>
            /// Remove HTML from string with Regex.
            /// </summary>
            public string StripTagsRegex(string source)
            {
                return Regex.Replace(source, "<.*?>", string.Empty);
            }

            /// <summary>
            /// Compiled regular expression for performance.
            /// </summary>
            static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

            /// <summary>
            /// Remove HTML from string with compiled Regex.
            /// </summary>
            public string StripTagsRegexCompiled(string source)
            {
                return _htmlRegex.Replace(source, string.Empty);
            }

            /// <summary>
            /// Remove HTML tags from string using char array.
            /// </summary>
            public string StripTagsCharArray(string source)
            {
                char[] array = new char[source.Length];
                int arrayIndex = 0;
                bool inside = false;

                for (int i = 0; i < source.Length; i++)
                {
                    char let = source[i];
                    if (let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    if (let == '>')
                    {
                        inside = false;
                        continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = let;
                        arrayIndex++;
                    }
                }
                return new string(array, 0, arrayIndex);
            }
        }

}