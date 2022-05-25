using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace RunServices.Models
{
    public class ItemsCollection : ConfigurationElementCollection
    {
        public List<SectionDetail> SectionDetail { get; set; }

        public ItemsCollection(List<SectionDetail> sectionDetail)
        {
            SectionDetail = sectionDetail;
        }

        public ItemsCollection()
        {
            
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SectionDetail();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SectionDetail) element).ServiceName;
        }
    }

    public class SectionDetail : ConfigurationElement
    {
        [ConfigurationProperty("DisplayName", IsRequired = false)]
        public string DisplayName
        {
            get { return (string)this["DisplayName"]; }
            set { this["DisplayName"] = value; }
        }

        [ConfigurationProperty("ServiceName", IsRequired = false)]
        public string ServiceName
        {
            get { return (string)this["ServiceName"]; }
            set { this["ServiceName"] = value; }
        }

        [ConfigurationProperty("Interval", IsRequired = false)]
        public string Interval
        {
            get { return (string)this["Interval"]; }
            set { this["Interval"] = value; }
        }

        [ConfigurationProperty("IntervalUnit", IsRequired = false)]
        public string IntervalUnit
        {
            get { return (string)this["IntervalUnit"]; }
            set { this["IntervalUnit"] = value; }
        }

        [ConfigurationProperty("Url", IsRequired = false)]
        public string Url
        {
            get { return (string)this["Url"]; }
            set { this["Url"] = value; }
        }

        [ConfigurationProperty("RequestTimeOut", IsRequired = false)]
        public string RequestTimeOut
        {
            get { return (string)this["RequestTimeOut"]; }
            set { this["RequestTimeOut"] = value; }
        }
    }
}