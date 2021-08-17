using System;
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

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(celestialObject => celestialObject.Name == name)
                .ToList();

            if (celestialObjects.Count == 0)
                return NotFound();

            foreach (var cObject in celestialObjects)
            {
                cObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == cObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach(var cObject in celestialObjects)
            {
                cObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == cObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObjectFromDB = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObjectFromDB == null)
                return NotFound();

            celestialObjectFromDB.Name = celestialObject.Name;
            celestialObjectFromDB.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectFromDB.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObjectFromDB);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var interestedObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (interestedObject == null)
                return NotFound();

            interestedObject.Name = name;
            _context.CelestialObjects.Update(interestedObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var interestedObjects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();

            if (interestedObjects.Count == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(interestedObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
