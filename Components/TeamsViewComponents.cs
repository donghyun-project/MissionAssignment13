using Microsoft.AspNetCore.Mvc;
using MissionAssignment13.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissionAssignment13.Components
{
    public class TeamsViewComponents : ViewComponent
    {
        // getting Bowler context
        private BowlersDbContext bowler { get; set; }

        // constructor
        public TeamsViewComponents(BowlersDbContext temp)
        {
            bowler = temp;
        }

        public IViewComponentResult Invoke()
        {
            // get the route data of teamName and assign in ViewBag.SelectedTeam
            ViewBag.SelectedTeam = RouteData?.Values["teamName"] ?? "";

            // get team names from context
            var teams = bowler.Bowlers
                .Select(x => x.Team.TeamName)
                .Distinct()
                .OrderBy(x => x);

            return View(teams);
        }
    }
}
