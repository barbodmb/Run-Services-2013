using System.Collections.Generic;
using System.Configuration;

namespace RunServices.Models
{
    public class ConfigService : ConfigurationSection
    {
        public const string SectionName = "ConfigService";

        //public ConfigService()
        //{
        //    SectionDetails = new List<SectionDetail>();
        //}

        [ConfigurationProperty("ServiceName", IsRequired = false)]
        public string ServiceName { get; set; }

        [ConfigurationProperty("IsPending", IsRequired = false)]
        public bool IsPending { get; set; }

        [ConfigurationProperty("ServiceStatus", IsRequired = false)]
        public string ServiceStatus { get; set; }

        //[ConfigurationProperty("SectionDetails", IsRequired = false)]
        //public ItemsCollection SectionDetails { get; set; }

        [ConfigurationProperty("Items")]
        [ConfigurationCollection(typeof(ItemsCollection), AddItemName = "add")]
        public ItemsCollection Items
        {
            get { return (ItemsCollection)base["Items"]; }
            set { this["Items"] = value; }
        }
    }
}