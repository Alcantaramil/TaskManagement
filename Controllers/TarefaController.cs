using Microsoft.AspNetCore.Mvc;
using TaskManagement.Services;
using TaskManagement.DTOs;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefaController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }
        
        [HttpGet]        
        public async Task<IActionResult> GetAll([FromQuery] int? status, [FromQuery] DateOnly? dataVencimento)
        {
            var tarefas = await _tarefaService.GetAllTarefasAsync(status, dataVencimento);
                                                                                 
            return Ok(tarefas);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var tarefa = await _tarefaService.GetTarefaByIdAsync(id);
                return Ok(tarefa);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(TarefaRequestDto tarefaDto)
        {
            //await _tarefaService.AddTarefaAsync(tarefaDto);
            //return CreatedAtAction(nameof(GetById), new { id = tarefaDto.Id }, tarefaDto);

            var id = await _tarefaService.AddTarefaAsync(tarefaDto);

            return CreatedAtAction(nameof(GetById),new { id },new { id });

        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TarefaRequestDto tarefaDto)
        {
            try
            {
                await _tarefaService.UpdateTarefaAsync(id, tarefaDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tarefaService.DeleteTarefaAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
