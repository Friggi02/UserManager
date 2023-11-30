using Project.Dal.Entities;

namespace Project.Dal.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public List<User> GetAllByRole(Role role);
    }
}