namespace Project.Dal.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepo { get; }
        IUserRoleRepository UserRoleRepo { get; }
    }
}