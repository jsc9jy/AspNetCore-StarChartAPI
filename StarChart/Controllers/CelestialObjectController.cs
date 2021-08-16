using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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

    }
}
