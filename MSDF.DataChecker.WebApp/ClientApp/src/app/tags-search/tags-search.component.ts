import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ApiService } from "../services/api.service";
import { Tag, SearchRuleInputs } from '../models/tag.model';
import { Rule } from '../models/rule.model';
import { Category } from '../models/category.model';
import { DatabaseEnvironment } from "../models/databaseEnvironment.model";
import { UtilService } from '../services/util.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import sqlFormatter from "sql-formatter";

@Component({
  selector: 'tags-search',
  templateUrl: './tags-search.component.html',
  styleUrls: ['./tags-search.component.css']
})
export class TagsSearchComponent implements OnInit {

  @Input() selectedDatabaseEnvironment: DatabaseEnvironment = new DatabaseEnvironment();
  @Output() notifyChanges = new EventEmitter<boolean>();

  listTags: Tag[];
  rules: Rule[];
  destinationTypes = [];
  searchModel: SearchRuleInputs;
  allRulesSelected: boolean;

  environmentTypes = [];
  newRule: Rule = new Rule();
  oldRule: Rule = new Rule();
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

  listTagsAssign: Tag[];
  tagsSelectedAssign: Tag[];
  assignTagsToRuleModal: any;

  listChildContainers: Category[];
  copyRulesToModal: any;
  allContainersSelected: boolean;

  clickToSearch: boolean;

  dropdownSettings = {};
  collectionList = [];
  selectedCollectionItems = [];
  containerList = [];
  selectedContainerItems = [];
  tagList = [];
  selectedTagItems = [];

  isDeleteDisabled: boolean;
  ruleDetailsLogResult: any;

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

    this.isDeleteDisabled = true;
    this.searchModel = new SearchRuleInputs();
    this.searchModel.ruleDestination = '0';
    this.searchModel.ruleSeverity = '0';
    this.searchModel.ruleName = '';
    this.searchModel.ruleSql = '';
    this.rules = [];

    //Loading the collections list
    this.apiService.container.getParentContainers().subscribe(result => {
      let tmp = [];
      result.forEach(rec => tmp.push({
        item_id: rec.id,
        item_text: rec.name
      }));
      this.collectionList = tmp;
    });

    //Loading the containers list
    this.apiService.container.getChildContainers().subscribe(result => {
      let tmp = [];
      result.forEach(rec => tmp.push({
        item_id: rec.id,
        item_text: rec.name
      }));
      this.containerList = tmp;
    });

    //Loading the tags list
    this.apiService.tag.getTags().subscribe(result => {
      //this.listTags = result;
      let tmp = [];
      result.forEach(rec => tmp.push({
        item_id: rec.id,
        item_text: rec.name
      }));
      this.tagList = tmp;
    });

    //Loading the destination types catalog
    this.apiService.catalog.getByType('RuleDetailsDestinationType').subscribe(result => {
      this.destinationTypes = result;
    });

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item_id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true
    };
  }

  search() {

    let tmpCollections = [];
    let tmpContainers = [];
    let tmpTags = [];

    this.selectedCollectionItems.forEach(rec => tmpCollections.push(rec.item_id));
    this.selectedContainerItems.forEach(rec => tmpContainers.push(rec.item_id));
    this.selectedTagItems.forEach(rec => tmpTags.push(rec.item_id));

    this.clickToSearch = false;

    let searchRequest = {
      tags: tmpTags,
      containers: tmpContainers,
      collections: tmpCollections,
      name: this.searchModel.ruleName,
      diagnosticSql: this.searchModel.ruleSql,
      detailsDestination: null,
      severity: null
    };

    if (this.searchModel.ruleDestination != '0')
      searchRequest.detailsDestination = parseInt(this.searchModel.ruleDestination);

    if (this.searchModel.ruleSeverity != '0')
      searchRequest.severity = parseInt(this.searchModel.ruleSeverity);

    this.apiService.rule.searchRules(searchRequest).subscribe(result => {
      if (result != null && result != undefined) {
        this.rules = result.rules;
      }
      this.clickToSearch = true;
      this.isDeleteDisabled = !(this.rules.length > 0 && this.rules.filter(rec => rec.selected).length > 0);
    });
  }

  allRulesSelectedChange(event: any) {
    if (this.rules.length > 0) {
      this.rules.forEach(rec => rec.selected = this.allRulesSelected);
    }
    this.isDeleteDisabled = !(this.rules.length > 0 && this.rules.filter(rec => rec.selected).length > 0);
  }  

  ruleSelectedChange(event: any) {
    this.isDeleteDisabled = !(this.rules.length > 0 && this.rules.filter(rec => rec.selected).length > 0);
  }  

  //Rule Actions

  copyRule(ruleChild: Rule) {
    this.apiService.rule.copyRuleTo({
      ruleId: ruleChild.id,
      containerId: ruleChild.containerId
    }).subscribe(result => {
      this.search();
      this.toastr.success("Sql Data Check " + ruleChild.name + " copied", "Success");
      this.notifyChanges.emit(true);
    });
  }

  deleteRuleChild(ruleChild: Rule) {
    this.apiService.rule.deleteRule(ruleChild.id).subscribe(result => {
      this.search();
      this.toastr.success("Sql Data Check " + ruleChild.name + " deleted", "Success");
      this.notifyChanges.emit(true);
    });
  }

  //Rule Actions

  //Modify Rule

  loadCatalogs() {
    //Loading the environment types catalog
    this.apiService.catalog.getByType('EnvironmentType').subscribe(result => {
      this.environmentTypes = result;
    });

    //Loading the destination types catalog
    this.apiService.catalog.getByType('RuleDetailsDestinationType').subscribe(result => {
      this.destinationTypes = result;
    });
  }

  loadTags() {
    //Loading the tags list
    this.apiService.tag.getTags().subscribe(result => {
      this.listTags = result;
    });
  }

  modifyRule(ruleContent, ruleChild: Rule) {

    this.loadCatalogs();
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

    this.newRule = Object.assign({}, ruleChild);
    this.newRule.tags = Object.assign([], ruleChild.tags);

    this.oldRule = Object.assign({}, ruleChild);
    this.oldRule.tags = Object.assign([], ruleChild.tags);
    this.oldRule.containerId = this.newRule.containerId;

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

    if (this.ruleIncreaseVersionModal != null) {
      this.ruleIncreaseVersionModal.close();
      this.ruleIncreaseVersionModal = null;
    }

    this.apiService.rule.modifyRule(this.newRule).subscribe(data => {
      this.ruleModal.close();
      if (this.ruleWarningModal != null)
        this.ruleWarningModal.close();
      this.toastr.success("Rule " + this.newRule.name + " modified ", "Success");
      if (newTags > 0)
        this.toastr.success("You have created " + newTags + " new tags.", "Success");
      this.search();
      this.notifyChanges.emit(true);
      this.ruleModal = null;
    });
  }

  validateIncreaseVersion(increaseVersionNumberMessageContent) {
    if (this.newRule.version != this.oldRule.version) {
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

  cancelRule(forceClose, ruleWarningContent) {
    if (forceClose) {
      if (this.ruleWarningModal != null)
        this.ruleWarningModal.close();
      this.ruleModal.close();
      this.ruleModal = null;
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
          this.ruleModal = null;
        }
      }
      else {
        this.ruleModal.close();
        this.ruleModal = null;
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

  //Modify Rule

  //Copy Rules to Containers

  showCopyRulesSelected(ruleCopyRulesToContent) {
    if (this.validateRulesSelected()) {
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
  }

  saveCopyRulesTo() {

    if (this.ruleModal != null) {
      this.apiService.rule.copyRulesToContainers({ ruleIds: [this.newRule.id], containerIds: [this.newRule.containerId] }).subscribe(result => {
        this.allRulesSelected = false;
        this.toastr.success("Rule copied successfully.", "Copy Rules To");
        this.search();
        this.copyRulesToModal.close();
        this.ruleModal.close();
        this.notifyChanges.emit(true);
        this.ruleModal = null;
      });
      return;
    }

    let containersSelected = this.listChildContainers.filter(rec => rec.selected);
    if (containersSelected.length == 0) {
      this.toastr.error("Select one container at least.", "Copy Rules To");
      return;
    }
    let rulesId = [];
    let containersId = [];
    this.rules.filter(rec => rec.selected).forEach(rec => rulesId.push(rec.id));
    containersSelected.forEach(rec => containersId.push(rec.id));
    this.apiService.rule.copyRulesToContainers({ ruleIds: rulesId, containerIds: containersId }).subscribe(result => {
      this.allRulesSelected = false;
      this.toastr.success("Rules copied successfully.", "Copy Rules To");
      this.search();
      this.copyRulesToModal.close();
      this.notifyChanges.emit(true);
    });

  }

  allContainersSelectedChange(event: any) {
    if (this.listChildContainers.length > 0) {
      this.listChildContainers.forEach(rec => rec.selected = this.allContainersSelected);
    }
  }

  //Copy Rules to Containers

  //Assign Tags to Rules

  showAssignTagsRulesSelected(ruleAssignTagsContent) {
    this.assignTagsToRuleModal = null;
    this.listTagsAssign = [];
    this.tagsSelectedAssign = [];

    if (this.validateRulesSelected()) {
      this.apiService.tag.getTags().subscribe(result => {
        this.listTagsAssign = result;
        this.assignTagsToRuleModal = this.modalService.open(ruleAssignTagsContent, {
          ariaLabelledBy: "modal-basic-title",
          size: "lg",
          backdrop: "static"
        });
      });
    }
  }

  onAddingTagAssign(tag: any) {
    let existTag = this.tagsSelectedAssign.find(rec => rec.id == tag.id);
    if (existTag == null) {
      this.tagsSelectedAssign.push(tag);
    }
  }

  onRemovingTagAssign(tag: any) {
    this.tagsSelectedAssign = this.tagsSelectedAssign.filter(rec => rec.id != tag.id);
  }

  saveAssignTags() {
    if (this.tagsSelectedAssign.length == 0) {
      this.toastr.error("Add one tag at least.", "Assign Tags");
      return;
    }
    this.tagsSelectedAssign.filter(rec => rec.id.toString() == rec.name).forEach(rec => rec.id = -1);
    let rulesId = [];
    this.rules.filter(rec => rec.selected).forEach(rec => rulesId.push(rec.id));
    this.apiService.rule.assignTagsToRules({ ruleIds: rulesId, tags: this.tagsSelectedAssign }).subscribe(result => {
      this.allRulesSelected = false;
      this.toastr.success("Tags assigned successfully.", "Assign Tags");
      this.search();
      this.assignTagsToRuleModal.close();
      this.notifyChanges.emit(true);
    });
  }

  //Assign Tags to Rules

  deleteRulesSelected() {
    if (this.validateRulesSelected()) {
      let rulesId = [];
      this.rules.filter(rec => rec.selected).forEach(rec => rulesId.push(rec.id));
      this.apiService.rule.deleteRules({ ruleIds: rulesId }).subscribe(result => {
        this.allRulesSelected = false;
        this.toastr.success("Rules deleted.", "Delete Rules");
        this.search();
        this.notifyChanges.emit(true);
      });
    }
  }

  validateRulesSelected(): boolean {
    let result = this.rules.length > 0 && this.rules.filter(rec => rec.selected).length > 0;
    if (!result) this.toastr.error("Select one rule at least.", "Error");
    return result;
  }

  //copy rule from rule edit screen

  copyRuleSelected(ruleCopyRulesToContent) {
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

  //delete rule from rule edit screen

  deleteRuleSelected() {
    this.apiService.rule.deleteRule(this.newRule.id).subscribe(result => {
      this.search();
      this.toastr.success("Sql Data Check " + this.newRule.name + " deleted", "Success");
      this.notifyChanges.emit(true);
      this.ruleModal.close();
      this.ruleModal = null;
    });
  }

  //View RuleExecutionLogDetails

  viewRuleExecutionLogDetailsFromOptions(ruleExecutionLogDetailRuleEditSearchContent, ruleChild: Rule) {

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
                this.modalService.open(ruleExecutionLogDetailRuleEditSearchContent, {
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

  viewRuleExecutionLogDetails(ruleExecutionLogDetailRuleEditSearchContent) {
    this.ruleDetailsLogResult = null;
    let idExecutionLog = this.newRule.idLastRuleExecutionLog;
    this.apiService.ruleExecutionLogDetail.getByRuleExecutionLogAsync(idExecutionLog).subscribe(
      result => {
        if (result != null) {
          this.ruleDetailsLogResult = result;
          this.modalService.open(ruleExecutionLogDetailRuleEditSearchContent, {
            ariaLabelledBy: "modal-basic-title",
            size: "xl",
            windowClass: "modal-custom-xl",
            backdrop: "static"
          });
        } else {
          this.toastr.info("No execution log details for rule " + this.newRule.name, "Information");
        }
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

  //Move Rule to Container  

  showMoveRulesSelected(moveRuleRuleSearchToContent) {
    this.containerSelectedToMove = undefined;
    this.containerIdSelectedToMove = null;
    if (this.validateRulesSelected()) {
      this.listChildContainers = [];
      this.apiService.container.getChildContainers().subscribe(result => {
        for (let i = 0; i < result.length; i++) {
          let rec = result[i];
          rec.selected = false;
          rec.checked = "";
          this.listChildContainers.push(rec);
        }
        this.moveRuleModal = this.modalService.open(moveRuleRuleSearchToContent, {
          ariaLabelledBy: "modal-basic-title",
          backdrop: "static"
        });
      });
    }
  }

  showMoveRule(ruleChild: Rule, moveRuleRuleSearchToContent: any) {
    this.containerSelectedToMove = undefined;
    this.containerIdSelectedToMove = null;
    this.listChildContainers = [];
    this.apiService.container.getChildContainers().subscribe(result => {
      for (let i = 0; i < result.length; i++) {
        let rec = result[i];
        if (ruleChild.containerId != rec.id) {
          rec.selected = false;
          rec.checked = "";
          this.listChildContainers.push(rec);
        }
      }
      this.moveRuleModal = this.modalService.open(moveRuleRuleSearchToContent, {
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
    if (this.ruleToMove == null)
      this.rules.filter(rec => rec.selected).forEach(rec => rulesId.push(rec.id));
    else
      rulesId.push(this.ruleToMove.id);

    this.apiService.rule.moveRuleToContainer(rulesId, this.containerIdSelectedToMove).subscribe(result => {
      this.search();
      this.toastr.success("Rule moved to container successfully.", "Move Rule");
      this.notifyChanges.emit(true);
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


