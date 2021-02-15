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
    }
}
