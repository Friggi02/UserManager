using Project.Dal.Entities;
using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Repositories
{
    internal class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ProjectDbContext ctx) : base(ctx)
        {
        }

        public Result Update(UserRole entity)
        {
            throw new NotImplementedException();
        }
    }
}