using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartyInvite.Models
{
    public enum Attending : int
    {
        Unknown = 0,
        Yes = 1,
        Maybe = 2,
        No = 3
    }
    public class Rsvp
    {
        public int PartyId { get; set; }
        public int PersonId { get; set; }
        public Attending Attending { get; set; }
        public string PartyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}