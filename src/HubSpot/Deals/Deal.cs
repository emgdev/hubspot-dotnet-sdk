using System;
using System.Collections.Generic;
using System.Text;

namespace HubSpot.Deals
{
    public interface IHubSpotDealEntity : IHubSpotEntity
    {
        IReadOnlyDictionary<string, IReadOnlyList<long>> Associations { get; set;  }
    }

    public class Deal : IHubSpotDealEntity
    {
        [DefaultProperty("portalId")]
        public long PortalId { get; set; }

        [DefaultProperty("dealId")]
        public long Id { get; set; }

        [DefaultProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [CustomProperty("dealname")]
        public string Name { get; set; }

        [CustomProperty("amount")]
        public decimal Amount { get; set; }

        [CustomProperty("createdate")]
        public DateTimeOffset Created { get; set; }

        [CustomProperty("num_associated_contacts")]
        public long NumberOfAssociatedContacts { get; set; }

        public IReadOnlyList<long> AssociatedCompanyIds { get; set; } = Array.Empty<long>();

        public IReadOnlyList<long> AssociatedContactIds { get; set; } = Array.Empty<long>();

        public IReadOnlyList<long> AssociatedDealIds { get; set; } = Array.Empty<long>();

        Dictionary<string, object> IHubSpotEntity.Properties { get; set; } = new Dictionary<string, object>();

        IReadOnlyDictionary<string, IReadOnlyList<long>> IHubSpotDealEntity.Associations { get; set; } = new Dictionary<string, IReadOnlyList<long>>();
    }
}
