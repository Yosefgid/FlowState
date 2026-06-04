using Blazorise;
using FlowState.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Http.Headers;
using FlowState.Blazer.Services;

namespace FlowState.Blazer.Components.Functionality
{

    public abstract class BaseTodoItems : ComponentBase
    {

        [Inject]
        protected HttpClient Http { get; set; } = default!;
        [Inject]
        protected TokenService TokenService { get; set; } = default!;

        [Inject]
        protected TaskStateService TaskState { get; set; } = default!;

        protected Validations nameValidations;
        protected Validations descValidations;


        protected string name;

        protected string description;

        protected string searchTerm = string.Empty;

        protected Filter filter = Filter.All;

        protected int selectedSession = -1;

        public bool loaded = false;

        protected Dictionary<int, string> sessions { get; set; } = new();

        protected List<ToDoTask> todos = new() { 
            new ToDoTask(0,"Finish API", "Complete CRUD endpoints for ToDo API", "google-1"),
            new ToDoTask(0,"Study EF Core", "Review tracking, migrations, and relationships", "google-2"),
            new ToDoTask(0,"Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3"),
            new ToDoTask(0,"Frontend UI", "Build Blazor or React task UI", "google-4"),
            new ToDoTask(0,"Write Tests", "Add unit tests for service layer", "google-5")

        };

        protected IEnumerable<ToDoTask> Todos
        {
            get
            {
                var query = from t in todos select t;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    query = query.Where(q =>
                        q.Description.Contains(searchTerm.Trim(), StringComparison.OrdinalIgnoreCase));

                if (filter == Filter.Active)
                    query = from q in query where !q.IsCompleted select q;

                if (filter == Filter.Completed)
                    query = from q in query where q.IsCompleted select q;
         
                return query;
            }

        }

        protected int TotalCount => todos.Count;

        protected int ActiveCount => todos.Count(x => x.IsCompleted == false);

        protected int CompletedCount => todos.Count(x => x.IsCompleted);

        protected int FilteredCount => Todos.Count();

        protected bool HasTodos => TotalCount > 0;

        protected bool HasCompletedTodos => CompletedCount > 0;

        protected bool IsAllChecked => HasTodos && todos.All(x => x.IsCompleted);

        protected int CompletionPercentage => TotalCount == 0 ? 0 : CompletedCount * 100 / TotalCount;

        private async Task SetAuthHeaderAsync()
        {
            var token = await TokenService.GetTokenAsync();
            Console.WriteLine($"[BaseTodoItems] token = {(string.IsNullOrEmpty(token) ? "NULL" : "present")}");
            Http.DefaultRequestHeaders.Authorization =
                string.IsNullOrWhiteSpace(token)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", token);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;
            await SetAuthHeaderAsync();
            await TaskState.RefreshTasks();
            todos = TaskState.Tasks;
            StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {

            await TaskState.RefreshTasks();
            await TaskState.RefreshSessions(false);

            TaskState.Sessions.ForEach(x => sessions.Add(x.Id, x.Name));
            todos = TaskState.Tasks;
            loaded = true;
            foreach (var task in TaskState.Tasks)
            {
                Console.WriteLine(task.Description);
            }
            Console.WriteLine("Tasks");

        }

        public async Task SessionFilter(int sessionId)
        {
            selectedSession = sessionId;
            if (sessionId == -1)
            {
                await TaskState.RefreshTasks();
            }
            else
            {
                await TaskState.RefreshTasks(sessionId);
            }

            todos = TaskState.Tasks;

        }

        public void OrderTodos()
        {
            todos = todos.OrderBy(x => x.Name).ToList();
        }
        protected void SetFilter(Filter filter)
        {
            this.filter = filter;
        }

        protected  async void OnCheckAll(bool isChecked)
        {
            todos.ForEach(x => x.IsCompleted = isChecked);

            try
            {
                await Http.PatchAsJsonAsync($"/api/tasks/set-all/{isChecked}", Todos.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async void OnClearCompleted()
        {

            List<ToDoTask> tasks = new List<ToDoTask>();
            tasks.AddRange(Todos.Where(x => x.IsCompleted));

            todos.RemoveAll(x => x.IsCompleted);


            try
            {
                await Http.PatchAsJsonAsync($"/api/tasks/delete-selected/", tasks);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            filter = Filter.All;
        }

        protected async Task OnAddTodo()
        {
            await SetAuthHeaderAsync();
            Console.WriteLine($"[OnAddTodo] Auth header = {Http.DefaultRequestHeaders.Authorization}");

            if (await nameValidations.ValidateAll() && await descValidations.ValidateAll())
            {
                Console.WriteLine("Pressed");
                ToDoTask task = new(TaskState.UserId,name?.Trim(), description?.Trim(), null);
                task.SessionId = selectedSession == -1 ? 0 : selectedSession; 
                todos.Add(task);
                OrderTodos();
                description = null;
                name = null;
                try
                {
                    var response = await Http.PostAsJsonAsync("/api/tasks", task);
                    if (response.IsSuccessStatusCode)
                    {
                        var createdTask = await response.Content.ReadFromJsonAsync<ToDoTask>();
                        if (createdTask != null)
                        {
                            todos.Add(createdTask);
                            OrderTodos();
                        }
                    } else
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[OnAddTodo] error body = {body}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                name = null;
                description = null;
                await nameValidations.ClearAll();
                await descValidations.ClearAll();

            }
        }

        protected async Task OnRemoveTodo(ToDoTask todo)
        {
            

            try
            {
                var response = await Http.DeleteAsync($"/api/tasks/{todo.Id}");
                todos.Remove(todo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (FilteredCount == 0)
                filter = Filter.All;
        }

        //protected void ResetTodos()
        //{
        //    todos = CreateTodos();
        //    filter = Filter.All;
        //    description = null;
        //}

        protected Task OnTodoStatusChanged(bool isChecked)
        {
            return InvokeAsync(StateHasChanged);
        }

        protected Color GetCompletionColor()
        {
            return CompletionPercentage switch
            {
                100 => Color.Success,
                >= 50 => Color.Info,
                > 0 => Color.Warning,
                _ => Color.Secondary,
            };
        }

        protected Color GetFilterColor()
        {
            return filter switch
            {
                Filter.Active => Color.Info,
                Filter.Completed => Color.Success,
                _ => Color.Primary,
            };
        }

        protected string GetFilterLabel()
        {
            return filter switch
            {
                Filter.Active => "Active tasks",
                Filter.Completed => "Completed tasks",
                _ => "All tasks",
            };
        }

        protected string GetEmptyTitle()
        {
            return filter switch
            {
                Filter.Active => "No active tasks",
                Filter.Completed => "No completed tasks",
                _ => "No tasks yet",
            };
        }

        protected string GetEmptyDescription()
        {
            return filter switch
            {
                Filter.Active => "Everything in the list is complete.",
                Filter.Completed => "Complete a task and it will appear here.",
                _ => "Add a task to start the list.",
            };
        }

        protected string GetSummaryText()
        {
            if (TotalCount == 0)
                return "Add the first task to start tracking progress.";

            if (CompletedCount == TotalCount)
                return "All tasks are complete.";

            return $"{ActiveCount} active task{(ActiveCount == 1 ? string.Empty : "s")} remaining.";
        }

        
    }
}
