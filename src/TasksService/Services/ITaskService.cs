using TasksService.Contracts;

namespace TasksService.Services
{

    public interface ITaskService
    {
        /// <summary>
        /// Cria uma nova tarefa para o usuário autenticado
        /// </summary>
        Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid userId);

        /// <summary>
        /// Retorna uma tarefa pelo Id (somente se pertencer ao usuário)
        /// </summary>
        Task<TaskResponse?> GetByIdAsync(Guid id, Guid userId);

        /// <summary>
        /// Retorna tarefas paginadas do usuário
        /// </summary>
        Task<(IEnumerable<TaskResponse> Items, int TotalCount)>
            GetPagedAsync(int page, int size, Guid userId);

        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request, Guid userId);

        /// <summary>
        /// Remove uma tarefa do usuário
        /// </summary>
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}