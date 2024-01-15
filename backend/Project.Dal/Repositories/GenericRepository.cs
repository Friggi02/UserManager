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
        public List<T> GetByFilter(Func<T, bool> query) => _ctx.Set<T>().Where(query).ToList();
        public List<T> GetByFilterIncluding(Func<T, bool> query, string navigationProperty) => _ctx.Set<T>().Include(navigationProperty).Where(query).ToList();
        public IQueryable GetAllOData(ODataQueryOptions<T> options) => options.ApplyTo(_ctx.Set<T>().AsQueryable<T>());

        #endregion

        #region Create

        public bool Create(T entity)
        {
            try
            {
                _ctx.Add(entity);
                if (_ctx.SaveChanges() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception) { return false; }
        }

        #endregion

        #region Delete

        public bool Delete(int id)
        {
            T? entity = GetById(id);
            if (entity is null) return false;
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return true;
            else
                return false;
        }
        public bool Delete(string id)
        {
            T? entity = GetById(id);
            if (entity is null) return false;
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return true;
            else
                return false;
        }
        public bool Delete(Guid id)
        {
            T? entity = GetById(id);
            if (entity is null) return false;
            _ctx.Remove(entity);
            if (_ctx.SaveChanges() > 0)
                return true;
            else
                return false;
        }
        public void DeleteByFilter(Func<T, bool> query)
        {
            _ctx.RemoveRange(GetByFilter(query));
        }

        #endregion

        #region Exist
        public bool Exist(string propertyName, int value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"The property '{propertyName}' does not exist in the type '{typeof(T).Name}'.");
            if (property.PropertyType != typeof(int))
                throw new InvalidCastException($"The type of the property '{propertyName}' is not 'int', but '{property.PropertyType.Name}'.");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (int?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (int?)property.GetValue(x) == value);

            return exist;
        }


        public bool Exist(string propertyName, Guid value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);

            if (property == null)
                throw new ArgumentException($"The property '{propertyName}' does not exist in the type '{typeof(T).Name}'.");
            if (property.PropertyType != typeof(int))
                throw new InvalidCastException($"The type of the property '{propertyName}' is not 'Guid', but '{property.PropertyType.Name}'.");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (Guid?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (Guid?)property.GetValue(x) == value);

            if (exist)
                return true;
            else
                return false;
        }
        public bool Exist(string propertyName, string value, Func<T, bool>? predicate)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);

            if (property == null)
                throw new ArgumentException($"The property '{propertyName}' does not exist in the type '{typeof(T).Name}'.");
            if (property.PropertyType != typeof(int))
                throw new InvalidCastException($"The type of the property '{propertyName}' is not 'string', but '{property.PropertyType.Name}'.");

            bool exist = predicate == null
                ? _ctx.Set<T>().Any(x => (string?)property.GetValue(x) == value)
                : _ctx.Set<T>().Where(predicate).Any(x => (string?)property.GetValue(x) == value);

            if (exist)
                return true;
            else
                return false;
        }

        #endregion

        public bool Patch(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<T> patch, T current)
        {
            patch.ApplyTo(current);
            if (_ctx.SaveChanges() > 0)
                return true;
            else
                return false;
        }
    }
}