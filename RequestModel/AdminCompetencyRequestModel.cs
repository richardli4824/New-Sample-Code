using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLC.Models.Requests
{
    public class AdminCompetencyRequestModel
    {
        public string UserId { get; set; }
        public int CompetencyId { get; set; }
        public string AuthorizedBy { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string ActivityType { get; set; }
        public string Attachment { get; set; } 
        public int competencyUserId { get; set; }
        public string Id { get; set; }

    }

}