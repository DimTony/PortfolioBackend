using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Data;
using PortfolioBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects
                .Include(p => p.Comments)
                .Include(p => p.Desc)
                    .ThenInclude(d => d.Objectives)
                .ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Comments)
                .Include(p => p.Desc)
                    .ThenInclude(d => d.Objectives)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/Projects
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Reset IDs to ensure database generates new ones
            project.Id = 0;

            if (project.Desc != null)
            {
                project.Desc.Id = 0;

                if (project.Desc.Objectives != null)
                {
                    foreach (var objective in project.Desc.Objectives)
                    {
                        objective.Id = 0;
                    }
                }
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Reload the project with all related data
            var createdProject = await _context.Projects
                .Include(p => p.Comments)
                .Include(p => p.Desc)
                    .ThenInclude(d => d.Objectives)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, createdProject);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // First, remove existing related entities to avoid conflicts
            var existingProject = await _context.Projects
                .Include(p => p.Desc)
                    .ThenInclude(d => d.Objectives)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProject == null)
            {
                return NotFound();
            }

            // Update the project properties
            _context.Entry(existingProject).CurrentValues.SetValues(project);

            // Handle ProjectDescription
            if (existingProject.Desc != null && project.Desc != null)
            {
                _context.Entry(existingProject.Desc).CurrentValues.SetValues(project.Desc);

                // Handle Objectives
                if (existingProject.Desc.Objectives != null)
                {
                    // Remove objectives not in the updated list
                    foreach (var existingObjective in existingProject.Desc.Objectives.ToList())
                    {
                        if (project.Desc.Objectives == null ||
                            !project.Desc.Objectives.Any(o => o.Id == existingObjective.Id))
                        {
                            _context.Objectives.Remove(existingObjective);
                        }
                    }

                    // Update existing and add new objectives
                    if (project.Desc.Objectives != null)
                    {
                        foreach (var updatedObjective in project.Desc.Objectives)
                        {
                            var existingObjective = existingProject.Desc.Objectives
                                .FirstOrDefault(o => o.Id == updatedObjective.Id);

                            if (existingObjective != null)
                            {
                                _context.Entry(existingObjective).CurrentValues.SetValues(updatedObjective);
                            }
                            else
                            {
                                existingProject.Desc.Objectives.Add(new Objective
                                {
                                    Text = updatedObjective.Text
                                });
                            }
                        }
                    }
                }
                else if (project.Desc.Objectives != null)
                {
                    // If existing objectives is null but updated has objectives
                    existingProject.Desc.Objectives = new List<Objective>();
                    foreach (var objective in project.Desc.Objectives)
                    {
                        existingProject.Desc.Objectives.Add(new Objective
                        {
                            Text = objective.Text
                        });
                    }
                }
            }
            else if (project.Desc != null)
            {
                // If existing desc is null but updated has desc
                existingProject.Desc = new ProjectDescription
                {
                    Title = project.Desc.Title,
                    Summary = project.Desc.Summary,
                    Footer = project.Desc.Footer,
                    Objectives = project.Desc.Objectives?.Select(o => new Objective
                    {
                        Text = o.Text
                    }).ToList() ?? new List<Objective>()
                };
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Desc)
                    .ThenInclude(d => d.Objectives)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}