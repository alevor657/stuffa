using Stuffa;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class MasterPlaylist
    {
        SQLiteConnection dbConnection;

        public MasterPlaylist()
        {

            //create/ connect
            dbConnection =
new SQLiteConnection("Data Source=MasterPlaylist.sqlite;Version=3;");
            dbConnection.Open();
            if (dbConnection == null)
            {
                Console.WriteLine("creating new db...");
                SQLiteConnection.CreateFile("MasterPlaylist.sqlite");
            }

            //insert
            string sql = "CREATE TABLE IF NOT EXISTS Words (word varchar(20), songNr INT)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE IF NOT EXISTS SongPaths (paths varchar(100), songNr INT PRIMARY KEY AUTOINCREMENT)";
            command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE IF NOT EXISTS Bpm (bpm INT, songNr INT)";
            command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();



            



            //--------
            /*
            sql = "select * from highscores order by score desc";
            command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
*/
            
        }
        public void insertNewMusic(Music m)
        {
            string sql = "INSERT INTO SongPaths(paths) VALUES(" + m.getFullPath() + ")";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public List<string> search(string s)
        {
            List<string> res = new List<string>();

            string sql = "SELECT paths FROM SongPaths WHERE paths LIKE '%" + s + "%'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
                res.Add(reader["paths"].ToString());

            return res;

        }

    }
}
