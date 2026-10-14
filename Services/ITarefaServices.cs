using TaskManagement.DTOs;

namespace TaskManagement.Services
{ 
    public interface ITarefaService
    {
        Task<IEnumerable<TarefaResponseDto>> GetAllTarefasAsync(int? status = null, DateOnly? dataVencimento = null);
        Task<TarefaResponseDto> GetTarefaByIdAsync(int id);
        Task<int> AddTarefaAsync(TarefaRequestDto tarefaDto);
        Task UpdateTarefaAsync(int id, TarefaRequestDto tarefaDto);
        Task DeleteTarefaAsync(int id);
    }
}
