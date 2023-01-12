// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Hangfire;
using Hangfire.Storage;
using MSDF.DataChecker.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace MSDF.DataChecker.Services
{
    public interface IJobService
    {
        List<JobBO> Get();
        JobBO Get(int id);
        JobBO Add(JobBO job);
        JobBO Update(JobBO job);
        void Delete(int id);
        void Enqueue(int jobId);
        void RunAndForget(JobBO job);
    }

    public class JobService : IJobService
    {
        private JobBO MapEntityToModel(RecurringJobDto source)
        {
            if (source == null) return null;
            JobBO job = (JobBO)source.Job.Args.ToList().FirstOrDefault(rec => rec.GetType().Name == "JobBO");

            if (job != null)
            {
                job.Id = int.Parse(source.Id);
                job.LastFinishedDateTime = source.LastExecution;
                job.Cron = source.Cron;
                job.Status = source.LastJobState;
                return job;
            }

            return new JobBO
            {
                Id = int.Parse(source.Id),
                Cron = source.Cron,
                LastFinishedDateTime = source.LastExecution,
                Status = source.LastJobState
            };
        }

        private int NewId()
        {
            return GetAll().Select(j => j.Id).ToList().OrderByDescending(i => i).FirstOrDefault() + 1;
        }

        private List<JobBO> GetAll()
        {
            return JobStorage.Current.GetConnection().GetRecurringJobs().Select(x => MapEntityToModel(x)).ToList();
        }

        public List<JobBO> Get()
        {
            return GetAll();
        }

        public JobBO Get(int id)
        {
            return GetAll().Where(j => j.Id == id).FirstOrDefault();
        }

        public JobBO Add(JobBO job)
        {
            if (job == null) return null;
            if (job.DatabaseEnvironmentId == null) return null;
            if (string.IsNullOrWhiteSpace(job.Cron)) return null;
            job.Id = NewId();
            RecurringJob.AddOrUpdate<JobBO.JobRunner>(job.Id.ToString(), (j) => j.Run(job), job.Cron, System.TimeZoneInfo.Local);
            return job;
        }

        public JobBO Update(JobBO job)
        {
            if (job == null) return null;
            if (job.DatabaseEnvironmentId == null) return null;
            if (string.IsNullOrWhiteSpace(job.Cron)) return null;
            if (job.Id < 1) return null;
            RecurringJob.AddOrUpdate<JobBO.JobRunner>(job.Id.ToString(), (j) => j.Run(job), job.Cron, System.TimeZoneInfo.Local);
            return job;
        }

        public void Delete(int id)
        {
            if (id < 1) return;
            RecurringJob.RemoveIfExists(id.ToString());
        }

        public void Enqueue(int id)
        {
            if (id < 1) return;
            JobBO job = Get(id);
            if (job == null) return;
            RecurringJob.Trigger(job.Id.ToString());
        }

        public void RunAndForget(JobBO job)
        {
            if (job == null) return;
            if (job.DatabaseEnvironmentId == null) return;
            BackgroundJob.Enqueue<JobBO.JobRunner>((jr) => jr.Run(job));
        }

    }
}
