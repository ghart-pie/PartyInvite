using System;
using System.Collections.Generic;
using System.Web.Http;
using PartyInvite.Models;
using System.Data.SQLite;
using System.Globalization;

namespace PartyInvite.Controllers
{
    public class PartyController : ApiController
    {
        static string dbLocation = "c:\\users\\ghart\\InterviewDB.sqlite";

        public int CreateParty([FromBody]Party party)
        {
            var newId = 0;
            var sql = "insert into party (name, date) values (@PartyName,@PartyDate);"
                + "select last_insert_rowid()";

            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@PartyName", System.Data.DbType.String).Value = party.Name;
                    command.Parameters.Add("@PartyDate", System.Data.DbType.String).Value = party.Date;
                    var result = command.ExecuteReader();
                    if (result.Read())
                        newId = Convert.ToInt32(result[0]);
                }
            }
            return newId;
        }

        public int CreatePerson([FromBody]Person person)
        {
            var newId = 0;
            var sql = "insert into person (firstName, lastName) values (@FirstName,@LastName);"
                + "select last_insert_rowid()";

            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@FirstName", System.Data.DbType.String).Value = person.FirstName;
                    command.Parameters.Add("@LastName", System.Data.DbType.String).Value = person.LastName;
                    var result = command.ExecuteReader();
                    if (result.Read())
                        newId = Convert.ToInt32(result[0]);
                }
            }
            return newId;
        }

        public Party GetParty(int id)
        {
            Party party = null;
            string sql = "select * from party where id = @PartyId";
            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@PartyId", System.Data.DbType.Int64).Value = id;
                    var queryResult = command.ExecuteReader();
                    if (queryResult.Read())
                    {
                        party =  new Party
                        {
                            Id = Convert.ToInt32(queryResult["id"]),
                            Name = queryResult["name"].ToString(),
                            Date = DateTime.ParseExact(queryResult["date"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            Cancelled = (Cancelled)Convert.ToInt32(queryResult["cancelled"])
                        };
                    }
                }
            }

            return party;
        }

        // RSVP enum 0 - unknown, 1- YES, 2 - Maybe, 3 - NO

        public void InvitePeople(int partyId, IEnumerable<int> people)
        {
            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                foreach (var p in people)
                {
                    var sql = "insert into rsvp (partyId, personId, attending) values (@PartyId,@PersonId, 0)";
                    using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                    {
                        command.Parameters.Add("@PartyId", System.Data.DbType.Int64).Value = partyId;
                        command.Parameters.Add("@PersonId", System.Data.DbType.Int64).Value = p;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RsvpToParty(int partyId, int personId, Attending attending)
        {
            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                var sql = "update rsvp set attending = @Attending where partyId = @PartyId and personId = @PersonId";
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@Attending", System.Data.DbType.Int64).Value = (int)attending;
                    command.Parameters.Add("@PartyId", System.Data.DbType.Int64).Value = partyId;
                    command.Parameters.Add("@PersonId", System.Data.DbType.Int64).Value = personId;
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Rsvp> GetPartyRsvps(int partyId)
        {
            IList<Rsvp> rsvpList = new List<Rsvp>();

            var sql = "select r.attending, r.partyId, r.personId, par.name, per.firstName, per.lastName "
                + "FROM rsvp as r "
                + "JOIN party as par on par.Id = r.partyId "
                + "JOIN person as per on per.ID = r.personId "
                + "where r.partyId = @PartyId";
            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@PartyId", System.Data.DbType.Int64).Value = partyId;
                    var queryResult = command.ExecuteReader();
                    while (queryResult.Read())
                    {
                        rsvpList.Add(new Rsvp
                        {
                            PartyId = Convert.ToInt32(queryResult["partyId"]),
                            PersonId = Convert.ToInt32(queryResult["personId"]),
                            PartyName = queryResult["name"].ToString(),
                            FirstName = queryResult["firstName"].ToString(),
                            LastName = queryResult["lastName"].ToString(),
                            Attending = (Attending)Convert.ToInt32(queryResult["attending"])
                        });
                    }
                }
            }

            return rsvpList;
        }

        public void CancelParty(int partyId)
        {
            using (SQLiteConnection dbConnection = 
                new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;"))
            {
                dbConnection.Open();
                var sql = "update party set cancelled = 1 where id = @PartyId"; 
                using (SQLiteCommand command = new SQLiteCommand(sql, dbConnection))
                {
                    command.Parameters.Add("@PartyId", System.Data.DbType.Int64).Value = partyId;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
