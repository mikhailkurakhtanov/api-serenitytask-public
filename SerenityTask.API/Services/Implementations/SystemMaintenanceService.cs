using System;
using System.Collections.Generic;
using System.Linq;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services.Implementations
{
    public class SystemMaintenanceService : ISystemMaintenanceService
    {
        private readonly SerenityTaskDbContext _dbContext;

        public SystemMaintenanceService(SerenityTaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateProblemReport(ProblemReport newProblemReport, Guid currentUserid)
        {
            newProblemReport.CreationDate = DateTime.UtcNow;
            newProblemReport.UserId = currentUserid;

            _dbContext.ProblemReports.Add(newProblemReport);
            await _dbContext.SaveChangesAsync();
        }

        public List<Changelog> GetChangelog()
        {
            var changelog = _dbContext.Changelog.ToList();
            for (var index = 0; index < changelog.Count; index++)
            {
                changelog[index].CreationDateString = changelog[index].CreationDate.ToString("u");
            }

            return changelog;
        }
    }
}