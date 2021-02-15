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
                return NotFound();

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
                return NotFound();

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

        [HttpGet]
        public IActionResult GetAll()
        {
           var allCelestialObjects = (IEnumerable<CelestialObject>)_context.CelestialObjects;

            foreach (var star in allCelestialObjects)
            {
                var nextStarId = star.Id +1;
                if (nextStarId > allCelestialObjects.Count())
                {
                    var previousStarId = star.Id - 1;
                    star.Satellites.Add(allCelestialObjects.Where(x => x.Id == previousStarId).FirstOrDefault());
                }
                star.Satellites.Add(allCelestialObjects.Where(x => x.Id == nextStarId).FirstOrDefault());
            }
            
            return Ok(allCelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { Id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (celestialObjectToUpdate == null)
                return NotFound();

            celestialObjectToUpdate.Name = celestialObject.Name;
            celestialObjectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(celestialObjectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (celestialObjectToUpdate == null)
                return NotFound();

            celestialObjectToUpdate.Name = name;
            _context.CelestialObjects.Update(celestialObjectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjectsToDelete = _context.CelestialObjects.Where(x => x.Id == id  || x.OrbitedObjectId == id);
            if (celestialObjectsToDelete.Count() == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjectsToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
