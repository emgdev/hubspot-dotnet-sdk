using System.Collections.Generic;

namespace HubSpot
{
    public interface IHubSpotEntity
    {
        Dictionary<string, object> Properties { get; set; }
    }

    public struct PropertyData
    {
        public PropertyData(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; set; }

        public object Value { get; set; }
    }
}