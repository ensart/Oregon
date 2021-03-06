﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oregon.Core.Repository;
using Oregon.Data.Model;
using Oregon.Data.Model.TeamProfile;
using Oregon.Data.Model.TeamStats;
using Oregon.Data.Model.Tournaments;
using Oregon.Data.Model.TournamentTeams;

namespace Oregon.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> tournamentIdList = new List<string>();
            List<string> teamIdList = new List<string>();
            var teamProfileRepository = new TeamProfileRepository();
            var teamStatsRepository = new TeamStatsRepository();


            using (WebClient wc = new WebClient())
            {
                var tournamentIdJson =
                    wc.DownloadString(
                        "https://api.sportradar.us/soccer-t3/eu/en/tournaments.json?api_key=9a8uwb6awpsek5pghan6wxg5");
                Thread.Sleep(1000);
                var tournamentIdJsonData = JsonConvert.DeserializeObject<Tournaments>(tournamentIdJson);
                foreach (var item in tournamentIdJsonData.tournaments)
                {
                    tournamentIdList.Add(item.id);
                }
            }

            foreach (var tournamentId in tournamentIdList)
            {
                using (WebClient wc = new WebClient())
                {
                    var teamIdJson =
                        wc.DownloadString(
                            $"https://api.sportradar.us/soccer-t3/eu/en/tournaments/{tournamentId}/info.json?api_key=9a8uwb6awpsek5pghan6wxg5");
                    Thread.Sleep(1000);
                    var teamIdJsonData = JsonConvert.DeserializeObject<TournamentTeams>(teamIdJson);

                    foreach (var item in teamIdJsonData.groups)
                    {
                        foreach (var team in item.teams)
                        {
                            teamIdList.Add(team.id);
                        }
                    }
                }
            }


            using (WebClient wc = new WebClient())
            {
                foreach (var teamId in teamIdList)
                {
                    var jsonString =
                        wc.DownloadString(
                            $"https://api.sportradar.us/soccer-t3/eu/tr/teams/{teamId}/profile.json?api_key=9a8uwb6awpsek5pghan6wxg5");
                    Thread.Sleep(1000);
                    var teamProfileData = JsonConvert.DeserializeObject<TeamProfileModel>(jsonString);
                    teamProfileRepository.Insert(teamProfileData);
                    teamProfileRepository.Save();
                }
            }


            using (WebClient wc = new WebClient())
            {
                foreach (var tournamentId in tournamentIdList)
                {
                    foreach (var teamId in teamIdList)
                    {
                        try
                        {
                            var jsonString = wc.DownloadString($"https://api.sportradar.us/soccer-t3/eu/tr/tournaments/{tournamentId}/teams/{teamId}/statistics.json?api_key=9a8uwb6awpsek5pghan6wxg5");
                            Thread.Sleep(1000);
                            var teamStatsData = JsonConvert.DeserializeObject<TeamStats>(jsonString);
                            teamStatsRepository.Insert(teamStatsData);
                            teamStatsRepository.Save();
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }
}
