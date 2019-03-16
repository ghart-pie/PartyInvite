using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartyInvite.Models
{
    public enum Cancelled : int
    {
        No = 0,
        Yes = 1
    }
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Cancelled Cancelled { get; set; }
    }
}