using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSDF.DataChecker.Persistence.EntityFramework;

namespace MSDF.DataChecker.Persistence.ValidationsRun
{
   public interface IValidationRunQueries
    {
        Task<List<ValidationRun>> GetAsync();
        Task<ValidationRun> GetAsync(int id);
    }
    public class ValidationRunQueries : IValidationRunQueries
    { 
        private readonly DatabaseContext _db;
        public ValidationRunQueries(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<ValidationRun>> GetAsync()
        {
            var models = await _db.ValidationRuns
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return models;
        }

        public async Task<ValidationRun> GetAsync(int  id)
        {
            var model = await _db.ValidationRuns
                .Where(rec => rec.Id == id)
                .FirstOrDefaultAsync();
            return model;
        }
    }
}
