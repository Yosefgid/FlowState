using FlowState.Models;
using Microsoft.AspNetCore.Components;

namespace FlowState.Blazer.Components.Functionality
{
    public class TaskStateService
    {

        private readonly HttpClient Http;

        public TaskStateService(HttpClient http)
        {
            Http = http;
        }
        public List<ToDoTask> Tasks { get; set; } = new() {
            
        };


        public async Task RefreshTasks()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<List<ToDoTask>>("/api/tasks");
                Tasks = response == null ? Tasks : response;
                //foreach (var task in Tasks)
                //{
                //    Console.WriteLine(task.Description);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task OnCheckedChanged(ToDoTask Todo, bool isChecked , EventCallback<bool> StatusChanged)
        {
            Todo.IsCompleted = isChecked;

            Console.WriteLine("Pressed");


            try
            {
                await Http.PatchAsync($"/api/tasks/{Todo.Id}/toggle", null);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await StatusChanged.InvokeAsync(isChecked);
        }
    }

}
