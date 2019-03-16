using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyInvite.Controllers;
using PartyInvite.Models;

namespace PartyInvite.Tests.Controllers
{
    [TestClass]
    public class PartyControllerTest
    {
        [TestMethod]
        public void CreatePartyTest()
        {
            string testName = "my new event";
            DateTime testDate = new DateTime(2012, 3, 6, 11, 3, 5);
            PartyController controller = new PartyController();

            int newId = controller.CreateParty(new Party
            {
                Name = testName,
                Date = testDate
            });

            Party party = controller.GetParty(newId);

            Assert.AreEqual(party.Id, newId);
            Assert.AreEqual(party.Name, testName);
            Assert.AreEqual(party.Date, testDate);
            Assert.AreEqual(party.Cancelled, Cancelled.No);
        }

        [TestMethod]
        public void AddInvitesTest()
        {
            string testName = "my new event";
            DateTime testDate = new DateTime(2012, 3, 6, 11, 3, 5);
            Person person1 = new Person { FirstName = "Bob", LastName = "Smith" };
            Person person2 = new Person { FirstName = "Alice", LastName = "Jones" };
            Person person3 = new Person { FirstName = "George", LastName = "Clinton" };
            PartyController controller = new PartyController();

            int partyId = controller.CreateParty(new Party
            {
                Name = testName,
                Date = testDate
            });

            int personId1 = controller.CreatePerson(person1);
            int personId2 = controller.CreatePerson(person2);
            int personId3 = controller.CreatePerson(person3);

            controller.InvitePeople(partyId, new List<int> { personId1, personId2 });

            IEnumerable<Rsvp> rsvpList = controller.GetPartyRsvps(partyId);

            Assert.AreEqual(rsvpList.Count(), 2);

            bool foundPerson1 = false;
            bool foundPerson2 = false;

            foreach (var rsvp in rsvpList)
            {
                if (rsvp.FirstName == person1.FirstName && 
                    rsvp.LastName == person1.LastName && 
                    rsvp.PersonId == personId1)
                    foundPerson1 = true;
                if (rsvp.FirstName == person2.FirstName && 
                    rsvp.LastName == person2.LastName &&
                    rsvp.PersonId == personId2)
                    foundPerson2 = true;
                Assert.AreEqual(rsvp.PartyId, partyId);
                Assert.AreEqual(rsvp.PartyName, testName);
            }
            Assert.IsTrue(foundPerson1);
            Assert.IsTrue(foundPerson2);
        }

        [TestMethod]
        public void RsvpToPartyTest()
        {
            string testName = "my new event";
            DateTime testDate = new DateTime(2012, 3, 6, 11, 3, 5);
            Person person1 = new Person { FirstName = "Bob", LastName = "Smith" };
            Person person2 = new Person { FirstName = "Alice", LastName = "Jones" };
            Person person3 = new Person { FirstName = "George", LastName = "Clinton" };
            PartyController controller = new PartyController();

            int partyId = controller.CreateParty(new Party
            {
                Name = testName,
                Date = testDate
            });

            int personId1 = controller.CreatePerson(person1);
            int personId2 = controller.CreatePerson(person2);
            int personId3 = controller.CreatePerson(person3);

            controller.InvitePeople(partyId, new List<int> { personId1, personId2 });

            controller.RsvpToParty(partyId, personId1, Attending.No);
            controller.RsvpToParty(partyId, personId2, Attending.Yes);

            IEnumerable<Rsvp> rsvpList = controller.GetPartyRsvps(partyId);

            Assert.AreEqual(rsvpList.Count(), 2);

            bool foundPerson1 = false;
            bool foundPerson2 = false;

            foreach (var rsvp in rsvpList)
            {
                if (rsvp.FirstName == person1.FirstName &&
                    rsvp.LastName == person1.LastName &&
                    rsvp.PersonId == personId1)
                {
                    Assert.AreEqual(rsvp.Attending, Attending.No);
                    foundPerson1 = true;
                }
                if (rsvp.FirstName == person2.FirstName &&
                    rsvp.LastName == person2.LastName &&
                    rsvp.PersonId == personId2)
                {
                    Assert.AreEqual(rsvp.Attending, Attending.Yes);
                    foundPerson2 = true;
                }
                Assert.AreEqual(rsvp.PartyId, partyId);
                Assert.AreEqual(rsvp.PartyName, testName);
            }
            Assert.IsTrue(foundPerson1);
            Assert.IsTrue(foundPerson2);

        }

        [TestMethod]
        public void DeleteParty()
        {
            string testName = "my new event";
            DateTime testDate = new DateTime(2012, 3, 6, 11, 3, 5);
            PartyController controller = new PartyController();

            int newId = controller.CreateParty(new Party
            {
                Name = testName,
                Date = testDate
            });

            controller.CancelParty(newId);

            Party party = controller.GetParty(newId);

            Assert.AreEqual(party.Id, newId);
            Assert.AreEqual(party.Name, testName);
            Assert.AreEqual(party.Date, testDate);
            Assert.AreEqual(party.Cancelled, Cancelled.Yes);
        }
    }
}
