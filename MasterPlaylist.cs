using System.Data.SQLite;
using Stuffa;
using System;
using System.Collections.Generic;
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
            else
            {
                Console.WriteLine("Connecting to db file...");
            }

            //insert
            string sql = "CREATE VIRTUAL TABLE IF NOT EXISTS Titles USING fts3(title TEXT, songNr INTEGER)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE IF NOT EXISTS SongPaths (paths varchar(100), songNr INTEGER PRIMARY KEY AUTOINCREMENT)";
            command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE IF NOT EXISTS Bpm (bpm INTEGER, songNr INTEGER)";
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
            string sql = "INSERT INTO SongPaths(paths) VALUES(?)";
            List<string> SQLParams = new List<string>()
            {
                m.getFullPath()
            };
            SQLiteCommand cmd = createCmd(sql, SQLParams);
            cmd.ExecuteNonQuery();
            
            

            sql = "SELECT last_insert_rowid() AS id";

            cmd = new SQLiteCommand(dbConnection);
            cmd.CommandText = sql;
            object reader = cmd.ExecuteScalar();
            

            string lastIdStr;
            lastIdStr = reader.ToString();
            try
            {

                //insert into BPM
                sql = "INSERT INTO Bpm (bpm, songNr) VALUES ( ?, ?)";
                SQLParams = new List<string>()
                {
                    m.getBpm().ToString(),lastIdStr
                };
                createCmd(sql, SQLParams).ExecuteNonQuery();

                //insert into Titles

                //get Titles (all the Titles in the title and artist name)
                char[] spliters = new char[3];
                spliters[0] = ' ';
                spliters[1] = '-';
                spliters[2] = '_';
                List<string> Titles = new List<string>();

                //TODO: fix! have all in one!


                sql = "INSERT INTO Titles (title, songNr) VALUES (?, ?)";
                SQLParams = new List<string>()
                {
                    m.getArtist(),lastIdStr
                };

                createCmd(sql, SQLParams).ExecuteNonQuery();
                


                sql = "INSERT INTO Titles (title, songNr) VALUES ( ?, ?)";
                SQLParams = new List<string>()
                {
                    m.getTitle(),lastIdStr
                };

                createCmd(sql, SQLParams).ExecuteNonQuery();
                
                Console.WriteLine("completed inserting music into DB");

            }
            catch
            {
                Console.WriteLine("could not get songNr or other error in database");
            }



        }

        public List<Music> search(string s)
        {
            Console.WriteLine("searching...");

            List<Music> res = new List<Music>();
            /*
            string sql = "SELECT paths FROM SongPaths WHERE paths LIKE '%" + s + "%'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
                res.Add(reader["paths"].ToString());
                */

            string sql = "SELECT sp.paths as path FROM SongPaths AS sp " +
                "INNER JOIN Titles AS t ON t.songNr  = sp.songNr " +
                "AND t.title MATCH ?";

            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);

            command.Parameters.Add(new SQLiteParameter("param1", s));


            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                //res.Add(reader["title"].ToString());
                res.Add(new Music(reader["path"].ToString()));
                Console.WriteLine(res[res.Count - 1].ToString());
            }
            /*
            //* test
            Console.WriteLine("start test...");

            sql = "SELECT paths AS path FROM SongPaths";

            command = new SQLiteCommand(sql, dbConnection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["path"].ToString());
            }
            Console.WriteLine("end test...");

            ///test
            */

            return res;

        }

        
        private SQLiteCommand createCmd(string text, List<string> param = null)
        {
            SQLiteCommand command = new SQLiteCommand(text, dbConnection);

            int nr = 1;
            if (param != null)
            {
                foreach (string s in param)
                {
                    command.Parameters.Add(new SQLiteParameter("param" + nr, s));
                    nr++;
                }
            }


            return command;
        }

        public void insertNewMusic(List<string> paths)
        {
            Music m;
            foreach(string s in paths)
            {
                m = new Music(s);
                if(!(search(m.Artist).Count > 0 && search(m.getTitle()).Count > 0))
                {
                    insertNewMusic(new Music(s));
                }

            }
        }

        

    }
}
