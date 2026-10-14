
using Moq;
using TaskManagement.Entities;
using TaskManagement.Repositories;
using TaskManagement.Services;
using Xunit;

namespace TaskManagement.Tests
{
    public class TarefaServiceTests
    {
        [Fact]
        public async Task GetTarefaByIdAsync_QuandoTarefaExiste_DeveRetornarTarefa()
        {
            // ARRANGE - Preparar os dados e as dependências
            var tarefa = new Tarefa
            {
                id = 10,
                titulo = "Estudar testes unitários",
                descricao = "Aprender xUnit e Moq",
                status = 0,
                datavencimento = new DateOnly(2026, 10, 30)
            };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(tarefa);

            var service = new TarefaService(repositoryMock.Object);

            // ACT - Executar o método que queremos testar
            var resultado = await service.GetTarefaByIdAsync(10);

            // ASSERT - Verificar o resultado
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal("Estudar testes unitários", resultado.Titulo);
            Assert.Equal("Aprender xUnit e Moq", resultado.Descricao);
            Assert.Equal("Pendente", resultado.Status);
            Assert.Equal(new DateOnly(2026, 10, 30), resultado.DataVencimento);
        }

        [Fact]
        public async Task GetTarefaByIdAsync_QuandoTarefaNaoExiste_DeveLancarExcecao()
        {
            // ARRANGE - Preparação
            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Tarefa)null!);

            var service = new TarefaService(repositoryMock.Object);

            // ACT + ASSERT - Execução e verificação
            var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.GetTarefaByIdAsync(999)
            );

            Assert.Equal("Tarefa não encontrada", excecao.Message);
        }

        [Fact]
        public async Task AddTarefaAsync_QuandoDadosValidos_DeveCadastrarTarefa()
        {
            // ARRANGE - Preparação
            var tarefaDto = new TaskManagement.DTOs.TarefaRequestDto
            {
                Titulo = "Aprender testes unitários",
                Descricao = "Testando criação com Moq",
                Status = 0,
                DataVencimento = new DateOnly(2026, 11, 10)
            };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Tarefa>()))
                .Callback<Tarefa>(tarefa => tarefa.id = 15)
                .Returns(Task.CompletedTask);

            var service = new TarefaService(repositoryMock.Object);

            // ACT - Execução
            var idRetornado = await service.AddTarefaAsync(tarefaDto);

            // ASSERT - Verificação
            Assert.Equal(15, idRetornado);

            repositoryMock.Verify(
                r => r.AddAsync(It.Is<Tarefa>(t =>
                    t.titulo == tarefaDto.Titulo &&
                    t.descricao == tarefaDto.Descricao &&
                    t.status == tarefaDto.Status &&
                    t.datavencimento == tarefaDto.DataVencimento
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateTarefaAsync_QuandoTarefaExiste_DeveAtualizarDados()
        {
            // ARRANGE
            var tarefaExistente = new Tarefa
            {
                id = 10,
                titulo = "Título antigo",
                descricao = "Descrição antiga",
                status = 0,
                datavencimento = new DateOnly(2026, 10, 20)
            };

            var tarefaDto = new TaskManagement.DTOs.TarefaRequestDto
            {
                Titulo = "Título atualizado",
                Descricao = "Descrição atualizada",
                Status = 1,
                DataVencimento = new DateOnly(2026, 11, 15)
            };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(tarefaExistente);

            var service = new TarefaService(repositoryMock.Object);

            // ACT
            await service.UpdateTarefaAsync(10, tarefaDto);

            // ASSERT
            repositoryMock.Verify(
                r => r.UpdateAsync(It.Is<Tarefa>(t =>
                    t.id == 10 &&
                    t.titulo == "Título atualizado" &&
                    t.descricao == "Descrição atualizada" &&
                    t.status == 1 &&
                    t.datavencimento == new DateOnly(2026, 11, 15)
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateTarefaAsync_QuandoTarefaNaoExiste_DeveLancarExcecao()
        {
            // ARRANGE
            var tarefaDto = new TaskManagement.DTOs.TarefaRequestDto
            {
                Titulo = "Tarefa inexistente",
                Descricao = "Testando atualização inválida",
                Status = 1,
                DataVencimento = new DateOnly(2026, 11, 20)
            };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Tarefa)null!);

            var service = new TarefaService(repositoryMock.Object);

            // ACT + ASSERT
            var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.UpdateTarefaAsync(999, tarefaDto)
            );

            Assert.Equal("Tarefa não encontrada", excecao.Message);

            repositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Tarefa>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteTarefaAsync_QuandoTarefaExiste_DeveExcluirTarefa()
        {
            // ARRANGE
            var tarefaExistente = new Tarefa
            {
                id = 10,
                titulo = "Tarefa para excluir",
                descricao = "Testando exclusão com Moq",
                status = 0,
                datavencimento = new DateOnly(2026, 11, 20)
            };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(tarefaExistente);

            var service = new TarefaService(repositoryMock.Object);

            // ACT
            await service.DeleteTarefaAsync(10);

            // ASSERT
            repositoryMock.Verify(
                r => r.DeleteAsync(10),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteTarefaAsync_QuandoTarefaNaoExiste_DeveLancarExcecao()
        {
            // ARRANGE
            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Tarefa)null!);

            var service = new TarefaService(repositoryMock.Object);

            // ACT + ASSERT
            var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.DeleteTarefaAsync(999)
            );

            Assert.Equal("Tarefa não encontrada", excecao.Message);

            repositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<int>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetAllTarefasAsync_SemFiltros_DeveRetornarTodasTarefas()
        {
            // ARRANGE
            var tarefas = new List<Tarefa>
    {
        new Tarefa
        {
            id = 1,
            titulo = "Estudar C#",
            descricao = "Revisar conceitos de C#",
            status = 0,
            datavencimento = new DateOnly(2026, 11, 10)
        },
        new Tarefa
        {
            id = 2,
            titulo = "Estudar testes unitários",
            descricao = "Aprender xUnit e Moq",
            status = 1,
            datavencimento = new DateOnly(2026, 11, 15)
        }
    };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetFilteredAsync(null, null))
                .ReturnsAsync(tarefas);

            var service = new TarefaService(repositoryMock.Object);

            // ACT
            var resultado = (await service.GetAllTarefasAsync()).ToList();

            // ASSERT
            Assert.Equal(2, resultado.Count);

            Assert.Equal("Estudar C#", resultado[0].Titulo);
            Assert.Equal("Pendente", resultado[0].Status);

            Assert.Equal("Estudar testes unitários", resultado[1].Titulo);
            Assert.Equal("Em Andamento", resultado[1].Status);

            repositoryMock.Verify(
                r => r.GetFilteredAsync(null, null),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllTarefasAsync_ComFiltros_DeveEncaminharFiltrosAoRepository()
        {
            // ARRANGE
            int status = 1;
            var dataVencimento = new DateOnly(2026, 11, 15);

            var tarefasFiltradas = new List<Tarefa>
    {
        new Tarefa
        {
            id = 2,
            titulo = "Estudar testes unitários",
            descricao = "Aprender xUnit e Moq",
            status = 1,
            datavencimento = dataVencimento
        }
    };

            var repositoryMock = new Mock<ITarefaRepository>();

            repositoryMock
                .Setup(r => r.GetFilteredAsync(status, dataVencimento))
                .ReturnsAsync(tarefasFiltradas);

            var service = new TarefaService(repositoryMock.Object);

            // ACT
            var resultado = (await service.GetAllTarefasAsync(
                status, dataVencimento)).ToList();

            // ASSERT
            Assert.Single(resultado);
            Assert.Equal(2, resultado[0].Id);
            Assert.Equal("Em Andamento", resultado[0].Status);
            Assert.Equal(dataVencimento, resultado[0].DataVencimento);

            repositoryMock.Verify(
                r => r.GetFilteredAsync(status, dataVencimento),
                Times.Once
            );
        }

    }
}
