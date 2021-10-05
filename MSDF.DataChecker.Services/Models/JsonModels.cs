using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class CollectionJson
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EnvironmentType { get; set; }
        public List<ContainerJson> Containers { get; set; }
        public List<TagJson> Tags { get; set; }
        public string DestinationTable { get; set; }
        public string DestinationStructure { get; set; }
        public CollectionJson()
        {
            Containers = new List<ContainerJson>();
            Tags = new List<TagJson>();
        }
    }

    public class ContainerJson
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RuleJson> Rules { get; set; }
        public List<TagJson> Tags { get; set; }

        public ContainerJson()
        {
            Rules = new List<RuleJson>();
            Tags = new List<TagJson>();
        }
    }

    public class RuleJson
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public int SeverityLevel { get; set; }
        public string Resolution { get; set; }
        public string Sql { get; set; }
        public string Version { get; set; }
        public string ExternalRuleId { get; set; }
        public int? MaxNumberResults { get; set; }
        public List<TagJson> Tags { get; set; }

        public RuleJson()
        {
            Tags = new List<TagJson>();
        }
    }

    public class TagJson
    {
        public string Name { get; set; }
    }
}
