import { CommunityOrganization } from "./communityOrganization.model";

export class CommunityUser {
  id: string;
  communityOrganizationId: string;
  name: string;
  email: string;
  organization: CommunityOrganization;
}
