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
            var celestialObject = _context.CelestialObjects
                .FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
                return NotFound();

            foreach (var cObject in _context.CelestialObjects)
            {
                if (cObject.OrbitedObjectId == id)
                {
                    celestialObject.Satellites.Add(cObject);
                }
            }

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

            foreach (var cObject in _context.CelestialObjects)
            {
                var matchingCelestialObject = celestialObjects.FirstOrDefault(
                    co => co.Id == cObject.OrbitedObjectId);

                if (matchingCelestialObject != null)
                {
                    matchingCelestialObject.Satellites.Add(cObject);
                }
            }

            return Ok(celestialObjects);
        }

    }
}
