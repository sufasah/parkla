using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using HeyRed.Mime;
using Microsoft.AspNetCore.Hosting;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.Core.Helpers;
using Parkla.Core.Models;

namespace Parkla.Business.Concrete;
public class ParkAreaService : EntityServiceBase<ParkArea>, IParkAreaService
{
    private readonly IParkAreaRepo _parkAreaRepo;
    private readonly IValidator<ParkArea> _validator;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IParklaHubService _parklaHubService;

    public ParkAreaService(
        IParkAreaRepo parkAreaRepo,
        IValidator<ParkArea> validator,
        IWebHostEnvironment hostEnvironment,
        IParklaHubService parklaHubService
    ) : base(parkAreaRepo, validator)
    {
        _parkAreaRepo = parkAreaRepo;
        _validator = validator;
        _hostEnvironment = hostEnvironment;
        _parklaHubService = parklaHubService;
    }

    public override async Task<ParkArea?> GetAsync<TKey>(
        TKey id,
        CancellationToken cancellationToken = default
    ) {
        return await _parkAreaRepo.GetAsync(
            id,
            new Expression<Func<ParkArea, object>>[]{
                x => x.Pricings!
            },
            cancellationToken
        );
    }

    public async Task DeleteAsync(ParkArea parkArea, int userId, CancellationToken cancellationToken = default)
    {
        await ThrowIfUserNotMatch((int)parkArea.Id!, userId, cancellationToken).ConfigureAwait(false);
        var (newArea, park, deletedSpaces) = await _parkAreaRepo.DeleteAsync(parkArea, cancellationToken).ConfigureAwait(false);
        
        if(park != null)
            _ = _parklaHubService.ParkChangesAsync(park, false); 

        if(newArea != null) {
            _ = _parklaHubService.ParkAreaChangesAsync(newArea, true);

            foreach (var item in deletedSpaces)
                _ = _parklaHubService.ParkSpaceChangesAsync(item, true);
        }
    }

    public async Task<ParkArea> UpdateAsync(
        ParkArea parkArea,
        int userId,
        bool templateMode,
        CancellationToken cancellationToken = default
    ) {
        var result = await _validator.ValidateAsync(
            parkArea,
            o => o.IncludeRuleSets(templateMode ? "templateUpdate" : "update", "id"),
            cancellationToken
        ).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.Errors.First().ToString(), HttpStatusCode.BadRequest);

        await ThrowIfUserNotMatch((int)parkArea.Id!, userId, cancellationToken).ConfigureAwait(false);

        if (templateMode) {
            var templateImageB64Validation = _validator.Validate(parkArea, o => o.IncludeRuleSets("template"));

            if(parkArea.TemplateImage != null && templateImageB64Validation.IsValid) {
                var www = _hostEnvironment.WebRootPath;
                var split = parkArea.TemplateImage!.Split(',');
                var mime = split[0];
                var b64 = split[1];
                var extension = MimeTypesMap.GetExtension(mime);
                var fileName = $"{DateTime.UtcNow:yymmssffff}_{Path.GetRandomFileName()}.{extension}";
                var path = Path.Combine(www, "Templates", fileName);
                var fileData = Convert.FromBase64String(b64);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.CreateNew);
                await stream.WriteAsync(fileData, cancellationToken).ConfigureAwait(false);

                try {
                    parkArea.TemplateImage = fileName;
                    var (nArea, nPark, tDeletedSpaces) = await _parkAreaRepo.UpdateTemplateAsync(parkArea, cancellationToken);

                    if(nPark != null)
                        _ = _parklaHubService.ParkChangesAsync(nPark, false);

                    _ = _parklaHubService.ParkAreaChangesAsync(nArea, false);

                    foreach (var item in nArea.Spaces)
                        _ = _parklaHubService.ParkSpaceChangesAsync(item, false);

                    foreach (var item in tDeletedSpaces)
                        _ = _parklaHubService.ParkSpaceChangesAsync(item, true);

                    return nArea;
                }
                catch(Exception) {
                    File.Delete(path);
                    throw;
                }
            }

            var (newArea, newPark, deletedSpaces) = await _parkAreaRepo.UpdateTemplateAsync(parkArea, cancellationToken);

            if(newPark != null)
                _ = _parklaHubService.ParkChangesAsync(newPark, false);

            _ = _parklaHubService.ParkAreaChangesAsync(newArea, false);
            
            foreach (var item in newArea.Spaces)
                _ = _parklaHubService.ParkSpaceChangesAsync(item, false);

            foreach (var item in deletedSpaces)
                _ = _parklaHubService.ParkSpaceChangesAsync(item, true);

            return newArea;
        }
        else
        {
            var (areaResult, parkResult) = await _parkAreaRepo.UpdateAsync(parkArea, cancellationToken).ConfigureAwait(false);

            if(parkResult != null)
                _ = _parklaHubService.ParkChangesAsync(parkResult, false);
            
            _ = _parklaHubService.ParkAreaChangesAsync(areaResult, false);

            foreach (var item in areaResult.Spaces)
                _ = _parklaHubService.ParkSpaceChangesAsync(item, false);

            return areaResult;
        }

    }

    public override async Task<ParkArea> AddAsync(ParkArea entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("update").IncludeProperties(x => x.ParkId), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.Errors.First().ToString(), HttpStatusCode.BadRequest);

        entity.TemplateImage = null;
        entity.StatusUpdateTime = null;
        entity.EmptySpace = 0;
        entity.OccupiedSpace = 0;
        entity.MinPrice = null;
        entity.AvaragePrice = null;
        entity.MaxPrice = null;
        entity.Spaces = null!;

        return await _parkAreaRepo.AddAsync(
            entity,
            cancellationToken
        ).ConfigureAwait(false);
    }

    private async Task ThrowIfUserNotMatch(int parkAreaId, int userId, CancellationToken cancellationToken)
    {
        var parkArea = await _parkAreaRepo.GetAsync(
            new Expression<Func<ParkArea, object>>[] { x => x.Park! },
            x => x.Id == parkAreaId,
            cancellationToken
        ).ConfigureAwait(false);

        if (parkArea != null && parkArea.Park!.UserId != userId)
            throw new ParklaException("User requested is not permitted to update or delete other user's parkArea", HttpStatusCode.BadRequest);
    }

    public async Task<PagedList<InstantParkAreaReservedSpace>> GetPageAsync(
        Guid parkId, 
        int nextRecord, 
        int pageSize, 
        string? search, 
        string? orderBy, 
        bool ascending, 
        CancellationToken cancellationToken = default
    ) {
        NullOrTrim(ref search);
        NullOrTrim(ref orderBy);

        Expression eFilter = (ParkArea x) => x.ParkId == parkId;
        if(search != null) {
            eFilter = (ParkArea x) => (
                x.Name!.ToLower().Contains(search) ||
                x.MinPrice.ToString()!.ToLower().Contains(search) ||
                x.AvaragePrice.ToString()!.ToLower().Contains(search) ||
                x.MaxPrice.ToString()!.ToLower().Contains(search) ||
                x.EmptySpace.ToString()!.ToLower().Contains(search) ||
                x.OccupiedSpace.ToString()!.ToLower().Contains(search) ||
                x.Description!.ToLower().Contains(search) || 
                x.StatusUpdateTime.ToString()!.ToLower().Contains(search)
            ) && x.ParkId == parkId;
        }

        return await _parkAreaRepo.GetParkAreaPage(
            nextRecord,
            pageSize,
            (Expression<Func<ParkArea,bool>>)eFilter,
            GetPropertyLambdaExpression(orderBy),
            ascending,
            cancellationToken
        );
    }

    public async Task<List<InstantParkAreaIdReservedSpace>> GetParkAreasReserverdSpaceCountAsync(int[] ids, CancellationToken cancellationToken)
    {
        return await _parkAreaRepo.GetParkAreasReserverdSpaceCountAsync(ids, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<Pricing>> GetAreaPricingsAsync(int areaId, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<ParkArea, object>>[] {
            x => x.Pricings
        };
        
        var result = await _parkAreaRepo.GetListAsync(
            includes,
            x => x.Id == areaId,
            cancellationToken
        ).ConfigureAwait(false);

        return result.SelectMany(x => x.Pricings).ToList();
    }
}