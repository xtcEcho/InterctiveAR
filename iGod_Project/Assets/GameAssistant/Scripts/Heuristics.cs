using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.GameAssistant.Scripts
{
    /// <summary>
    /// Heuristics helper
    /// </summary>
    public static class Heuristics
    {
        /// <summary>
        /// Evaluates how the question matches with faq entry
        /// </summary>
        public static int Evaluate(FaqEntry faqEntry, string question)
        {
            var value = 0;
            var questionFaq = faqEntry.Question + ' ' + faqEntry.Tags.Replace(',', ' ');
            var commonSubstrings = new List<string>();

            foreach (var a in SplitText(CleanText(questionFaq)))
            {
                foreach (var q in SplitText(CleanText(question)))
                {
                    var commonSubstring = GetLongestCommonSubstring(a, q);

                    if (commonSubstring.Length >= 3 && !commonSubstrings.Contains(commonSubstring))
                    {
                        commonSubstrings.Add(commonSubstring);
                        value += commonSubstring.Length * commonSubstring.Length;
                    }
                }
            }

            //Debug.LogFormat("{0} > {1} = {2} ({3})", questionFaq, question, value, string.Join(" ", commonSubstrings.ToArray()));

            return value;
        }

        private static string CleanText(string text)
        {
            text = Regex.Replace(text, @"\[.+?\]", " ");
            text = Regex.Replace(text, @"[^\w\s]", " ");
            text = text.ToLower();

            return text;
        }

        private static IEnumerable<string> SplitText(string text)
        {
            return text.Split().Where(i => i.Length >= 3).ToList();
        }

        private static string GetLongestCommonSubstring(string str1, string str2)
        {
            var num = new int[str1.Length, str2.Length];
            var maxLen = 0;
            var lastSubsBegin = 0;
            var sequenceBuilder = new StringBuilder();

            for (var i = 0; i < str1.Length; i++)
            {
                for (var j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                    {
                        num[i, j] = 0;
                    }
                    else
                    {
                        if (i == 0 || j == 0) num[i, j] = 1;
                        else num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxLen)
                        {
                            maxLen = num[i, j];

                            var thisSubsBegin = i - num[i, j] + 1;

                            if (lastSubsBegin == thisSubsBegin)
                            {
                                // If the current LCS is the same as the last time this block ran
                                sequenceBuilder.Append(str1[i]);
                            }
                            else
                            {
                                // Reset the string builder if a different LCS is found
                                lastSubsBegin = thisSubsBegin;
                                sequenceBuilder.Length = 0;
                                sequenceBuilder.Append(str1.Substring(lastSubsBegin, i + 1 - lastSubsBegin));
                            }
                        }
                    }
                }
            }

            return sequenceBuilder.ToString();
        }
    }
}
