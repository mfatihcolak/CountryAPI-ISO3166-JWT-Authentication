using CountryAPI.DataAccess;
using CountryAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CountryAPI.Repository
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, new()
    {
        private readonly MyDbContext _context;

        protected EntityRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entity = await _context.Set<TEntity>().FirstAsync(filter);
            return entity;
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter)
        {
            var resp = await _context.Set<TEntity>().Where(filter).ToListAsync();
            return resp;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
