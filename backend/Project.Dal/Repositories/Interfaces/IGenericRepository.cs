using Microsoft.AspNetCore.OData.Query;
using Project.Dal.Entities;

namespace Project.Dal.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        #region Get

        /// <summary>
        /// Performs a get single on a generic table
        /// </summary>
        /// <param name="id">Id of the searched entity</param>
        /// <returns>The searched <typeparamref name="T"/> or null if it doesn't exist</returns>
        T? GetById(int id);

        /// <summary>
        /// Performs a get single on a generic table
        /// </summary>
        /// <param name="id">Id of the searched entity</param>
        /// <returns>The searched <typeparamref name="T"/> or null if it doesn't exist</returns>
        T? GetById(string id);

        /// <summary>
        /// Performs a get single on a generic table
        /// </summary>
        /// <param name="id">Id of the searched entity</param>
        /// <returns>The searched <typeparamref name="T"/> or null if it doesn't exist</returns>
        T? GetById(Guid id);

        /// <summary>
        /// Performs a get all on a generic table
        /// </summary>
        /// <returns>A list of <typeparamref name="T"/></returns>
        List<T> GetAll();

        /// <summary>
        /// Performs a get all on a generic table with a <paramref name="query"/>
        /// </summary>
        /// <param name="query">Lambda expression</param>
        /// <returns>A list of the <typeparamref name="T"/> that match the given <paramref name="query"/></returns>
        List<T> GetByFilter(Func<T, bool> query);

        /// <summary>
        /// Performs a get all on a generic table with a <paramref name="query"/> and an include on a specified <paramref name="navigationProperty"/>
        /// </summary>
        /// <param name="query">Lambda expression</param>
        /// <param name="navigationProperty">String representi the name of the navigation property</param>
        /// <returns></returns>
        List<T> GetByFilterIncluding(Func<T, bool> query, string navigationProperty);

        /// <summary>
        /// Performs an OData get on a generic table
        /// </summary>
        /// <param name="options">OData query's parameters</param>
        /// <returns>The <typeparamref name="T"/> that match the query's parameters</returns>
        IQueryable GetAllOData(ODataQueryOptions<T> options);

        #endregion

        #region Create

        /// <summary>
        /// Saves the <paramref name="entity"/> in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The operation result</returns>
        bool Create(T entity);

        #endregion

        #region Delete

        /// <summary>
        /// Delete the <typeparamref name="T"/> with the specified <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the entity to delete</param>
        /// <returns>The operation result</returns>
        bool Delete(int id);

        /// <summary>
        /// Delete the <typeparamref name="T"/> with the specified <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the entity to delete</param>
        /// <returns>The operation result</returns>
        bool Delete(string id);

        /// <summary>
        /// Delete the <typeparamref name="T"/> with the specified <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the entity to delete</param>
        /// <returns>The operation result</returns>
        bool Delete(Guid id);

        /// <summary>
        ///  Delete all the <typeparamref name="T"/> that match the specified <paramref name="query"/>
        /// </summary>
        /// <param name="query">Lambda expression</param>
        /// <returns></returns>
        void DeleteByFilter(Func<T, bool> query);

        #endregion

        #region Exist

        /// <summary>
        /// Checks if a specific <typeparamref name="T"/> exists
        /// </summary>
        /// <param name="propertyName">The name of the property to check (usually the id).</param>
        /// <param name="value">The integer value to compare with the property's value.</param>
        /// <param name="query">An optional lambda expression to filter the set of objects.</param>
        /// <returns>True if the value exists in the property of any object in the (optionally filtered) set, and false otherwise.</returns>
        bool Exist(string propertyName, int value, Func<T, bool>? query);

        /// <summary>
        /// Checks if a specific <typeparamref name="T"/> exists
        /// </summary>
        /// <param name="propertyName">The name of the property to check (usually the id).</param>
        /// <param name="value">The Guid value to compare with the property's value.</param>
        /// <param name="query">An optional lambda expression to filter the set of objects.</param>
        /// <returns>True if the value exists in the property of any object in the (optionally filtered) set, and false otherwise.</returns>
        bool Exist(string propertyName, Guid value, Func<T, bool>? query);

        /// <summary>
        /// Checks if a specific <typeparamref name="T"/> exists
        /// </summary>
        /// <param name="propertyName">The name of the property to check (usually the id).</param>
        /// <param name="value">The string value to compare with the property's value.</param>
        /// <param name="query">An optional lambda expression to filter the set of objects.</param>
        /// <returns>True if the value exists in the property of any object in the (optionally filtered) set, and false otherwise.</returns>
        bool Exist(string propertyName, string value, Func<T, bool>? query);

        #endregion

        /// <summary>
        /// Applay the JSON patch on a specific <typeparamref name="T"/>
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="current"></param>
        /// <returns>The operation result</returns>
        bool Patch(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<T> patch, T current);
        bool Update(T entity);
    }
}