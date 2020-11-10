import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ToastrService } from "ngx-toastr";
import { ApiService } from "../services/api.service";
import { Category } from "../models/category.model";
import sqlFormatter from "sql-formatter";

@Component({
  selector: "nav-community-collection",
  templateUrl: "./nav-community-collection.component.html",
  styleUrls: ["./nav-community-collection.component.css"]
})
export class NavCommunityCollectionComponent implements OnInit {

  @Input() category: Category = new Category();
  @Output() notifyDownloadedCollection = new EventEmitter<boolean>();

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

  downloadCollectionMessageModal: any;
  newRule: any;

  constructor(
    private apiService: ApiService,
    private modalService: NgbModal,
    private toastr: ToastrService) { }

  ngOnInit() {
  }

  validDownloadCollection(downloadCollectionMessageContent) {
    this.apiService.container.getContainerByName(this.category).subscribe(result => {
      if (result == null) {
        this.downloadCollection(true);
      }
      else {
        this.downloadCollectionMessageModal = this.modalService.open(downloadCollectionMessageContent, {
          ariaLabelledBy: "modal-basic-title",
          backdrop: "static"
        });
      }
    });
  }

  downloadCollection(newCollection) {
    this.category.createNewCollection = newCollection;
    this.apiService.container.validateDestinationTable(this.category.containerDestination).subscribe(resultValidation => {
      if (resultValidation) {
        this.apiService.container.addContainerFromCommunity(this.category).subscribe(result => {
          if (this.downloadCollectionMessageModal != null) {
            this.downloadCollectionMessageModal.close();
            this.downloadCollectionMessageModal = null;
          }
          if (result == '' || result == null) {
            this.notifyDownloadedCollection.emit(true);
            this.toastr.success("Collection downloaded successfully.", "Collection Download");
          }
          else {
            this.toastr.error(result, "Collection Download Error");
          }
        });
      }
      else {
        if (this.downloadCollectionMessageModal != null) {
          this.downloadCollectionMessageModal.close();
          this.downloadCollectionMessageModal = null;
        }
        this.toastr.error("You can't download a collection if the DestinationTable does not have the same structure.", "Collection Download Failed");
      }
    });
  }

  showRule(rule, ruleCommunityContent) {
    this.newRule = rule;
    this.newRule.diagnosticSql = sqlFormatter.format(this.newRule.diagnosticSql);
    this.modalService.open(ruleCommunityContent, {
      ariaLabelledBy: "modal-basic-title",
      size: "xl",
      windowClass: "modal-custom-xl",
      backdrop: "static"
    });
  }
}
