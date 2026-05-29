using FlowState.Models;
using FlowState.Repositories;
using Google.Apis.Calendar.v3.Data;

namespace FlowState.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly IToDoTaskRepo _taskRepo;
        private readonly IGoogleCalendarClient _googleCalendarClient;

        public GoogleService(
            IToDoTaskRepo taskRepo,
            IGoogleCalendarClient googleCalendarClient)
        {
            _taskRepo = taskRepo;
            _googleCalendarClient = googleCalendarClient;
        }

        public string GetGoogleAuthUrl(string userId)
        {
            return _googleCalendarClient.GetGoogleAuthUrl(userId);
        }

        public async Task ConnectGoogleCalendarAsync(string code, string userId)
        {
            await _googleCalendarClient.ExchangeCodeForTokensAsync(code, userId);
        }

        //Method that adds tasks to db by calling the AddTask method in the ToDoTaskRepo class
        public async Task<List<ToDoTask>> ImportGoogleCalendarEventsAsync(string userId)
        {
            var googleEvents = await _googleCalendarClient.GetCalendarEventsAsync(userId);

            var importedTasks = new List<ToDoTask>();

            foreach (var googleEvent in googleEvents)
            {
                var task = MapGoogleEventToTask(googleEvent);

                _taskRepo.AddTask(task);

                importedTasks.Add(task);
            }

            return importedTasks;
        }

        public ToDoTask MapGoogleEventToTask(Event googleEvent)
        {
            return new ToDoTask(
                googleEvent.Summary ?? "Untitled Google Event",
                googleEvent.Description ?? "",
                googleEvent.Id ?? ""
            );
        }
    }

    public interface IGoogleService
    {
        string GetGoogleAuthUrl(string userId);

        Task ConnectGoogleCalendarAsync(string code, string userId);

        Task<List<ToDoTask>> ImportGoogleCalendarEventsAsync(string userId);

        ToDoTask MapGoogleEventToTask(Event googleEvent);
    }
}