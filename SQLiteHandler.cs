using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    internal class SQLiteHandler
    {
        private SQLiteConnection connection;
        private string dbFile = "keywords.sqlite"; // Database file name


        // Connects, creates DB and Table if they don't exist
        public void ConnectToDb()
        {
            // This try block will now catch the *real* problem.
            try
            {
                if (!File.Exists(dbFile))
                {
                    SQLiteConnection.CreateFile(dbFile);
                }

                connection = new SQLiteConnection($"Data Source={dbFile};Version=3;");
                connection.Open();

                // Create table Keywords (id, keyword)
                string sql = "CREATE TABLE IF NOT EXISTS Keywords (id INTEGER PRIMARY KEY AUTOINCREMENT, keyword TEXT NOT NULL UNIQUE)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // THIS IS THE NEW PART:
                // Show the real error in a MessageBox so we can see it.
                MessageBox.Show($"Database connection failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Make sure the connection object is null if it failed
                connection = null;
            }
        }


        // Disconnects from the database 
        public void DisconnectFromDb()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        // Inserts a new keyword 
        public void InsertKeyword(string k)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                ConnectToDb(); // Ensure connection is open
            }

            try
            {
                // "INSERT OR IGNORE" prevents crashes on duplicate UNIQUE keywords
                string sql = "INSERT OR IGNORE INTO Keywords (keyword) VALUES (@keyword)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@keyword", k.ToLower());
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Gets all keywords
        public List<string> GetAllKeywords()
        {
            List<string> keywords = new List<string>();

            // Ensure connection is open
            
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                ConnectToDb(); // This will initialize and open the 'connection' object
            }

            string sql = "SELECT keyword FROM Keywords";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    keywords.Add(reader["keyword"].ToString());
                }
            }
            return keywords;
        }

    }
}
