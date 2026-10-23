using TodoApp_Backend.Constant;
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
                TodoPriority = t.TodoPriority.ToString(),
                totalSubTodos = t.Sub_Todos.Count(),
                completedSubTodos = t.Sub_Todos.Count(st => st.IsDone)
            }).ToList();
        }

        public async Task<GetTodoDetailModel?> GetTodoById(int id, Guid userId, CancellationToken cancellationToken)
        {
            var todo = await _t.GetTodoById(id, userId, cancellationToken);
            if (todo == null)
            {
                return null;
            }

            return new GetTodoDetailModel
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                CreatedAt = todo.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss"),
                FinishedAt = todo.FinishedDate.HasValue ? todo.FinishedDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "-",
                StartDate = todo.StartDate.ToString("dd-MM-yyyy HH:mm:ss"),
                EndDate = todo.EndDate.ToString("dd-MM-yyyy HH:mm:ss"),
                IsCompleted = todo.IsFinished,
                TodoPriority = todo.TodoPriority.ToString(),
                SubTodos = todo.Sub_Todos.Select(st => new GetSubTodoModel
                {
                    Id = st.Id,
                    Title = st.Title,
                    IsDone = st.IsDone
                }).ToList()
            };
        }

        public async Task<string> PostTodo(PostTodoModel req, Guid userId, CancellationToken cancellationToken)
        {
            PriorityEnum finalPriority = PriorityEnum.Low;
            if(!string.IsNullOrEmpty(req.TodoPriority))
            {
                Enum.TryParse<PriorityEnum>(req.TodoPriority, true, out finalPriority);
            }

            List<Sub_Todo> sub_Todos = new List<Sub_Todo>();

            if(req.SubTodos != null)
            {
                foreach(var subTodo in req.SubTodos)
                {
                    sub_Todos.Add(new Sub_Todo
                    {
                        Id = Guid.NewGuid(),
                        Title = subTodo.Title,
                        IsDone = subTodo.IsDone
                    });
                }
            }

            var newData = new Models.Todo
            {
                Title = req.title,
                Description = req.description,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.SpecifyKind(Convert.ToDateTime(req.startDate), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(Convert.ToDateTime(req.endDate), DateTimeKind.Utc),
                IsFinished = false,
                TodoPriority = finalPriority,
                UserId = userId,
                Sub_Todos = sub_Todos
            };

            
            _t.AddTodo(newData);
            await _t.SaveTodo(cancellationToken);

            return "Success";
        }

        public async Task<string> PutTodo(int id, PutTodoModel edit, Guid userId, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, userId, cancellationToken);

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            isIdExist.Title = edit.Title;
            isIdExist.Description = edit.Description;
            isIdExist.StartDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.startDate), DateTimeKind.Utc);
            isIdExist.EndDate = DateTime.SpecifyKind(Convert.ToDateTime(edit.endDate), DateTimeKind.Utc);
            isIdExist.TodoPriority = Enum.Parse<PriorityEnum>(edit.TodoPriority, true);

            isIdExist.Sub_Todos.Clear();

            if (edit.SubTodos != null && edit.SubTodos.Any())
            {
                foreach (var sub in edit.SubTodos)
                {
                    isIdExist.Sub_Todos.Add(new Sub_Todo
                    {
                        Title = sub.Title,
                        IsDone = sub.IsDone
                    });
                }
            }


            await _t.SaveTodo(cancellationToken);

            return "Success";
        }

        public async Task<string> PatchTodo(int id, Guid userId, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, userId, cancellationToken);

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

            return "Success";
        }

        public async Task<string> DeleteTodo(int id, Guid userId, CancellationToken cancellationToken)
        {
            var isIdExist = await _t.GetTodoById(id, userId, cancellationToken);

            if (isIdExist == null)
            {
                return "Id not Found";
            }

            _t.RemoveTodo(isIdExist);

            await _t.SaveTodo(cancellationToken);

            return "Success";
        }
    }
}
