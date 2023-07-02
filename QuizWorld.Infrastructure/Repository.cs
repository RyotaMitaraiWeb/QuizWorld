using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Infrastructure.Data;

namespace QuizWorld.Infrastructure
{
    /// <summary>
    /// Implementation of repository access methods
    /// for Relational Database Engine
    /// </summary>
    /// <typeparam name="T">Type of the data table to which the
    /// current repository is attached</typeparam>
    public class Repository : IRepository
    {
        /// <summary>
        /// Entity Framework dbContext holding connection information and properties
        /// and tracking entity states 
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// Representation of a table in database
        /// </summary>
        protected DbSet<T> DbSet<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        public Repository(QuizWorldDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Adds an entity to the database
        /// </summary>
        /// 
        /// <param name="entity">An entity to add</param>
        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        /// <summary>
        /// Adds a collection of entities to the database
        /// </summary>
        /// 
        /// <param name="entities">An enumerable list of entities</param>
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await DbSet<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// All records in a table
        /// </summary>
        /// 
        /// <returns>Queryable expression tree</returns>
        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>().AsQueryable();
        }

        public IQueryable<T> All<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>().Where(search);
        }

        /// <summary>
        /// The result collection won't be tracked by the context
        /// </summary>
        /// 
        /// <returns>Expression tree</returns>
        public IQueryable<T> AllReadonly<T>() where T : class
        {
            return this.DbSet<T>()
                .AsNoTracking();
        }
        public IQueryable<T> AllReadonly<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>()
                .Where(search)
                .AsNoTracking();
        }

        /// <summary>
        /// Deletes a record from the database
        /// </summary>
        /// <param name="id">Identificator of the record to be deleted</param>
        public async Task DeleteAsync<T>(object id) where T : class
        {
            T entity = await GetByIdAsync<T>(id);

            Delete<T>(entity);
        }

        /// <summary>
        /// Deletes a record from the database
        /// </summary>
        /// <param name="entity">An entity representing the record to be deleted</param>
        public void Delete<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DbSet<T>().Attach(entity);
            }

            entry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Detaches the given entity from the context
        /// </summary>
        /// <param name="entity">An entity to be detached</param>
        public void Detach<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            entry.State = EntityState.Detached;
        }

        /// <summary>
        /// Disposing the context when it is not needed
        /// Don't have to call this method explicitely
        /// Leave it to the IoC container
        /// </summary>
        public void Dispose()
        {
            this.Context.Dispose();
        }

        /// <summary>
        /// Gets a specific record from the database by primary key
        /// </summary>
        /// <param name="id">A record identificator</param>
        /// <returns>A single record</returns>
        public async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        /// <summary>
        /// Gets a specific record from the database by primary keys
        /// </summary>
        /// <param name="id">An array of record identificators</param>
        /// 
        /// <returns>A single record</returns>
        public async Task<T> GetByIdsAsync<T>(object[] id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        /// <summary>
        /// Saves all made changes in trasaction
        /// </summary>
        /// 
        /// <returns>Error code</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await this.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates a record in database
        /// </summary>
        /// 
        /// <param name="entity">The entity for record to be updated</param>
        public void Update<T>(T entity) where T : class
        {
            this.DbSet<T>().Update(entity);
        }

        /// <summary>
        /// Updates a set of records in the database
        /// </summary>
        /// 
        /// <param name="entities">An enumerable collection of the entities to be updated</param>
        public void UpdateRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().UpdateRange(entities);
        }

        /// <summary>
        /// Deletes a set of records in the database
        /// </summary>
        /// <param name="entities">An enumerable collection of the entities to be deleted</param>
        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            this.DbSet<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Deletes a set of records in the database based on a boolean expression
        /// </summary>
        /// <param name="deleteWhereClause">An expression that determines which records to be deleted</param>
        public void DeleteRange<T>(Expression<Func<T, bool>> deleteWhereClause) where T : class
        {
            var entities = All<T>(deleteWhereClause);
            DeleteRange(entities);
        }
    }
}
