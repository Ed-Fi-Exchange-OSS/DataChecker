import { Rule } from "./rule.model";
import { Tag } from "./tag.model"

export class DestinationTableStructure {
  type: string;
  name: string;
  isNullable: boolean;
}

export class Category {
  id: string;
  rules: Rule[];
  childContainers: Category[];
  name: string;
  showRules: boolean;
  isRunning: boolean;
  lastStatus: number;
  validRules: number;
  invalidRules: number;
  sintaxRules: number;
  runningRules: number;

  showChilds: boolean;
  amountRules: number;
  isDefault: boolean;
  description: string;

  containerTypeId?: number;
  createdByUserId?: string;
  parentContainerId?: string;

  userId: string;
  organizationId: string;
  userName: string;
  email: string;
  organizationDescription: string;
  organizationName: string;

  environmentType: number;

  tags: Tag[];
  parentContainerName: string;
  selected: boolean;

  ruleDetailsDestinationId: number;
  createNewCollection: boolean;

  uploaded: Date;
  updated: Date;

  publicTagsName: string;
  environmentTypeName: string;
  destinationTable: string;
  totalRules: number;
  administrator: string;

  checked: string;
  containerDestination: {
    destinationStructure: string;
  };

  containerDestinationStructure: DestinationTableStructure[];
  showDestinationStructure: boolean;
}
