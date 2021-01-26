using DbFacade.DataLayer.Models;
using DbFacade.DataLayer.Models.Attributes;
using System;

namespace ServiceLayer.DomainLayer.Models.Data
{
    public interface ICACUser
    {
        string EDIPI { get; }
        string SSN { get; }
        string FirstName { get; }
        string MiddleInitial { get; }
        string LastName { get; }
        string Email { get;  }
    }
    public class CACUser : DbDataModel, ICACUser
    {
        [DbColumn("EDIPI")]
        public string EDIPI { get; set; }
        [DbColumn("SSN")]
        public string SSN { get; set; }
        [DbColumn("FirstName")]
        public string FirstName { get; set; }
        [DbColumn("MiddleInitial")]
        public string MiddleInitial { get; set; }
        [DbColumn("LastName")]
        public string LastName { get; set; }
        [DbColumn("Email")]
        public string Email { get; set; }
    }
}
