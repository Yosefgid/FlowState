using Blazorise;
using FlowState.Models;
using Microsoft.AspNetCore.Components;

namespace FlowState.Blazer.Components.Functionality
{
    public class BaseSessions : ComponentBase
    {
        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Inject]
        protected TaskStateService TaskState { get; set; } = default!;

        protected Validations nameValidations;
   


        protected string name;

        protected string searchTerm = string.Empty;

        public bool loaded = false;

        protected List<Session> sessions { get; set; } = new();

        

        protected IEnumerable<Session> Sessions
        {
            get
            {
                var query = from t in sessions select t;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    query = query.Where(q =>
                        q.Name.Contains(searchTerm.Trim(), StringComparison.OrdinalIgnoreCase));

                return query;
            }

        }

        protected int TotalCount => sessions.Count;

        protected int FilteredCount => Sessions.Count();

        protected override async Task OnInitializedAsync()
        {
            await TaskState.RefreshTasks();
            await TaskState.RefreshSessions(true);

            TaskState.Sessions.ForEach(x => sessions.Add(x));
            sessions = TaskState.Sessions;
            loaded = true;
            

        }

        public void OrderSessions()
        {
            sessions = sessions.OrderBy(x => x.Name).ToList();
        }              

        protected async Task OnAddSession()
        {

            if (await nameValidations.ValidateAll())
            {
                Console.WriteLine("Pressed");
                Session session = new(0, TaskState.UserId,name);               
                OrderSessions();           
                name = null;
                try
                {
                    Session? newSession = await TaskState.CreateSession(session);
                    sessions.Add(newSession);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await nameValidations.ClearAll();

            }
        }

        protected async Task OnRemoveSession(Session session)
        {
            try
            {
                var response = await Http.DeleteAsync($"/api/sessions/{session.Id}");
                sessions.Remove(session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        protected async Task OnLeaveSession(Session session)
        {
            try
            {
                var response = await Http.DeleteAsync($"/api/sessions/{TaskState.UserId}/session-user/{session.Id}");
                sessions.Remove(session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

