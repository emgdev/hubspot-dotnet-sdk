using System;
using System.Collections.Generic;

namespace HubSpot.Contacts
{
    public class Contact : IHubSpotEntity
    {
        [DefaultProperty("vid")]
        public long Id { get; set; }

        [DefaultProperty("portal-id")]
        public long PortalId { get; set; }

        [DefaultProperty("is-contact")]
        public bool IsContact { get; set; }

        [DefaultProperty("profile-token")]
        public string UserToken { get; set; }

        [DefaultProperty("profile-url")]
        public Uri ProfileUrl { get; set; }

        [CustomProperty("email")]
        public string Email { get; set; }

        [CustomProperty("createdate", IsReadOnly = true)]
        public DateTimeOffset Created { get; set; }

        [CustomProperty("firstname")]
        public string FirstName { get; set; }

        [CustomProperty("lastname")]
        public string LastName { get; set; }

        [CustomProperty("lastmodifieddate", IsReadOnly = true)]
        public DateTimeOffset LastModified { get; set; }

        [CustomProperty("associatedcompanyid")]
        public long AssociatedCompanyId { get; set; }

        Dictionary<string, object> IHubSpotEntity.Properties { get; set; } = new Dictionary<string, object>();
    }
}