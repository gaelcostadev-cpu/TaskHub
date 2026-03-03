namespace TasksService.Domain.Enums;

public enum AssignUserResult
{
    Success,
    TaskNotFound,
    NotAllowed,
    AlreadyAssigned
}