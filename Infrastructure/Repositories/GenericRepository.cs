using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IPrimaryKey
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _db.Set<T>().AsQueryable();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<T> InsertAndReturnAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _dbSet.Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        public void Remove(int id)
        {
            _dbSet.Remove(GetAsync(id).GetAwaiter().GetResult());
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
