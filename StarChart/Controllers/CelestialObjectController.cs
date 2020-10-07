using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      this._context = context;
    }

    [HttpGet("{id:int}", Name = "GetById")]
    public IActionResult GetById(int id)
    {
      var celestialObject = _context.CelestialObjects.Find(id);
      if (celestialObject == null)
      {
        return NotFound();
      }
      celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
      return Ok(celestialObject);
    }

    [HttpGet("{name}", Name = "GetByName")]
    public IActionResult GetByName(string name)
    {
      var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
      if (!celestialObjects.Any()) return NotFound();
      foreach (var celestialObject in celestialObjects)
      {
        celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
      }
      return Ok(celestialObjects);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
      var celestialObjects = _context.CelestialObjects.ToList();
      foreach (var celestialObject in celestialObjects)
      {
        celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
      }
      return Ok(celestialObjects);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CelestialObject celestialObject)
    {
      var result = _context.CelestialObjects.Add(celestialObject);
      _context.SaveChanges();
      return CreatedAtRoute(
        "GetById",
        new { id = celestialObject.Id },
        celestialObject);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CelestialObject celestialObjectForUpdate)
    {
      var celestialObject = _context.CelestialObjects.Find(id);
      if (celestialObject == null) return NotFound();
      celestialObject.Name = celestialObjectForUpdate.Name;
      celestialObject.OrbitalPeriod = celestialObjectForUpdate.OrbitalPeriod;
      celestialObject.OrbitedObjectId = celestialObjectForUpdate.OrbitedObjectId;
      _context.Update(celestialObject);
      _context.SaveChanges();
      return NoContent();
    }

    [HttpPatch("{id}/{name}")]
    public IActionResult RenameObject(int id, string name)
    {
      var celestialObject = _context.CelestialObjects.Find(id);
      if (celestialObject == null) return NotFound();
      celestialObject.Name = name;
      _context.Update(celestialObject);
      _context.SaveChanges();
      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
      if (!celestialObjects.Any()) return NotFound();
      _context.CelestialObjects.RemoveRange(celestialObjects);
      _context.SaveChanges();
      return NoContent();
    }
  }
}
