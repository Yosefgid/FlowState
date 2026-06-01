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
            new ToDoTask("Finish API", "Complete CRUD endpoints for ToDo API", "google-1"),
            new ToDoTask("Study EF Core", "Review tracking, migrations, and relationships", "google-2"),
            new ToDoTask("Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3"),
            new ToDoTask("Frontend UI", "Build Blazor or React task UI", "google-4"),
            new ToDoTask("Write Tests", "Add unit tests for service layer", "google-5")
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


        public async Task OnCheckedChanged(ToDoTask Todo, bool isChecked)
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
         
        }
    }

}
