import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { Category } from "../models/category.model";
import { ToastrService } from "ngx-toastr";
import { ApiService } from "../services/api.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Rule } from "../models/rule.model";
import sqlFormatter from "sql-formatter";
import { DatabaseEnvironment } from "../models/databaseEnvironment.model";
import { Tag } from "../models/tag.model";
import { User } from "../models/user.model";
import { UtilService } from '../services/util.service';

@Component({
  selector: "nav-collection",
  templateUrl: "./nav-collection.component.html",
  styleUrls: ["./nav-collection.component.css"]
})
export class NavCollectionComponent implements OnInit {

  @Input() selectedDatabaseEnvironment: DatabaseEnvironment = new DatabaseEnvironment();
  @Input() categorySelected: Category = new Category();
  @Input() category: Category = new Category();
  @Input() collections: Category[];

  @Output() selectCategoryFromChild = new EventEmitter<Category>();
  @Output() deleteCollectionFromChild = new EventEmitter<Category>();
  @Output() reloadDataFromChild = new EventEmitter<boolean>();

  newCollection: Category = new Category();
  newRule: Rule = new Rule();
  oldRule: Rule = new Rule();
  modalContainerDescription = "Update Collection";
  modalRuleDescription = "Add Sql Data Check";

  runSqlTest: boolean;
  messageSqlTest: string;
  ruleIDIsValid: boolean;
  ruleModal: any;
  ruleWarningModal: any;
  ruleCategoryChild: Category;

  sqlOptions = {
    lineNumbers: true, theme: 'material', mode: 'sql', lineWrapping: 'true', smartIndent: true,
    extraKeys: { 'Ctrl-Space': 'autocomplete' },
    hint: "CodeMirror.hint.sql",
    hintOptions:
    {
      tables: {
        users: ["name", "score", "birthDate"],
        users2: ["name", "score", "birthDate"],
        countries: ["name", "population", "size"]
      }
    }
    , viewportMargin: Infinity
  };

  environmentTypes = [];
  destinationTypes = [];
  listTags: Tag[];

  listChildContainers: Category[];
  copyRulesToModal: any;
  allContainersSelected: boolean;

  ruleDetailsLogResult: any;
  userSignIn: User;
  signInModal: any;
  uploadCollectionMessageModal: any;
  tokenInformation: any;

  deleteCollectionContainerMessage: string;

  ruleIncreaseVersionModal: any;
  moveRuleModal: any;
  containerIdSelectedToMove: Category;
  ruleToMove: Rule;
  containerSelectedToMove: string;

  constructor(
    private apiService: ApiService,
    private modalService: NgbModal,
    private toastr: ToastrService
  ) { }

  ngOnInit() {
    this.category.amountRules = 0;
    if (this.category.childContainers != null) {
      this.category.childContainers.forEach(rec => (this.category.amountRules += rec.amountRules));
    }

    //Loading the environment types catalog
    this.apiService.catalog.getByType('EnvironmentType').subscribe(result => {
      this.environmentTypes = result;
    });

    //Loading the destination types catalog
    this.apiService.catalog.getByType('RuleDetailsDestinationType').subscribe(result => {
      this.destinationTypes = result;
    });

    this.loadTags();
  }

  loadTags() {
    //Loading the tags list
    this.apiService.tag.getTags().subscribe(result => {
      this.listTags = result;
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

  updateCollection(content) {

    this.loadTags();

    this.modalContainerDescription = "Update Collection";
    this.newCollection = Object.assign({}, this.category);
    this.newCollection.tags = Object.assign([], this.category.tags);

    if (this.newCollection.ruleDetailsDestinationId == undefined || this.newCollection.ruleDetailsDestinationId == null)
      this.newCollection.ruleDetailsDestinationId = 0;

    this.modalService
      .open(content, { ariaLabelledBy: "modal-basic-title", backdrop: "static" })
      .result.then(
        result => {

          if (this.newCollection.containerTypeId == 1) {
            if (this.newCollection.ruleDetailsDestinationId != undefined &&
              this.newCollection.ruleDetailsDestinationId != null &&
              this.newCollection.ruleDetailsDestinationId > 0)
              this.newCollection.ruleDetailsDestinationId = parseInt(this.newCollection.ruleDetailsDestinationId.toString());
            else
              this.newCollection.ruleDetailsDestinationId = null;
          }

          this.newCollection.environmentType = parseInt(this.newCollection.environmentType.toString());
          this.apiService.container
            .updateContainer(this.newCollection)
            .subscribe(localUser => {
              this.category.name = this.newCollection.name;
              this.category.description = this.newCollection.description;
              this.category.environmentType = this.newCollection.environmentType;
              this.category.ruleDetailsDestinationId = this.newCollection.ruleDetailsDestinationId;
              this.category.tags = Object.assign([], this.newCollection.tags);
              if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
              this.toastr.success("Collection " + this.newCollection.name + " updated", "Success");
            });
        }, error => { });
  }

  setDeleteCollectionMessage() {
    this.deleteCollectionContainerMessage = '';
    if (this.category.amountRules > 0 || (this.category.childContainers != undefined && this.category.childContainers.length > 0)) {

      let totalContainers = this.category.childContainers.length;
      let totalRules = 0;

      this.category.childContainers.forEach(rec => totalRules += rec.rules.length);
      this.deleteCollectionContainerMessage = "Are you sure to delete " + totalContainers + " " + (totalContainers == 1 ? "container" : "containers");

      if (totalRules > 0)
        this.deleteCollectionContainerMessage += " and " + totalRules + " " + (totalRules == 1 ? "rule" : "rules");
      this.deleteCollectionContainerMessage +=  "?";
    }
  }

  setDeleteContainerMessage(categoryChild: Category) {
    this.deleteCollectionContainerMessage = '';
    if (categoryChild.amountRules > 0 || categoryChild.rules != undefined || categoryChild.rules != null) {
      let totalRules = categoryChild.rules.length;
      if (totalRules > 0) {
        this.deleteCollectionContainerMessage += "Are you sure to delete " + totalRules + " " + (totalRules == 1 ? "rule" : "rules");
        this.deleteCollectionContainerMessage += "?";
      }
    }
  }

  deleteCollection() {
    this.apiService.container.deleteContainer(this.category.id).subscribe(rec => {
      if (this.category.isDefault) this.deleteCollectionFromChild.emit(this.category);
      else {
        var index = this.collections.indexOf(this.category);
        this.category.childContainers.splice(index, 1);
      }
      this.toastr.success("Container " + this.category.name + " deleted", "Success");
      this.deleteCollectionFromChild.emit(this.category);
    });
  }

  addContainer(content) {

    this.loadTags();

    this.modalContainerDescription = "Add Container";
    this.newCollection = new Category();
    this.newCollection.parentContainerId = this.category.id;
    this.newCollection.containerTypeId = 2;
    this.newCollection.tags = [];

    this.modalService
      .open(content, { ariaLabelledBy: "modal-basic-title", backdrop: "static" })
      .result.then(
        result => {
          this.apiService.container
            .addContainer(this.newCollection)
            .subscribe(addedContainer => {
              this.category.childContainers.push(Object.assign({}, addedContainer));
              if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
              this.toastr.success("Container " + addedContainer.name + " added", "Success");
            });
        }, error => { });
  }

  updateContainer(content, categoryChild) {

    this.loadTags();

    this.modalContainerDescription = "Update Container";
    this.newCollection = Object.assign({}, categoryChild);
    this.newCollection.tags = Object.assign([], categoryChild.tags);
    this.newCollection.ruleDetailsDestinationId = null;

    this.modalService
      .open(content, { ariaLabelledBy: "modal-basic-title", backdrop: "static" })
      .result.then(
        result => {
          this.apiService.container
            .updateContainer(this.newCollection)
            .subscribe(localUser => {
              categoryChild.name = this.newCollection.name;
              categoryChild.description = this.newCollection.description;
              categoryChild.tags = Object.assign([], this.newCollection.tags);
              if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
              this.toastr.success("Collection " + this.newCollection.name + " updated", "Success");
            });
        }, error => { });
  }

  deleteContainer(categoryChild: Category) {
    this.apiService.container.deleteContainer(categoryChild.id).subscribe(result => {
      var index = this.category.childContainers.indexOf(categoryChild);
      this.category.childContainers.splice(index, 1);
      this.setCategoryCounters();
      if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
      this.toastr.success("Container " + categoryChild.name + " deleted", "Success");
    });
  }

  setCategoryCounters() {
    this.category.amountRules = 0;
    if (this.category.childContainers != null) {
      this.category.childContainers.forEach(rec => {
        rec.amountRules = rec.rules.length;
        this.category.amountRules += rec.amountRules;
      });
    }
  }

  setDefault() {
    this.apiService.container.setDefaultContainer(this.category).subscribe(rec => {
      this.toastr.success("Collection " + this.category.name + " set to default", "Success");
    });
  }

  setEnvironmentTypeFromCollection() {
    if (this.category.environmentType != undefined && this.category.environmentType != null && this.category.environmentType > 0) {
      let existEnvironment = this.environmentTypes.find(rec => rec.id == this.category.environmentType);
      if (existEnvironment) {
        this.newRule.environmentTypeText = existEnvironment.name;
      }
    }
  }

  validateSqlDiagnostic(): boolean {

    let result = true;
    if (this.newRule.diagnosticSql != undefined &&
      this.newRule.diagnosticSql != null &&
      this.newRule.diagnosticSql != '') {

      result = false;
      let sqlDiagnostic = this.newRule.diagnosticSql.toLowerCase();
      if (sqlDiagnostic.indexOf('select') == 0) {
        let sqlSplit = sqlDiagnostic.split('from');
        //console.log(sqlSplit);
        if (sqlSplit.length > 1) {
          if (sqlSplit[0].indexOf('educationorganizationid') > 0) {
            result = true;
          }
          else {
            this.toastr.error('The Diagnostic Sql should have the column EducationOrganizationId', 'Error Diagnostic Sql');
          }
        }
        else {
          this.toastr.error('The Diagnostic Sql should have the FROM statement', 'Error Diagnostic Sql');
        }
      }
      else {
        this.toastr.error('The Diagnostic Sql should start with the SELECT statement', 'Error Diagnostic Sql');
      }
    }

    return result;
  }

  addRule(ruleContent, categoryChild: Category) {

    this.loadTags();
    this.runSqlTest = false;

    if (this.selectedDatabaseEnvironment == null || this.selectedDatabaseEnvironment == undefined) {
      this.toastr.error("Select an Environment first", "Error");
      return;
    }

    this.apiService.databaseEnvironment.getDatabaseInfo(this.selectedDatabaseEnvironment).subscribe(result => {
      if (result.mapTables != null && result.mapTables != '')
        this.sqlOptions.hintOptions.tables = JSON.parse(result.mapTables);
    });

    this.modalRuleDescription = "Add Sql Data Check";
    this.newRule = new Rule();
    this.newRule.id = '00000000-0000-0000-0000-000000000000';
    this.newRule.version = '1';
    this.newRule.collectionName = this.category.name;
    this.newRule.containerName = categoryChild.name;
    this.newRule.containerId = categoryChild.id;
    this.newRule.tags = [];
    this.newRule.diagnosticSql = '';
    this.setEnvironmentTypeFromCollection();

    this.ruleCategoryChild = categoryChild;
    this.oldRule = null;
    this.oldRule = Object.assign({}, this.newRule);

    this.newRule.idLastRuleExecutionLog = 0;

    this.ruleModal = null;
    this.ruleModal = this.modalService.open(ruleContent, {
      ariaLabelledBy: "modal-basic-title",
      size: "xl",
      windowClass: "modal-custom-xl",
      backdrop: "static"
    });
  }

  modifyRule(ruleContent, ruleChild: Rule, categoryChild: Category) {

    this.loadTags();
    this.runSqlTest = false;

    if (this.selectedDatabaseEnvironment == null || this.selectedDatabaseEnvironment == undefined) {
      this.toastr.error("Select an Environment first", "Error");
      return;
    }

    this.apiService.databaseEnvironment.getDatabaseInfo(this.selectedDatabaseEnvironment).subscribe(result => {
      if (result.mapTables != null && result.mapTables != '')
        this.sqlOptions.hintOptions.tables = JSON.parse(result.mapTables);
    });

    ruleChild.diagnosticSql = sqlFormatter.format(ruleChild.diagnosticSql);
    ruleChild.collectionName = this.category.name;
    ruleChild.containerName = categoryChild.name;

    this.modalRuleDescription = "Modify Sql Data Check";
    this.newRule = Object.assign({}, ruleChild);
    this.newRule.containerId = categoryChild.id;
    this.setEnvironmentTypeFromCollection();
    this.newRule.tags = Object.assign([], ruleChild.tags);
    this.newRule.idLastRuleExecutionLog = 0;

    this.oldRule = Object.assign({}, ruleChild);
    this.oldRule.tags = Object.assign([], ruleChild.tags);
    this.oldRule.containerId = this.newRule.containerId;

    this.ruleCategoryChild = categoryChild;

    this.apiService.ruleExecutionLogDetail
      .getLastRuleExecutionLogByEnvironmentAndRuleAsync(this.selectedDatabaseEnvironment.id, this.newRule.id)
      .subscribe(result => {
        this.newRule.idLastRuleExecutionLog = result;
      });

    this.ruleModal = null;
    this.ruleModal = this.modalService.open(ruleContent, {
      ariaLabelledBy: "modal-basic-title",
      size: "xl",
      windowClass: "modal-custom-xl",
      backdrop: "static"
    });
  }

  validateIncreaseVersion(increaseVersionNumberMessageContent) {
    if (this.newRule.id == '00000000-0000-0000-0000-000000000000' || this.newRule.version != this.oldRule.version) {
      this.saveRule();
      return;
    }

    this.ruleIncreaseVersionModal = this.modalService.open(increaseVersionNumberMessageContent, {
      ariaLabelledBy: "modal-basic-title",
      backdrop: "static",
      centered: true
    });
  }

  increaseRuleVersion() {
    this.newRule.version = (parseInt(this.newRule.version) + 1).toString();
    this.saveRule();
  }

  saveRule() {
    let newTags = 0;

    this.newRule.errorSeverityLevel = parseInt(this.newRule.errorSeverityLevel.toString());

    if (this.newRule.maxNumberResults != undefined && this.newRule.maxNumberResults != null)
      this.newRule.maxNumberResults = parseInt(this.newRule.maxNumberResults.toString());

    if (this.newRule.tags != undefined && this.newRule.tags != null && this.newRule.tags.length > 0) {
      this.newRule.tags.forEach(rec => {
        if (rec.name == rec.id.toString()) {
          rec.id = -1;
          newTags++;
        }
      });
    }

    if (this.modalRuleDescription == 'Add Sql Data Check') {
      this.apiService.rule.addRule(this.newRule).subscribe(data => {
        this.ruleModal.close();
        if (this.ruleWarningModal != null)
          this.ruleWarningModal.close();
        this.ruleCategoryChild.rules.push(data);
        this.setCategoryCounters();
        if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
        this.toastr.success("Sql Data Check " + this.newRule.name + " added ", "Success");
        if (newTags > 0) {
          this.loadTags();
          this.toastr.success("You have created " + newTags + " new tags.", "Success");
        }
      });
    }
    else {

      if (this.ruleIncreaseVersionModal != null) {
        this.ruleIncreaseVersionModal.close();
        this.ruleIncreaseVersionModal = null;
      }

      this.apiService.rule.modifyRule(this.newRule).subscribe(data => {
        this.ruleModal.close();
        if (this.ruleWarningModal != null)
          this.ruleWarningModal.close();
        this.ruleCategoryChild.rules.forEach((rec, index) => {
          if (rec.id == this.newRule.id) {
            this.ruleCategoryChild.rules[index] = Object.assign({}, this.newRule);
          }
        });
        if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
        this.toastr.success("Rule " + this.newRule.name + " modified ", "Success");
        if (newTags > 0) {
          this.loadTags();
          this.toastr.success("You have created " + newTags + " new tags.", "Success");
        }
      });
    }
  }

  cancelRule(forceClose, ruleWarningContent) {
    if (forceClose) {
      if (this.ruleWarningModal != null)
        this.ruleWarningModal.close();
      this.ruleModal.close();
    }
    else {
      if (this.oldRule != null) {
        this.ruleWarningModal = null;
        let countDifferences = 0;
        let existDifferences = UtilService.compareObj(this.newRule, this.oldRule);
        for (let key in existDifferences) {
          if (key != 'environmentTypeText' && key != 'id' && key != 'idLastRuleExecutionLog') {
            countDifferences++;
          }
        }

        if (countDifferences > 0) {
          this.ruleWarningModal = this.modalService.open(ruleWarningContent, {
            ariaLabelledBy: "modal-basic-title",
            size: "sm",
            backdrop: "static",
            centered: true
          });
        }
        else {
          this.ruleModal.close();
        }
      }
      else {
        this.ruleModal.close();
      }
    }
  }

  onAddingRuleTag(tag: any) {
    let existTag = this.newRule.tags.find(rec => rec.id == tag.id);
    if (existTag == null) {
      this.newRule.tags.push(tag);
    }
  }

  onRemovingRuleTag(tag: any) {
    this.newRule.tags = this.newRule.tags.filter(rec => rec.id != tag.id);
  }

  deleteRuleChild(ruleChild: Rule, categoryChild: Category) {
    this.apiService.rule.deleteRule(ruleChild.id).subscribe(result => {
      var index = categoryChild.rules.indexOf(ruleChild);
      categoryChild.rules.splice(index, 1);
      this.setCategoryCounters();
      if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
      this.toastr.success("Sql Data Check " + ruleChild.name + " deleted", "Success");
    });
  }

  getSeverityLevel(valueSelected: any) {
    switch (valueSelected) {
      case 1:
      case "1":
        return "Warning";
      case 2:
      case "2":
        return "Major";
      case 3:
      case "3":
        return "Critical";
    }
    return "";
  }

  runSqlRule(runType: string) {
    this.runSqlTest = false;
    this.messageSqlTest = '';
    if (runType == 'sql') {

      if (this.newRule.diagnosticSql == '') {
        return;
      }

      this.apiService.rule.executeRuleTest({ diagnosticSql: this.newRule.diagnosticSql }, this.selectedDatabaseEnvironment.id).subscribe(result => {
        if (result.result >= 0) {
          this.messageSqlTest = 'Counts returned: ' + result.result;
        }
        else {
          this.messageSqlTest = 'Error: ' + result.errorMessage;
        }
        this.runSqlTest = true;
      });
    }
  }

  copyRule(ruleChild: Rule, categoryChild: Category) {
    this.apiService.rule.copyRuleTo({
      ruleId: ruleChild.id,
      containerId: categoryChild.id
    }).subscribe(result => {
      categoryChild.rules.push(result);
      this.setCategoryCounters();
      if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
      this.toastr.success("Sql Data Check " + ruleChild.name + " copied", "Success");
    });
  }

  //Rule Edit/Copy and delete actions

  copyRuleSelected(ruleCopyRulesToContent) {
    this.allContainersSelected = false;
    this.listChildContainers = [];
    this.apiService.container.getChildContainers().subscribe(result => {
      this.listChildContainers = result;
      this.copyRulesToModal = this.modalService.open(ruleCopyRulesToContent, {
        ariaLabelledBy: "modal-basic-title",
        size: "lg",
        backdrop: "static"
      });
    });
  }

  deleteRuleSelected() {
    this.apiService.rule.deleteRule(this.newRule.id).subscribe(result => {
      var index = this.ruleCategoryChild.rules.indexOf(this.newRule);
      this.ruleCategoryChild.rules.splice(index, 1);
      this.setCategoryCounters();
      if (this.category.isDefault) this.selectCategoryFromChild.emit(this.category);
      this.toastr.success("Sql Data Check " + this.newRule.name + " deleted", "Success");
      this.ruleModal.close();
    });
  }

  allContainersSelectedChange(event: any) {
    if (this.listChildContainers.length > 0) {
      this.listChildContainers.forEach(rec => rec.selected = this.allContainersSelected);
    }
  }

  saveCopyRulesTo() {
    let containersSelected = this.listChildContainers.filter(rec => rec.selected);
    if (containersSelected.length == 0) {
      this.toastr.error("Select one container at least.", "Copy Rules To");
      return;
    }
    let rulesId = [];
    let containersId = [];
    rulesId.push(this.newRule.id);
    containersSelected.forEach(rec => containersId.push(rec.id));
    this.apiService.rule.copyRulesToContainers({ ruleIds: rulesId, containerIds: containersId }).subscribe(result => {
      this.toastr.success("Rule copied successfully.", "Copy Rules To");
      this.copyRulesToModal.close();
      this.reloadDataFromChild.emit(true);
      this.ruleModal.close();
    });
  }

  //Rule Edit/Copy and delete actions

  //View RuleExecutionLogDetails

  viewRuleExecutionLogDetailsFromOptions(ruleExecutionLogDetailRuleEditContent, ruleChild: Rule) {

    if (this.selectedDatabaseEnvironment == null || this.selectedDatabaseEnvironment == undefined) {
      this.toastr.error("Select an Environment first", "Error");
      return;
    }

    this.ruleDetailsLogResult = null;
    let idExecutionLog = 0;

    this.apiService.ruleExecutionLogDetail
      .getLastRuleExecutionLogByEnvironmentAndRuleAsync(this.selectedDatabaseEnvironment.id, ruleChild.id)
      .subscribe(idResult => {
        idExecutionLog = idResult;
        if (idExecutionLog > 0) {
          this.apiService.ruleExecutionLogDetail.getByRuleExecutionLogAsync(idExecutionLog).subscribe(
            result => {
              if (result != null) {
                this.ruleDetailsLogResult = result;
                this.modalService.open(ruleExecutionLogDetailRuleEditContent, {
                  ariaLabelledBy: "modal-basic-title",
                  size: "xl",
                  windowClass: "modal-custom-xl",
                  backdrop: "static"
                });
              }
              else {
                this.toastr.info("No execution log details for rule " + ruleChild.name, "Information");
              }
            });
        }
        else {
          this.toastr.info("No execution log details for rule " + ruleChild.name, "Information");
        }        
      });
  }

  viewRuleExecutionLogDetails(ruleExecutionLogDetailRuleEditContent) {
    this.ruleDetailsLogResult = null;
    let idExecutionLog = this.newRule.idLastRuleExecutionLog;
    this.apiService.ruleExecutionLogDetail.getByRuleExecutionLogAsync(idExecutionLog).subscribe(
      result => {
        this.ruleDetailsLogResult = result;
        this.modalService.open(ruleExecutionLogDetailRuleEditContent, {
          ariaLabelledBy: "modal-basic-title",
          size: "xl",
          windowClass: "modal-custom-xl",
          backdrop: "static"
        });
      });
  }

  exportToCsv() {
    if (this.ruleDetailsLogResult != null && this.ruleDetailsLogResult != undefined && this.ruleDetailsLogResult.ruleExecutionLogId > 0) {
      let url = this.apiService.ruleExecutionLogDetail.exportToCsvByRuleExecutionLogAsync(this.ruleDetailsLogResult.ruleExecutionLogId);
      window.open(url, "_blank");
    }
  }

  exportToTable() {
    if (this.ruleDetailsLogResult != null && this.ruleDetailsLogResult != undefined && this.ruleDetailsLogResult.ruleExecutionLogId > 0) {
      this.apiService.ruleExecutionLogDetail.exportToTableByRuleExecutionLogAsync(this.ruleDetailsLogResult.ruleExecutionLogId).subscribe(result => {
        if (result.alreadyExist) {
          this.toastr.success("Rule execution log details already exported to table " + result.tableName, "Export to Table");
        }
        else if (result.created) {
          this.toastr.success("Rule execution log details exported to table " + result.tableName, "Export to Table");
        }
      });
    }
  }

  //View RuleExecutionLogDetails

  //SignIn to repository

  showSignInToRepository(signInContent, uploadCollectionMessageContent) {

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
        this.uploadCollection(uploadCollectionMessageContent);
      }, error => {
        UtilService.tokenInformation = null;
        this.signInModal = this.modalService.open(signInContent, {
          ariaLabelledBy: "modal-basic-title",
          backdrop: "static"
        });
      });
    }
  }

  signInToRepository(uploadCollectionMessageContent) {
    UtilService.communityUrlUser = this.userSignIn.server;
    this.apiService.community.signInToCommunity(this.userSignIn).subscribe(result => {
      this.tokenInformation = result;
      UtilService.tokenInformation = this.tokenInformation;
      this.uploadCollection(uploadCollectionMessageContent);
    }, error => {
        if (error.status == 401) {
          this.toastr.error("Email/Password incorrect.", "Sign In");
        }
        else {
          this.toastr.error("Please verify you have connection with the Community server.", "Sign In Error");
        }        
    });
  }

  uploadCollection(uploadCollectionMessageContent) {
    this.apiService.community.getCollectionByName(this.category, this.tokenInformation).subscribe(result => {
      if (result == null) {
        this.continueUploadCollection();
      }
      else if (result.canUpload) {
        if (this.signInModal != null) {
          this.signInModal.close();
          this.signInModal = null;
        }
        this.uploadCollectionMessageModal = this.modalService.open(uploadCollectionMessageContent, {
          ariaLabelledBy: "modal-basic-title",
          backdrop: "static"
        });
      }
      else {
        if (this.signInModal != null) {
          this.signInModal.close();
          this.signInModal = null;
        }
        this.toastr.error("You can't upload the selected collection because exist one collection in the community with the same name and you can't overwrite it.", "Upload Collection");
      }
    });
  }

  continueUploadCollection() {
    this.apiService.container.getContainerToCommunity(this.category.id).subscribe(result => {
      this.apiService.community.validateDestinationTable(result.containerDestination, this.tokenInformation).subscribe(resultValidation => {
        if (resultValidation) {
          this.apiService.community.uploadCollection(result, this.tokenInformation).subscribe(result => {
            if (this.signInModal != null) {
              this.signInModal.close();
              this.signInModal = null;
            }
            if (this.uploadCollectionMessageModal != null) {
              this.uploadCollectionMessageModal.close();
              this.uploadCollectionMessageModal = null;
            }
            this.toastr.success("Collection uploaded successfully.", "Upload Collection");
          });
        }
        else {
          if (this.signInModal != null) {
            this.signInModal.close();
            this.signInModal = null;
          }
          if (this.uploadCollectionMessageModal != null) {
            this.uploadCollectionMessageModal.close();
            this.uploadCollectionMessageModal = null;
          }
          this.toastr.error("You can't upload a collection if the DestinationTable does not have the same structure.", "Upload Collection Failed");
        }
      });
    });
  }

  //SignIn to repository

  //Move Rule to Container  

  showMoveRule(ruleChild: Rule, categoryChild: Category, moveRuleEditToContent: any) {
    this.containerSelectedToMove = undefined;
    this.containerIdSelectedToMove = null;
    this.listChildContainers = [];
    this.apiService.container.getChildContainers().subscribe(result => {
      for (let i = 0; i < result.length; i++) {
        let rec = result[i];
        if (categoryChild.id != rec.id) {
          rec.selected = false;
          rec.checked = "";
          this.listChildContainers.push(rec);
        }
      }      
      this.moveRuleModal = this.modalService.open(moveRuleEditToContent, {
        ariaLabelledBy: "modal-basic-title",
        backdrop: "static"
      });
    });
    this.ruleToMove = ruleChild;
  }

  onContainerToMoveItemChange(item) {
    this.containerIdSelectedToMove = item;
  }

  saveMoveRuleTo() {

    if (this.containerIdSelectedToMove == undefined || this.containerIdSelectedToMove == null) {
      this.toastr.error("Select a Container.", "Move Rule Error");
      return;
    }

    let rulesId = [];
    rulesId.push(this.ruleToMove.id);

    this.apiService.rule.moveRuleToContainer(rulesId, this.containerIdSelectedToMove).subscribe(result => {
      this.toastr.success("Rule moved to container successfully.", "Move Rule");
      this.reloadDataFromChild.emit(true);
      if (this.moveRuleModal != undefined && this.moveRuleModal != null) {
        this.moveRuleModal.close();
        this.moveRuleModal = null;
      }
      this.containerIdSelectedToMove = null;
      this.ruleToMove = null;
    });
  }

  //Move Rule to Container
}



