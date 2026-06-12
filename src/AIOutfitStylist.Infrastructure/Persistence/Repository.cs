using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AIOutfitStylist.Infrastructure.Persistence;

public sealed class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    public IQueryable<T> Query() => dbContext.Set<T>();
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => dbContext.Set<T>().AddAsync(entity, cancellationToken).AsTask();
    public void Update(T entity) => dbContext.Set<T>().Update(entity);
    public void Delete(T entity) => dbContext.Set<T>().Remove(entity);
}
