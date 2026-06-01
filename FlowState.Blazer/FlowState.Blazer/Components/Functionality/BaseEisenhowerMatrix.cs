using Blazorise;
using FlowState.Models;
using Microsoft.AspNetCore.Components;

namespace FlowState.Blazer.Components.Functionality
{  
    public abstract class BaseEisenhowerMatrix : ComponentBase
    {
        // ── Parameters ──────────────────────────────────────────────────────────

        /// <summary>
        /// Feed in your existing ToDo list from a parent page or the Dashboard.
        /// Tasks are distributed across quadrants on first render.
        /// </summary>

        [Inject]
        protected TaskStateService TaskState { get; set; } = default!;

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        public List<ToDoTask> Tasks { get; set; } = new();

        public readonly Dictionary<string, EisenCat> StringToEisen = new()
        {
            { "Do", EisenCat.Do },
            { "Schedule", EisenCat.Schedule },
            { "Delegate", EisenCat.Delegate },
            { "Eliminate", EisenCat.Eliminate }
        };

        public readonly Dictionary<EisenCat, string> EisenToString = new()
        {
            { EisenCat.Eliminate, "Eliminate" },
            { EisenCat.Delegate, "Delegate" },
            { EisenCat.Schedule, "Schedule" },
            { EisenCat.Do, "Do" }
        };



        // ── Lifecycle ────────────────────────────────────────────────────────────

        protected override async Task OnInitializedAsync()
        {
            // Assign any new tasks that don't yet have a quadrant.
            // Default: DoFirst — caller can pre-assign via OnTaskQuadrantChanged.


            await TaskState.RefreshTasks();
          
            Tasks = TaskState.Tasks;
            foreach (var task in TaskState.Tasks)
            {
                Console.WriteLine(task.Description);
            }
            Console.WriteLine("Eisen");

            await InvokeAsync(StateHasChanged);

        }

        

        // ── Computed helpers ─────────────────────────────────────────────────────

        protected List<ToDoTask> AllTasks => Tasks;



        // ── Drag & drop ──────────────────────────────────────────────────────────

        public async Task ItemDropped(DraggableDroppedEventArgs<ToDoTask> dropItem)
        {
            dropItem.Item.Category = StringToEisen[dropItem.DropZoneName];

            await Http.PatchAsJsonAsync($"/api/tasks/assign-eisen/{dropItem.Item.Id}", dropItem.Item.Category);
        }

        // ── Task toggle ──────────────────────────────────────────────────────────


        protected async Task ToggleTask(ToDoTask task, bool isChecked)
        {
            Console.WriteLine("DO SOMETHING");
            await TaskState.OnCheckedChanged(task,  isChecked);
            await InvokeAsync(StateHasChanged);
        }
        

    }
}
