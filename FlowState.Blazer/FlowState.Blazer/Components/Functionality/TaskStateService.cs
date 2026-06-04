using FlowState.Blazer.Services;
using FlowState.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace FlowState.Blazer.Components.Functionality
{
    public class TaskStateService
    {

        private readonly HttpClient Http;
        private readonly TokenService _tokenService;

        public TaskStateService(HttpClient http, TokenService tokenService)
        {
            Http = http;
            _tokenService = tokenService;
        }
        //public List<ToDoTask> Tasks { get; set; } = new() {
        //    new ToDoTask("Finish API", "Complete CRUD endpoints for ToDo API", "google-1").setEndDate(DateTime.Now),
        //    new ToDoTask("Study EF Core", "Review tracking, migrations, and relationships", "google-2").setEndDate(DateTime.Now),
        //    new ToDoTask("Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3").setEndDate(DateTime.Now),
        //    new ToDoTask("Frontend UI", "Build Blazor or React task UI", "google-4").setEndDate(DateTime.Now),
        //    new ToDoTask("Write Tests", "Add unit tests for service layer", "google-5").setEndDate(DateTime.Now)
        //};

        public List<ToDoTask> Tasks { get; set; } = new();


        private async Task SetAuthHeader()
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
                Http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task RefreshTasks()
        {
            await SetAuthHeader();
            try
            {
                var response = await Http.GetFromJsonAsync<List<ToDoTask>>("/api/tasks");
                if (response != null && response.Count > 0)
                {
                    Tasks.Clear();
                    Tasks.AddRange(response);
                    OrderTasksByName();
                }
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


        public void OrderTasksByName()
        {
            //Tasks = Tasks.OrderBy(x => x.Name).ToList();
            Tasks.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task OnCheckedChanged(ToDoTask Todo, bool isChecked , EventCallback<bool> StatusChanged)
        {
            await SetAuthHeader();
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

            await SetAuthHeader();
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

        public int CatCount(EisenCat cat) => Tasks.Where(x => x.Category == cat).Count();


        public int CompletedCatCount(EisenCat cat) => Tasks.Where(x => x.Category == cat && x.IsCompleted).Count();
    }

}
