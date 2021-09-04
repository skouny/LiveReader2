using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Score
{
    public static bool HomeScored(string newScore, string prevScore)
    {
        var newGoals = SplitInt(newScore);
        var prevGoals = SplitInt(prevScore);
        if (newGoals[0] > prevGoals[0]) return true;
        return false;
    }
    public static bool AwayScored(string newScore, string prevScore)
    {
        var newGoals = SplitInt(newScore);
        var prevGoals = SplitInt(prevScore);
        if (newGoals[1] > prevGoals[1]) return true;
        return false;
    }
    public static int[] SplitInt(string score)
    {
        int scoreHome = 0, scoreAway = 0;
        try
        {
            var s = score.Split('-');
            if (s.Length == 2)
            {
                scoreHome = int.Parse(s[0]);
                scoreAway = int.Parse(s[1]);
            }
        }
        catch { }
        return new int[] { scoreHome, scoreAway };
    }
    public static string[] Split(string score)
    {
        string scoreHome = "", scoreAway = "";
        try
        {
            var s = score.Split('-');
            if (s.Length == 2)
            {
                scoreHome = s[0].Trim();
                scoreAway = s[1].Trim();
            }
        }
        catch { }
        return new string[] { scoreHome, scoreAway };
    }
    public static bool ScoreIsValid(string score)
    {
        if (score != null)
        {
            var s = score.Split('-');
            int i0, i1;
            if (s.Length == 2 && int.TryParse(s[0], out i0) && int.TryParse(s[1], out i1))
            {
                return true;
            }
        }
        return false;
    }
}
