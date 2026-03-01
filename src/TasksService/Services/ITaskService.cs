using TasksService.Contracts;
using TasksService.Domain.Enums;

namespace TasksService.Services
{

    public interface ITaskService
    {
        /// <summary>
        /// Cria uma nova tarefa para o usuário autenticado
        /// </summary>
        Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid userId);
   
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="assignedUserId"></param>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        Task<bool> AssignUserAsync(Guid taskId, Guid assignedUserId, Guid requesterId);

        /// <summary>
        /// Retorna uma tarefa pelo Id (somente se pertencer ao usuário)
        /// </summary>
        Task<TaskResponse?> GetByIdAsync(Guid id, Guid userId);

        /// <summary>
        /// Retorna tarefas paginadas do usuário
        /// </summary>
        Task<PagedResponse<TaskResponse>> GetPagedAsync(TaskQueryParameters query, Guid userId);

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