import { Component, OnInit } from "@angular/core";
import { ApiService } from '../services/api.service';
import { ToastrService } from 'ngx-toastr';
import { Tag } from '../models/tag.model';
import { Category } from "../models/category.model";
import { Job } from "../models/job.model";
import { DatabaseEnvironment } from "../models/databaseEnvironment.model";
import { FormGroup } from '@angular/forms';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'jobs',
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.css']
})
export class JobsComponent implements OnInit {
  newJob = new Job();
  databaseEnvironments: DatabaseEnvironment[] = [];
  tags: Tag[] = [];
  jobs: Job[] = [];
  containers: Category[] = [];
  jobMessageModalObj: any;
  jobFormObj: FormGroup;

  constructor(
    private apiService: ApiService,
    private toastr: ToastrService,
    private modalService: NgbModal) {
  }

  ngOnInit() {
    this.setInitialValues();
    this.loadJobs();
    this.apiService.databaseEnvironment.getDatabaseEnvironments().subscribe(databaseEnvironments => {
      this.databaseEnvironments = databaseEnvironments;
    });
    this.apiService.tag.getTags().subscribe(result => {
      this.tags = result;
    });
    this.apiService.container.getAllCollections().subscribe(collections => {
      if (collections != null) {
        collections.forEach(rec => {
          let container = new Category();
          container.id = rec.id;
          container.name = rec.name;
          this.containers.push(container);
          if (rec.childContainers != null && rec.childContainers.length > 0) {
            rec.childContainers.forEach(rec2 => {
              let containerChild = new Category();
              containerChild.id = rec2.id;
              containerChild.name = rec.name + '/' + rec2.name;
              this.containers.push(containerChild);
            });
          }
        });
      }
    });
  }

  setInitialValues() {
    this.newJob = new Job();
    this.newJob.id = 0;
    this.newJob.type = 0;
  }

  loadJobs() {
    this.apiService.job.getJobs().subscribe(result => {
      this.jobs = result;
    });
  }

  getDatabaseEnvironmentName(job: Job)
  {
    var dbe = (this.databaseEnvironments||[]).find(d => d.id == job.databaseEnvironmentId);
    if(dbe)
    {
      return dbe.name;
    }
    return "";
  }

  getTargetName(job: Job)
  {
    if(!job) return null;
    if(job.type==1) {
      var tag = (this.tags||[]).find(d => d.id == job.tagId);
      if(tag) return tag.name;
    }
    if(job.type==2) {
      var container = (this.containers||[]).find(d => d.id == job.containerId);
      if(container) return container.name;
    }
    return null;
  }

  getTypeName(job: Job)
  {
    if(!job) return null;
    if(job.type==1) return "By Tag";
    if(job.type==2) return "By Container";
    return null;
  }

  saveJob(jobForm: FormGroup, jobMessageModal) {

    if (!jobForm.valid) {
      this.markFormGroupTouched(jobForm);
      return;
    }

    this.newJob.type = parseInt(this.newJob.type.toString());
    if(this.newJob.type == 1) this.newJob.containerId = null;
    if(this.newJob.type == 2) this.newJob.tagId = null;

    if(!this.newJob.cron) {
      this.apiService.job.runAndForget(this.newJob).subscribe(result => {
          jobForm.reset();
          this.setInitialValues();
          this.toastr.success('Job Launched', 'Success');
      });
      return;
    }

    if (this.newJob.type == 1)
      this.newJob.tagId = parseInt(this.newJob.tagId.toString());

    let existJobSameCron = null;
    if (this.jobs != null && this.jobs.length > 0) {
      existJobSameCron = this.jobs.find(rec => rec.cron == this.newJob.cron && rec.id != this.newJob.id);
    }

    this.jobFormObj = jobForm;

    if (existJobSameCron != null) {
      this.jobMessageModalObj = this.modalService.open(jobMessageModal, {
        ariaLabelledBy: "modal-basic-title",
        backdrop: "static"
      });
      return;
    }

    this.continueSaveJob();
  }

  continueSaveJob() {
    if (this.newJob.id == 0)
      this.apiService.job.addJob(this.newJob).subscribe(result => {
        if (result != null) {
          this.jobFormObj.reset();
          this.setInitialValues();
          this.loadJobs();
          this.toastr.success('Job created', 'Success');
        }
      });
    else
      this.apiService.job.modifyJob(this.newJob).subscribe(result => {
        this.jobFormObj.reset();
        this.setInitialValues();
        this.loadJobs();
        this.toastr.success('Job updated', 'Success');
      });

    if (this.jobMessageModalObj != null) {
      this.jobMessageModalObj.close();
      this.jobMessageModalObj = null;
    }
  }

  edit(job) {
    this.newJob = Object.assign({}, job);
  }

  enqueue(job)
  {
    this.apiService.job.enqueueJob(job.id).subscribe(result => {
        this.toastr.success('Job Enqueued', 'Success');
    });
  }

  delete(job) {
    this.apiService.job.deleteJob(job.id).subscribe(result => {
      this.setInitialValues();
      this.loadJobs();
      this.toastr.success('Job deleted', 'Success');
    });
  }

  cancelJob(jobForm: FormGroup) {
    jobForm.reset();
    this.setInitialValues();
  }

  showCronInfo()
  {
    window.open('https://crontab.guru/', '_blank').focus();
  }

  private markFormGroupTouched(form: FormGroup) {
    Object.values(form.controls).forEach(control => {
      control.markAsTouched();
      if ((control as any).controls) {
        this.markFormGroupTouched(control as FormGroup);
      }
    });
  }
}
