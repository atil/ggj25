using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace JamKit
{
    public class LeaderboardEntry
    {
        public string PlayerName;
        public int Score;
    }
    
    public partial class JamKit
    {
        [SerializeField] private TextAsset _leaderboardEnv;

        public bool IsLeaderboardRequestRunning { get; private set; }

        private bool _isLeaderboardEnabled = false;
        private const string DreamloUrl = "https://dreamlo.com/lb";
        private string _dreamloPrivateKey = "";
        private string _dreamloPublicKey = "";

        private void StartLeaderboard()
        {
            string[] lines = _leaderboardEnv?.text.Split('\n');
            _isLeaderboardEnabled = lines != null && lines.Length == 2;

            if (_isLeaderboardEnabled)
            {
                _dreamloPrivateKey = lines[0];
                _dreamloPublicKey = lines[1];
            }
        }

        public void GetLeaderboard(Action<LeaderboardEntry[]> onLeaderboardFetched)
        {
            if (!_isLeaderboardEnabled) return;

            string req = $"{DreamloUrl}/{_dreamloPublicKey}/pipe/5";
            Run(GetCoroutine(req, onLeaderboardFetched));
        }

        private IEnumerator GetCoroutine(string url, Action<LeaderboardEntry[]> onLeaderboardFetched)
        {
            IsLeaderboardRequestRunning = true;
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            IsLeaderboardRequestRunning = false;

            string[] lines = www.downloadHandler.text.Split('\n');
            if (lines.Length == 0) yield break; // Empty leaderboard
            
            // NOTE: We're assuming a leaderboard with a single entry has a newline character at the end
            LeaderboardEntry[] entries = new LeaderboardEntry[lines.Length - 1];
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] fields = lines[i].Split('|');
                Debug.Assert(fields.Length == 6, $"Line is malformed: {lines[i]}");

                string playerName = fields[0];
                int.TryParse(fields[1], out int score);
                int.TryParse(fields[2], out int unusedInt);
                string unusuedString = fields[3];
                DateTime.TryParse(fields[4], out DateTime unusedDateTime);
                int.TryParse(fields[5], out int rank);

                entries[i] = new LeaderboardEntry
                {
                    PlayerName = playerName,
                    Score = score
                };
            }

            onLeaderboardFetched(entries);
        }

        public void PostLeaderboardScore(string playerName, int score)
        {
            if (!_isLeaderboardEnabled) return;

            string req = $"{DreamloUrl}/{_dreamloPrivateKey}/add/{playerName}/{score}";
            Run(PostCoroutine(req));
        }

        private IEnumerator PostCoroutine(string url)
        {
            IsLeaderboardRequestRunning = true;
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            IsLeaderboardRequestRunning = false;
        }
    }
}