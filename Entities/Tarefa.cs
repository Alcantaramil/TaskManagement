namespace TaskManagement.Entities
{
    public class Tarefa
    {
        public int id { get; set; }
        public string titulo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public int status { get; set; }
        public DateOnly datavencimento { get; set; }
    }
}
