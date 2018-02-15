using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Stuffa;

namespace WpfApp2
{
    class Playlist
    {
        
        // sorted lists decleared. first item is BPM value/artist/title second value is index refering to index to songs
        private List<Tuple<int, int>> BPM;
        private List<Tuple<string, int>> artists;
        private List<Tuple<string, int>> titles;

        //seperators for words
        private static Char[] seperators = new Char[] { ' ', '-', '_' };
        
        //ex: path = E:/dir/
        private string path;
        //ex: this_is_a_song
        private string name;
        //ex .mp3
        private string filetype;
        
        //all the music in the playlist
        private List<Stuffa.Music> music;

        
        //sort all BPM in BPM list
        private void sortBPM()
        {
            //sorting with an avrige of O(n log(n))?    (list.Sort is (n log(n)))
            //sorting on BPM
            BPM = BPM.OrderBy(e => e.Item1).ToList();
        }
        //sort all titles in titles list
        private void sortTitles()
        {
            //sort the musics titles
            this.titles = titles.OrderBy(e => e.Item1).ToList();
        }
        //sort all Artist in artist list
        private void sortArtists()
        {
            //sorting with an avrige of O(n log(n))?    (list.Sort is n logn)
            //sorting on BPM
            artists = artists.OrderBy(e => e.Item1).ToList();
        }

        //whnen one instance of the BPM is found, find all the other ones near it matching the value on position pos
        private List<int> getBPMpos(int pos)
        {
            // return value
            List<int> ret = new List<int>();
            //get the vaule we are searching for
            int BPMserach = BPM[pos].Item1;
            //add the BPM we are searching for to the return variable 
            ret.Add(BPM[pos].Item2);

            //go back in list to find other values
            for (int i = pos - 1; i >= 0; i--)
            {
                // if BPM[i] conntains the same value we are searching for
                if (BPM[i].Item1 == BPMserach)
                {
                    //put the index in ret
                    ret.Add(BPM[i].Item2);
                }
                else
                {
                    //end loop if the value is not the one we are looking for
                    // ceep in mind this is a sorted list
                    i = -1;
                }

            }
            //revers return list to get the value in the correct order
            ret.Reverse();

            //go forward in list to find other values
            for (int i = pos + 1; i < BPM.Count; i++)
            {
                if (BPM[i].Item1 == BPMserach)
                {
                    //put the index in ret
                    ret.Add(BPM[i].Item2);
                }
                else
                {
                    //end loop
                    i = BPM.Count;
                }

            }

            return ret;


        }

        private int TupleBinarySearchRecusuve(List<Tuple<string, int>> container, string search, int startPos, int endPos)
        {
            if (startPos < endPos)
            {
                string startVal = container[startPos].Item1;
                string endVal = container[endPos].Item1;


                if (startVal == search)
                {
                    return startPos;
                }
                else if (endVal == search)
                {
                    return endPos;
                }
                else
                {
                    int middlePos = (startPos + endPos) / 2;
                    string middleVal = container[middlePos].Item1;
                    if (middleVal == search)
                    {
                        return middlePos;

                    }
                    else if (search.CompareTo(middleVal) < 0)
                    {
                        return this.TupleBinarySearchRecusuve(container, search, startPos +1 , middlePos -1);


                    }
                    else
                    {
                        return this.TupleBinarySearchRecusuve(container, search, middlePos + 1, endPos - 1);

                    }

                }
            }
            else
            {

                int ret = endPos / 2 + startPos / 2;

                return ret;
            }


        }

        //search binary on forst Tuple and get the position or where it should be
        private int TupleBinarySearch(List<Tuple<string, int>> container, string search)
        {
            return TupleBinarySearchRecusuve(container, search, 0, container.Count -1);
        }

        // gets all indexes for the given BPM. Binary search.
        // TODO: change to (if posible) "this.BPM.binarySearch(....);"
        private List<int> getBPMpos(int BPMsearch, int startPos, int endPos)
        {if (startPos < endPos)
            {
                int startVal = this.BPM[startPos].Item1;
                int endVal = this.BPM[endPos].Item1;


                if (startVal == BPMsearch)
                {
                    return getBPMpos(startPos);
                }
                else if (endVal == BPMsearch)
                {
                    return getBPMpos(endPos);
                }
                else
                {
                    int middlePos = (startPos + endPos) / 2;
                    int middleVal = this.BPM[middlePos].Item1;
                    if (middleVal == BPMsearch)
                    {
                        return getBPMpos(middlePos);

                    }
                    else if (middleVal > BPMsearch)
                    {
                        return this.getBPMpos(BPMsearch, startPos + 1, middlePos - 1);


                    }
                    else
                    {
                        return this.getBPMpos(BPMsearch, middlePos + 1, endPos - 1);

                    }

                }
            }
            else
            {
                List<int> ret = new List<int>();

                if (startPos == endPos)
                {
                    if(BPM[endPos].Item1 == BPMsearch)
                    {
                        ret.Add(BPM[endPos].Item2);
                    }
                }
                return ret;
            }

        }
        //get a list of music given the BPM
        public List<Music> searchBPM(int nr)
        {
            //if the BPM list is empty
            if (BPM.Count == 0)
            {  
                //fill the BPM list with all the music´s BPM
                loadBPM();
            }
            
            //  defining return value
            List<Music> ret = new List<Music>();
            //get every index of music containing the BPM
            List<int> indexes = getBPMpos(nr, 0, BPM.Count-1);
            //for every index...
            foreach (int i in indexes)
            {
                //... add the music on the given index in the music list;
                ret.Add(music[i]);
            }
            return ret;
        }

        //search a given title
        public List<Music> searchTitles(string search)
        {
            // if the titles list is empty
            if(this.titles.Count == 0)
            {
                // fill the titles list with all the titles
                loadTitles();
            }

            // define return value
            List<Music> ret = new List<Music>();
            List<Tuple<int, int>> indexes = similarSentence(titles, search);
            //for every index...
            foreach(Tuple<int, int> i in indexes)
            {
                Console.WriteLine("search res: " + music[i.Item1] + " : " + i.Item2);
                //...add the music on index
                ret.Add(music[i.Item1]);
            }
            return ret;
        }

        //returns a list of indexes witch corresponds to music where some part or the hole search string is defined
        //this function orders the indexes based on the amount matched in the container
        private  List<Tuple<int, int>> similarSentence(List<Tuple<string, int>> container, string search)
        {
            // define return value
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();


            // split the search strin into words
            foreach(string i in search.Split(seperators))
            {
                // check if the search string word is in the container word
                foreach (Tuple<int, int> k in similarWords(container, i))
                {
                    ret.Add(k);
                }
            }
            ret = groupByFirstTupleAndAddSecond(ret);
            return ret;
        }

        //group for exaple indexes and add there scors
        private List<Tuple<int, int>> groupByFirstTupleAndAddSecond(List<Tuple<int, int>> container)
        {
            //define return value
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            // while there are more elements in the container
            while(container.Count >0)
            {
                //take the first elements first Tuple and search for that
                int searchfor = container[0].Item1;
                // secondTuple is for adding together the second Tuple in the container wherever the variable "searchfor" is found
                int secondTuple = container[0].Item2;
                //remove found search values
                container.RemoveAt(0);
                for(int i = 0; i < container.Count; ++i)
                {
                    //go throu the intire container to search for similar "searcfor"
                    if(container[i].Item1 == searchfor)
                    {
                        //when found similar "searchfor". add to secondTupler and remove element from container
                        secondTuple += container[i].Item2;
                        container.RemoveAt(i);
                        --i;
                    }
                }
                //add the thing to the return value
                ret.Add(new Tuple<int, int>(searchfor, secondTuple));
            }

            //order by the second Tuple decending
            ret = ret.OrderBy(e => e.Item2).ToList();
            ret.Reverse();

            return ret;

        }

        //scheck if to words are similar
        //the first Tuple value in the return is the items index the second is the score
        private List<Tuple<int, int>> similarWords(List<Tuple<string, int>> container, string search)
        {
            //define return value
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            //check if any of the strings in container contains the search word
            //linear time
            /*foreach(Tuple<string, int> i in container)
            {
                if(i.Item1 == search)
                {
                    Console.WriteLine("searched for \"" + search + "\" found in: \"" + i.Item1 + "\"");
                    ret.Add(new Tuple<int, int>(i.Item2, 6));
                }

            }*/

            // binary search time to execute (almoste 5% linear)

                int nrOfElements = container.Count;
                int searchArea;
                int pos;
                //if the container have fewer than 561 element search throu all elements
                // to see if it starts or ends with the beginning or end of the search string
                if (nrOfElements > 561)
                {
                    //get the pos thre the search word is or sould be in the container
                    pos = TupleBinarySearch(container, search);
                    // search only 250 + 5% of all the elements
                    searchArea = 250 + nrOfElements / 20;
                }
                else
                {
                    pos = nrOfElements / 2;
                    searchArea = pos;
                }

                //checks every element in range if they match the criterias
                for (int limit = 0; limit < searchArea; limit++)
                {
                    int characters = search.Length;
                    string tempStr = search;
                    while (characters > 0)
                    {
                        if (pos + limit < container.Count && container[pos + limit].Item1.StartsWith(tempStr))
                        {
                            if (characters == search.Length)
                            {

                                //searchword is exactly the same as the string we are evaluating. Give it then a higher score
                                ret.Add(new Tuple<int, int>(container[pos + limit].Item2, characters * 3));

                            }
                            else if(tempStr.Length > 2) //dont have a match if there is fewer than 3 matches on a long word
                            {
                                //searchword only matches partly give a lover score
                                ret.Add(new Tuple<int, int>(container[pos + limit].Item2, characters));
                            }
                        }
                        if (pos - limit > 0 && container[pos - limit].Item1.StartsWith(tempStr))
                        {
                            if (characters == search.Length)
                            {
                                //searchword is exactly the same as the string we are evaluating. Give it then a higher score

                                ret.Add(new Tuple<int, int>(container[pos - limit].Item2, characters * 3));

                            }
                            else if (tempStr.Length > 2)
                            {
                                //searchword only matches partly give a lover score

                                ret.Add(new Tuple<int, int>(container[pos - limit].Item2, characters));
                            }
                        }
                        characters--;
                        tempStr = tempStr.Substring(0, characters);
                    }

                }
            if (search.Length > 2)
            {
                for (int limit = 0; limit < searchArea; limit++)
                {
                    string tempStr = search;
                    while (tempStr.Length - 2 > 0)
                    {

                        if (pos + limit < container.Count && container[pos + limit].Item1.EndsWith(tempStr))
                        {

                            ret.Add(new Tuple<int, int>(container[pos + limit].Item2, search.Length));


                        }
                        if (pos - limit > 0 && container[pos - limit].Item1.EndsWith(tempStr))
                        {

                            ret.Add(new Tuple<int, int>(container[pos - limit].Item2, search.Length));


                        }

                        tempStr = tempStr.Substring(1);
                    }

                }
            }
            

            return ret;
        }

        // default contructor
        public Playlist()
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artists = new List<Tuple<string, int>>();
            this.titles = new List<Tuple<string, int>>();
        }

        // constructor that takes the hole path to the Playlist file. the path is a absulute path
        public Playlist(string fullPath)
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artists = new List<Tuple<string, int>>();
            this.titles = new List<Tuple<string, int>>();

            int pathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (pathPos > 0 && fileTypePos > 0)
            {
                this.path = fullPath.Substring(0, pathPos);
                this.name = fullPath.Substring(pathPos + 1, fileTypePos - pathPos - 1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
        }
        
        // save playlist to file
        public bool savePlaylist()
        {
            // define return value
            bool retVal = false;

            try
            {
                //save all musics absulute path
                List<string> musicTracks = new List<string>();
                foreach (Music i in music)
                {
                    musicTracks.Add(i.getFullPath());
                }
                // package Newtonsoft.Json (Json.Net) need to be installed. 
                // to install go to "project" > "manage NuGet packages..." > "Brows" > type "Newtonsoft.Json" / "Json.Net" > "install"
                
                // convert the musicTracks to a JSON object and save it ass a .txt file where this playlist file exists
                string json = JsonConvert.SerializeObject(musicTracks.ToArray());
                System.IO.File.WriteAllText(this.getFullPath(), json);
                retVal = true;
            }catch { }
            return retVal;
        }
        // open the explorer and save music to the given playlist
        public bool loadNewMusic()
        {
            bool retVal = false;
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";
                dlg.Multiselect = true;

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {

                    // Open document 
                    string[] musicPaths = dlg.FileNames;

                    //check if music allready added
                    foreach (string musicPath in musicPaths)
                    {
                        bool add = true;
                        foreach (Music i in music)
                        {
                            if (i.getFullPath() == musicPath)
                            {
                                add = false;
                            }
                        }
                        if (add)
                        {
                            music.Add(new Music(musicPath));
                        }
                    }

                    //add to array


                    //save to file
                    savePlaylist();
                    retVal = true;

                }

            }
            catch
            {

            }
            return retVal;
        }
        // get music on given index
        public Music getMusic(int index)
        {
            Music ret = null;
            try
            {
                ret = this.music[index];
            }
            catch { }
            return ret;
        }
        
        // load the music specified in the Playlist file
        public void loadMusic()
        {
            try
            {
                using (StreamReader r = new StreamReader(@getFullPath()))
                {
                    //get JSONm object as string
                    string json = r.ReadToEnd();
                    //convert JSON string to a List array of strings
                    List<string> musicTracks = JsonConvert.DeserializeObject<List<string>>(json);

                    //remove any old data
                    music.Clear();

                    //for every music. add to music
                    foreach (string i in musicTracks.ToArray())
                    {
                        Music newMusic = new Music(i);
                        music.Add(newMusic);

                    }
                }
            }
            catch
            {
                Console.WriteLine("could not acces text files on " + path);
            }
        }

        public List<Music> getMusic()
        {
            return this.music;
        }

        public override string ToString()
        {
            return this.name;

        }
        
        // return the path to the Playlist file
        public string getFullPath()
        {
            if(path.EndsWith("\\"))
            {
                return this.path + this.name + this.filetype;
            }
            else
            {
                return this.path + "\\" + this.name + this.filetype;

            }
            
        }


        // insert the BPM and indexes in the BPM list inside this Playlist
        private void loadBPM()
        {
            //clear any old 
            BPM.Clear();
            int index = 0;
            foreach (Music i in music)
            {
                BPM.Add(Tuple.Create<int, int>(i.getBPM(), index));
                index++;
            }
            sortBPM();
        }

        private void loadTitles()
        {
            titles.Clear();
            int index = 0;
            foreach(Music i in music)
            {
                string[] words = i.getTitle().Split(seperators);
                foreach(string word in words)
                {
                    titles.Add(Tuple.Create<string, int>(word.ToLower(), index));

                }
                index++;
            }
            sortTitles();
            
        }

        public void loadArtists()
        {
            artists.Clear();

            int index = 0;

            foreach (Music i in music)
            {
                artists.Add(Tuple.Create<string, int>(i.getArtist(), index));
                index++;
            }
            sortArtists();
        }


    }
}
