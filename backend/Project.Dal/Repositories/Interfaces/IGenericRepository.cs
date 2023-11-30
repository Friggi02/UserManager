using Microsoft.AspNetCore.OData.Query;
using Project.Dal.Entities;

namespace Project.Dal.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        #region Get
        T? GetById(int id);
        T? GetById(string id);
        T? GetById(Guid id);
        List<T> GetAll();
        List<T> GetByFilter(Func<T, bool> predicate);
        List<T> GetByFilterIncluding(Func<T, bool> query, string navigationProperty);
        IQueryable GetAllOData(ODataQueryOptions<T> options);

        #endregion

        #region Create
        Result Create(T entity);
        #endregion

        #region Delete
        Result Delete(int id);
        Result Delete(string id);
        Result Delete(Guid id);
        Result DeleteByFilter(Func<T, bool> predicate);

        #endregion

        #region Exist
        Result Exist(string propertyName, int value, Func<T, bool>? query);
        Result Exist(string propertyName, Guid value, Func<T, bool>? query);
        Result Exist(string propertyName, string value, Func<T, bool>? query);
        #endregion

        Result Patch(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<T> patch, T current);
        Result Update(T entity);
    }
}