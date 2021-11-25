using System.Threading.Tasks;
using VAPI.Entities;

namespace VAPI.Interfaces
{
    public interface IGroupRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        Task<bool> SaveAllAsync();

    }
}