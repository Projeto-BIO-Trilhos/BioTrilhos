using Estufa.Api.Models;
using Estufa.Api.Repositories;

namespace Estufa.Api.Services
{
    public class SensorService : ISensorService
    {
        private readonly ISensorRepository _sensorRepo;

        public SensorService(ISensorRepository sensorRepo)
        {
            _sensorRepo = sensorRepo;
        }

        public async Task<Sensor> CreateAsync(Sensor sensor)
        {
            await _sensorRepo.AddAsync(sensor);
            await _sensorRepo.SaveChangesAsync();
            return sensor;
        }

        public async Task DeleteAsync(int id)
        {
            var s = await _sensorRepo.GetByIdAsync(id);
            if (s == null) return;
            _sensorRepo.Remove(s);
            await _sensorRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            return await _sensorRepo.GetAllAsync();
        }

        public async Task<Sensor?> GetByIdAsync(int id)
        {
            return await _sensorRepo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Sensor sensor)
        {
            _sensorRepo.Update(sensor);
            await _sensorRepo.SaveChangesAsync();
        }
    }
}
