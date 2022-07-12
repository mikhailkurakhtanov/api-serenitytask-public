using System;
using System.Collections.Generic;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface ISystemMaintenanceService
    {
        Task CreateProblemReport(ProblemReport newProblemReport, Guid currentUserid);

        List<Changelog> GetChangelog();
    }
}