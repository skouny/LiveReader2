using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;

namespace LiveReader
{
    public enum MatchStatus
    {
        Unknown = 0, Waiting = 1, Qrt1 = 2, Half1 = 3, Qrt2 = 4, HT = 5, Qrt3 = 6, Half2 = 7, Qrt4 = 8,
        FT = 9, OT = 10, Pause = 11, Cancelled = 12, Abd = 13
    }
    public enum SourceName { Opap = 1, NowGoal = 2, SportingBet = 3 }
    public enum OddName
    {
        Odd1 = 0, OddX = 1, Odd2 = 2, Odd1orX = 3, Odd1or2 = 4, OddXor2 = 5, OddHt1 = 9, OddHtX = 10, OddHt2 = 11,
        OddHtFt11 = 12, OddHtFtX1 = 13, OddHtFt21 = 14, OddHtFt1X = 15, OddHtFtXX = 16, OddHtFt2X = 17, OddHtFt12 = 18, OddHtFtX2 = 19, OddHtFt22 = 20,
        Goals0to1 = 21, Goals2to3 = 22, Goals4to6 = 23, Goals7p = 24, Under = 25, Over = 26, Goal = 29, NoGoal = 30,
        Score00 = 36, Score10 = 37, Score20 = 38, Score30 = 39, Score40 = 40, Score50 = 41,
        Score21 = 42, Score31 = 43, Score41 = 44, Score51 = 45, Score32 = 46,
        Score42 = 47, Score52 = 48, Score43 = 49, Score53 = 50, Score54 = 51,
        Score11 = 52, Score22 = 53, Score33 = 54, Score44 = 55, Score55 = 56,
        Score45 = 57, Score34 = 58, Score35 = 59, Score23 = 60, Score24 = 61,
        Score25 = 62, Score12 = 63, Score13 = 64, Score14 = 65, Score15 = 66,
        Score01 = 67, Score02 = 68, Score03 = 69, Score04 = 70, Score05 = 71
    }
    [Serializable]
    public class TeamInfo : ICloneable
    {
        #region Init
        UInt16 _Id = UInt16.MaxValue;
        public TeamInfo() { } // For Deserializing
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        #region Properties
        [XmlAttribute("Id")]
        public UInt16 Id
        {
            get
            {
                if (_Id == UInt16.MaxValue) _Id = MyDatabase.GetTeamId(this.Name);
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Score")]
        public string Score;
        [XmlAttribute("ScoreHT")]
        public string ScoreHT;
        [XmlAttribute("ScoreQ1")]
        public string ScoreQ1;
        [XmlAttribute("ScoreQ2")]
        public string ScoreQ2;
        [XmlAttribute("ScoreQ3")]
        public string ScoreQ3;
        [XmlAttribute("ScoreQ4")]
        public string ScoreQ4;
        [XmlAttribute("YellowCards")]
        public string YellowCards;
        [XmlAttribute("RedCards")]
        public string RedCards;
        [XmlAttribute("StandingPoints")]
        public string StandingPoints;
        [XmlAttribute("CornerKicks")]
        public string CornerKicks;
        [XmlAttribute("FirstYellowCard")]
        public string FirstYellowCard;
        [XmlAttribute("LastYellowCard")]
        public string LastYellowCard;
        [XmlAttribute("Shots")]
        public string Shots;
        [XmlAttribute("ShotsOnGoal")]
        public string ShotsOnGoal;
        [XmlAttribute("Fouls")]
        public string Fouls;
        [XmlAttribute("BallPossession")]
        public string BallPossession;
        [XmlAttribute("Saves")]
        public string Saves;
        [XmlAttribute("Offsides")]
        public string Offsides;
        [XmlAttribute("Substitutions")]
        public string Substitutions;
        [XmlAttribute("FirstSubstitution")]
        public string FirstSubstitution;
        [XmlAttribute("LastSubstitution")]
        public string LastSubstitution;
        [XmlAttribute("KickOff")]
        public string KickOff;
        [XmlAttribute("Scored")]
        public DateTime Scored;
        [XmlElement("Event")]
        public List<TeamEventInfo> Events = new List<TeamEventInfo>();
        #endregion
        #region Readonly Properties
        public bool ScoredInLast5Minutes { get { return ScoredInLastMinutes(5); } }
        public bool ScoredInLastMinutes(double value)
        {
            if (DateTime.Compare(this.Scored.AddMinutes(value), DateTime.Now) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ScoredInLast20Seconds { get { return ScoredInLastSeconds(20); } }
        public bool ScoredInLastSeconds(double value)
        {
            if (DateTime.Compare(this.Scored.AddSeconds(value), DateTime.Now) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool HasScored(Opap.API.Sport sport)
        {
            if (sport == Opap.API.Sport.soccer)
            {
                return ScoredInLastMinutes(5);
            }
            else if (sport == Opap.API.Sport.basketball)
            {
                return ScoredInLastSeconds(20);
            }
            return false;
        }
        #endregion
        #region Self Update
        public int Update(TeamInfo newTeam)
        {
            int modified = 0;
            modified += UpdateName(newTeam.Name);
            modified += UpdateScore(newTeam.Score);
            modified += UpdateScoreHT(newTeam.ScoreHT);
            modified += UpdateScoreQ1(newTeam.ScoreQ1);
            modified += UpdateScoreQ2(newTeam.ScoreQ2);
            modified += UpdateScoreQ3(newTeam.ScoreQ3);
            modified += UpdateScoreQ4(newTeam.ScoreQ4);
            UpdateDetails(newTeam);
            UpdateEvents(newTeam.Events);
            return modified;
        }
        public int UpdateName(string value)
        {
            int modified = 0;
            if (this.Name != value)
            {
                this.Name = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScore(string value)
        {
            int modified = 0;
            if (this.Score != value)
            {
                if (value != null && value.Length > 0 && this.Score != null && this.Score.Length > 0)
                {
                    if (this.Scored == null || value == "0")
                    {
                        this.Scored = DateTime.MinValue;
                    }
                    else
                    {
                        if (My.iInt(value) > My.iInt(this.Score))
                        {
                            this.Scored = DateTime.Now;
                        }
                        else
                        {
                            this.Scored = DateTime.MinValue;
                            MyDatabase.WriteLog("TeamInfo", "UpdateScore", "Corrected {0}=>{1}, {2}", this.Score, value, this.Scored);
                        }
                    }
                }
                this.Score = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScoreHT(string value)
        {
            int modified = 0;
            if (this.ScoreHT != value)
            {
                this.ScoreHT = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScoreQ1(string value)
        {
            int modified = 0;
            if (this.ScoreQ1 != value)
            {
                this.ScoreQ1 = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScoreQ2(string value)
        {
            int modified = 0;
            if (this.ScoreQ2 != value)
            {
                this.ScoreQ2 = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScoreQ3(string value)
        {
            int modified = 0;
            if (this.ScoreQ3 != value)
            {
                this.ScoreQ3 = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateScoreQ4(string value)
        {
            int modified = 0;
            if (this.ScoreQ4 != value)
            {
                this.ScoreQ4 = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateDetails(TeamInfo newTeam)
        {
            int modified = 0;
            modified += UpdateYellowCards(newTeam.YellowCards);
            modified += UpdateRedCards(newTeam.RedCards);
            modified += UpdateStandingPoints(newTeam.StandingPoints);
            modified += UpdateCornerKicks(newTeam.CornerKicks);
            modified += UpdateFirstYellowCard(newTeam.FirstYellowCard);
            modified += UpdateLastYellowCard(newTeam.LastYellowCard);
            modified += UpdateShots(newTeam.Shots);
            modified += UpdateShotsOnGoal(newTeam.ShotsOnGoal);
            modified += UpdateFouls(newTeam.Fouls);
            modified += UpdateBallPossession(newTeam.BallPossession);
            modified += UpdateSaves(newTeam.Saves);
            modified += UpdateOffsides(newTeam.Offsides);
            modified += UpdateSubstitutions(newTeam.Substitutions);
            modified += UpdateFirstSubstitution(newTeam.FirstSubstitution);
            modified += UpdateLastSubstitution(newTeam.LastSubstitution);
            modified += UpdateKickOff(newTeam.KickOff);
            return modified;
        }
        public int UpdateYellowCards(string value)
        {
            int modified = 0;
            if (this.YellowCards != value)
            {
                this.YellowCards = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateRedCards(string value)
        {
            int modified = 0;
            if (this.RedCards != value)
            {
                this.RedCards = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateStandingPoints(string value)
        {
            int modified = 0;
            if (this.StandingPoints != value)
            {
                this.StandingPoints = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateCornerKicks(string value)
        {
            int modified = 0;
            if (this.CornerKicks != value)
            {
                this.CornerKicks = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateFirstYellowCard(string value)
        {
            int modified = 0;
            if (this.FirstYellowCard != value)
            {
                this.FirstYellowCard = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateLastYellowCard(string value)
        {
            int modified = 0;
            if (this.LastYellowCard != value)
            {
                this.LastYellowCard = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateShots(string value)
        {
            int modified = 0;
            if (this.Shots != value)
            {
                this.Shots = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateShotsOnGoal(string value)
        {
            int modified = 0;
            if (this.ShotsOnGoal != value)
            {
                this.ShotsOnGoal = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateFouls(string value)
        {
            int modified = 0;
            if (this.Fouls != value)
            {
                this.Fouls = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateBallPossession(string value)
        {
            int modified = 0;
            if (this.BallPossession != value)
            {
                this.BallPossession = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateSaves(string value)
        {
            int modified = 0;
            if (this.Saves != value)
            {
                this.Saves = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateOffsides(string value)
        {
            int modified = 0;
            if (this.Offsides != value)
            {
                this.Offsides = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateSubstitutions(string value)
        {
            int modified = 0;
            if (this.Substitutions != value)
            {
                this.Substitutions = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateFirstSubstitution(string value)
        {
            int modified = 0;
            if (this.FirstSubstitution != value)
            {
                this.FirstSubstitution = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateLastSubstitution(string value)
        {
            int modified = 0;
            if (this.LastSubstitution != value)
            {
                this.LastSubstitution = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateKickOff(string value)
        {
            int modified = 0;
            if (this.KickOff != value)
            {
                this.KickOff = value;
                modified += 1;
            }
            return modified;
        }
        public int UpdateEvent(TeamEventInfo info)
        {
            int modified = 0;
            var query = from e in this.Events
                        where e.Type == info.Type && e.Minute == info.Minute && e.Value == info.Value
                        select e;
            if (query == null || query.Count() == 0)
            {
                this.Events.Add((TeamEventInfo)info.Clone());
                modified += 1;
            }
            return modified;
        }
        public int UpdateEvents(List<TeamEventInfo> events)
        {
            int modified = 0;
            foreach (var info in events)
            {
                UpdateEvent(info);
                modified += 1;
            }
            return modified;
        }
        #endregion
    }
    [Serializable]
    public class TeamEventInfo : ICloneable
    {
        #region Init
        public TeamEventInfo() { } // For Deserializing
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        [XmlAttribute("Type")]
        public string Type;
        [XmlAttribute("Minute")]
        public int Minute;
        [XmlAttribute("Value")]
        public string Value;
    }
    [Serializable]
    public class StartTimeInfo : ICloneable
    {
        #region Init
        [Serializable]
        public class StartTimeChange
        {
            public StartTimeChange() { } // For Deserializing
            public StartTimeChange(DateTime oldValue)
            {
                OldValue = oldValue;
                Time = DateTime.Now;
            }
            [XmlAttribute("Time")]
            public DateTime Time;
            [XmlAttribute("OldValue")]
            public DateTime OldValue;
        }
        DateTime _Value = DateTime.MinValue;
        public StartTimeInfo() { } // For Deserializing
        public StartTimeInfo(DateTime value) { Value = value; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        [XmlAttribute("Value")]
        public DateTime Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != DateTime.MinValue && _Value != value)
                {
                    Changes.Add(new StartTimeChange(_Value));
                }
                _Value = value;
            }
        }
        [XmlElement("Change")]
        public List<StartTimeChange> Changes = new List<StartTimeChange>();
    }
    [Serializable]
    public class OddInfo : ICloneable
    {
        #region Init
        [Serializable]
        public class OddChange
        {
            public OddChange() { } // For Deserializing
            public OddChange(float oldValue)
            {
                OldValue = oldValue;
                Time = DateTime.Now;
            }
            [XmlAttribute("Time")]
            public DateTime Time;
            [XmlAttribute("OldValue")]
            public float OldValue;
        }
        OddName _Key;
        float _Value;
        public OddInfo() { } // For Deserializing
        public OddInfo(OddName key, float value)
        {
            _Key = key;
            _Value = value;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        [XmlAttribute("Id")]
        public OddName Key { get { return _Key; } set { _Key = value; } }
        [XmlAttribute("Value")]
        public float Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != 0 && _Value != value)
                {
                    Changes.Add(new OddChange(_Value));
                }
                _Value = value;
            }
        }
        [XmlElement("Change")]
        public List<OddChange> Changes = new List<OddChange>();
    }
    [Serializable]
    public class OpapInfo : ICloneable
    {
        #region Init
        public OpapInfo() { } // For Deserializing
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        #region Properties
        [XmlAttribute("Code")]
        public string Code;
        [XmlAttribute("Status")]
        public string Status;
        [XmlAttribute("HomeAdv")]
        public float HomeAdv;
        [XmlAttribute("AwayAdv")]
        public float AwayAdv;
        [XmlIgnore]
        public Dictionary<OddName, OddInfo> Odds = new Dictionary<OddName, OddInfo>();
        [XmlElement("Odd")]
        public List<OddInfo> OddList
        {
            get
            {
                return Odds.Select(x => x.Value).ToList();
            }
            set
            {
                this.Odds = new Dictionary<OddName, OddInfo>();
                foreach (var odd in value)
                {
                    this.Odds.Add(odd.Key, odd);
                }
            }
        }
        [XmlIgnore]
        public DateTime OddLatestChange1X2
        {
            get
            {
                var lastChange = DateTime.MinValue;
                var lastChange1 = OddLatestChange(OddName.Odd1);
                if (lastChange < lastChange1) lastChange = lastChange1;
                var lastChangeX = OddLatestChange(OddName.Odd1);
                if (lastChange < lastChangeX) lastChange = lastChangeX;
                var lastChange2 = OddLatestChange(OddName.Odd1);
                if (lastChange < lastChange2) lastChange = lastChange2;
                return lastChange;
            }
        }
        public OddInfo.OddChange OddBegin(OddName name)
        {
            if (Odds.Keys.Contains(name))
            {
                var odd = Odds[name];
                if (odd.Changes != null && odd.Changes.Count > 0)
                {
                    return odd.Changes.OrderBy(x => x.Time).First();
                }
            }
            return null;
        }
        public OddInfo.OddChange OddLatest(OddName name)
        {
            if (Odds.Keys.Contains(name))
            {
                var odd = Odds[name];
                if (odd.Changes != null && odd.Changes.Count > 0)
                {
                    return odd.Changes.OrderByDescending(x => x.Time).First();
                }
            }
            return null;
        }
        public DateTime OddLatestChange(OddName name)
        {
            var lastChange = DateTime.MinValue;
            var lastOdd = OddLatest(name);
            if (lastOdd != null) return lastOdd.Time;
            return lastChange;
        }
        #endregion
        #region Self Upadate
        public int Update(OpapInfo opap)
        {
            int modified = 0;
            UpdateCode(opap.Code);
            UpdateHomeAdv(opap.HomeAdv);
            UpdateAwayAdv(opap.AwayAdv);
            foreach (var oddInfo in opap.Odds)
            {
                modified += this.UpdateOdd(oddInfo.Key, oddInfo.Value.Value);
            }
            return modified;
        }
        public int UpdateCode(string code)
        {
            int modified = 0;
            if (this.Code != code)
            {
                this.Code = code;
                modified += 1;
            }
            return modified;
        }
        public int UpdateHomeAdv(float adv)
        {
            int modified = 0;
            if (this.HomeAdv != adv)
            {
                this.HomeAdv = adv;
                modified += 1;
            }
            return modified;
        }
        public int UpdateAwayAdv(float adv)
        {
            int modified = 0;
            if (this.AwayAdv != adv)
            {
                this.AwayAdv = adv;
                modified += 1;
            }
            return modified;
        }
        public int UpdateOdd(OddName key, float value)
        {
            int modified = 0;
            if (this.Odds.ContainsKey(key))
            {
                if (this.Odds[key].Value != value)
                {
                    this.Odds[key].Value = value;
                    modified += 1;
                }
            }
            else
            {
                this.Odds.Add(key, new OddInfo(key, value));
                modified += 1;
            }
            return modified;
        }
        #endregion
    }
    [Serializable]
    public class MatchInfo : ICloneable
    {
        #region Init
        UInt64 _GlobalId = 0;
        public MatchInfo() { } // For Deserializing
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        #region Properties
        [XmlAttribute("GlobalId")]
        public UInt64 GlobalId
        {
            get
            {
                if (_GlobalId == 0) GetGlobalId();
                return _GlobalId;
            }
            set
            {
                _GlobalId = value;
            }
        }
        [XmlAttribute("Champ")]
        public string Champ;
        [XmlAttribute("Minute")]
        public string Minute;
        [XmlAttribute("Status")]
        public MatchStatus Status;
        [XmlAttribute("Modified")]
        public DateTime Modified;
        [XmlElement("StartTime")]
        public StartTimeInfo StartTime = new StartTimeInfo();
        [XmlElement("HomeTeam")]
        public TeamInfo HomeTeam = new TeamInfo();
        [XmlElement("AwayTeam")]
        public TeamInfo AwayTeam = new TeamInfo();
        [XmlElement("Opap")]
        public OpapInfo OpapInf = new OpapInfo();
        #endregion
        #region XmlIgnore Properties
        [XmlIgnore]
        public string Score
        {
            get
            {
                return JoinScore(HomeTeam.Score, AwayTeam.Score);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.Score = s[0];
                AwayTeam.Score = s[1];
            }
        }
        [XmlIgnore]
        public string ScoreHT
        {
            get
            {
                return JoinScore(HomeTeam.ScoreHT, AwayTeam.ScoreHT);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.ScoreHT = s[0];
                AwayTeam.ScoreHT = s[1];
            }
        }
        [XmlIgnore]
        public string ScoreQ1
        {
            get
            {
                if ((HomeTeam.ScoreQ1 == null || HomeTeam.ScoreQ1 == "")
                 && (AwayTeam.ScoreQ1 == null || AwayTeam.ScoreQ1 == "")) return null;
                return JoinScore(HomeTeam.ScoreQ1, AwayTeam.ScoreQ1);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.ScoreQ1 = s[0];
                AwayTeam.ScoreQ1 = s[1];
            }
        }
        [XmlIgnore]
        public string ScoreQ2
        {
            get
            {
                if ((HomeTeam.ScoreQ2 == null || HomeTeam.ScoreQ2 == "")
                 && (AwayTeam.ScoreQ2 == null || AwayTeam.ScoreQ2 == "")) return null;
                return JoinScore(HomeTeam.ScoreQ2, AwayTeam.ScoreQ2);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.ScoreQ2 = s[0];
                AwayTeam.ScoreQ2 = s[1];
            }
        }
        [XmlIgnore]
        public string ScoreQ3
        {
            get
            {
                if ((HomeTeam.ScoreQ3 == null || HomeTeam.ScoreQ3 == "")
                 && (AwayTeam.ScoreQ3 == null || AwayTeam.ScoreQ3 == "")) return null;
                return JoinScore(HomeTeam.ScoreQ3, AwayTeam.ScoreQ3);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.ScoreQ3 = s[0];
                AwayTeam.ScoreQ3 = s[1];
            }
        }
        [XmlIgnore]
        public string ScoreQ4
        {
            get
            {
                if ((HomeTeam.ScoreQ4 == null || HomeTeam.ScoreQ4 == "")
                 && (AwayTeam.ScoreQ4 == null || AwayTeam.ScoreQ4 == "")) return null;
                return JoinScore(HomeTeam.ScoreQ4, AwayTeam.ScoreQ4);
            }
            set
            {
                string[] s = SplitScore(value);
                HomeTeam.ScoreQ4 = s[0];
                AwayTeam.ScoreQ4 = s[1];
            }
        }
        [XmlIgnore]
        public bool FeedOpap;
        [XmlIgnore]
        public bool FeedNGoal;
        [XmlIgnore]
        public bool FeedSBet;
        #endregion
        #region Readonly Properties
        [XmlIgnore]
        public bool HasFinished
        {
            get
            {
                if (this.Status == MatchStatus.FT) return true;
                if (this.Status == MatchStatus.OT) return true;
                if (this.Status == MatchStatus.Cancelled) return true;
                if (this.Status == MatchStatus.Abd) return true;
                if (this.StartTime != null && this.StartTime.Value.AddHours(6) < DateTime.Now) return true; // after 6 hours is final...
                return false;
            }
        }
        [XmlIgnore]
        public bool HasHalfFinished
        {
            get
            {
                if (this.Status == MatchStatus.HT) return true;
                if (this.Status == MatchStatus.Half2) return true;
                if (this.Status == MatchStatus.Qrt3) return true;
                if (this.Status == MatchStatus.Qrt4) return true;
                if (this.HasFinished) return true;
                return false;
            }
        }
        public string FinalSign
        {
            get
            {
                if (this.HasFinished)
                {
                    float totalScoreHome = iFloat(this.HomeTeam.Score);
                    float totalScoreAway = iFloat(this.AwayTeam.Score);
                    if (totalScoreHome > totalScoreAway) { return "1"; }
                    else if (totalScoreHome < totalScoreAway) { return "2"; }
                    else { return "X"; }
                }
                else { return ""; }
            }
        }
        public string HalfSign
        {
            get
            {
                if (this.HasHalfFinished)
                {
                    float totalScoreHomeHT = iFloat(this.HomeTeam.ScoreHT);
                    float totalScoreAwayHT = iFloat(this.AwayTeam.ScoreHT);
                    if (totalScoreHomeHT > totalScoreAwayHT) { return "1"; }
                    else if (totalScoreHomeHT < totalScoreAwayHT) { return "2"; }
                    else { return "X"; }
                }
                else { return ""; }
            }
        }
        public string Signs
        {
            get
            {
                return string.Format("{0}/{1}", HalfSign, FinalSign);
            }
        }
        public bool IsLive
        {
            get
            {
                if (this.Status == MatchStatus.Qrt1) return true;
                if (this.Status == MatchStatus.Half1) return true;
                if (this.Status == MatchStatus.Qrt2) return true;
                if (this.Status == MatchStatus.HT) return true;
                if (this.Status == MatchStatus.Qrt3) return true;
                if (this.Status == MatchStatus.Half2) return true;
                if (this.Status == MatchStatus.Qrt4) return true;
                if (this.Status == MatchStatus.Pause) return true;
                return false;
            }
        }
        public bool IsLivePlus
        {
            get
            {
                if (this.IsLive) return true;
                if (DateTime.Compare(this.StartTime.Value, DateTime.Now) <= 0)
                {
                    if (DateTime.Compare(this.StartTime.Value.AddHours(2), DateTime.Now) > 0)
                    {
                        return true;
                    }
                }
                if (this.StartTime.Value == DateTime.MinValue) return true;
                return false;
            }
        }
        public bool HasGoalInLast5Minutes
        {
            get
            {
                if (HomeTeam.ScoredInLast5Minutes) return true;
                if (AwayTeam.ScoredInLast5Minutes) return true;
                return false;
            }
        }
        public bool HasGoalInLast20Seconds
        {
            get
            {
                if (HomeTeam.ScoredInLast20Seconds) return true;
                if (AwayTeam.ScoredInLast20Seconds) return true;
                return false;
            }
        }
        public bool HasGoal(Opap.API.Sport sport)
        {
            if (sport == Opap.API.Sport.soccer)
            {
                return HasGoalInLast5Minutes;
            }
            else if (sport == Opap.API.Sport.basketball)
            {
                return HasGoalInLast20Seconds;
            }
            return false;
        }
        public string MinuteText
        {
            get
            {
                if (this.Status == MatchStatus.HT) return "Ημ";
                if (this.Minute == null || this.Minute == "") return "";
                else if (this.Minute.EndsWith("+")) return this.Minute;
                else return this.Minute + "'";
            }
        }
        public double MinuteNum
        {
            get
            {
                try
                {
                    if (this.Minute == null || this.Minute == "") return 0;
                    else if (this.Minute == "45+") return 45.5;
                    else if (this.Minute == "90+") return 90.5;
                    else return double.Parse(this.Minute);
                }
                catch { }
                return 0;
            }
        }
        public string MinuteLevel(Opap.API.Sport sport)
        {
            if (sport == Opap.API.Sport.soccer) return MinuteLevelF;
            else if (sport == Opap.API.Sport.basketball) return MinuteLevelB;
            return "";
        }
        public string MinuteLevelF
        {
            get
            {
                if (this.Status == MatchStatus.Half1) return "1";
                else if (this.Status == MatchStatus.HT) return "2";
                else if (this.Status == MatchStatus.Half2)
                {
                    if (this.MinuteNum < 80) return "3";
                    else if (this.MinuteNum < 88) return "4";
                    else return "5";
                }
                return "";
            }
        }
        public string MinuteLevelB
        {
            get
            {
                if (this.Status == MatchStatus.Qrt1 || this.Status == MatchStatus.Half1 || this.Status == MatchStatus.Qrt2) return "1";
                else if (this.Status == MatchStatus.HT) return "2";
                else if (this.Status == MatchStatus.Half2) return "3";
                else if (this.Status == MatchStatus.Qrt3) return "3";
                else if (this.Status == MatchStatus.Qrt4) return "4";
                return "";
            }
        }
        #endregion
        #region Methods
        public void GetGlobalId()
        {
            if (StartTime.Value > DateTime.MinValue && HomeTeam.Id > 0 && AwayTeam.Id > 0)
            {
                this.GlobalId = GetGlobalId(StartTime.Value, HomeTeam.Id, AwayTeam.Id);
            }
        }
        static UInt64 GetGlobalId(DateTime StartTime, UInt16 HomeTeamId, UInt16 AwayTeamId)
        {
            UInt16 Year = (UInt16)StartTime.Year;
            Byte Month = (Byte)StartTime.Month;
            Byte Day = (Byte)StartTime.Day;
            return GetGlobalId(Year, Month, Day, HomeTeamId, AwayTeamId);
        }
        static UInt64 GetGlobalId(UInt16 Year, Byte Month, Byte Day, UInt16 HomeTeamId, UInt16 AwayTeamId)
        {
            byte[] bytes = new byte[8];
            BitConverter.GetBytes(Year).CopyTo(bytes, 0);
            BitConverter.GetBytes(Month).CopyTo(bytes, 2);
            BitConverter.GetBytes(Day).CopyTo(bytes, 3);
            BitConverter.GetBytes(HomeTeamId).CopyTo(bytes, 4);
            BitConverter.GetBytes(AwayTeamId).CopyTo(bytes, 6);
            return BitConverter.ToUInt64(bytes, 0);
        }
        static string JoinScore(string scoreHome, string scoreAway)
        {
            return string.Format("{0}-{1}", scoreHome, scoreAway);
        }
        static string[] SplitScore(string score)
        {
            string ScoreHome = "", ScoreAway = "";
            if (score != null)
            {
                string[] s = score.Split('-');
                if (score.Length >= 3 && s.Length == 2)
                {
                    ScoreHome = s[0].Trim();
                    ScoreAway = s[1].Trim();
                }
            }
            return new string[] { ScoreHome, ScoreAway };
        }
        #endregion
        #region Library
        static float iFloat(string s)
        {
            try
            {
                var info = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
                info.NumberFormat.CurrencyDecimalSeparator = ".";
                return float.Parse(s, System.Globalization.NumberStyles.Any, info);
            }
            catch { return 0; }
        }
        #endregion
        #region Self Update
        public int Update(MatchInfo newMatch)
        {
            int modified = 0;
            modified += UpdateChamp(newMatch.Champ);
            modified += UpdateMinute(newMatch.Minute);
            modified += UpdateStatus(newMatch.Status);
            modified += UpdateStartTime(newMatch.StartTime);
            modified += this.HomeTeam.Update(newMatch.HomeTeam);
            modified += this.AwayTeam.Update(newMatch.AwayTeam);
            modified += this.OpapInf.Update(newMatch.OpapInf);
            return modified;
        }
        public int UpdateChamp(string champ)
        {
            int modified = 0;
            if (this.Champ != champ)
            {
                this.Champ = champ;
                modified += 1;
            }
            return modified;
        }
        public int UpdateMinute(string minute)
        {
            int modified = 0;
            if (this.Minute != minute)
            {
                this.Minute = minute;
                modified += 1;
            }
            return modified;
        }
        public int UpdateStatus(MatchStatus status)
        {
            int modified = 0;
            if (this.Status != status)
            {
                this.Status = status;
                modified += 1;
            }
            return modified;
        }
        public int UpdateStartTime(StartTimeInfo startTime)
        {
            int modified = 0;
            if (this.StartTime.Value != startTime.Value)
            {
                this.StartTime.Value = startTime.Value;
                modified += 1;
            }
            return modified;
        }
        #endregion
    }
    [Serializable]
    [XmlRoot("MatchDay")]
    public class MatchDay : ICloneable
    {
        #region Init
        DateTime _Modified = DateTime.MinValue;
        public MatchDay() { }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
        #region Properties
        [XmlElement("Match")]
        public List<MatchInfo> Matches = new List<MatchInfo>();
        [XmlIgnore]
        public Stream FlatXmlStream
        {
            get
            {
                var view = new MatchDayView(this);
                return view.XmlStream;
            }
        }
        [XmlAttribute("Modified")]
        public DateTime Modified
        {
            get
            {
                foreach (var match in Matches)
                {
                    if (match.Modified > _Modified) _Modified = match.Modified;
                }
                return _Modified;
            }
            set
            {
                _Modified = value;
            }
        }
        #endregion
        #region XML Serialization Methods
        public static MatchDay ReadXml(string inputUri)
        {
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(inputUri))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                    MatchDay day = (MatchDay)serializer.Deserialize(xmlReader);
                    return day;
                }
            }
            catch { }
            return new MatchDay();
        }
        public static MatchDay ReadXml(Stream stream)
        {
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                    MatchDay day = (MatchDay)serializer.Deserialize(xmlReader);
                    return day;
                }
            }
            catch { return new MatchDay(); }
        }
        public static MatchDay ReadXml(byte[] data)
        {
            var stream = new MemoryStream(data);
            return ReadXml(stream);
        }
        public void WriteXml(string outputFileName)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                using (XmlWriter writer = XmlWriter.Create(outputFileName, settings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                    serializer.Serialize(writer, this, namespaces);
                }
            }
            catch { }
        }
        public static MatchDay ReadXmlGZ(string path)
        {
            try
            {
                using (Stream ungzipped = new MemoryStream())
                using (GZipStream gzipped = new GZipStream(File.OpenRead(path), CompressionMode.Decompress))
                {
                    gzipped.CopyTo(ungzipped);
                    ungzipped.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                    MatchDay day = (MatchDay)serializer.Deserialize(ungzipped);
                    return day;
                }
            }
            catch { }
            return new MatchDay();
        }
        public string WriteToTmpDirGZ(string tmpDir, string fileName)
        {
            string FullFileName = Path.Combine(tmpDir, fileName);
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                using (FileStream outputFile = File.Create(FullFileName))
                using (GZipStream gzipStream = new GZipStream(outputFile, CompressionMode.Compress))
                using (XmlWriter xmlWriter = XmlWriter.Create(gzipStream, settings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                    serializer.Serialize(xmlWriter, this, namespaces);
                }
            }
            catch { }
            return FullFileName;
        }
        public static MatchDay ReadXmlString(string data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MatchDay));
                return (MatchDay)serializer.Deserialize(new StringReader(data));
            }
            catch { }
            return new MatchDay();
        }
        #endregion
    }
    [Serializable]
    [XmlRoot("MatchDay")]
    public class MatchDayView
    {
        public class MatchView
        {
            [XmlElement("GlobalId")]
            public UInt64 GlobalId;
            [XmlElement("Champ")]
            public string Champ;
            [XmlElement("StartTime")]
            public DateTime StartTime;
            [XmlElement("Code")]
            public string OpapCode;
            [XmlElement("HomeTeam")]
            public string HomeTeam;
            [XmlElement("HomeTeamId")]
            public UInt16 HomeTeamId;
            [XmlElement("AwayTeam")]
            public string AwayTeam;
            [XmlElement("AwayTeamId")]
            public UInt16 AwayTeamId;
            [XmlElement("Q1")]
            public string ScoreQ1;
            [XmlElement("Q2")]
            public string ScoreQ2;
            [XmlElement("HT")]
            public string ScoreHT;
            [XmlElement("Q3")]
            public string ScoreQ3;
            [XmlElement("Q4")]
            public string ScoreQ4;
            [XmlElement("Score")]
            public string Score;
            [XmlElement("Status")]
            public MatchStatus Status;
            [XmlElement("Min")]
            public string Minute;
            [XmlElement("YH")]
            public string YellowCardsHome;
            [XmlElement("YA")]
            public string YellowCardsAway;
            [XmlElement("RH")]
            public string RedCardsHome;
            [XmlElement("RA")]
            public string RedCardsAway;
            [XmlElement("FdOp")]
            public bool FeedOpap;
            [XmlElement("FdNG")]
            public bool FeedNGoal;
            [XmlElement("FdSB")]
            public bool FeedSBet;
        }
        public MatchDayView() { }
        public MatchDayView(MatchDay matchDay)
        {
            Matches = new List<MatchView>();
            foreach (var match in matchDay.Matches)
            {
                var matchView = new MatchView();
                matchView.GlobalId = match.GlobalId;
                matchView.Champ = match.Champ;
                matchView.StartTime = match.StartTime.Value;
                matchView.OpapCode = match.OpapInf.Code;
                matchView.HomeTeam = match.HomeTeam.Name;
                matchView.HomeTeamId = match.HomeTeam.Id;
                matchView.AwayTeam = match.AwayTeam.Name;
                matchView.AwayTeamId = match.AwayTeam.Id;
                matchView.Status = match.Status;
                matchView.ScoreQ1 = match.ScoreQ1;
                matchView.ScoreQ2 = match.ScoreQ2;
                matchView.ScoreQ3 = match.ScoreQ3;
                matchView.ScoreQ4 = match.ScoreQ4;
                matchView.ScoreHT = match.ScoreHT;
                matchView.Score = match.Score;
                matchView.Minute = match.Minute;
                matchView.YellowCardsHome = match.HomeTeam.YellowCards;
                matchView.YellowCardsAway = match.AwayTeam.YellowCards;
                matchView.RedCardsHome = match.HomeTeam.RedCards;
                matchView.RedCardsAway = match.AwayTeam.RedCards;
                matchView.FeedOpap = match.FeedOpap;
                matchView.FeedNGoal = match.FeedNGoal;
                matchView.FeedSBet = match.FeedSBet;
                Matches.Add(matchView);
            }
            Matches = Matches.OrderBy(x => x.StartTime).ToList();
        }
        [XmlElement("Match")]
        public List<MatchView> Matches;
        public Stream XmlStream
        {
            get
            {
                var serializer = new XmlSerializer(this.GetType());
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, this);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}
