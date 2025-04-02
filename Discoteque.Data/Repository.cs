using System.Linq.Expressions;
using Discoteque.Data.IRepositories;
using Discoteque.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Discoteque.Data.Repository;
public class Repository<Tid, TEntity> : IRepository<Tid, TEntity>
where Tid : struct
where TEntity : BaseEntity<Tid>
{
    
    internal DiscotequeContext _context;
    internal DbSet<TEntity> _dbSet;

    public Repository(DiscotequeContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> FindAsync(Tid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet;
        if(filter is not null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if(orderBy is not null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Delete(TEntity entity)
    {
        if(_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);                
        }
        _dbSet.Remove(entity);

        await _context.SaveChangesAsync();

    }

    public virtual async Task Delete(Tid id)
    {
        TEntity? entitToDetelete = await _dbSet.FindAsync(id);
        if(entitToDetelete is not null)
        {
            await Delete(entitToDetelete);
        }
    }

    public virtual async Task Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        _context.ChangeTracker.DetectChanges();
        await _context.SaveChangesAsync();
    }

}