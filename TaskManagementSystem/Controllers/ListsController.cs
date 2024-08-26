using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IListRepository _listRepository;

        public ListsController(IListRepository listRepository)
        {
            _listRepository = listRepository;
        }

        // GET /lists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagementList>>> GetLists()
        {
            var lists = await _listRepository.GetListsAsync();
            return Ok(lists);
        }

        // GET /lists/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagementList>> GetList(string id)
        {
            var list = await _listRepository.GetListByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // POST /lists
        [HttpPost]
        public async Task<ActionResult<ManagementList>> CreateList(ManagementList list)
        {
            await _listRepository.CreateListAsync(list);
            return CreatedAtAction(nameof(GetList), new { id = list.Id }, list);
        }

        // PUT /lists/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateList(string id, ManagementList list)
        {
            if (id != list.Id)
            {
                return BadRequest();
            }

            await _listRepository.UpdateListAsync(id, list);
            return NoContent();
        }

        // DELETE /lists/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(string id)
        {
            var list = await _listRepository.GetListByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            await _listRepository.DeleteListAsync(id);
            return NoContent();
        }



    }
}
