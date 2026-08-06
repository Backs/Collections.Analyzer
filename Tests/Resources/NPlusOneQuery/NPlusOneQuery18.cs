using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Resources.NPlusOneQuery.Batch
{
    public class TradingPartnersSettings { }

    public interface ITradingPartnersSettingsRepository
    {
        Task<Guid[]> ReadAllKeysAsync();
        Task<TradingPartnersSettings[]> ReadBatchAsync(IEnumerable<Guid> ids);
    }

    public class TradingPartnerService
    {
        private readonly ITradingPartnersSettingsRepository _repository;

        public TradingPartnerService(ITradingPartnersSettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task ProcessInLoop(IEnumerable<Guid> allIds)
        {
            foreach (var id in allIds)
            {
                // Batch method should not call warning
                var settings = await _repository.ReadBatchAsync(new[] { id });
                Console.WriteLine(settings.Length);
            }
        }

        public async Task ProcessInBatches(IEnumerable<IEnumerable<Guid>> batches)
        {
            foreach (var batch in batches)
            {
                // This should NOT be a warning because we are processing data in batches.
                var settings = await _repository.ReadBatchAsync(batch);
                Console.WriteLine(settings.Length);
            }
        }
    }
}
