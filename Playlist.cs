using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Stuffa;
using static System.Net.Mime.MediaTypeNames;
using IdSharp.Tagging.ID3v2;


namespace WpfApp2
{
    public class Playlist
    {

        // sorted lists decleared. first item is BPM value/artist/title second value is index refering to index to songs
        private List<Tuple<int, int>> BPM;
        private List<Tuple<string, int>> artistsWords;
        private List<Tuple<string, int>> titlesWords;

        private List<Tuple<string, int>> fullTitle;

        //seperators for words
        private static Char[] seperators = new Char[] { ' ', '-', '_' };

        //ex: path = E:/dir/
        private string path;
        //ex: this_is_a_song
        private string name;
        //ex .mp3
        private string filetype;

        //all the music in the playlist
        private List<Music> music;

        public Playlist(string name, int blah)
        {
            emptyLists();
            this.name = name;
            music = new List<Music>();
        }

        public Playlist(Playlist pl)
        {
            name = pl.name;
            path = pl.path;
            music = new List<Music>();


        }
        public int getBPM(int pos)
        {
            return this.music[pos].getBpm();
        }

        //this function will add the music given to it
        public void addMusic(Music m)
        {
            int i = music.Count;
            music.Add(m);
            if (this.titlesWords.Count != 0)
            {
                this.addToTitleWordLists(m, i);
            }
            if (this.artistsWords.Count != 0)
            {
                addToArtistWordLists(m, i);
            }
            if (this.BPM.Count != 0)
            {
                addToBPMList(m, i);
            }
            if (fullTitle.Count != 0)
            {
                addToFullTile(m.getTitle(), i);
            }
        }
        public int getSize()
        {
            return this.music.Count();
        }

        //sort full names
        private void sortFullPath()
        {
            this.fullTitle = this.fullTitle.OrderBy(e => e.Item1).ToList();
        }

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
            this.titlesWords = titlesWords.OrderBy(e => e.Item1).ToList();
        }
        //sort all Artist in artist list
        private void sortArtists()
        {
            //sorting with an avrige of O(n log(n))?    (list.Sort is n logn)
            //sorting on BPM
            artistsWords = artistsWords.OrderBy(e => e.Item1).ToList();
        }

        private List<int> getBPMpos(int pos, int range, int searchVal)
        {
            int minVal = searchVal - range;
            int maxVal = searchVal + range;

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
                if (BPM[i].Item1 >= minVal)
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
                if (BPM[i].Item1 <= maxVal)
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

        //whnen one instance of the BPM is found, find all the other ones near it matching the value on position pos
        private List<int> getBPMpos(int pos, int range)
        {
            if (pos <= 0 && pos > music.Count)
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
                    if (BPM[i].Item1 >= BPMserach - range)
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
                    if (BPM[i].Item1 <= BPMserach + range)
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
            return null;

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
                        return this.TupleBinarySearchRecusuve(container, search, startPos + 1, middlePos - 1);


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
            return TupleBinarySearchRecusuve(container, search, 0, container.Count - 1);
        }

        private bool inRange(int searchVal, int range, int val)
        {
            return val >= searchVal - range && val <= searchVal + range;
        }

        // gets all indexes for the given BPM. Binary search.
        // TODO: change to (if posible) "this.BPM.binarySearch(....);"
        private List<int> getBPMpos(int BPMsearch, int startPos, int endPos, int range)
        {
            if (startPos < endPos)
            {
                int startVal = this.BPM[startPos].Item1;
                int endVal = this.BPM[endPos].Item1;

                if (inRange(BPMsearch, range, startVal))
                {
                    //startPos, range,
                    return getBPMpos(startPos, range, BPMsearch);
                }
                else if (inRange(BPMsearch, range, endVal))
                {
                    return getBPMpos(endPos, range, BPMsearch);
                }
                else
                {
                    int middlePos = (startPos + endPos) / 2;
                    int middleVal = this.BPM[middlePos].Item1;
                    if (inRange(BPMsearch, range, middleVal))
                    {
                        return getBPMpos(middlePos, range, BPMsearch);

                    }
                    else if (middleVal > BPMsearch)
                    {
                        return this.getBPMpos(BPMsearch, startPos + 1, middlePos - 1, range);


                    }
                    else
                    {
                        return this.getBPMpos(BPMsearch, middlePos + 1, endPos - 1, range);

                    }

                }
            }
            else
            {
                List<int> ret = new List<int>();

                if (startPos == endPos)
                {
                    if (inRange(BPMsearch, range, BPM[endPos].Item1))
                    {
                        ret.Add(BPM[endPos].Item2);
                    }
                }
                return ret;
            }

        }
        //get a list of music given the BPM
        public List<Music> searchBPM(int nr, int range = 0)
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
            List<int> indexes = getBPMpos(nr, 0, BPM.Count - 1, range);
            //for every index...
            foreach (int i in indexes)
            {
                //... add the music on the given index in the music list;
                ret.Add(music[i]);
            }
            return ret;
        }
        //get a list of music given the BPM
        public List<int> searchBPMIndex(int nr, int range = 0)
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
            return getBPMpos(nr, 0, BPM.Count - 1, range);
        }

        //search a given artist
        public List<Music> searchArtist(String search)
        {
            //if the artists are empty
            if (this.artistsWords.Count == 0)
            {
                //fill artists witch artists
                loadArtists();
            }

            //define return value
            List<Music> ret = new List<Music>();
            List<Tuple<int, int>> indexes = similarSentence(artistsWords, search);

            //for every index
            foreach (Tuple<int, int> i in indexes)
            {
                Console.WriteLine("search res: " + music[i.Item1].getArtist() + " : " + i.Item2);
                // ... add the music on index
                ret.Add(music[i.Item1]);
            }
            return ret;
        }

        //search a given title
        public List<Music> searchTitles(string search)
        {
            // if the titles list is empty
            if (this.titlesWords.Count == 0)
            {
                // fill the titles list with all the titles
                loadTitles();
            }

            // define return value
            List<Music> ret = new List<Music>();
            List<Tuple<int, int>> indexes = similarSentence(titlesWords, search);
            //for every index...
            foreach (Tuple<int, int> i in indexes)
            {
                Console.WriteLine("search res: " + music[i.Item1] + " : " + i.Item2);
                //...add the music on index
                ret.Add(music[i.Item1]);
            }
            return ret;
        }

        public List<Music> searchArtistBpmTitle(String search)
        {
            if (!getIfLoaded())
            {
                loadMusic();
            }

            //if the artists or titles are empty
            if (this.artistsWords.Count == 0)
            {
                //fill artists witch artists
                loadArtists();
            }
            if (this.titlesWords.Count == 0)
            {
                //fill the title list
                loadTitles();
            }



            //define return value
            List<Music> ret = new List<Music>();

            List<Tuple<int, int>> indexes = similarSentence(artistsWords, search);
            indexes.AddRange(similarSentence(titlesWords, search));

            int searchBpm = getInt(search);
            if (indexes.Count == 0 && searchBpm == -1)
            {
                ret = this.music;
            }
            else
            {
                //get a combined score
                indexes = groupByFirstTupleAndAddSecond(indexes);

                
                if (searchBpm != -1)
                {
                    if (this.BPM.Count == 0)
                    {
                        //fill the bpm list
                        loadBPM();
                    }
                    //if there is songs that have been found previusly
                    if (indexes.Count > 0 || search.Contains(' '))
                    {
                        //check if there is any with the bpm or title artist with number
                        for (int i = 0; i < indexes.Count; i++)
                        {
                            //if it does not contain the number
                            if (!(this.music[indexes[i].Item1].getBpm() == searchBpm || this.music[indexes[i].Item1].getArtist().Contains(searchBpm.ToString()) || this.music[indexes[i].Item1].getTitle().Contains(searchBpm.ToString())))
                            {
                                indexes.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        ret = searchBPM(searchBpm, 0);
                        return ret;
                    }
                }

                //for every index
                foreach (Tuple<int, int> i in indexes)
                {
                    Console.WriteLine("search res: " + music[i.Item1].getArtist() + " : " + i.Item2);
                    // ... add the music on index
                    ret.Add(music[i.Item1]);
                }
            }
            return ret;
            
        }

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

        public void SortListOnBPM()
        {
            this.music.Sort((x, y) => x.Bpm.CompareTo(y.Bpm));
            this.savePlaylist();
            this.emptyLists();
        }
        public void SortListOnTitle()
        {
            this.music.Sort((x, y) => x.Title.CompareTo(y.Title));
            this.savePlaylist();
            this.emptyLists();
        }
        public void SortListOnArtist()
        {
            this.music.Sort((x, y) => x.Artist.CompareTo(y.Artist));
            this.savePlaylist();
            this.emptyLists();
        }

        //returns a list of indexes witch corresponds to music where some part or the hole search string is defined
        //this function orders the indexes based on the amount matched in the container
        private List<Tuple<int, int>> similarSentence(List<Tuple<string, int>> container, string search)
        {

            // define return value
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            search = search.ToLower();
            // split the search strin into words
            foreach (string i in search.Split(seperators))
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
            while (container.Count > 0)
            {
                //take the first elements first Tuple and search for that
                int searchfor = container[0].Item1;
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



            // binary search time to execute (almoste 5% linear)

            int nrOfElements = container.Count;
            int pos;
            //if the container have fewer than 561 element search throu all elements
            // to see if it starts or ends with the beginning or end of the search string

            //get the pos thre the search word is or sould be in the container
            pos = TupleBinarySearch(container, search);
            // search only 250 + 5% of all the elements

            bool continueSearch = true;
            int offset = 0;

            //checks every element in range if they match the criterias
            while (continueSearch)// (int limit = 0; limit < searchArea; limit++)
            {
                int characters = search.Length;
                string tempStr = search;

                continueSearch = false;

                while (characters > 0)
                {
                    if (pos + offset < container.Count && container[pos + offset].Item1.StartsWith(tempStr))
                    {
                        if (characters == search.Length)
                        {

                            //searchword is exactly the same as the string we are evaluating. Give it then a higher score
                            ret.Add(new Tuple<int, int>(container[pos + offset].Item2, characters * 3));

                        }
                        else if (tempStr.Length > 2) //dont have a match if there is fewer than 3 matches on a long word
                        {
                            //searchword only matches partly give a lover score
                            ret.Add(new Tuple<int, int>(container[pos + offset].Item2, characters));
                        }
                        continueSearch = true;
                    }
                    if (pos - offset > 0 && container[pos - offset].Item1.StartsWith(tempStr))
                    {
                        if (characters == search.Length)
                        {
                            //searchword is exactly the same as the string we are evaluating. Give it then a higher score

                            ret.Add(new Tuple<int, int>(container[pos - offset].Item2, characters * 3));

                        }
                        else if (tempStr.Length > 2)
                        {
                            //searchword only matches partly give a lover score

                            ret.Add(new Tuple<int, int>(container[pos - offset].Item2, characters));
                        }
                        continueSearch = true;

                    }
                    offset++;
                    characters--;
                    tempStr = tempStr.Substring(0, characters);
                }

            }



            return ret;
        }

        // default contructor
        public Playlist()
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artistsWords = new List<Tuple<string, int>>();
            this.titlesWords = new List<Tuple<string, int>>();
            this.fullTitle = new List<Tuple<string, int>>();
        }

        // constructor that takes the hole path to the Playlist file. the path is a absulute path
        public Playlist(string fullPath)
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artistsWords = new List<Tuple<string, int>>();
            this.titlesWords = new List<Tuple<string, int>>();
            this.fullTitle = new List<Tuple<string, int>>();


            int pathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (pathPos > 0 && fileTypePos > 0)
            {
                this.path = fullPath.Substring(0, pathPos);
                this.name = fullPath.Substring(pathPos + 1, fileTypePos - pathPos - 1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
            //loadMusic();
            //savePlaylist();
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
            }
            catch { }
            return retVal;
        }

        public bool addNewSong(Music song)
        {
            bool didAdd = false;

            if (!music.Contains(song))
            {
                music.Add(song);
                didAdd = true;
            }
            return didAdd;
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
                    List<string> musicPaths = dlg.FileNames.ToList<string>();

                    List<Music> notAdded = loadNewMusic(musicPaths, false);

                    Console.WriteLine("not added: ");
                    foreach (Music NA in notAdded)
                    {
                        Console.WriteLine(NA.ToString());
                    }
                    Console.WriteLine("----------");
                    /*
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
                    }*/

                    //add to array


                    //save to file
                    //savePlaylist();
                    retVal = true;

                }

            }
            catch
            {

            }
            return retVal;
        }


        private List<Music> addNotSameMusic(List<Music> search)
        {
            //define return value
            List<Music> ret = new List<Music>();

            //if container have not been defined

            List<Tuple<string, int>> container = new List<Tuple<string, int>>();
            int index = 0;

            //for every music in playlist
            foreach (Music m in music)
            {
                // insert all music into container and index
                container.Add(new Tuple<string, int>(m.getTitle(), index));
                index++;
            }
            container = container.OrderBy(e => e.Item1).ToList();


            // sortera på musik titlar

            //if there are music in the playlist



            foreach (Music s in search)
            {
                //if the music does not exists (the ID3 tag is unacssesible)
                if (s.getTitle() == "unknown" && s.getTitle() == "unknown" && s.getBpm() == -1)
                {
                    ret.Add(s);
                }
                else
                {




                    if (container.Count != 0 && container.Count < 1500)
                    {
                        //get the postition of a title. This position is not garantid to contain the title name
                        int pos = TupleBinarySearch(container, s.getTitle());
                        bool stop = false;

                        // if the search result is a 100% match
                        if (container[pos].Item1 == s.getTitle())
                        {
                            //go down in the sorted list and get all items with the right title
                            for (int i = pos; i >= 0 && !stop; i--)
                            {
                                //if the artist is the same too (or not defined "unknown")

                                if (container[i].Item1 == s.getTitle() && (music[container[i].Item2].getArtist() == s.getArtist() || s.getArtist() == "unknown"))
                                {

                                    // there is a dublet
                                    ret.Add(s);
                                    stop = true; //stops the search

                                }


                            }
                            //go up in the sorted list and get all items with the right title

                            for (int i = pos + 1; i < container.Count && !stop; i++)
                            {
                                if (container[i].Item1 == s.getTitle())
                                {
                                    if (search[container[i].Item2].getArtist() == s.getArtist())
                                    {
                                        ret.Add(search[container[i].Item2]);
                                        i = container.Count;
                                        stop = true;
                                    }

                                }
                            }
                        }
                        if (!stop)
                        {
                            addMusic(s);

                            container.Add(new Tuple<string, int>(s.getTitle(), index));
                            index++;
                        }

                    }

                    else if (container.Count == 0 && container.Count < 1500)
                    {
                        addMusic(s);

                        container.Add(new Tuple<string, int>(s.getTitle(), index));
                        index++;
                    }
                }
            }
        

    

            return ret;
        }


        //loads new music into the playlist given the paths to the music. returns list with not inserted music if not addAll is defined as "true"
        public List<Music> loadNewMusic(List<string> paths, bool addAll = false)
        {
            List<Music> m = new List<Music>();
            if (paths != null)
            {
                // return value defined
                List<Music> same = new List<Music>();




                foreach (string path in paths)
                {
                    m.Add(new Music(path));

                }
            }

            return loadNewMusic(m, addAll);

        }
        public List<Music> loadNewMusic(List<Music> m, bool addAll = false)
        {
            // return value defined
            List<Music> same = new List<Music>();


            if (addAll)
            {
                for(int i = 0; i < m.Count && this.getSize() < 1500; i++)
                {
                    addMusic(m[i]);
                }



            }
            else
            {
                //if we shall not add all music. return the music that is not inserted
                same = addNotSameMusic(m);
            }


            savePlaylist();
            return same;
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

        public List<Music> getAllMusic()
        {
            return this.music;
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
                    music = new List<Music>();

                    //for every music. add to music
                    foreach (string i in musicTracks.ToArray())
                    {
                        Music newMusic = new Music(i);
                        music.Add(newMusic);

                    }
                    musicTracks = null;
                }
            }
            catch
            {
                Console.WriteLine("could not acces text files on " + path + " or the file is not properly writen");
            }



        }


        public bool getIfLoaded()
        {
            return this.music.Count > 0 ? true : false;
        }

        public override string ToString()
        {
            return this.name;

        }

        // return the path to the Playlist file
        public string getFullPath()
        {
            if (path.EndsWith("\\"))
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
                BPM.Add(Tuple.Create<int, int>(i.getBpm(), index));
                index++;
            }
            sortBPM();
        }

        private void loadTitles()
        {
            titlesWords.Clear();
            int index = 0;
            foreach (Music i in music)
            {
                string[] words = i.getTitle().Split(seperators);
                foreach (string word in words)
                {
                    titlesWords.Add(Tuple.Create<string, int>(word.ToLower(), index));

                }
                index++;
            }
            sortTitles();

        }

        private void loadFullTitle()
        {
            this.fullTitle.Clear();

            int index = 0;

            foreach (Music i in music)
            {
                string title = i.getTitle();

                this.fullTitle.Add(Tuple.Create<string, int>(title, index));

                index++;


            }
            sortFullPath();
        }

        //move music from index to new index
        public void MoveMusic(int index, int newIndex)
        {
            if (index >= 0 && index < music.Count && newIndex >= 0 && newIndex < music.Count - 1)
            {
                Music toMv = music[index];
                RemoveMusic(index);
                music.Insert(newIndex, toMv);
                savePlaylist();


                //TODO not have cant se last music when any music moved without this
                music = new List<Music>();
            }
        }

        public bool RemoveMusic(int index)
        {
            bool ret = false;
            if (index >= 0 && index < music.Count())
            {
                music.RemoveAt(index);
                emptyLists();
                ret = true;
                savePlaylist();

            }
            return ret;
        }


        //kan ta bort fel låt om samma låt finns på flera ställe kanske
        public bool RemoveMusic(Music remove)
        {
            bool removed = false;
            if (this.fullTitle.Count == 0)
            {
                loadFullTitle();
            }
            int rmv = TupleBinarySearch(this.fullTitle, remove.getTitle());

            //see if the position given by the search is the one
            //see if there is any lower with the same title
            //look that the value is not removed first beacuse it will dealete all lists
            for (int i = rmv; !removed && i >= 0; i--)
            {
                //if the title in the list "fullTitle" is the same as the one being removed
                if (fullTitle[i].Item1 == remove.getTitle())
                {
                    //if the path is the same
                    if (music[fullTitle[i].Item2].getFullPath() == remove.getFullPath())
                    {

                        removed = RemoveMusic(fullTitle[i].Item2);


                    }
                }
                else
                {
                    i = -1;
                }
            }
            //see if there is any higher with the same title
            for (int i = rmv; !removed && i < this.fullTitle.Count; i++)
            {
                //if the title in the list "fullTitle" is the same as the one being removed
                if (fullTitle[i].Item1 == remove.getTitle())
                {
                    //if the artist is allso the same
                    if (music[fullTitle[i].Item2].getArtist() == remove.getArtist())
                    {
                        music.RemoveAt(fullTitle[i].Item2);
                        //remove previus search data. The indexes have now changed
                        emptyLists();
                        removed = true;
                    }
                }
                else
                {
                    i = this.fullTitle.Count;
                }
            }
            return removed;
        }

        //if the sorted list does not match up with the playlist reset them
        private void emptyLists()
        {
            this.BPM = new List<Tuple<int, int>>();
            this.titlesWords = new List<Tuple<string, int>>();
            this.artistsWords = new List<Tuple<string, int>>();
            this.fullTitle = new List<Tuple<string, int>>();
        }

        private void loadArtists()
        {
            artistsWords.Clear();

            int index = 0;

            foreach (Music i in music)
            {
                string[] words = i.getArtist().Split(seperators);
                foreach (string word in words)
                {
                    artistsWords.Add(Tuple.Create<string, int>(word.ToLower(), index));
                }
                index++;


            }
            sortArtists();
        }

        private void addToFullTile(string title, int index)
        {
            //define characters to split the string
            char[] spliters = new char[3];
            spliters[0] = ' ';
            spliters[1] = '_';
            spliters[2] = '-';


            //get index at with to be inserted
            int binarySearchIndex = this.TupleBinarySearch(this.fullTitle, title);
            //insert at given index
            this.titlesWords.Insert(binarySearchIndex, new Tuple<string, int>(title, index));


        }

        private void addToTitleWordLists(Music m, int index)
        {
            //define characters to split the string
            char[] spliters = new char[3];
            spliters[0] = ' ';
            spliters[1] = '_';
            spliters[2] = '-';

            //for every word in the title insert it on right position in titleWords

            foreach (string s in m.getTitle().Split(spliters))
            {
                //get index at with to be inserted
                int binarySearchIndex = this.TupleBinarySearch(this.titlesWords, s);
                //insert at given index
                this.titlesWords.Insert(binarySearchIndex, new Tuple<string, int>(s, index));

            }


        }

        private void addToArtistWordLists(Music m, int index)
        {
            //define characters to split the string
            char[] spliters = new char[3];
            spliters[0] = ' ';
            spliters[1] = '_';
            spliters[2] = '-';

            //for every word in the artists name insert it on right position in artistWord
            foreach (string s in m.getArtist().Split(spliters))
            {
                //get index at with to be inserted
                int binarySearchIndex = this.TupleBinarySearch(this.artistsWords, s);
                //insert at given index
                this.artistsWords.Insert(binarySearchIndex, new Tuple<string, int>(s, index));
            }
        }
        private void addToBPMList(Music m, int index)
        {
            /*
            List<int> BPMpos = getBPMpos(m.getBpm(), 0);
            this.BPM.Insert(BPMpos[0], new Tuple<int, int>(m.getBpm(), index));*/
            BPM = new List<Tuple<int, int>>();

        }
        public void generateTestPlaylist()
        {
            // adding 20 0r 19 test music data
            Music test = new Music();
            test.generateTestData("D:\\Nedladdningar\\", "wayback", ".mp3", 100, "Black Jack", "Black Jack - Du Vet");
            music.Add(test);

            test = new Music();
            string path = "D:\\Nedladdningar\\";

            test.generateTestData(path, "Vicetone - Way Back(feat.Cozi Zuehlsdorff)", ".mp3", 101, "Lasse", "Alla Tiders");
            music.Add(test);

            test = new Music();
            test.generateTestData("D:\\Nedladdningar\\", "Gustav_final", ".mp3", 100, "Laleh", "Elephant");
            music.Add(test);

            test = new Music();
            test.generateTestData("D:\\Nedladdningar\\", "Gustav2", ".mp3", 0, "Laleh", "我爱你");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01-Millioner_röda-rosor", ".m4a", 0, "너를 사랑해.", "من عاشقت هستم");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Chandelier", ".m4a", 102, "Brittney", "Chandelier");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Hurt 1", ".mp3", 104, "", "Hurt");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Hurt", ".mp3", 104, "я тебя люблю", "");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 När filmen är slut", ".mp3", 99, "मैं तुम्हें प्यार करता हूँ", "När filmen är slut");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 När löven faller", ".mp3", 100, "ég elska þig", "När löven faller");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Somethings Got A Hold On Me", ".mp3", 109, "Brittney", "Something got a hold on me, maybe a wery long Title but I dont know, do YOU");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Speaking Of Truth", ".m4a", 188, "Gurgle", "Speaking of Truth");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 spår 01 12", ".mp3", 99, "The amaxing", "Don`t hold me");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 spår 01 13", ".mp3", 97, "The amaxing", "Moving Up");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Spår 01 14", ".mp3", 104, "The amaxing", "Song title");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Spår 1 1", ".mp3", 106, "the amaxing", "Fell the rain");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Spår 1", ".mp3", 104, "Rolex", "Never move");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Spår 1 ", ".mp3", 109, "Rolex", "Tick, Tack");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Till landet Ingenstans", ".mp3", 133, "Rymden", "Till landet ingenstans");
            music.Add(test);

            test = new Music();
            test.generateTestData("C:\\music\\", "01 Try", ".m4a", 88, "Jos", "Try");
            music.Add(test);

        }
    }
}