using TaskManagement.Repositories;
using TaskManagement.Entities;
using TaskManagement.DTOs;
using TaskManagement.Enum;

namespace TaskManagement.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _tarefaRepository;

        public TarefaService(ITarefaRepository tarefaRepository)
        {
            _tarefaRepository = tarefaRepository;
        }
        
        public async Task<IEnumerable<TarefaResponseDto>> GetAllTarefasAsync(int? status = null, DateOnly? dataVencimento = null)
        {
            var tarefas = await _tarefaRepository.GetFilteredAsync(status, dataVencimento); // Fetch all tarefas from repository
            
            return tarefas.Select(t => new TarefaResponseDto        
            {
                Id = t.id,
                Titulo = t.titulo,
                Descricao = t.descricao,
                Status = ((EnumStatus)t.status).GetDescription(),
                DataVencimento = t.datavencimento
            });
        }
        
        public async Task<TarefaResponseDto> GetTarefaByIdAsync(int id)
        {
            var tarefa = await _tarefaRepository.GetByIdAsync(id); // Fetch tarefa by ID
            
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada");
            
            return new TarefaResponseDto    
            {
                Id = tarefa.id,
                Titulo = tarefa.titulo,
                Descricao = tarefa.descricao,
                Status = ((EnumStatus)tarefa.status).GetDescription(),
                DataVencimento = tarefa.datavencimento
            };
        }
        
        public async Task<int> AddTarefaAsync(TarefaRequestDto tarefaDto)
        {            
            var tarefa = new Tarefa
            {                
                titulo = tarefaDto.Titulo,
                descricao = tarefaDto.Descricao,
                status = tarefaDto.Status,
                datavencimento = tarefaDto.DataVencimento!.Value
            };
            
            await _tarefaRepository.AddAsync(tarefa);

            return tarefa.id;
        }
        
        public async Task UpdateTarefaAsync(int id, TarefaRequestDto tarefaDto)
        {
            var tarefa = await _tarefaRepository.GetByIdAsync(id);
            
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada");
            
            tarefa.titulo = tarefaDto.Titulo;
            tarefa.descricao = tarefaDto.Descricao;
            tarefa.status = tarefaDto.Status;
            tarefa.datavencimento = tarefaDto.DataVencimento!.Value;
            
            await _tarefaRepository.UpdateAsync(tarefa);
        }
                
        public async Task DeleteTarefaAsync(int id)
        {
            var tarefa = await _tarefaRepository.GetByIdAsync(id);

            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada");

            await _tarefaRepository.DeleteAsync(id);
        }
    }
}
