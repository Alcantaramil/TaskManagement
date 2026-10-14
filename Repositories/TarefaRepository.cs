using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Entities;

namespace TaskManagement.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly ApplicationDbContext _context;

        public TarefaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Tarefa>> GetAllAsync()
        {            
            return await _context.tarefas.ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> GetFilteredAsync(int? status,DateOnly? dataVencimento)
        {
            var query = _context.tarefas.AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(t => t.status == status.Value);
            }

            if (dataVencimento.HasValue)
            {
                query = query.Where(
                    t => t.datavencimento == dataVencimento.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Tarefa> GetByIdAsync(int id)
        {            
            return await _context.tarefas.FindAsync(id);
        }
        
        public async Task AddAsync(Tarefa tarefa)
        {            
            await _context.tarefas.AddAsync(tarefa);            
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Tarefa tarefa)
        {            
            _context.tarefas.Update(tarefa);         
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {            
            var tarefa = await _context.tarefas.FindAsync(id);
            
            if (tarefa != null)
            {
                _context.tarefas.Remove(tarefa);
                await _context.SaveChangesAsync();
            }
        }
    }
}
