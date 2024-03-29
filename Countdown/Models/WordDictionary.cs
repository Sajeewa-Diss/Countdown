﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Countdown.ViewModels;

namespace Countdown.Models
{

    internal class WordDictionary
    {
        private const string cResourceName = "Countdown.Resources.wordlist.dat";
        private const byte cLine_seperator = 46; // full stop
        private const byte cWord_seperator = 32; // space 

        public const int cMinLetters = 4;
        public const int cMaxLetters = 9;

        // conundrum words are 9 letters long with only one solution
        private readonly Dictionary<string, byte[]> conundrumWords = new Dictionary<string, byte[]>();
        private readonly Dictionary<string, byte[]> otherWords = new Dictionary<string, byte[]>();

        private ManualResetEventSlim loadingEvent = new ManualResetEventSlim();


        public WordDictionary()
        {
            Task.Factory.StartNew(() => LoadResourceFile()).ContinueWith((x) => loadingEvent.Set());
        }


        /// <summary>
        /// Finds all matches for the letters supplied. Assumes that the 
        /// letters are all in lower case.
        /// </summary>
        /// <param name="letters"></param>
        /// <returns></returns>
        public List<WordItem> Solve(char[] letters)
        {
            if ((letters is null) || (letters.Length != cMaxLetters))
                throw new ArgumentOutOfRangeException(nameof(letters));

            loadingEvent.Wait();  // until finished loading resources

            List<WordItem> results = new List<WordItem>();

            for (int k = letters.Length; k >= cMinLetters; --k)
            {
                foreach (List<char> chars in new Combinations<char>(letters, k))
                {
                    AddDictionaryWordsToList(chars, otherWords, results);

                    if (k == letters.Length)
                        AddDictionaryWordsToList(chars, conundrumWords, results);
                }
            }

            return results;
        }




        private static void AddDictionaryWordsToList(List<char> keyChars, Dictionary<string, byte[]> dictionary, List<WordItem> list)
        {
            string key = new string(keyChars.ToArray());

            if (dictionary.TryGetValue(key, out byte[] data))
            {
                string line = new string(GetChars(data, data.Length));

                if (line.Length == key.Length)
                    list.Add(new WordItem(line));
                else
                {
                    foreach (string s in line.Split((char)cWord_seperator))
                        list.Add(new WordItem(s));
                }
            }
        }



        public string SolveConundrum(string[] letters)
        {
            if ((letters is null) || (letters.Length != cMaxLetters))
                throw new ArgumentOutOfRangeException(nameof(letters));

            loadingEvent.Wait();  // until finished loading resources

            // convert the data to sorted lower case
            // to build the dictionary key
            char[] conundrum = new char[letters.Length];

            for (int index = 0; index < letters.Length; ++index)
                conundrum[index] = (char)(letters[index][0] | 0x20); // to lower

            Array.Sort(conundrum);
            string key = new string(conundrum);

            if (conundrumWords.TryGetValue(key, out byte[] data))
                return new string(GetChars(data, data.Length, true));

            return null;
        }



        public char[] GetConundrum(Random random)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            loadingEvent.Wait();  // until finished loading resources

            if (conundrumWords.Count > 0)
            {
                // move a random distance into dictionary
                int index = random.Next(conundrumWords.Count) ;

                IEnumerator<byte[]> e = conundrumWords.Values.GetEnumerator();

                while (e.MoveNext() && (index-- > 0)) { }; // empty statement

                return GetChars(e.Current, e.Current.Length, true);
            }

            return null;
        }


        public bool HasConundrums
        {
            get
            {
                loadingEvent.Wait();  // until finished loading resources
                return conundrumWords.Count > 0;
            }
        }



        private void LoadResourceFile()
        {
            Stream resourceStream = typeof(App).Assembly.GetManifestResourceStream(cResourceName);

            if (resourceStream != null)
            {
                using (DeflateStream stream = new DeflateStream(resourceStream, CompressionMode.Decompress))
                {
                    StreamManager sm = new StreamManager(stream);
                    byte[] line;

                    while ((line = sm.ReadLine()) != null)
                    {
                        int keyLength = line.Length;
                        
                        // check for a word break within the line
                        if (keyLength > (cMinLetters * 2))
                        {
                            for (keyLength = cMinLetters; keyLength < line.Length; ++keyLength)
                            {
                                if (line[keyLength] == cWord_seperator)
                                    break;
                            }
                        }
                        
                        // make key
                        char[] c = GetChars(line, keyLength);
                        Array.Sort(c);
                        string key = new string(c);

                        // add to dictionary
                        if ((keyLength == cMaxLetters) && (keyLength == line.Length))
                            conundrumWords[key] = line;
                        else
                            otherWords[key] = line;
                    }
                }
            }
        }


        // a simple encoder, the source is known quantity
        private static char[] GetChars(byte[] bytes, int count, bool toUpper = false)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            if ((count < 0) || (count > bytes.Length))
                throw new ArgumentOutOfRangeException(nameof(count));

            char[] chars = new char[count];
            int index = 0;

            unchecked
            {
                if (toUpper)
                {
                    while (index < count)
                    {
                        chars[index] = (char)(bytes[index++] & ~0x20);
                    }
                }
                else
                {
                    while (index < count)
                    {
                        chars[index] = (char)bytes[index++];
                    }
                }
            }

            return chars;
        }


        ~WordDictionary()
        {
            if (loadingEvent != null)
                loadingEvent.Dispose();
        }


        /// <summary>
        /// Hides the complexity of reading lines from the stream.
        /// Assumes that the maximum line length is going to be 
        /// less than or equal to the buffer size.
        /// </summary>
        private sealed class StreamManager
        {
            private const int cBufferSize = 1024 * 8;

            private readonly DeflateStream stream;
            private readonly byte[] buffer = new byte[cBufferSize];

            private int bufferEnd = 0;
            private int position = 0;
            private bool endOfStream = false;

            public StreamManager(DeflateStream s)
            {
                stream = s ?? throw new ArgumentNullException(nameof(s));
            }


            public byte[] ReadLine()
            {
                int length = SeekNextLine();

                if (length > 0) // simple case
                {
                    // copy line
                    byte[] line = new byte[length];
                    Buffer.BlockCopy(buffer, position, line, 0, length);

                    // update position
                    position += length + 1;
                    return line;
                }
                else if (!endOfStream) // part or all of the line is still in the stream
                {
                    byte[] temp = null;
                    int sizeLeft = bufferEnd - position;

                    if (sizeLeft > 0)  // copy remaining if any
                    {
                        temp = new byte[sizeLeft];
                        Buffer.BlockCopy(buffer, position, temp, 0, sizeLeft);
                    }

                    // always refill the whole buffer
                    bufferEnd = stream.Read(buffer, 0, buffer.Length);
                    endOfStream = bufferEnd < buffer.Length;
                    position = 0;

                    length = SeekNextLine();

                    if (length >= 0)  // its not the end of the stream, it shouldn't be
                    {
                        byte[] line = new byte[sizeLeft + length];

                        if (sizeLeft > 0)  // copy remaining and new part of the buffer
                        {
                            Buffer.BlockCopy(temp, 0, line, 0, temp.Length);
                            Buffer.BlockCopy(buffer, 0, line, temp.Length, length);
                        }
                        else
                            Buffer.BlockCopy(buffer, 0, line, 0, length);

                        // update position
                        position += length + 1;
                        return line;
                    }
                }

                return null;
            }


            private int SeekNextLine()
            {
                for (int index = position; index < bufferEnd; ++index)
                {
                    if (buffer[index] == cLine_seperator)
                        return index - position;
                }

                return -1;   // the buffer doesn't contain a full line
            }
        }
    }
}

