export class Tag {
  id: number;
  name: string;
  description: string;
  isPublic: boolean;
  created: Date;
  updated: Date;
  isUsed: boolean;
}

export class SearchRuleInputs {
  ruleName: string;
  ruleSql: string;
  ruleSeverity: string;
  ruleDestination: string;
}

