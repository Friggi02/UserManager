using Project.Dal.Entities;

namespace Project.Dal.Repositories.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        public Task<HashSet<string>> GetPermissionsAsync(string userId);
    }
}