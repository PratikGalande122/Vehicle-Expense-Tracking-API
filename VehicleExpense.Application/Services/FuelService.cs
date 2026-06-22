using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Handles fuel log creation and retrieval for a user's vehicles.
/// </summary>
public class FuelService : IFuelService
{
    private readonly IFuelRepository _fuelRepository;

    public FuelService(IFuelRepository fuelRepository)
    {
        _fuelRepository = fuelRepository;
    }

    public async Task<IEnumerable<FuelLog>> GetFuelLogsAsync(int userId) =>
        await _fuelRepository.GetByUserIdAsync(userId);

    public async Task<FuelLog?> AddFuelLogAsync(int userId, AddFuelRequest request)
    {
        var log = new FuelLog
        {
            UserId = userId,
            VehicleId = request.VehicleId,
            LitresFilled = request.LitresFilled,
            PricePerLitre = request.PricePerLitre,
            TotalCost = request.LitresFilled * request.PricePerLitre,
            OdometerReading = request.OdometerReading,
            FilledAt = request.FilledAt,
            Notes = request.Notes
        };

        var id = await _fuelRepository.CreateAsync(log);
        log.Id = id;
        return log;
    }

    public async Task<FuelLog?> UpdateFuelLogAsync(int userId, int id, UpdateFuelRequest request)
    {
        var log = await _fuelRepository.GetByIdAsync(id, userId);
        if (log is null)
            return null;

        log.LitresFilled = request.LitresFilled;
        log.PricePerLitre = request.PricePerLitre;
        log.TotalCost = request.LitresFilled * request.PricePerLitre;
        log.OdometerReading = request.OdometerReading;
        log.FilledAt = request.FilledAt;
        log.Notes = request.Notes;
        log.UpdatedAt = DateTime.UtcNow;

        await _fuelRepository.UpdateAsync(log);
        return log;
    }

    public async Task<bool> DeleteFuelLogAsync(int userId, int id) =>
        await _fuelRepository.DeleteAsync(id, userId);
}
