using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        // GET /groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagementGroup>>> GetGroups()
        {
            var groups = await _groupRepository.GetGroupsAsync();
            return Ok(groups);
        }

        // GET /groups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagementGroup>> GetGroup(string id)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }

        // POST /groups
        [HttpPost]
        public async Task<ActionResult<ManagementGroup>> CreateGroup(ManagementGroup group)
        {
            await _groupRepository.CreateGroupAsync(group);
            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
        }

        // PUT /groups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(string id, ManagementGroup group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            await _groupRepository.UpdateGroupAsync(id, group);
            return NoContent();
        }

        // DELETE /groups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            await _groupRepository.DeleteGroupAsync(id);
            return NoContent();
        }


    }
}
