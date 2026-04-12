using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class UserFacade(
    LauncherDbContext ctx,
    IUserRepository userRepository,
    IModelMapper<UserEntity, UserListModel, UserDetailModel> mapper)
    : FacadeBase<UserEntity, UserListModel, UserDetailModel>(mapper), IUserFacade
{
    public override async Task<IEnumerable<UserListModel>> GetAsync()
        => _mapper.MapToListModel(await userRepository.Get().ToListAsync());

    public async Task<IEnumerable<UserListModel>> GetAsync(string? searchTerm, string? sortBy, bool ascending)
    {
        IQueryable<UserEntity> query = userRepository.Get();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => u.UserName.Contains(searchTerm) || u.DisplayName.Contains(searchTerm));   
        }

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

    public override async Task<IEnumerable<UserListModel>> GetAsync(QueryObject query)
    {
        return await Task.FromResult(Enumerable.Empty<UserListModel>());
    }
    
    public override async Task<UserDetailModel?> GetAsync(Guid id)
    {
        var entity = await userRepository.Get().FirstOrDefaultAsync(u => u.Id == id);
        return entity is null ? null : _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(UserDetailModel detailModel)
    {
        var entity = _mapper.MapToEntity(detailModel);
        if (detailModel.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            userRepository.Insert(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }
        
        await userRepository.UpdateAsync(entity);
        await ctx.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await userRepository.DeleteAsync(id);
        await ctx.SaveChangesAsync();
    }
}