using System.Data;
using Microsoft.EntityFrameworkCore;
using Project.Dal.Entities;
using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public new readonly ProjectDbContext _ctx;
        public PermissionRepository(ProjectDbContext ctx) : base(ctx)
        {
            _ctx = ctx;
        }

        public async Task<HashSet<string>> GetPermissionsAsync(string userId)
        {
            ICollection<Role>[] roles = await _ctx.Set<User>()
                .Include(x => x.Roles)
                .ThenInclude(x => x.Permissions)
                .Where(x => x.Id == userId.ToString())
                .Select(x => x.Roles)
                .ToArrayAsync();

            return roles
                .SelectMany(x => x)
                .SelectMany(x => x.Permissions)
                .Select(x => x.Name)
                .ToHashSet();
        }

        public bool Update(Permission entity)
        {
            throw new NotImplementedException();
        }
    }
}