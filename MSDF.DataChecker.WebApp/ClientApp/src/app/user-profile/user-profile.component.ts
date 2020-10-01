import { Component, OnInit } from "@angular/core";
import { ApiService } from "../services/api.service";
import { CommunityUser } from "../models/communityUser.mode";
import { CommunityOrganization } from "../models/communityOrganization.model";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "user-profile",
  templateUrl: "./user-profile.component.html",
  styleUrls: ["./user-profile.component.css"]
})
export class UserProfileComponent implements OnInit {
  localUser: CommunityUser = new CommunityUser();

  constructor(private apiService: ApiService, private toastr: ToastrService) {
    this.localUser.name = "";
    this.localUser.email = "";
    this.localUser.organization = new CommunityOrganization();
  }

  ngOnInit() {
    this.apiService.localUser.getLocalUser().subscribe(localUser => {
      this.localUser = localUser;
    });
  }

  saveInformation() {
    this.apiService.localUser
      .updateUserInformation(this.localUser)
      .subscribe(localUser => {
        this.toastr.success("User Updated", "Success");
      });
  }
}
