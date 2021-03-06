﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("{id:int}", Name ="GetById")]        
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name ="GetByName")]
         public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Name.Equals(name)).ToList();
            if(!celestialObjects.Any())
                return NotFound();

            foreach(var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);

        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach(var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);            
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObj = _context.CelestialObjects.Find(id);
            if (celestialObj == null)
                return NotFound();
            celestialObj.Name = celestialObject.Name;
            celestialObj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObj.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(celestialObj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        } 


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Id.Equals(id) || e.OrbitedObjectId.Equals(id)).ToList();
            if (!celestialObjects.Any())
                return NotFound();
            foreach(var celestialObject in celestialObjects)
            {
                _context.RemoveRange(celestialObjects);
            }
            _context.SaveChanges();
            return NoContent();
        }
    }
}
