import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { ApiService } from "../services/api.service";
import { Rule } from "../models/rule.model";
import { RuleFilter } from "../models/rule.model";
import { Category } from "../models/category.model";
import { ToastrService } from "ngx-toastr";
import { DatabaseEnvironment } from "../models/databaseEnvironment.model";
import { Tag } from "../models/tag.model";
import { User } from "../models/user.model";
import { UtilService } from '../services/util.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit {
  collections: Category[];
  categories: Category[];
  categoriesFilter: Category[];
  isRunningAll: boolean;
  collectionName: "";
  newCollection: Category = new Category();
  addCollectionObjModal: any;

  databaseEnvironments: DatabaseEnvironment[];
  selectedDatabaseEnvironment: DatabaseEnvironment;
  selectedCategory: Category;

  environmentTypes = [];
  listTags: Tag[];
  existChangesInRuleSearch: boolean;
  existChangesInDownloadCommunityCollection: boolean;

  destinationTypes = [];

  userSignIn: User;
  signInModal: any;
  uploadCollectionMessageModal: any;
  tokenInformation: any;
  allCommunityCollections: Category[];
  rulesFilter: RuleFilter;
  collectionToUpload: any;
  collectionSqlInjectModal: any;

  @ViewChild('uploader')
  myFileInput: ElementRef;

  constructor(
    private apiService: ApiService,
    private modalService: NgbModal,
    private toastr: ToastrService
  ) { }

  ngOnInit() {
    this.rulesFilter = new RuleFilter();
    this.rulesFilter.rulesError = true;
    this.rulesFilter.rulesSuccess = true;
    this.rulesFilter.rulesValidation = true;
    this.rulesFilter.rulesNotRan = true;

    // Loading all configured database environments
    this.apiService.databaseEnvironment.getDatabaseEnvironments().subscribe(databaseEnvironments => {
      this.databaseEnvironments = databaseEnvironments;
    });

    // Loading the left navigation Collections
    this.apiService.container.getAllCollections().subscribe(collections => {
      if (collections != null) {
        collections.forEach(rec => rec.isDefault = false);
        this.collections = collections;
      }
    });

    //Loading the environment types catalog
    this.apiService.catalog.getByType('EnvironmentType').subscribe(result => {
      this.environmentTypes = result;
    });

    //Loading the destination types catalog
    this.apiService.catalog.getByType('RuleDetailsDestinationType').subscribe(result => {
      this.destinationTypes = result;
    });
  }

  onAddingTag(tag: any) {
    let existTag = this.newCollection.tags.find(rec => rec.id == tag.id);
    if (existTag == null) {
      this.newCollection.tags.push(tag);
    }
  }

  onRemovingTag(tag: any) {
    this.newCollection.tags = this.newCollection.tags.filter(rec => rec.id != tag.id);
  }

  loadTags() {
    //Loading the tags list
    this.apiService.tag.getTags().subscribe(result => {
      this.listTags = result;
    });
  }

  getFormatDate(currentDate: Date) {
    let currentMonth = '0';
    if (currentDate.getMonth() + 1 >= 10)
      currentMonth = (currentDate.getMonth()+1).toString();
    else
      currentMonth += (currentDate.getMonth() + 1).toString();

    let currentDay = '0';
    if (currentDate.getDate() >= 10)
      currentDay = currentDate.getDate().toString();
    else
      currentDay += currentDate.getDate().toString();

    return currentMonth + '/' + currentDay + '/' + currentDate.getFullYear().toString() + ' ' + currentDate.getHours().toString() + ':' + currentDate.getMinutes().toString();
  }

  addCollection(content) {

    this.loadTags();
    this.newCollection = new Category();
    this.newCollection.name = '';
    this.newCollection.description = '';
    this.newCollection.tags = [];
    this.newCollection.ruleDetailsDestinationId = 0;
    this.newCollection.dateUpdated = new Date();
    this.newCollection.strDateUpdated = this.getFormatDate(this.newCollection.dateUpdated);

    this.addCollectionObjModal = this.modalService.open(content, { ariaLabelledBy: "modal-basic-title", backdrop: "static" });
    this.addCollectionObjModal.result.then(
      result => {

        if (this.newCollection.ruleDetailsDestinationId != undefined &&
          this.newCollection.ruleDetailsDestinationId != null &&
          this.newCollection.ruleDetailsDestinationId > 0)
          this.newCollection.ruleDetailsDestinationId = parseInt(this.newCollection.ruleDetailsDestinationId.toString());
        else
          this.newCollection.ruleDetailsDestinationId = null;

        this.newCollection.environmentType = parseInt(this.newCollection.environmentType.toString());
        this.apiService.container
          .addCollectionContainer(this.newCollection)
          .subscribe(result => {
            this.collections.unshift(result);
            this.toastr.success("Collection " + this.newCollection.name + " Added", "Success");
          });
      }, error => { });
  }

  changeCollection(category: Category) {
    this.selectedCategory = category;
    if (this.selectedDatabaseEnvironment != undefined) {
      this.apiService.container
        .getContainerByContainerIdAndDatabaseEnvironmentId(category.id, this.selectedDatabaseEnvironment.id)
        .subscribe(collection => {
          this.selectedCategory = collection;
          this.collectionName = collection.name;
          this.categories = collection.childContainers;
          this.filterRulesOfCategories();
        });
    }
    else {
      this.toastr.info("Select or create an Environment", "Information");
    }
  }

  executeAll() {
    this.apiService.databaseEnvironment.testDatabaseEnvironmentById(this.selectedDatabaseEnvironment).subscribe(isConnectedMessage => {
      if (isConnectedMessage == null || isConnectedMessage == '') {
        this.isRunningAll = true;
        this.categories.forEach(category => {
          category.rules.forEach(rec => rec.lastStatus = 0);
          if (category.rules.length > 0) {
            this.executeCategory(category);
          }
        });
      }
      else {
        this.toastr.error("Review the connection with your environment:" + isConnectedMessage, "Environment Connection Error");
      }
    });
  }

  executeCategory(category: Category) {
    if (category.rules.length > 0) {
      this.apiService.databaseEnvironment.testDatabaseEnvironmentById(this.selectedDatabaseEnvironment).subscribe(isConnectedMessage => {
        if (isConnectedMessage == null || isConnectedMessage == '') {
          category.isRunning = true;
          category.validRules = 0;
          category.invalidRules = 0;
          category.sintaxRules = 0;
          category.lastStatus = 0;
          category.showRules = true;
          category.rules.forEach(rule => {
            rule.isExecuting = true;
            this.apiService.rule
              .executeRule({ ruleId: rule.id, databaseEnvironmentId: this.selectedDatabaseEnvironment.id })
              .subscribe(
                result => {
                  rule.counter = result.result;
                  rule.isExecuting = false;
                  rule.lastStatus = result.status;
                  rule.lastExecution = new Date();
                  rule.results = result.testResults;
                  if (rule.lastStatus == 1)
                    category.validRules++
                  else if (rule.lastStatus == 3)
                    category.sintaxRules++
                  else
                    category.invalidRules++;
                  this.setContainerStatus(category);
                  this.filterRulesOfCategories();
                },
                error => {
                  rule.lastStatus = 0;
                  rule.lastExecution = new Date();
                  rule.isExecuting = false;
                  category.validRules++;
                  this.setContainerStatus(category);
                  this.filterRulesOfCategories();
                  throw (error);
                });
          });
        }
        else {
          this.toastr.error("Review the connection with your environment:" + isConnectedMessage, "Environment Connection Error");
        }
      });
    }
  }

  setContainerStatus(category: Category) {
    if (category.rules.length == (category.validRules + category.invalidRules + category.sintaxRules)) {
      category.isRunning = false;
      if (category.sintaxRules > 0)
        category.lastStatus = 3;
      else if (category.invalidRules > 0)
        category.lastStatus = 2;
      else
        category.lastStatus = 1;
      //set the status of the category after all rules ran, 1 success, 2 failed
      let remainingCategories = this.categories.filter(rec => rec.rules.length>0 && rec.lastStatus == 0);
      if (remainingCategories.length == 0) {
        this.isRunningAll = false;
      }
    }
  }

  selectCategoryFromChild(category: Category) {
    this.collections.forEach(m => (m.isDefault = false));
    category.isDefault = true;
    this.changeCollection(category);
  }

  deleteCollectionFromChild(element) {
    this.apiService.container.getAllCollections().subscribe(collections => {
      this.collections = collections;
      if (element.isDefault && this.collections.length > 0) {

        this.selectedCategory = null;
        this.collectionName = "";
        this.categories = [];
        this.filterRulesOfCategories();

        let existCategoryDefault = this.collections.find(rec => rec.isDefault);
        if (existCategoryDefault != null && existCategoryDefault != undefined) {
          this.collections.forEach(m => (m.isDefault = false));
          existCategoryDefault.isDefault = true;
          this.changeCollection(existCategoryDefault);
        }
      }
    });
  }

  updateDatabaseEnvironmentFromChild(databaseEnvironment: DatabaseEnvironment) {
    this.selectedDatabaseEnvironment = Object.assign({}, databaseEnvironment);
    if (this.selectedCategory != null) {
      this.apiService.container
        .getContainerByContainerIdAndDatabaseEnvironmentId(this.selectedCategory.id, databaseEnvironment.id)
        .subscribe(collection => {
          this.selectedCategory = collection;
          this.collectionName = collection.name;
          this.categories = collection.childContainers;
          this.filterRulesOfCategories();
        });
    }
  }

  updateRuleResultFromChild(ruleFromChild: any) {
    let container = this.selectedCategory.childContainers.find(rec => rec.id == ruleFromChild.containerId);
    if (container != null) {
      let rule = container.rules.find(rec => rec.id == ruleFromChild.id);
      if (rule != null) {
        rule.isExecuting = ruleFromChild.isExecuting;
        rule.lastStatus = ruleFromChild.lastStatus;
        rule.lastExecution = ruleFromChild.lastExecution;
        rule.results = ruleFromChild.results;
        let rulesInvalid = container.rules.filter(rec => rec.lastStatus == 2);
        let rulesError = container.rules.filter(rec => rec.lastStatus != 1 && rec.lastStatus != 2);
        if (rulesError.length > 0)
          container.lastStatus = 0;//0 is when not all rules have been ran
        else if (rulesInvalid.length > 0)
          container.lastStatus = 2;//2 is when exist at least one error
        else
          container.lastStatus = 1;//1 is when all rules ran properly
        container.validRules = rulesInvalid.length;
      }
    }
    this.filterRulesOfCategories();
  }

  showTags(contentTags:any) {
    this.modalService
      .open(contentTags, {
        backdrop: "static", ariaLabelledBy: "modal-basic-title", size: "xl"
      })
      .result.then(result => {}, error => { });
  }

  showSearchByTags(contentTagsSearch:any) {

    this.existChangesInRuleSearch = false;

    if (this.selectedDatabaseEnvironment == null || this.selectedDatabaseEnvironment == undefined) {
      this.toastr.error("Select an Environment first", "Error");
      return;
    }

    this.modalService.open(contentTagsSearch, {
      backdrop: "static",
      ariaLabelledBy: "modal-basic-title",
      size: "xl",
      windowClass: "modal-custom-xl"
    }).result.then(result => {
      this.reloadDataFromModal();
    }, error => {
      this.reloadDataFromModal();
    });
  }

  reloadDataFromModal() {
    if (this.existChangesInRuleSearch || this.existChangesInDownloadCommunityCollection) {
      this.apiService.container.getAllCollections().subscribe(collections => {
        this.collections = [];
        if (collections != null) {
          collections.forEach(rec => rec.isDefault = false);
          this.collections = collections;
        }
      });
      this.selectedCategory = null;
      this.collectionName = null;
      this.categories = [];
      this.filterRulesOfCategories();
    }
    this.existChangesInDownloadCommunityCollection = false;
    this.existChangesInRuleSearch = false;
  }

  notifyChangesFromRuleSearch(existChanges: boolean) {
    this.existChangesInRuleSearch = existChanges;
  }

  showJobs(contentJobs) {
    this.modalService
      .open(contentJobs, {
        backdrop: "static", ariaLabelledBy: "modal-basic-title", size: "xl"
      })
      .result.then(result => {}, error => { });
  }

  reloadDataFromChild() {
    this.apiService.container.getAllCollections().subscribe(collections => {
      this.collections = [];
      if (collections != null) {
        collections.forEach(rec => rec.isDefault = false);
        this.collections = collections;
      }
    });
    this.selectedCategory = null;
    this.collectionName = null;
    this.categories = [];
    this.filterRulesOfCategories();
  }

  //SignIn to repository

  showSignInToRepository(signInContent, downloadCommunityContent) {

    this.userSignIn = new User();
    this.userSignIn.email = '';
    this.userSignIn.password = '';
    this.userSignIn.server = UtilService.communityUrl();

    this.tokenInformation = UtilService.tokenInformation;
    if (this.tokenInformation == null) {
      this.signInModal = this.modalService.open(signInContent, {
        ariaLabelledBy: "modal-basic-title",
        backdrop: "static"
      });
    }
    else {
      this.apiService.community.isTokenValid(this.tokenInformation).subscribe(result => {
        this.showCommunityCollections(downloadCommunityContent);
      }, error => {
          UtilService.tokenInformation = null;
          this.signInModal = this.modalService.open(signInContent, {
            ariaLabelledBy: "modal-basic-title",
            backdrop: "static"
          });
      });
    }
  }

  signInToRepository(downloadCommunityContent) {
    UtilService.communityUrlUser = this.userSignIn.server;
    this.apiService.community.signInToCommunity(this.userSignIn).subscribe(result => {
      this.tokenInformation = result;
      UtilService.tokenInformation = this.tokenInformation;
      this.showCommunityCollections(downloadCommunityContent);
    }, error => {
      if (error.status == 401) {
        this.toastr.error("Email/Password incorrect.", "Sign In");
      }
      else {
        this.toastr.error("Please verify you have connection with the Community server.", "Sign In Error");
      }
    });
  }

  showCommunityCollections(downloadCommunityContent) {

    this.existChangesInDownloadCommunityCollection = false;

    this.apiService.community.getCommunityCollections(this.tokenInformation).subscribe(result => {

      if (result != null && result != undefined && result.length > 0) {
        result.forEach(rec => {
          rec.showDestinationStructure = false;
          if (rec.containerDestination != null && rec.containerDestination != undefined &&
            rec.containerDestination.destinationStructure != null && rec.containerDestination.destinationStructure != undefined) {
            rec.containerDestinationStructure = JSON.parse(rec.containerDestination.destinationStructure);
          }
        });
      }

      if (this.signInModal != null) {
        this.signInModal.close();
        this.signInModal = null;
      }
      this.allCommunityCollections = result;
      this.modalService.open(downloadCommunityContent, {
        backdrop: "static",
        ariaLabelledBy: "modal-basic-title",
        size: "xl",
        windowClass: "modal-custom-xl"
      }).result.then(result => {
        this.reloadDataFromModal();
      }, error => {
        this.reloadDataFromModal();
      });
    });
  }

  notifyDownloadedCollection(existChanges: boolean) {
    this.existChangesInDownloadCommunityCollection = existChanges;
  }

  //SignIn to repository

  filterRulesOfCategories() {

    this.categoriesFilter = [];

    for (let i = 0; i < this.categories.length; i++) {

      let rules: Rule[] = [];

      if (this.categories[i].rules != undefined && this.categories[i].rules != null && this.categories[i].rules.length > 0) {

        for (let j = 0; j < this.categories[i].rules.length; j++) {

          if (this.rulesFilter.rulesSuccess && this.categories[i].rules[j].lastStatus == 1)
            rules.push(this.categories[i].rules[j]);

          if (this.rulesFilter.rulesValidation && this.categories[i].rules[j].lastStatus == 2)
            rules.push(this.categories[i].rules[j]);

          if (this.rulesFilter.rulesError && this.categories[i].rules[j].lastStatus == 3)
            rules.push(this.categories[i].rules[j]);

          if (this.rulesFilter.rulesNotRan && this.categories[i].rules[j].lastStatus == 0)
            rules.push(this.categories[i].rules[j]);
        }

        if (rules.length > 0) {
          let newContainer: Category;
          newContainer = Object.assign({}, this.categories[i]);
          newContainer.showRules = true;
          newContainer.rules = rules;
          newContainer.validRules = rules.filter(rec => rec.lastStatus == 2).length;
          this.categoriesFilter.push(newContainer);
        }
      }
      else {
        let newContainer: Category;
        newContainer = Object.assign({}, this.categories[i]);
        this.categoriesFilter.push(newContainer);
      }
    }
  }

  expandAll() {
    if (this.categories.length > 0) {
      this.categories.forEach(rec => rec.showRules = true);
      this.categoriesFilter.forEach(rec => rec.showRules = true);
    }
  }

  collapseAll() {
    if (this.categories.length > 0) {
      this.categories.forEach(rec => rec.showRules = false);
      this.categoriesFilter.forEach(rec => rec.showRules = false);
    }
  }

  //upload Collection

  uploadFile($event:any, uploadCollectionExist:any, warningSqlInjectMessageContentHome:any) {

    this.collectionSqlInjectModal = null;
    this.uploadCollectionMessageModal = null;
    this.collectionToUpload = null;

    if ($event.target.files.length > 0) {
      if ($event.target.files[0].name.indexOf('.json') > 0) {
        const fileReader = new FileReader();
        fileReader.readAsText($event.target.files[0], "UTF-8");
        fileReader.onload = () => {
          let jsonToUpload = fileReader.result as string;
          let currentCollection = JSON.parse(jsonToUpload);
          this.collectionToUpload = currentCollection;
          if (this.collections != null && this.collections != undefined && this.collections.length > 0 &&
            this.collections.filter(rec => rec.name == currentCollection.Name).length > 0) {
            this.uploadCollectionMessageModal = this.modalService.open(uploadCollectionExist, {
              backdrop: "static",
              ariaLabelledBy: "modal-basic-title",
            });
          }
          else {
            this.uploadOverwriteCollection(warningSqlInjectMessageContentHome);
          }
        }
        fileReader.onerror = (error) => {
          console.log(error);
          this.toastr.error("Error uploading json file", "Error");
          this.myFileInput.nativeElement.value = '';
        }
      }
      else {
        this.toastr.error("You can only upload json files.", "Error");
      }
    }
  }

  uploadOverwriteCollection(warningSqlInjectMessageContentHome:any) {

    if (warningSqlInjectMessageContentHome != null) {
      for (let i = 0; i < this.collectionToUpload.Containers.length; i++) {
        let container = this.collectionToUpload.Containers[i];
        for (let j = 0; j < container.Rules.length; j++) {
          let rule = container.Rules[j];
          if (rule.Sql.toLowerCase().indexOf('insert') >= 0 || rule.Sql.toLowerCase().indexOf('update') >= 0
            || rule.Sql.toLowerCase().indexOf('delete') >= 0 || rule.Sql.toLowerCase().indexOf('drop') >= 0
            || rule.Sql.toLowerCase().indexOf('alter') >= 0 || rule.Sql.toLowerCase().indexOf('commit') >= 0) {

            this.collectionSqlInjectModal = this.modalService.open(warningSqlInjectMessageContentHome, {
              ariaLabelledBy: "modal-basic-title",
              backdrop: "static"
            });
            return;
          }
        }
      }
    }

    if (this.collectionSqlInjectModal != null)
      this.collectionSqlInjectModal.close();

    if (this.uploadCollectionMessageModal != null)
      this.uploadCollectionMessageModal.close();

    this.apiService.container.uploadContainerJson(this.collectionToUpload).subscribe(result => {
      if (result == null || result == '') {
        this.apiService.container.getAllCollections().subscribe(collections => {
          this.collections = [];
          if (collections != null) {
            collections.forEach(rec => rec.isDefault = false);
            this.collections = collections;
          }
        });
        this.selectedCategory = null;
        this.collectionName = null;
        this.categories = [];
        this.filterRulesOfCategories();
        this.toastr.success("Uploading Completed", "Success");
      }
      else {
        this.toastr.error("Error uploading json file:" + result, "Error");
      }
      this.myFileInput.nativeElement.value = '';
    });
  }

  cancelWarningSqlInjectHome() {
    if (this.collectionSqlInjectModal != null)
      this.collectionSqlInjectModal.close();

    if (this.uploadCollectionMessageModal != null)
      this.uploadCollectionMessageModal.close();

    this.myFileInput.nativeElement.value = '';
  }
}

function r(r: any, arg1: (any: any) => void, arg2: (error: any) => void) {
    throw new Error("Function not implemented.");
}
