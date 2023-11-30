using System.Reflection;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Project.Dal.Entities;

namespace Project.Dal.Repositories
{
    public class GenericRepository<T> where T : class
    {
        protected readonly ProjectDbContext _ctx;

        public GenericRepository(ProjectDbContext ctx)
        {
            _ctx = ctx;
        }

        #region Get

        public T? GetById(int id) => _ctx.Find<T>(id);
        public T? GetById(string id) => _ctx.Find<T>(id);
        public T? GetById(Guid id) => _ctx.Find<T>(id);
        public List<T> GetAll() => _ctx.Set<T>().ToList();
        public List<T> GetByFilter(Func<T, bool> predicate) => _ctx.Set<T>().Where(predicate).ToList();
        public List<T> GetByFilterIncluding(Func<T, bool> predicate, string navigationProperty) => _ctx.Set<T>().Include(navigationProperty).Where(predicate).ToList();
        public IQueryable GetAllOData(ODataQueryOptions<T> options) => options.ApplyTo(_ctx.Set<T>().AsQueryable<T>());

        #endregion

        #region Create

        public Result Create(T entity)
        {
            try
            {
                _ctx.Add(entity);
                if (_ctx.SaveChanges() > 0)
                    return new Result(true, "Entity deleted successfully");
                else
                    return new Result(false, "Entity creation failed");
            }
            catch (Exception) { return new Result(false, "Entity creation faild"); }
        }

        #endregion

        #region Delete

        public Result Delete(int id)
        {
            T? entity = GetById(id);
            if (entity is null) return new Result(false, "Entity not found");
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return new Result(true, "Entity deleted successfully");
            else
                return new Result(false, "Entity deletion failed");
        }
        public Result Delete(string id)
        {
            T? entity = GetById(id);
            if (entity is null) return new Result(false, "Entity not found");
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return new Result(true, "Entity deleted successfully");
            else
                return new Result(false, "Entity deletion failed");
        }
        public Result Delete(Guid id)
        {
            T? entity = GetById(id);
            if (entity is null) return new Result(false, "Entity not found");
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return new Result(true, "Entity deleted successfully");
            else
                return new Result(false, "Entity deletion failed");
        }
        public Result DeleteByFilter(Func<T, bool> predicate)
        {
            _ctx.RemoveRange(GetByFilter(predicate));
            if (_ctx.SaveChanges() > 0)
                return new Result(true, "Entities deleted successfully");
            else
                return new Result(false, "Entities deletion failed");
        }

        #endregion

        #region Exist
        public Result Exist(string propertyName, int value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);

            if (property == null || property.PropertyType != typeof(int)) return new Result(false, "Invalid property name or type");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (int?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (int?)property.GetValue(x) == value);

            if (exist)
                return new Result(true, "It exists");
            else
                return new Result(false, "It doesn't exist");
        }
        public Result Exist(string propertyName, Guid value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);

            if (property == null || property.PropertyType != typeof(Guid)) return new Result(false, "Invalid property name or type");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (Guid?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (Guid?)property.GetValue(x) == value);

            if (exist)
                return new Result(true, "It exists");
            else
                return new Result(false, "It doesn't exist");
        }
        public Result Exist(string propertyName, string value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);

            if (property == null || property.PropertyType != typeof(string)) return new Result(false, "Invalid property name or type");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (string?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (string?)property.GetValue(x) == value);

            if (exist)
                return new Result(true, "It exists");
            else
                return new Result(false, "It doesn't exist");
        }

        #endregion

        public Result Patch(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<T> patch, T current)
        {
            patch.ApplyTo(current);
            if (_ctx.SaveChanges() > 0)
                return new Result(true, "Entity updated successfully");
            else
                return new Result(false, "Entity update failed");
        }
    }
}