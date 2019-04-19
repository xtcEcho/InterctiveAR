using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.GameAssistant.Scripts
{
    /// <summary>
    /// Assistant will download and cache FAQ table from Google Docs and then will use it for finding answers
    /// </summary>
    public class Assistant
    {
        private readonly string _url;
        private readonly string _dataKey = "Assistant.Data.{0}";
        private readonly string _timestampKey = "Assistant.Timestamp.{0}";
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromDays(1);
        private const string DefaultQuestion = "Default"; // Used when no answer found
        private readonly string _preloadedCsv;

        /// <summary>
        /// Let's say your table has the following url https://docs.google.com/spreadsheets/d/1kxU29-vc2C_xq9qC88HWJlHoak7M5qkngNCfBaU0DhA/edit#gid=2082792765
        /// So your [tableId] will be "1kxU29-vc2C_xq9qC88HWJlHoak7M5qkngNCfBaU0DhA" and [sheetId] will be "2082792765" (gid parameter)
        /// </summary>
        public Assistant(string tableId, string sheetId, string preloadedCsv = null)
        {
            _url = string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}", tableId, sheetId);
            _dataKey = string.Format(_dataKey, _url.GetHashCode());
            _timestampKey = string.Format(_timestampKey, _url.GetHashCode());
            _preloadedCsv = preloadedCsv;
        }

        /// <summary>
        /// Answer to a question with callback returning success status and answer
        /// You can also store preloaded CSV in Resources and pass it so Assistent will work even with no Internet connection
        /// </summary>
        public void Ask(string question, Action<bool, string> callback)
        {
            if (PlayerPrefs.HasKey(_dataKey) && PlayerPrefs.HasKey(_timestampKey))
            {
                var data = PlayerPrefs.GetString(_dataKey);
                var timestamp = DateTime.Parse(PlayerPrefs.GetString(_timestampKey));

                if ((DateTime.UtcNow - timestamp).TotalSeconds > UpdateInterval.TotalSeconds)
                {
                    UpdateData(_dataKey, _timestampKey, (success, error) => { });
                }

                TryHeuristics(question, data, callback);
            }
            else
            {
                UpdateData(_dataKey, _timestampKey, callback: (success, data) =>
                {
                    if (success)
                    {
                        TryHeuristics(question, data, callback);
                    }
                    else if (_preloadedCsv != null)
                    {
                        TryHeuristics(question, _preloadedCsv, callback);
                    }
                    else
                    {
                        callback(false, data);
                    }
                });
            }
        }

        private void UpdateData(string dataKey, string timestampKey, Action<bool, string> callback)
        {
            Downloader.Download(_url, www =>
            {
                if (www.error == null)
                {
                    PlayerPrefs.SetString(dataKey, www.text);
                    PlayerPrefs.SetString(timestampKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    callback(true, www.text);
                }
                else
                {
                    callback(false, www.error);
                }
            });
        }

        private static void TryHeuristics(string question, string data, Action<bool, string> callback)
        {
            try
            {
                var csvReader = new CsvReader(data);
                var faq = csvReader.Rows.Select(t => new FaqEntry { Question = t[0], Tags = t[1], Answer = t[2].Replace("\n\n", "\n") }).ToList();

                faq.RemoveAt(0);
                faq.ForEach(i => i.Match = Heuristics.Evaluate(i, question));
                faq = faq.OrderByDescending(i => i.Match).ToList();

                var faqEntity = faq[0].Match > 0 ? faq[0] : faq.Single(i => i.Question == DefaultQuestion);

                callback(true, faqEntity.Answer);
            }
            catch (Exception e)
            {
                callback(false, e.Message);
            }
        }
    }
}