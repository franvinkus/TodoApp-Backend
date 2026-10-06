using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TodoApp_Backend.Constant;
using TodoApp_Backend.Data;
using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;
using TodoApp_Backend.Repositories.Interface;
using TodoApp_Backend.Services.Interface;

namespace TodoApp_Backend.Services.Implementation
{
    public class TodoServices : ITodoService
    {
        private readonly ITodoRepository _t;

        public TodoServices(ITodoRepository t)
        {
            _t = t;
        }

        public async Task<List<GetTodoModel>> GetTodo(string? title, string? sort, string? prioritySort, Guid userId, CancellationToken cancellationToken)
        {
            List<Todo> rawTodo = await _t.GetRawTodoById(userId, cancellationToken);
            var query = rawTodo.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(Q => Q.Title.ToLower().Contains(title.ToLower().Trim()));
            }

            IOrderedEnumerable<Todo> orderedQuery;
            var sortDirection = string.IsNullOrWhiteSpace(sort) ? "latest" : sort.ToLower();

            if (!string.IsNullOrWhiteSpace(prioritySort))
            {
                orderedQuery = prioritySort.ToLower() == "low" 
                    ? query.OrderBy(t => t.TodoPriority) 
                    : query.OrderByDescending(t => t.TodoPriority);

                orderedQuery = sortDirection.ToLower() == "latest"
                    ? orderedQuery.ThenBy(t => t.CreatedDate)
                    : orderedQuery.ThenByDescending(t => t.CreatedDate);
            }
            else
            {
                orderedQuery = sortDirection.ToLower() == "latest"
                    ? query.OrderBy(t => t.CreatedDate)
                    : query.OrderByDescending(t => t.CreatedDate);
            }

            return orderedQuery.Select(t => new GetTodoModel
            {
                id = t.Id,
                title = t.Title,
                description = t.Description,
                createdAt = t.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss"),
                finishedAt = t.FinishedDate.HasValue ? t.FinishedDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "-",
                startDate = t.StartDate.ToString("dd-MM-yyyy HH:mm:ss"),
                endDate = t.EndDate.ToString("dd-MM-yyyy HH:mm:ss"),
                isCompleted = t.IsFinished,
                TodoPriority = t.TodoPriority.ToString()
            }).ToList();
        }

        public async Task<string> PostTodo(PostTodoModel req, Guid userId, CancellationToken cancellationToken)
        {
            var newData = new Models.Todo
            {
                Title = req.title,
                Description = req.description,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.SpecifyKind(Convert.ToDateTime(req.startDate), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(Convert.ToDateTime(req.endDate), DateTimeKind.Utc),
                IsFinished = false,
                TodoPriority = Enum.Parse<PriorityEnum>(req.TodoPriority, true),
                UserId = userId,
            };

            
            _t.AddTodo(newData);
            await _t.SaveTodo(cancellationToken);
            await _t.RemoveCache(userId, cancellationToken);

            return "Success";
        }

        public async Task<string> PutTodo(int id, PutTodoModel edit, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, cancellationToken);

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.Title = edit.Title;
            isIdExist.Description = edit.Description;
            isIdExist.StartDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.startDate), DateTimeKind.Utc);
            isIdExist.EndDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.endDate), DateTimeKind.Utc);
            isIdExist.TodoPriority = Enum.Parse<PriorityEnum>(edit.TodoPriority, true);


            await _t.SaveTodo(cancellationToken);
            await _t.RemoveCache(isIdExist.UserId, cancellationToken);

            return "Success";
        }

        public async Task<string> PatchTodo(int id, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, cancellationToken);

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.IsFinished = !isIdExist.IsFinished;

            if (!isIdExist.IsFinished)
            {
                isIdExist.FinishedDate = null;
            }
            else
            {
                isIdExist.FinishedDate = DateTime.UtcNow;
            }

            await _t.SaveTodo(cancellationToken);
            await _t.RemoveCache(isIdExist.UserId, cancellationToken);

            return "Success";
        }

        public async Task<string> DeleteTodo(int id, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, cancellationToken);

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            _t.RemoveTodo(isIdExist);

            await _t.SaveTodo(cancellationToken);
            await _t.RemoveCache(isIdExist.UserId, cancellationToken);

            return "Success";
        }
    }
}
