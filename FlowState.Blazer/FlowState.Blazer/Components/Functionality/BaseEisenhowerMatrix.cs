using Blazorise;
using FlowState.Blazer.Services;
using FlowState.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

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
        [Inject] protected TokenService TokenService { get; set; } = default!;

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Parameter]
        public string SessionId { get; set; } 

        public List<ToDoTask> Tasks { get; set; } = new();





        // ── Lifecycle ────────────────────────────────────────────────────────────

        protected override async Task OnParametersSetAsync()
        {
          
            await TaskState.RefreshTasks(int.Parse(SessionId));
          
            Tasks = TaskState.Tasks;
            foreach (var task in TaskState.Tasks)
            {
                Console.WriteLine(task.Description);
            }
            Console.WriteLine("Eisen");

            await InvokeAsync(StateHasChanged);*/

        }

        

        // ── Computed helpers ─────────────────────────────────────────────────────

        protected List<ToDoTask> AllTasks => Tasks;


       


        // ── Drag & drop ──────────────────────────────────────────────────────────

        public async Task ItemDropped(DraggableDroppedEventArgs<ToDoTask> dropItem)
        {
            /*dropItem.Item.Category = TaskState.StringToEisen[dropItem.DropZoneName];
            TaskState.OrderTasksByName();

            await Http.PatchAsJsonAsync($"/api/tasks/assign-eisen/{dropItem.Item.Id}", dropItem.Item.Category);*/
            dropItem.Item.Category = TaskState.StringToEisen[dropItem.DropZoneName];

            var response = await Http.PatchAsJsonAsync(
                $"/api/tasks/assign-eisen/{dropItem.Item.Id}", dropItem.Item.Category);

            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"[ItemDropped] assign-eisen failed: {response.StatusCode}");

            TaskState.OrderTasksByName();
            await InvokeAsync(StateHasChanged);
        }

        // ── Task toggle ──────────────────────────────────────────────────────────


        public async Task ToggleTask(ToDoTask task, bool isChecked)
        {
            Console.WriteLine("DO SOMETHING");

            task.IsCompleted = isChecked;
           
           await TaskState.OnCheckedChanged(task,  isChecked);


            await InvokeAsync(StateHasChanged);
        }
        

    }
}
