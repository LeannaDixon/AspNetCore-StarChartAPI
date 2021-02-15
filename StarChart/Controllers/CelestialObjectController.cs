using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = "GetById";

            var orbitalObject = _context.CelestialObjects
                .Where(x => x.OrbitedObjectId == celestialObject.Id)
                .FirstOrDefault();

            if (orbitalObject != null)
            {
                celestialObject.Satellites.Add(orbitalObject);
            }

            return Ok(celestialObject);
            
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(x => x.Name == name);

            if (celestialObjects.Count() == 0)
            {
                return NotFound();
            }

            var orbitalObjects = _context.CelestialObjects
                .Where(x => x.OrbitedObjectId == celestialObjects.FirstOrDefault().Id);

            if (orbitalObjects != null)
            {
                foreach (var star in celestialObjects)
                {
                    star.Satellites.AddRange(orbitalObjects);
                }
            }

            return Ok(celestialObjects);
        }
    }
}
