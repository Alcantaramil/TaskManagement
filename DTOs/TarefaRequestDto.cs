using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTOs
{
    public class TarefaRequestDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Range(0, 2, ErrorMessage = "O status deve ser 0, 1 ou 2.")]
        public int Status { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
        public DateOnly? DataVencimento { get; set; }
    }
}