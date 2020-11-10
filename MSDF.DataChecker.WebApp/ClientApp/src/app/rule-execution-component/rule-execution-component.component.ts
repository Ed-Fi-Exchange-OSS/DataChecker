import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { ApiService } from "../services/api.service";
import { Rule } from "../models/rule.model";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import * as moment from "moment";
import sqlFormatter from "sql-formatter";
import { DatabaseEnvironment } from "../models/databaseEnvironment.model";

@Component({
  selector: 'rule-execution-component',
  templateUrl: './rule-execution-component.component.html',
  styleUrls: ['./rule-execution-component.component.css']
})
export class RuleExecutionComponentComponent implements OnInit {

  @Input() rule: Rule = new Rule();
  @Input() selectedDatabaseEnvironment: DatabaseEnvironment = new DatabaseEnvironment();

  @Output() updateRuleResultFromChild = new EventEmitter<Rule>();

  diagnosticsResult: any;
  ruleDetailsLogResult: any;
  hideInfo: boolean;
  diffTime: any;
  executedSql = "";
  ruleDiagnosticsSql: string;
  sqlOptions = {
    lineNumbers: true, theme: 'material', mode: 'sql', lineWrapping: 'true', smartIndent: true,
    extraKeys: { 'Ctrl-Space': 'autocomplete' }, hint: "CodeMirror.hint.sql",
    hintOptions:
    {
      tables: {
        users: ["name", "score", "birthDate"],
        countries: ["name", "population", "size"]
      }
    }
    , viewportMargin: Infinity
    , readOnly: true
  };

  constructor(
    private apiService: ApiService,
    private toastr: ToastrService,
    private modalService: NgbModal) {
  }

  ngOnInit() { }

  executeRule() {
    this.rule.isExecuting = true;
    this.apiService.databaseEnvironment.testDatabaseEnvironmentById(this.selectedDatabaseEnvironment).subscribe(isConnectedMessage => {
      if (isConnectedMessage == null || isConnectedMessage == '') {
        this.apiService.rule
          .executeRule({ ruleId: this.rule.id, databaseEnvironmentId: this.selectedDatabaseEnvironment.id })
          .subscribe(result => {
            this.rule.counter = result.result;
            this.rule.isExecuting = false;
            this.rule.lastStatus = result.status;
            this.rule.lastExecution = new Date();
            this.rule.results = result.testResults;
            this.updateRuleResultFromChild.emit(this.rule);
          });
      }
      else {
        this.toastr.error("Review the connection with your environment:" + isConnectedMessage, "Environment Connection Error");
        this.rule.isExecuting = false;
      }
    });
  }

  getRuleResults() {
    if (this.rule.displayResults) {
      this.rule.displayResults = false;
      return;
    }

    this.apiService.rule.getRuleDetail(this.rule.id, this.selectedDatabaseEnvironment.id).subscribe(
      result => {
        if (this.rule.results != null && this.rule.results.length == 1) {
          this.rule.results = this.rule.results;
        } else {
          this.rule.results = result;
        }
        if (this.rule.results.length > 5) {
          this.rule.results.length = 5;
        }
        this.rule.displayResults = true;
      },
      error => {
        this.rule.displayResults = false;
      }
    );
  }

  calculateDiffTime(dt: Date) {
    if (dt == null) {
      return "";
    } else {
      return moment.duration(moment().diff(moment.utc(dt).toDate())).humanize();
    }
  }

  loadPopover(result) {
    this.executedSql = sqlFormatter.format(result.executedSql);
  }

  copyDiagnostic(result: any) {
    let selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = result.diagnosticSql;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
    this.toastr.success("Copied", "Copy Diagnostic SQL");
  }

  viewRuleExecutionLogDetails(ruleLogDetailsContent, ruleResult: any) {
    this.ruleDetailsLogResult = null;
    let idExecutionLog = ruleResult.id;
    this.apiService.ruleExecutionLogDetail.getByRuleExecutionLogAsync(idExecutionLog).subscribe(
      result => {
        if (result != null) {
          this.ruleDetailsLogResult = result;
          this.modalService.open(ruleLogDetailsContent, {
            ariaLabelledBy: "modal-basic-title",
            size: "xl",
            windowClass: "modal-custom-xl",
            backdrop: "static"
          });
        }
        else {
          this.toastr.info("No execution log details for rule " + ruleResult.name, "Information");
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

  executeSqlDiagnostic(ruleExecutionDiagnosticLogDetailsContent:any, result: any) {
    this.ruleDetailsLogResult = null;
    let idExecutionLog = result.id;
    this.apiService.ruleExecutionLogDetail.executeDiagnosticSqlFromLogIdAsync(idExecutionLog).subscribe(
      infoResult => {
        if (infoResult != null) {
          this.ruleDetailsLogResult = infoResult;
          this.modalService.open(ruleExecutionDiagnosticLogDetailsContent, {
            ariaLabelledBy: "modal-basic-title",
            size: "xl",
            windowClass: "modal-custom-xl",
            backdrop: "static"
          });
        }
        else {
          this.toastr.info("No information.", "Information");
        }
      });
  }
}
