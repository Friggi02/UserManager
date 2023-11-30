using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepo { get; private set; }
        public IUserRoleRepository UserRoleRepo { get; private set; }

        public UnitOfWork(ProjectDbContext ctx)
        {
            UserRepo = new UserRepository(ctx);
            UserRoleRepo = new UserRoleRepository(ctx);
        }
    }
}