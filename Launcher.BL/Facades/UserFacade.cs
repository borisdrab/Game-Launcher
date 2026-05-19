using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class UserFacade(
    IModelMapper<UserEntity, UserListModel, UserDetailModel> mapper,
    IDbContextFactory<LauncherDbContext> dbContextFactory)
    : FacadeBase<UserEntity, UserListModel, UserDetailModel>(mapper), IUserFacade
{
    public override async Task<IEnumerable<UserListModel>> GetAsync()
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        return _mapper.MapToListModel(await ctx.Users.AsNoTracking().ToListAsync());
    }

    public async Task<IEnumerable<UserListModel>> GetAsync(string? searchTerm, string? sortBy, bool ascending)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        IQueryable<UserEntity> query = ctx.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(u => u.UserName.Contains(searchTerm) || u.DisplayName.Contains(searchTerm));

        query = (sortBy?.ToLower(), ascending) switch
        {
            ("username", true) => query.OrderBy(u => u.UserName),
            ("username", false) => query.OrderByDescending(u => u.UserName),
            ("displayname", true) => query.OrderBy(u => u.DisplayName),
            ("displayname", false) => query.OrderByDescending(u => u.DisplayName),
            ("email", true) => query.OrderBy(u => u.Email),
            ("email", false) => query.OrderByDescending(u => u.Email),
            _ => query.OrderBy(u => u.UserName)
        };

        return _mapper.MapToListModel(await query.ToListAsync());
    }

    public override async Task<UserDetailModel?> GetAsync(Guid id)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var entity = await ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return entity is null ? null : _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(UserDetailModel model)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var entity = _mapper.MapToEntity(model);

        if (model.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            await ctx.Users.AddAsync(entity);
        }
        else
        {
            var existing = await ctx.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (existing is not null)
            {
                existing.UserName = entity.UserName;
                existing.Email = entity.Email;
                existing.DisplayName = entity.DisplayName;
                existing.AvatarUrl = entity.AvatarUrl;
            }
        }

        await ctx.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var entity = await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (entity is not null)
        {
            ctx.Users.Remove(entity);
            await ctx.SaveChangesAsync();
        }
    }
}
