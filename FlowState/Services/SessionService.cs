using FlowState.Models;
using FlowState.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Services
{
    public interface ISessionService
    {
        public List<Session> GetSessionsByUser(int userId);

        public Session? GetSession(int id);

        public Session AddSession(int userId, Session session);

        public Session? UpdateSession(int id, Session updatedSession);

        public bool DeleteSession(int id);

        public SessionUser AddSessionUser(int sessionId, int userId);

        public SessionUser? GetSessionUser(int sessionUserId);

        public List<SessionUser>? GetSessionUsersBySession(int sessionId);


        public bool DeleteSessionUser(int userId, int sessionId);

        public SessionInvite CreateSessionInvite(SessionInvite invite);

    }
    public class SessionService : ISessionService
    {

        private ISessionRepo _sessionRepo;

        public SessionService(ISessionRepo sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }
        

        public List<Session> GetSessionsByUser(int userId)
        {
            return _sessionRepo.GetSessionsByUser(userId);
        }

        public Session? GetSession(int id)
        {

            return _sessionRepo.GetSession(id);
        }


        public Session AddSession(int userId, Session session)
        {
            return _sessionRepo.AddSession(userId, session);
        }


        public Session? UpdateSession(int id, Session updatedSession)
        {
            return _sessionRepo.UpdateSession(id, updatedSession);
        }

        public bool DeleteSession(int id)
        {
           return _sessionRepo.DeleteSession(id);
        }

        public SessionUser AddSessionUser(int sessionId, int userId)
        {
            return _sessionRepo.AddSessionUser(sessionId, userId);
        }

        public bool DeleteSessionUser(int userId, int sessionId)
        {
            return _sessionRepo.DeleteSessionUser(userId,sessionId);  
        }

        public SessionUser? GetSessionUser(int sessionUserId)
        {
            return _sessionRepo.GetSessionUser(sessionUserId);
        }

        public List<SessionUser>? GetSessionUsersBySession(int sessionId)
        {

            return _sessionRepo.GetSessionUsersBySession(sessionId);    
        }

        public SessionInvite CreateSessionInvite(SessionInvite invite)
        {
            return _sessionRepo.CreateSessionInvite(invite);    
        }


    }
}
