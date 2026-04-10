using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Launcher.BL.Facades
{
    public class LibraryFacade(
    LauncherDbContext ctx,
    LibraryRepository libraryRepository,
    IModelMapper<LibraryEntity, LibraryListModel, LibraryDetailModel> mapper)
        : FacadeBase<LibraryEntity, LibraryListModel, LibraryDetailModel>(mapper)
    {
        public async Task<IEnumerable<LibraryListModel>> GetAllAsync()
        {
            IQueryable<LibraryEntity> query = libraryRepository.Get();


            return _mapper.MapToListModel(await query.ToListAsync());
        }

        public async Task<IEnumerable<LibraryListModel>> GetByName(Guid userId, string? searchTerm, string? sortBy, bool ascending, params string[] genres)
        {
            LibraryEntity userLibrary = libraryRepository.Get().Where(lib => lib.UserId == userId).Single();

            var usersGames = userLibrary.LibraryTitles;


            return _mapper.MapToListModel(await userLibrary);
        }

        public override async Task<LibraryDetailModel?> GetAsync(Guid id)
        {
            var entity = await libraryRepository.Get().FirstOrDefaultAsync(u => u.Id == id);
            return entity is null ? null : _mapper.MapToDetailModel(entity);
        }

        public override async Task<Guid> SaveAsync(LibraryDetailModel detailModel)
        {
            var entity = _mapper.MapToEntity(detailModel);
            if (detailModel.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                libraryRepository.Insert(entity);
                await ctx.SaveChangesAsync();
                return entity.Id;
            }

            await libraryRepository.UpdateAsync(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }
        }

        public override async Task DeleteAsync(Guid id)
        {
            await libraryRepository.DeleteAsync(id);
            await ctx.SaveChangesAsync();
        }

        public override async Task<IEnumerable<LibraryListModel>> GetAsync()
            => _mapper.MapToListModel(await libraryRepository.Get().ToListAsync());

    }
}
