using Blazorise;
using FlowState.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FlowState.Blazer.Components.Functionality
{

    public abstract class BaseTodoItems : ComponentBase
    {

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Inject]
        protected TaskStateService TaskState { get; set; } = default!;

        protected Validations nameValidations;
        protected Validations descValidations;


        protected string name;

        protected string description;

        protected string searchTerm = string.Empty;

        protected Filter filter = Filter.All;

        protected List<ToDoTask> todos = new() { 
            new ToDoTask("Finish API", "Complete CRUD endpoints for ToDo API", "google-1"),
            new ToDoTask("Study EF Core", "Review tracking, migrations, and relationships", "google-2"),
            new ToDoTask("Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3"),
            new ToDoTask("Frontend UI", "Build Blazor or React task UI", "google-4"),
            new ToDoTask("Write Tests", "Add unit tests for service layer", "google-5")

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


        protected override async Task OnInitializedAsync()
        {
            await TaskState.RefreshTasks();
            todos = TaskState.Tasks;
            foreach (var task in TaskState.Tasks)
            {
                Console.WriteLine(task.Description);
            }
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
      
            if (await nameValidations.ValidateAll() && await descValidations.ValidateAll())
            {
                Console.WriteLine("Pressed");
                ToDoTask task = new(name?.Trim(), description?.Trim(), "N/A");
                todos.Add(task);
                description = null;
                name = null;
                try
                {
                    await Http.PostAsJsonAsync("/api/tasks", task);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
