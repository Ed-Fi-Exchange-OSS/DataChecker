import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ApiService } from '../services/api.service';
import { DatabaseEnvironment } from '../models/databaseEnvironment.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { UserParam } from '../models/userParam.model';

@Component({
  selector: 'database-environment',
  templateUrl: './database-environment.component.html',
  styleUrls: ['./database-environment.component.css']
})
export class DatabaseEnvironmentComponent implements OnInit {

  @Input() databaseEnvironments: DatabaseEnvironment[];
  @Input() selectedDatabaseEnvironment: DatabaseEnvironment = new DatabaseEnvironment();

  @Output() updateDatabaseEnvironmentFromChild = new EventEmitter<DatabaseEnvironment>();

  titleEnvironment = "Add enviroment"
  newDatabaseEnvironment = new DatabaseEnvironment();
  userParams: UserParam[] = new Array<UserParam>();

  environmentTypes = [];
  maxNumberResultsDefault: number;

  constructor(private apiService: ApiService,
    private modalService: NgbModal,
    private toastr: ToastrService) {
  }

  ngOnInit() {
    this.apiService.catalog.getByType('EnvironmentType').subscribe(result => {
      this.environmentTypes = result;
    });

    this.apiService.databaseEnvironment.getMaxNumberResults().subscribe(result => {
      this.maxNumberResultsDefault = result;
    });
  }

  addEnvironment(environmentContent) {
    this.newDatabaseEnvironment = new DatabaseEnvironment();
    this.newDatabaseEnvironment.securityIntegrated = false;
    this.newDatabaseEnvironment.id = '00000000-0000-0000-0000-000000000000';
    this.newDatabaseEnvironment.password = '';
    this.userParams = [];
    this.newDatabaseEnvironment.maxNumberResults = this.maxNumberResultsDefault;

    this.modalService
      .open(environmentContent, {
        backdrop: "static", ariaLabelledBy: "modal-basic-title", size: "xl"
      })
      .result.then(
        result => {
          this.newDatabaseEnvironment.version = parseInt(this.newDatabaseEnvironment.version.toString());
          this.newDatabaseEnvironment.maxNumberResults = parseInt(this.newDatabaseEnvironment.maxNumberResults.toString());
          this.apiService.databaseEnvironment
            .addDatabaseEnvironment(this.newDatabaseEnvironment)
            .subscribe(resultEnvironment => {
              this.databaseEnvironments.push(resultEnvironment);
              this.changeDatabaseEnvironment(resultEnvironment);
              this.apiService.userParam.updateUserParams(this.userParams, resultEnvironment.id).subscribe(userParamsResult => {
                resultEnvironment.userParams = userParamsResult;
                this.toastr.success("Environment  " + this.newDatabaseEnvironment.name + " Added", "Success");
              });
            });
        }, error => { });
  }

  updateDatabaseEnvironmentModal(environmentContent) {
    this.titleEnvironment = "Update environment";
    this.newDatabaseEnvironment = Object.assign({}, this.selectedDatabaseEnvironment);
    this.userParams = Object.assign([], this.selectedDatabaseEnvironment.userParams);

    this.modalService
      .open(environmentContent, { ariaLabelledBy: "modal-basic-title", size: "xl", backdrop: "static" })
      .result.then(
        result => {
          this.newDatabaseEnvironment.version = parseInt(this.newDatabaseEnvironment.version.toString());
          this.newDatabaseEnvironment.maxNumberResults = parseInt(this.newDatabaseEnvironment.maxNumberResults.toString());
          this.apiService.databaseEnvironment
            .updateDatabaseEnvironment(this.newDatabaseEnvironment)
            .subscribe(result => {
              this.selectedDatabaseEnvironment = Object.assign({}, this.newDatabaseEnvironment);
              this.apiService.userParam.updateUserParams(this.userParams, this.newDatabaseEnvironment.id).subscribe(m => {
                this.selectedDatabaseEnvironment.userParams = Object.assign([], this.userParams);
                this.databaseEnvironments.forEach((rec,index) => {
                  if (rec.id == this.selectedDatabaseEnvironment.id) {
                    this.databaseEnvironments[index] = Object.assign({}, this.selectedDatabaseEnvironment);
                  }
                });
              });
              this.toastr.success("Environment " + this.newDatabaseEnvironment.name + " updated","Success");
            });
        }, error => { });
  }

  deleteDatabaseEnvironment() {
    this.apiService.databaseEnvironment
      .deleteDatabaseEnvironment(this.selectedDatabaseEnvironment)
      .subscribe(result => {
        var index = this.databaseEnvironments.indexOf(this.selectedDatabaseEnvironment);
        this.databaseEnvironments.splice(index, 1);
        this.changeDatabaseEnvironment(this.databaseEnvironments[0]);
        this.toastr.success("Environment " + this.newDatabaseEnvironment.name + " deleted","Success");
      });
  }

  duplicateDatabaseEnvironment() {

    this.userParams = Object.assign([], this.selectedDatabaseEnvironment.userParams);
    this.apiService.databaseEnvironment
      .duplicateDatabaseEnvironment(this.selectedDatabaseEnvironment)
      .subscribe(result => {

        this.toastr.success("Environment " + this.selectedDatabaseEnvironment.name + " duplicated","Success");

        this.databaseEnvironments.push(result);
        this.changeDatabaseEnvironment(result);
        this.apiService.userParam.getUserParams(result.id).subscribe(resultParams => {
          this.userParams = Object.assign([], resultParams);
        });
        
      });
  }

  changeDatabaseEnvironment(dbEnvItem: DatabaseEnvironment) {
    this.selectedDatabaseEnvironment = dbEnvItem;
    this.updateDatabaseEnvironmentFromChild.emit(this.selectedDatabaseEnvironment);
  } 

  onChangeSecurityIntegrated(e) {
    if (this.newDatabaseEnvironment.securityIntegrated) {
      this.newDatabaseEnvironment.user = '';
      this.newDatabaseEnvironment.password = '';
    }
  }

  testConnection() {

    if (this.newDatabaseEnvironment.version != undefined && this.newDatabaseEnvironment.version != null)
      this.newDatabaseEnvironment.version = parseInt(this.newDatabaseEnvironment.version.toString());

    if (this.newDatabaseEnvironment.maxNumberResults != undefined && this.newDatabaseEnvironment.maxNumberResults != null)
      this.newDatabaseEnvironment.maxNumberResults = parseInt(this.newDatabaseEnvironment.maxNumberResults.toString());

    this.apiService.databaseEnvironment.testDatabaseEnvironment(this.newDatabaseEnvironment).subscribe(result => {
      if (!result.isValid) {
        this.toastr.error("Connection error: " + result.message, "Error");
      }
      else {
        this.toastr.success("Connection succeeded!", "Success");
      }
    });
  }

  getPasswordWithFormat() {
    if (this.newDatabaseEnvironment != null && this.newDatabaseEnvironment != undefined
      && this.newDatabaseEnvironment.password != null && this.newDatabaseEnvironment.password != undefined) {
      return "".padStart(this.newDatabaseEnvironment.password.length, "*");
    }
    return "";
  }

  //Pop Up parameters functions

  loadParams() {
    this.userParams = new Array<UserParam>();
    if (this.selectedDatabaseEnvironment.userParams != null && this.selectedDatabaseEnvironment.userParams != undefined) {
      this.userParams = Object.assign([], this.selectedDatabaseEnvironment.userParams);
      if (this.userParams != null)
        this.userParams.forEach(rec => rec.isReadMode = true);
    }
  }

  addParameter() {
    this.userParams.push(new UserParam());
  }

  removeUserParam(userParam: UserParam) {
    var index = this.userParams.indexOf(userParam);
    this.userParams.splice(index, 1);
  }

  saveUserParams() {
    this.apiService
      .userParam
      .updateUserParams(this.userParams, this.selectedDatabaseEnvironment.id)
      .subscribe(result => {
        this.selectedDatabaseEnvironment.userParams = result;
    });
  }
}


