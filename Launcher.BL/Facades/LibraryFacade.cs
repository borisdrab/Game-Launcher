using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher.BL.Facades
{
    public class LibraryFacade(
        LauncherDbContext ctx,
        LibraryRepository libraryRepository,
        IModelMapper<LibraryEntity, LibraryListModel, LibraryDetailModel> mapper)
        : FacadeBase<LibraryEntity, LibraryListModel, LibraryDetailModel>(mapper), ILibraryFacade
    {
        public async Task<LibraryListModel> FilterAsync(Guid userId, string? gameName, string? sortBy, bool ascending, params string[] genres)
        {
            var query = libraryRepository.Get()
                .Include(l => l.LibraryTitles)
                    .ThenInclude(lt => lt.GameTitle)
                        .ThenInclude(gt => gt!.GameTitleGenres)
                            .ThenInclude(gtg => gtg.Genre)
                .Where(lib => lib.UserId == userId);

            LibraryEntity? userLibrary = await query.FirstOrDefaultAsync();
            if (userLibrary == null) return LibraryListModel.Empty;

            var filteredTitles = userLibrary.LibraryTitles.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(gameName))
            {
                filteredTitles = filteredTitles.Where(lt => lt.GameTitle?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (genres is { Length: > 0 })
            {
                filteredTitles = filteredTitles.Where(lt => 
                    lt.GameTitle?.GameTitleGenres?.Any(gtg => 
                        gtg.Genre != null && genres.Any(g => string.Equals(g, gtg.Genre.Name, StringComparison.OrdinalIgnoreCase))) == true);
            }

            filteredTitles = (sortBy?.ToLower(), ascending) switch
            {
                ("name", true) => filteredTitles.OrderBy(lt => lt.GameTitle?.Name ?? string.Empty),
                ("name", false) => filteredTitles.OrderByDescending(lt => lt.GameTitle?.Name ?? string.Empty),
                ("addedat", true) => filteredTitles.OrderBy(lt => lt.AddedAt),
                ("addedat", false) => filteredTitles.OrderByDescending(lt => lt.AddedAt),
                _ => filteredTitles.OrderBy(lt => lt.GameTitle?.Name ?? string.Empty)
            };

            userLibrary.LibraryTitles = filteredTitles.ToList();

            return _mapper.MapToListModel(userLibrary);
        }

        public override async Task<LibraryDetailModel?> GetAsync(Guid id)
        {
            var entity = await libraryRepository.Get()
                .Include(l => l.LibraryTitles)
                    .ThenInclude(lt => lt.GameTitle)
                .Include(l => l.User)
                .FirstOrDefaultAsync(u => u.Id == id);
            
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

        public override async Task DeleteAsync(Guid id)
        {
            await libraryRepository.DeleteAsync(id);
            await ctx.SaveChangesAsync();
        }

        public override async Task<IEnumerable<LibraryListModel>> GetAsync()
            => _mapper.MapToListModel(await libraryRepository.Get()
                .Include(l => l.LibraryTitles)
                .Include(l => l.User)
                .ToListAsync());
    }
}
