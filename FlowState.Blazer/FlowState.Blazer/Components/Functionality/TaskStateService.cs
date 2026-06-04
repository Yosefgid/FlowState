using FlowState.Models;
using Microsoft.AspNetCore.Components;
using FlowState.Blazer.Services;
using System.Text.Json;

namespace FlowState.Blazer.Components.Functionality
{
    public class TaskStateService
    {

        private readonly HttpClient Http;

        public int UserId { get; set; }

        public TaskStateService(HttpClient http)
        {
            Http = http;
        }
        public List<ToDoTask> Tasks { get; set; } = new() {
            new ToDoTask(0,"Finish API", "Complete CRUD endpoints for ToDo API", "google-1").setEndDate(DateTime.Now),
            new ToDoTask(0,"Study EF Core", "Review tracking, migrations, and relationships", "google-2").setEndDate(DateTime.Now),
            new ToDoTask(0,"Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3").setEndDate(DateTime.Now),
            new ToDoTask(0,"Frontend UI", "Build Blazor or React task UI", "google-4").setEndDate(DateTime.Now),
            new ToDoTask(0,"Write Tests", "Add unit tests for service layer", "google-5").setEndDate(DateTime.Now)
        };

        public List<Session> Sessions { get; set; } = new()
        {

        };

        public Dictionary<int, int[]> CompletedCount { get; set; }




        public async Task RefreshTasks()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<List<ToDoTask>>($"/api/tasks/user/{UserId}");
                if (response != null)
                {
                    Tasks.Clear();
                    Tasks.AddRange(response);
                    GetCompletedCount();
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

        public async Task RefreshTasks(int sessionId)
        {
            try
            {
                var response = await Http.GetFromJsonAsync<List<ToDoTask>>($"/api/tasks/user/{UserId}");
                if (response != null)
                {
                    Tasks.Clear();
                    Tasks.AddRange(response);
                    GetCompletedCount();
                    Tasks = Tasks.Where(x => x.SessionId == sessionId).ToList(); 
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

        public async Task RefreshSessions()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<List<Session>>($"/api/sessions/user/{UserId}");
                if (response != null)
                {
                    Sessions.Clear();
                    Sessions.AddRange(response);
                    Sessions.Add(new Session(-1, UserId, "All Tasks"));
                    Sessions.Add(new Session(0, UserId, "Personal"));
                    

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

        public async Task<Session?> CreateSession(Session newSession)
        {
            try
            {
                var response = await Http.PostAsJsonAsync(
                    $"/api/sessions/user/{UserId}",
                    newSession);

                response.EnsureSuccessStatusCode();

                Session? session = await response.Content.ReadFromJsonAsync<Session>();
                Sessions.Add(session);

                return session ;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        public void OrderTasksByName()
        {
            Tasks = Tasks.OrderBy(x => x.Name).ToList();
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

        public void GetCompletedCount()
        {
            Dictionary<int, int[]> result = new();
            int total = 0;
            int totalCompleted = 0;

            foreach (var x in Tasks)
            {
                int id = x.SessionId.Value;

                if (!result.ContainsKey(id))
                {
                    result[id] = new int[2];
                }

                if (x.IsCompleted)
                {
                    result[id][0]++;
                    totalCompleted++;
                }

                result[id][1]++;
                total++;
            }

            result[-1] = [totalCompleted,total];

            foreach(var y in Sessions)
            {
                if (!result.ContainsKey(y.Id))
                {
                    result[y.Id] = new int[2];
                }
            }

            

            CompletedCount = result;
        }

    }

}
