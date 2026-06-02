using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace FlowState.Services
{
    public class GoogleCalendarClient : IGoogleCalendarClient
    {
        private readonly IConfiguration _configuration;
        private readonly string[] _scopes = { CalendarService.Scope.CalendarReadonly };
        private readonly string _applicationName = "FlowState";

        public GoogleCalendarClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetGoogleAuthUrl(string userId)
        {
            var flow = CreateGoogleAuthorizationCodeFlow();

            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            var request = flow.CreateAuthorizationCodeRequest(redirectUri);

            request.State = userId;

            return request.Build().AbsoluteUri;
        }

        public async Task ExchangeCodeForTokensAsync(string code, string userId)
        {
            var flow = CreateGoogleAuthorizationCodeFlow();

            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            await flow.ExchangeCodeForTokenAsync(
                userId,
                code,
                redirectUri,
                CancellationToken.None
            );
        }

        public async Task<List<Event>> GetCalendarEventsAsync(string userId)
        {
            var flow = CreateGoogleAuthorizationCodeFlow();

            var token = await flow.LoadTokenAsync(userId, CancellationToken.None);

            if (token == null)
            {
                throw new InvalidOperationException("Google Calendar has not been connected for this user.");
            }

            var credential = new UserCredential(flow, userId, token);

            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });

            var request = calendarService.Events.List("primary");

            request.TimeMinDateTimeOffset = DateTimeOffset.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 20;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();

            return events.Items?.ToList() ?? new List<Event>();
        }

        private GoogleAuthorizationCodeFlow CreateGoogleAuthorizationCodeFlow()
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var clientSecret = _configuration["GoogleAuth:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("GoogleAuth:ClientId is missing.");
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException("GoogleAuth:ClientSecret is missing.");
            }

            return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = _scopes,
                DataStore = new FileDataStore("GoogleCalendarTokens", true)
            });
        }

        public async Task<bool> IsGoogleCalendarConnectedAsync(string userId)
        {
            var flow = CreateGoogleAuthorizationCodeFlow();

            var token = await flow.LoadTokenAsync(userId, CancellationToken.None);

            return token != null;
        }
    }

    public interface IGoogleCalendarClient
    {
        string GetGoogleAuthUrl(string userId);

        Task ExchangeCodeForTokensAsync(string code, string userId);

        Task<List<Event>> GetCalendarEventsAsync(string userId);

        Task<bool> IsGoogleCalendarConnectedAsync(string userId);
    }
}