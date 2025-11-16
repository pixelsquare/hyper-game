using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Game
{
    public static class Utils
    {
        public static string[] ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            MatchCollection matches = Regex.Matches(line, @"(?:^|,)(?=(?:[^""]*""[^""]*"")*[^""]*$)(?:""((?:[^""]|"""")*)""|([^,]*))");

            foreach (Match match in matches)
            {
                // Group 1 captures content within double quotes (handling escaped quotes)
                // Group 2 captures content without double quotes
                if (match.Groups[1].Success)
                {
                    // Unescape double quotes within the field
                    fields.Add(match.Groups[1].Value.Replace("\"\"", "\""));
                }
                else if (match.Groups[2].Success)
                {
                    fields.Add(match.Groups[2].Value);
                }
                else
                {
                    // Handle empty fields
                    fields.Add("");
                }
            }
            
            return fields.ToArray();
        }
        
        public static RoundData[] ParseGameData(string gameDataString)
        {
            var lines = gameDataString.Split("\n");
            var rounds = new HashSet<RoundData>();

            foreach (var line in lines)
            {
                var lineData = Utils.ParseCsvLine(line);
                rounds.Add(new RoundData
                {
                    Id = int.Parse(lineData[0]),
                    GameMode = int.Parse(lineData[1]),
                    Category = lineData[2],
                    Question = lineData[3],
                    Hints = new[] { lineData[4], lineData[5], lineData[6], lineData[7] },
                    Choices = new[] { lineData[8], lineData[9], lineData[10], lineData[11] },
                    AnswerIdx = int.Parse(lineData[12]),
                    AnswerSprite = null,
                });
            }

            return rounds.ToArray();
        }
    }
}
