using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;

namespace fut7Manager.Api.Helpers {
    public class MatchResultApplier {
        public void Apply(
            Dictionary<int, StandingAccumulator> table,
            Fut7Match m,
            bool usePenalties) {
            if (!table.TryGetValue(m.HomeTeamId, out var home))
                return;

            if (!table.TryGetValue(m.AwayTeamId, out var away))
                return;

            int hg = m.HomeGoals!.Value;
            int ag = m.AwayGoals!.Value;

            home.Played++;
            away.Played++;

            home.GoalsFor += hg;
            home.GoalsAgainst += ag;

            away.GoalsFor += ag;
            away.GoalsAgainst += hg;

            // HOME WIN
            if (hg > ag) {
                ApplyWin(home, away);
                return;
            }

            // AWAY WIN
            if (ag > hg) {
                ApplyWin(away, home);
                return;
            }

            // DRAW
            ApplyDraw(home, away, m, usePenalties);
        }

        private void ApplyWin(StandingAccumulator winner, StandingAccumulator loser) {
            winner.Won++;
            winner.Points += 3;
            Add(winner, "W");

            loser.Lost++;
            Add(loser, "L");
        }

        private void ApplyDraw(
            StandingAccumulator home,
            StandingAccumulator away,
            Fut7Match m,
            bool usePenalties) {
            home.Draw++;
            away.Draw++;

            if (!usePenalties) {
                home.Points++;
                away.Points++;
            } else {
                var hp = m.HomePenaltyGoals ?? 0;
                var ap = m.AwayPenaltyGoals ?? 0;

                if (hp > ap) {
                    home.Points += 2;
                    away.Points += 1;
                } else if (ap > hp) {
                    home.Points += 1;
                    away.Points += 2;
                } else {
                    home.Points++;
                    away.Points++;
                }
            }

            Add(home, "D");
            Add(away, "D");
        }

        private void Add(StandingAccumulator s, string r) {
            s.Last5Results.Insert(0, r);
            if (s.Last5Results.Count > 5)
                s.Last5Results.RemoveAt(5);
        }
    }
}
