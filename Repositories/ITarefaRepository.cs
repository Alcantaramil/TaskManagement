using TaskManagement.Entities;

namespace TaskManagement.Repositories
{
    public interface ITarefaRepository
    {
        Task<IEnumerable<Tarefa>> GetAllAsync();        
        Task<Tarefa> GetByIdAsync(int id);
        Task AddAsync(Tarefa tarefa);
        Task UpdateAsync(Tarefa tarefa);
        Task DeleteAsync(int id);
        Task<IEnumerable<Tarefa>> GetFilteredAsync(int? status = null, DateOnly? dataVencimento = null);
    }
}
