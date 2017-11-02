using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.data
{
    public class SimchaFundDb
    {
        private string _connectionString;

        public SimchaFundDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<Simcha> GetSimchas()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Simchas";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Simcha> result = new List<Simcha> (); 
            while(reader.Read())
            {
                Simcha s = new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"]
                };
                result.Add(s);
            }

            return result;
        }

        public void AddSimcha(Simcha simcha)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Simchas VALUES(@name, @date)";
            command.Parameters.AddWithValue("@name", simcha.Name);
            command.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public Simcha GetSimchaById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Simchas WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            Simcha s = new Simcha();
            while (reader.Read())
            {
                s.Id = (int)reader["Id"];
                s.Name = (string)reader["Name"];
                s.Date = (DateTime)reader["Date"];
            }
            return s;
        }

        public IEnumerable<Contributor> GetContributors()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributors";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contributor> result = new List<Contributor>();
            while (reader.Read())
            {
                Contributor c = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateAdded = (DateTime)reader["DateAdded"]
                };
                result.Add(c);
            }

            return result;
        }
        public int AddContributor(Contributor contr)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributors VALUES(@fName, @lName, @cell, @alwaysInclude, @dateA); SELECT @@Identity;";
            command.Parameters.AddWithValue("@fName", contr.FirstName);
            command.Parameters.AddWithValue("@lName", contr.LastName);
            command.Parameters.AddWithValue("@cell", contr.Cell);
            command.Parameters.AddWithValue("@alwaysInclude", contr.AlwaysInclude);
            command.Parameters.AddWithValue("@dateA", contr.DateAdded);
            connection.Open();
            contr.Id = (int)(decimal)command.ExecuteScalar();
            return contr.Id;
        }

        public void AddDeposit(Deposit deposit)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Deposits VALUES(@date, @amount, @contrId)";
            command.Parameters.AddWithValue("@date", deposit.Date);
            command.Parameters.AddWithValue("@amount", deposit.Amount);
            command.Parameters.AddWithValue("@contrId", deposit.ContributorId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void AddContribution(Contribution contribution)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributions (ContributorId,SimchaId,Amount,Date) VALUES (@contrId,@simchaId,@amount,GETDATE())";
            command.Parameters.AddWithValue("@contrId", contribution.ContributorId);
            command.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
            command.Parameters.AddWithValue("@amount", contribution.Amount);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DeleteSimchaContributions(int simchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public decimal GetContributionTotalForSimcha(int simchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT SUM(Amount) FROM Contributions WHERE SimchaId = @simchaId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            var total = command.ExecuteScalar();
            decimal result = 0;
            if (total != DBNull.Value)
            {
                result = (decimal)(total);
            }
            return result;
            
        }

        public int GetContrCountforSimcha(int simchaId)
        {

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"Select COUNT(*) FROM Contributors JOIN Contributions 
                ON Contributors.Id = Contributions.ContributorId WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id", simchaId);
            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public int GetContrCount()
        {

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "Select COUNT(*) FROM Contributors";
            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public IEnumerable<Contribution> GetContibutorsContributionsForSimcha(int simchaId, int contributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributions WHERE ContributorId = @contrId AND SimchaId =@simchaId";
            command.Parameters.AddWithValue("@contrId", contributorId);
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contribution> result = new List<Contribution>();
            while (reader.Read())
            {
                Contribution c = new Contribution();

                c.ContributorId = (int)reader["ContributorId"];
                c.SimchaId = (int)reader["SimchaId"];
                c.Amount = (decimal)reader["Amount"];
                if (reader["Date"] != DBNull.Value)
                {
                    c.Date = (DateTime?)reader["Date"];
                }

                result.Add(c);
            }

            return result;
        }
        public IEnumerable<Contribution> GetContributionsByContributor(int contributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributions WHERE ContributorId = @contrId";
            command.Parameters.AddWithValue("@contrId", contributorId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contribution> result = new List<Contribution>();
            while (reader.Read())
            {
                Contribution c = new Contribution();

                c.ContributorId = (int)reader["ContributorId"];
                c.SimchaId = (int)reader["SimchaId"];
                c.Amount = (decimal)reader["Amount"];
                    if(reader["Date"] != DBNull.Value)
                {
                    c.Date = (DateTime?)reader["Date"];
                }

                result.Add(c);
            }

            return result;
        }

        public IEnumerable<Deposit> GetDepositsByContributor(int contributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Deposits WHERE ContributorId = @contrId";
            command.Parameters.AddWithValue("@contrId", contributorId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Deposit> result = new List<Deposit>();
            while (reader.Read())
            {
                Deposit d = new Deposit
                {
                    Id = (int)reader["Id"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"],
                    ContributorId = (int)reader["ContributorId"]

                };
                result.Add(d);
            }

            return result;
        }

        public void UpdateContributor(Contributor contr)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Contributors SET FirstName = @fName, LastName = @lName, Cell = @cell, AlwaysInclude = @alwaysInclude, DateAdded = @dateA WHERE Id = @id";
            command.Parameters.AddWithValue("@fName", contr.FirstName);
            command.Parameters.AddWithValue("@lName", contr.LastName);
            command.Parameters.AddWithValue("@cell", contr.Cell ?? "");
            command.Parameters.AddWithValue("@alwaysInclude", contr.AlwaysInclude);
            command.Parameters.AddWithValue("@dateA", contr.DateAdded);
            command.Parameters.AddWithValue("@id", contr.Id);
            connection.Open();
            command.ExecuteNonQuery();

        }

        public Contributor GetContributorById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributors WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            Contributor c = new Contributor();
            while (reader.Read())
            {
                c.Id = (int)reader["Id"];
                c.FirstName = (string)reader["FirstName"];
                c.LastName = (string)reader["LastName"];
                c.Cell = (string)reader["Cell"];
                c.AlwaysInclude = (bool)reader["AlwaysInclude"];
                c.DateAdded = (DateTime)reader["DateAdded"];
            }
            return c;
        }
        public IEnumerable<Contributor> GetContributorsBySimchaId(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"Select * FROM Contributors JOIN Contributions 
                ON Contributors.Id = Contributions.ContributorId WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contributor> result = new List<Contributor>();
            while (reader.Read())
            {
                Contributor c = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateAdded = (DateTime)reader["DateAdded"]
                };
                result.Add(c);
            }

            return result;
        }
    }
}
