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
            var libraryEntity = await libraryRepository.Get()
                .AsNoTracking()
                .Include(l => l.User)
                .FirstOrDefaultAsync(lib => lib.UserId == userId);

            if (libraryEntity == null) return LibraryListModel.Empty;

            var query = ctx.LibraryTitles
                .AsNoTracking()
                .Include(lt => lt.GameTitle)
                    .ThenInclude(gt => gt!.GameTitleGenres)
                        .ThenInclude(gtg => gtg.Genre)
                .Where(lt => lt.LibraryId == libraryEntity.Id);

            if (!string.IsNullOrWhiteSpace(gameName))
            {
                query = query.Where(lt => lt.GameTitle!.Name.Contains(gameName));
            }

            if (genres is { Length: > 0 })
            {
                query = query.Where(lt => 
                    lt.GameTitle!.GameTitleGenres.Any(gtg => 
                        genres.Contains(gtg.Genre!.Name)));
            }

            query = (sortBy?.ToLower(), ascending) switch
            {
                ("name", true) => query.OrderBy(lt => lt.GameTitle!.Name),
                ("name", false) => query.OrderByDescending(lt => lt.GameTitle!.Name),
                ("addedat", true) => query.OrderBy(lt => lt.AddedAt),
                ("addedat", false) => query.OrderByDescending(lt => lt.AddedAt),
                _ => query.OrderBy(lt => lt.GameTitle!.Name)
            };

            var filteredTitles = await query.ToListAsync();

            var listModel = _mapper.MapToListModel(libraryEntity);
            
            listModel.LibraryTitles = filteredTitles.Select(lt => new LibraryTitleListModel
            {
                LibraryId = lt.LibraryId,
                GameTitleId = lt.GameTitleId,
                AddedAt = lt.AddedAt,
                IsFavorite = lt.IsFavorite,
                PriceCentsAtPurchase = lt.PriceCentsAtPurchase,
                GameTitle = lt.GameTitle is null ? null : new GameTitleListModel
                {
                    Id = lt.GameTitle.Id,
                    Name = lt.GameTitle.Name,
                    PegiRating = lt.GameTitle.PegiRating,
                    PriceCents = lt.GameTitle.PriceCents,
                    CoverImageUrl = lt.GameTitle.CoverImageUrl,
                    Publisher = lt.GameTitle.Publisher,
                    ReleaseDate = lt.GameTitle.ReleaseDate,
                    IsAvailable = lt.GameTitle.IsAvailable
                }
            }).ToList();

            return listModel;
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

        public async Task<bool> IsGameInLibraryAsync(Guid userId, Guid gameTitleId)
        {
            return await ctx.LibraryTitles.AnyAsync(lt => lt.Library!.UserId == userId && lt.GameTitleId == gameTitleId);
        }

        public async Task AddGameToLibraryAsync(Guid userId, Guid gameTitleId)
        {
            var library = await ctx.Libraries
                .Include(l => l.LibraryTitles)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (library == null)
            {
                library = new LibraryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Knihovna",
                    UserId = userId
                };
                ctx.Libraries.Add(library);
            }

            if (!library.LibraryTitles.Any(lt => lt.GameTitleId == gameTitleId))
            {
                var game = await ctx.GameTitles.FindAsync(gameTitleId);
                if (game != null)
                {
                    library.LibraryTitles.Add(new LibraryTitleEntity
                    {
                        LibraryId = library.Id,
                        GameTitleId = gameTitleId,
                        AddedAt = DateTime.UtcNow,
                        IsFavorite = false,
                        PriceCentsAtPurchase = game.PriceCents
                    });
                    await ctx.SaveChangesAsync();
                }
            }
        }

        public async Task ToggleFavoriteAsync(Guid libraryId, Guid gameTitleId)
        {
            var libraryTitle = await ctx.LibraryTitles
                .FirstOrDefaultAsync(lt => lt.LibraryId == libraryId && lt.GameTitleId == gameTitleId);

            if (libraryTitle != null)
            {
                libraryTitle.IsFavorite = !libraryTitle.IsFavorite;
                await ctx.SaveChangesAsync();
            }
        }
    }
}
