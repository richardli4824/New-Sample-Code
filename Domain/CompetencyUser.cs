using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLC.Domain
{
    public class CompetencyUser
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompetencyId { get; set; }
        public string CompetencyName { get; set; }
        public string Email { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string ActivityType { get; set; }
        public string Notes { get; set; }
        public string Attachments { get; set; }
        public int competencyUserId { get; set; }


    }
}