using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SamAllen_Rigor_Challenge.Helpers
{
    public class FileHelper
    {
        public FileHelper()
        {
        }

        public object GetJsonFromFile(HttpPostedFile postedFile)
        {
            JObject jsonObject = new JObject();
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                try
                {
                    string fileName = postedFile.FileName;
                    int fileLength = postedFile.ContentLength;
                    using (Stream inputFileStream = postedFile.InputStream)
                    {
                        using (StreamReader file = new StreamReader(inputFileStream))
                        {
                            using (JsonTextReader reader = new JsonTextReader(file))
                            {
                                jsonObject = (JObject)JToken.ReadFrom(reader);
                            }
                        }
                    }
                    if(ValidateHarFile(jsonObject) == false)
                    {
                        throw new Exception("Invalid HAR File");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                throw new Exception("File null or empty");
            }
            return jsonObject;
        }

        public object GetHarFileAnalysis(object objectFromCache)
        {
            JObject objectToAnalyze = (JObject)objectFromCache;
            if (ValidateHarFile(objectToAnalyze) == true)
            {
                var entries = objectToAnalyze["log"]["entries"];
                int totalBodySize = 0;
                Dictionary<string, int> blockedTimings = new Dictionary<string, int>();
                Dictionary<string, int> waitTimings = new Dictionary<string, int>();
                List<string> requestUrls = new List<string>();
                foreach (var entry in entries)
                {
                    totalBodySize += int.Parse(entry["request"]["bodySize"].ToString());
                    string requestUrl = entry["request"]["url"].ToString();
                    blockedTimings.Add(requestUrl, int.Parse(entry["timings"]["blocked"].ToString()));
                    waitTimings.Add(requestUrl, int.Parse(entry["timings"]["wait"].ToString()));
                    if (entry["request"]["queryString"].HasValues)
                    {
                        requestUrls.Add(requestUrl);
                    }
                }
                Dictionary<string, List<string>> filteredBlockedTimings = DetermineTimings(blockedTimings);
                Dictionary<string, List<string>> filteredWaitTimings = DetermineTimings(waitTimings);
                return new
                {
                    TotalBodySize = totalBodySize,
                    AverageBodySize = (totalBodySize / objectToAnalyze["log"]["entries"].Count()),
                    BlockedTimings = filteredBlockedTimings,
                    WaitTimings = filteredWaitTimings,
                    RequestURLs = requestUrls
                };
            }
            return new { };
        }

        private bool ValidateHarFile(JObject objectToValidate)
        {
            bool isValidated = false;
            if (objectToValidate["log"] != null)
            {
               if(objectToValidate["log"]["version"] != null && objectToValidate["log"]["creator"] != null && objectToValidate["log"]["entries"] != null)
               {
                    isValidated = true;
               }
            }
            return isValidated;
        }

        private Dictionary<string, List<string>> DetermineTimings(Dictionary<string, int> timings)
        {
            Dictionary<string, List<string>> filteredTimings = new Dictionary<string, List<string>>();
            int leastBlocked = timings.Values.ToList().Min();
            List<string> leastBlockedList = new List<string>();
            List<string> mostBlockedList = new List<string>();
            foreach (string key in timings.Where(pair => pair.Value == leastBlocked).Select(pair => pair.Key).ToList())
            {
                leastBlockedList.Add(key);
            }
            filteredTimings.Add("Least blocked: " + leastBlocked.ToString(), leastBlockedList);           
            if (leastBlockedList.Count != timings.Keys.Count)
            {
                int mostBlocked = timings.Values.ToList().Max();               
                foreach (string key in timings.Where(pair => pair.Value == mostBlocked).Select(pair => pair.Key).ToList())
                {
                    if (leastBlockedList.Contains(key) == false)
                    {
                        mostBlockedList.Add(key);
                    }
                }
                filteredTimings.Add("Most blocked: " + mostBlocked.ToString(), mostBlockedList);
            }
            if ((leastBlockedList.Count + mostBlockedList.Count) != timings.Keys.Count)
            {
                int secondLeastBlocked = timings.Values.OrderBy(x => x).SkipWhile(x => x == leastBlocked).Take(1).ToList()[0];
                List<string> secondLeastBlockedList = new List<string>();
                foreach (string key in timings.Where(pair => pair.Value == secondLeastBlocked).Select(pair => pair.Key).ToList())
                {
                    if (mostBlockedList.Contains(key) == false && leastBlockedList.Contains(key) == false)
                    {
                        secondLeastBlockedList.Add(key);
                    }
                }
                filteredTimings.Add("Second least blocked: " + secondLeastBlocked.ToString(), secondLeastBlockedList);
            }
            return filteredTimings;
        }

    }
}