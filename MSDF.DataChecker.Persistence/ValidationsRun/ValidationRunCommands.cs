using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;

namespace MSDF.DataChecker.Persistence.ValidationsRun
{
    public interface IValidationRunCommands
    {
        Task<ValidationRun> AddAsync(ValidationRun validationRun);
        Task<ValidationRun> UpdateAsync(ValidationRun validationRun);
    }

    public class ValidationRunCommands : IValidationRunCommands
    {
        private readonly DatabaseContext _db;
        public ValidationRunCommands(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<ValidationRun> AddAsync(ValidationRun validationRun)
        {
            _db.Entry(validationRun).State = EntityState.Added;
            var result = this._db.Add(validationRun);
            await this._db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ValidationRun> UpdateAsync(ValidationRun model)
        {
            var entity = await _db.ValidationRuns
               .FirstOrDefaultAsync(rec => rec.Id == model.Id);

            if (entity != null)
            {
                entity.EndTime = model.EndTime;
                entity.HostDatabase = model.HostDatabase;
                entity.HostServer = model.HostServer;
                entity.Id = model.Id;
                entity.RunStatus = model.RunStatus;
                entity.Source = model.Source;
                entity.StartTime = model.StartTime;

                _db.ValidationRuns.Update(entity);
                await _db.SaveChangesAsync();
                return entity;
            }
            return null;

        }
    }
}
