using Project.Dal.Entities;
using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Repositories
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ProjectDbContext ctx) : base(ctx)
        {
        }

        public bool Update(User entity)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAllByRole(Role role)
        {
            List<User> users = GetByFilterIncluding(x => !x.IsDeleted, "Roles");
            users.RemoveAll(x => !x.Roles.Contains(role));
            return users;
        }
    }
}