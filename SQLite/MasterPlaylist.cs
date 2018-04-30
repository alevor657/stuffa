using System.Data.SQLite;
using Stuffa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WpfApp2.SQLite;

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

                //get Titles (all the Titles and artist names)


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

        //get the last int sourounded of "split" characters
        private int getInt(string str)
        {
            char[] splitOn = new char[3];
            splitOn[0] = ' ';
            splitOn[1] = '-';
            splitOn[2] = '_';

            int ret = -1;
            int temp;
            string[] splitStr = str.Split(splitOn);

            foreach (string s in splitStr)
            {
                if (int.TryParse(s, out temp))
                {
                    ret = temp;
                }

            }
            //int.TryParse(splitStr[])

            return ret;
        }

        public List<Music> search(string s, bool searchExact = false)
        {

            List<Music> m = new List<Music>();


            Console.WriteLine("nr of titles : " + this.nrOfTitles());
            Console.WriteLine("searching...");

            List<Tuple<string, int>> res = new List<Tuple<string, int>>();
            /*
            string sql = "SELECT paths FROM SongPaths WHERE paths LIKE '%" + s + "%'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
                res.Add(reader["paths"].ToString());
                */

            List<string> searchWordList = new List<string>();

            char[] spliters = new char[4];
            spliters[0] = ' ';
            spliters[1] = '-';
            spliters[2] = '_';
            spliters[3] = '"';

            if (searchExact)
            {
                searchWordList.Add(s);
            }
            else
            {
                foreach (string searchWordAdd in s.Split(spliters))
                {
                    searchWordList.Add(searchWordAdd + "*");

                }
            }


            string searchWord;
            string sql;
            SQLiteCommand command;
            //split the search into words
            for (int i = 0; i < searchWordList.Count; i++)
                //(string searchWord in searchWordList)
            {
                searchWord = searchWordList[i];
                //sql code for search
                sql = "SELECT sp.paths as path FROM SongPaths AS sp " +
                "INNER JOIN Titles AS t ON t.songNr  = sp.songNr " +
                "AND t.title MATCH ?";

                //insert sql command
                command = new SQLiteCommand(sql, dbConnection);

                //insert parameter. Have a "*" at the end to indicate that it should take strings that have words beginning with the searchWord
                command.Parameters.Add(new SQLiteParameter("param1", searchWord));

                //execute the command
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    //read the result
                    while (reader.Read())
                    {
                        //add the result to res with a score
                        res.Add(new Tuple<string, int>(reader["path"].ToString(), searchWord.Length));
                    }
                }
                else if(searchWord.Length > 3 && !searchExact)
                {
                    searchWordList.Add(searchWord.Substring(0, searchWord.Length - 1));
                }
            }

            int bpm = this.getInt(s);
            if(bpm != -1)
            {
                if(res.Count > 0 && s.Split(spliters).Length > 1)
                {
                    
                    //sök på de obj i res
                    for(int i = 0; i < res.Count; i++)
                    {
                        Music mBpm = new Music(res[i].Item1);

                        if (!(mBpm.getArtist().Contains(bpm.ToString()) || mBpm.getTitle().Contains(bpm.ToString()) || mBpm.getBpm() == bpm))
                        {
                            //if the music does not contain the number remove it
                            res.RemoveAt(i);
                            i--;

                        }
                    }


                    res = this.groupByFirstTupleAndAddSecond(res);
                    //transform paths to music obj
                    for (int i = 0; i < res.Count; i++)
                    {
                        Music check = new Music(res[i].Item1);
                        if (check.getBpm() != bpm)
                        {
                            //the id3 tag have been updated externaly. change it in the database
                            this.updateMusic(check.getBpm(), check.getFullPath());
                        }
                        
                        
                        m.Add(new Music(res[i].Item1));
                        
                    }



                }
                else
                {
                    //search throu the db for all instances with correct bpm
                    //sql code for search
                    sql = "SELECT sp.paths as path FROM SongPaths AS sp " +
                    "INNER JOIN Bpm AS b ON b.songNr  = sp.songNr " +
                    "AND b.bpm = ?";
                    
                    //insert sql command
                    command = new SQLiteCommand(sql, dbConnection);

                    //insert parameter.
                    command.Parameters.Add(new SQLiteParameter("param1", bpm));

                    //execute the command
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //read the result
                        while (reader.Read())
                        {
                            //add the result to res with a 1
                            res.Add(new Tuple<string, int>(reader["path"].ToString(), 1));
                        }
                    }



                    res = this.groupByFirstTupleAndAddSecond(res);
                    //transform paths to music obj
                    for (int i = 0; i < res.Count; i++)
                    {
                        Music check = new Music(res[i].Item1);
                        if (check.getBpm() != bpm)
                        {
                            //the id3 tag have been updated externaly. change it in the database
                            this.updateMusic(check.getBpm(), check.getFullPath());
                        }
                        else
                        {
                            m.Add(new Music(res[i].Item1));
                        }
                    }




                }

                //check if bpm matches with the id3 tag



            }
            else
            {
                
                //make songs found multiple time on top of the list
                res = this.groupByFirstTupleAndAddSecond(res);
                res = this.removeLowSearchRes(res);

                //transform paths to music obj
                for (int i = 0; i < res.Count; i++)
                {
                    m.Add(new Music(res[i].Item1));
                }
            }






            return m;

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

        [STAThread]
        public void insertNewMusic(List<string> paths, loadingWindow loadingWin)
        {
            if(paths != null)
            { 
            //loadingWindow loadingWin = new loadingWindow("Saving new music.\ndo not exit program while loading");
            loadingWin.setMax(paths.Count);
            Music m;

                foreach (string s in paths)
                {
                m = new Music(s);
                List<Music> sameTitle = new List<Music>();

                if(!(search(m.Artist, true).Count > 0 && search(m.getTitle(), true).Count > 0))
                {
                    bool add = true;
                    foreach(Music same in sameTitle)
                    {
                        if(same.getArtist() == m.getArtist())
                        {
                            add = false;
                        }
                    }
                    if (add)
                    {
                        insertNewMusic(new Music(s));
                    }
                }
                loadingWin.increasePos();

                }
            }
            
        }

        private int nrOfTitles()
        {
            string sql = "SELECT Count(*) AS nr FROM Titles;";
            string ret = "-1";
            SQLiteCommand cmd = new SQLiteCommand(dbConnection);
            cmd.CommandText = sql;

            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                ret = reader["nr"].ToString();
            }
            try
            {
                return Int32.Parse(ret);
            }
            catch
            {
                return -1;
            }
        }

        private List<Tuple<string, int>> removeLowSearchRes(List<Tuple<string, int>> container)
        {
            List<Tuple<string, int>> ret = new List<Tuple<string, int>>();

            if (container.Count > 0)
            {
                int maxPoint = container[0].Item2;
                if(maxPoint > 4)
                {
                    int i;
                    for (i = 0; i < container.Count && container[i].Item2 > (int)((float)maxPoint / (float)2); i++)
                    {
                    }
                    ret.AddRange(container.GetRange(0, i));
                }
                else
                {
                    ret = container;
                }
                
            }
            return ret;
            
        }

        //group for exaple indexes and add there scors
        private List<Tuple<string, int>> groupByFirstTupleAndAddSecond(List<Tuple<string, int>> container)
        {
            //define return value
            List<Tuple<string, int>> ret = new List<Tuple<string, int>>();

            // while there are more elements in the container
            while (container.Count > 0)
            {
                //take the first elements first Tuple and search for that
                string searchfor = container[0].Item1;
                // secondTuple is for adding together the second Tuple in the container wherever the variable "searchfor" is found
                int secondTuple = container[0].Item2;
                //remove found search values
                container.RemoveAt(0);
                for (int i = 0; i < container.Count; ++i)
                {
                    //go throu the intire container to search for similar "searcfor"
                    if (container[i].Item1 == searchfor)
                    {
                        //when found similar "searchfor". add to secondTupler and remove element from container
                        secondTuple += container[i].Item2;
                        container.RemoveAt(i);
                        --i;
                    }
                }
                //add the thing to the return value
                ret.Add(new Tuple<string, int>(searchfor, secondTuple));
            }

            //order by the second Tuple decending
            ret = ret.OrderBy(e => e.Item2).ToList();
            ret.Reverse();

            return ret;

        }

        public void InsertNewMusicThread(List<string> paths)
        {
            if (paths != null)
            {
                if (paths.Count > 0)
                {


                    loadingWindow loadingWinMain = new loadingWindow("Saving new music.\ndo not exit program while loading");
                    //loadingWinMain.setMax(paths.Count);

                    Thread myNewThread = new Thread(() => insertNewMusic(paths, loadingWinMain));
                    myNewThread.IsBackground = true;
                    myNewThread.SetApartmentState(ApartmentState.STA);
                    myNewThread.Start();
                }
            }
        }
        /*
        public void updateMusic(Music oldMusic, Music newMusic)
        {
            string sql = "UPDATE Titles SET title = ? WHERE title = ?; " + //title
                "UPDATE Titles SET title = ? WHERE title = ?; " + //artist
                "UPDATE SongPaths SET paths = ? WHERE paths = ?; " +  //file path
                "UPDATE Bpm SET bpm = ? WHERE bpm = ?;";  // bpm value


            List<string> SQLParams = new List<string>()
                {
                    newMusic.getTitle(), oldMusic.getTitle(),
                    newMusic.getArtist(), oldMusic.getArtist(),
                    newMusic.getFullPath(), oldMusic.getFullPath(),
                    newMusic.getBpm().ToString(), oldMusic.getBpm().ToString()
                };
            createCmd(sql, SQLParams).ExecuteNonQuery();

        }*/

        public void updateMusic(int newBpm, string path)
        {
            string sql = "UPDATE Bpm SET bpm = ? WHERE (SELECT songNr FROM SongPaths WHERE songNr = Bpm.songNr AND SongPaths.paths = ?); ";

            /*
             * (SELECT bpm, SongPaths.paths AS path FROM Bpm INNER JOIN SongPaths ON SongPaths.songNr = Bpm.songNr)
             * 
            sql = "SELECT sp.paths as path FROM SongPaths AS sp " +
            "INNER JOIN Bpm AS b ON b.songNr  = sp.songNr " +
            "AND b.bpm = ?";
            */

            List<string> SQLParams = new List<string>()
                {
                   newBpm.ToString(), path
                };
            createCmd(sql, SQLParams).ExecuteNonQuery();

        }

        public void updateMusic(string path, int newBpm, string Title, string Artist)
        {
            updateMusic(newBpm, path);

            string sql = "UPDATE Titles SET title = ? WHERE (SELECT songNr FROM SongPaths WHERE songNr = Titles.songNr AND SongPaths.paths = ?); ";

            List<string> SQLParams = new List<string>()
                {
                   Title, path
                };
            createCmd(sql, SQLParams).ExecuteNonQuery();

            sql = "UPDATE Titles SET title = ? WHERE (SELECT songNr FROM SongPaths WHERE songNr = Titles.songNr AND SongPaths.paths = ?); ";

            SQLParams = new List<string>()
                {
                   Artist, path
                };
            createCmd(sql, SQLParams).ExecuteNonQuery();

        }

    }


}
