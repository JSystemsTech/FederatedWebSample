using DbFacade.DataLayer.Models;
using DbFacade.DataLayer.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.DomainLayer.Models.Data
{
    public class TestUser:DbDataModel
    {
        [DbColumn("Guid")]
        public Guid Guid { get; private set; }

        [DbColumn("FirstName")]
        public string FirstName { get; private set; }

        [DbColumn("LastName")]
        public string LastName { get; private set; }

        [DbColumn("MiddleInitial")]
        public char? MiddleInitial { get; private set; }

        [DbColumn("Roles")]
        public IEnumerable<string> Roles { get; private set; }

        public TestUser() { }
    }
}
