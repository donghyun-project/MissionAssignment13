using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MissionAssignment13.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MissionAssignment13.Controllers
{
    public class HomeController : Controller
    {
        private BowlersDbContext bowler { get; set; }

        // Constructor
        public HomeController(BowlersDbContext temp)
        { 
            bowler = temp;
        }

        // Index view
        public IActionResult Index(string teamName)
        {
            // See if key-value "id" exists
            HttpContext.Session.Remove("id");

            // assign teamName to ViewBag.TeamName
            ViewBag.TeamName = teamName ?? "Home";

            // get bowlers from the team
            var blah = bowler.Bowlers
                .Include(x => x.Team)
                .Where(x => x.Team.TeamName == teamName || teamName == null)
                .ToList();

            return View(blah);
        }

        [HttpGet]
        public IActionResult Form()
        {
            // assign a list of teams in ViewBag.Teams
            ViewBag.Teams = bowler.Teams.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Form(Bowler b)
        {
            // get the max Bowler ID
            int max = 0;

            foreach(var s in bowler.Bowlers)
            {
                if (max < s.BowlerID)
                {
                    max = s.BowlerID;
                }
            }

            // assign BowlerID yo max + 1 (to get a new Bowler ID)
            b.BowlerID = max + 1;

            if (ModelState.IsValid)
            {
                bowler.Add(b);
                bowler.SaveChanges();

                // redirect to Index
                return RedirectToAction("Index", new { teamName = "" });
            }
            else
            {
                return View();
            }
        }


        // Edit the bowler
        [HttpGet]
        public IActionResult Edit(int bowlerID)
        {
            // editing
            ViewBag.New = false;

            // get a list of teams and assign them in ViwBag.Teams
            ViewBag.Teams = bowler.Teams.ToList();

            // set a key-value "id" with BowlerID 
            HttpContext.Session.SetString("id", bowlerID.ToString());

            // get the bowler
            var record = bowler.Bowlers.Single(x => x.BowlerID == bowlerID);

            return View("Form", record);
        }

        [HttpPost]
        public IActionResult Edit(Bowler b)
        {
            // get the id and convert it to int type
            string id = HttpContext.Session.GetString("id");
            int int_id = int.Parse(id);

            // assign Bowler ID
            b.BowlerID = int_id;
            
            // model validation
            if (ModelState.IsValid)
            {
                // edit the model
                bowler.Update(b);
                bowler.SaveChanges();
                HttpContext.Session.Remove("id");

                return RedirectToAction("Index", new { teamName = "" });
            }

            
            ViewBag.New = false;
            ViewBag.Teams = bowler.Teams.ToList();

            return View("Form", b);

        }

        // Delete
        public IActionResult Delete(int bowlerID)
        {
            var record = bowler.Bowlers.Single(x => x.BowlerID == bowlerID);
            bowler.Bowlers.Remove(record);
            bowler.SaveChanges();

            return RedirectToAction("Index", new { teamName = "" });
        }
    }
}
