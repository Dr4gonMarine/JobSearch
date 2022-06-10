﻿using JobSearch.API.Database;
using JobSearch.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace JobSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private int numberOfRegistryByPage = 5;
        private JobSearchContext _data;

        public JobsController(JobSearchContext data)
        {
            _data = data;
        }

        [HttpGet]
        public IEnumerable<Job> GetJobs(string word, string cityState, int numberOfPage = 1)
        {
            if (word == null)
                word = "";
            if (cityState == null)
                cityState = "";

           return _data.Jobs.Where(
                a => a.PublicationDate >= System.DateTime.Now.AddDays(-15) &&
                a.CityState.ToLower().Contains(cityState.ToLower()) && a.JobTitle.ToLower().Contains(word.ToLower()) || a.TecnologyTools.ToLower().Contains(word.ToLower()) || a.Company.ToLower().Contains(word.ToLower())
                ).Skip(numberOfRegistryByPage * (numberOfPage -1))
                .Take(numberOfRegistryByPage)
                .ToList<Job>();
        }

        //api/Jobs/1
        [HttpGet("{id}")]
        public IActionResult GetJob(int id)
        {
            Job jobDb = _data.Jobs.Find(id);
            if(jobDb == null)
                return NotFound();

            return new JsonResult(jobDb);
        }
        public IActionResult AddJob(Job job)
        {
            _data.Jobs.Add(job);
            _data.SaveChanges();

            return CreatedAtAction("GetJob", new {id = job.Id}, job);
        }
    }
}
