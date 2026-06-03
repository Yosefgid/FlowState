using FlowState.Models;

namespace FlowState.Repositories
{
    public interface ISessionRepo
    {
        public List<Session> GetSessionsByUser(int userId);

        public Session? GetSession(int id);

        public Session AddSession(int userId,Session session);

        public Session? UpdateSession(int id, Session updatedSession);

        public bool DeleteSession(int id);

        public SessionUser AddSessionUser(int sessionId,int userId);

        public SessionUser? GetSessionUser(int sessionUserId);

        public List<SessionUser>? GetSessionUsersBySession(int sessionId);


        public bool DeleteSessionUser(int sessionUserId);

    }
    public class SessionRepo : ISessionRepo
    {
        private MyDbContext _dbContext;

        public SessionRepo(MyDbContext context)
        {
            _dbContext = context;
        }

        public List<Session> GetSessionsByUser(int userId)
        {
            return _dbContext.SessionUsers
                .Where(x => x.UserId == userId)
                .SelectMany(x => _dbContext.Sessions.Where(y => y.Id == x.SessionId)).ToList();
        }

        public Session? GetSession(int id)
        {

            var existingSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == id);

            if (existingSession == null)
                return null;

            return existingSession;
        }


        public Session AddSession(int userId,Session session)
        {
            _dbContext.SessionUsers.Add(new SessionUser(session.Id,userId));
            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();
            return session;
        }


        public Session? UpdateSession(int id, Session updatedSession)
        {
            var existingSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == id);

            if (existingSession == null)
                return null;

            _dbContext.Entry(existingSession).CurrentValues.SetValues(updatedSession);

            _dbContext.SaveChanges();

            return existingSession;
        }

        public bool DeleteSession(int id)
        {
            var session = _dbContext.Sessions.FirstOrDefault(s => s.Id == id);

            if (session == null)
                return false;

            _dbContext.Sessions.Remove(session);
            _dbContext.SaveChanges();

            return true;
        }

        public SessionUser AddSessionUser(int sessionId, int userId)
        {
            var existingSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId);

            if (existingSession == null)
                return null;

            SessionUser su = new SessionUser(sessionId, userId);
            _dbContext.SessionUsers.Add(su);

            _dbContext.SaveChanges();

            return su;
        }

        public bool DeleteSessionUser(int sessionUserId )
        {
            var sessionUser = _dbContext.SessionUsers.FirstOrDefault(su => su.Id == sessionUserId);

            if (sessionUser == null)
                return false;

            _dbContext.SessionUsers.Remove(sessionUser);
            _dbContext.SaveChanges();

            return true;
        }

        public SessionUser? GetSessionUser(int sessionUserId)
        {
            var sessionUser = _dbContext.SessionUsers.FirstOrDefault(su => su.Id == sessionUserId);
            

            if (sessionUser == null)
                return null;

            return sessionUser;
        }

        public List<SessionUser>? GetSessionUsersBySession(int sessionId)
        {

            var existingSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId);

            if (existingSession == null)
                return null;

            return _dbContext.SessionUsers.Where(y => y.SessionId == sessionId).ToList();
        }
    }
}
