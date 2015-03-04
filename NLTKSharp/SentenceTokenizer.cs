﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NLTKSharp
{
    public static class SentenceTokenizer
    {
        static string[] knownAbbreviations = new string[]{"etc.","mr.","mrs."};
        public static List<string> Tokenize(string input)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            int lastSpaceIndex = Int16.MaxValue;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == ' ')
                {
                    lastSpaceIndex = i;
                }
                sb.Append(ch);
                int nextNonWhitespaceCharDist = input.Substring(i + 1).TakeWhile(c => (char.IsWhiteSpace(c)||c=='\n'||c=='\r')).Count();
                if (nextNonWhitespaceCharDist > 2)
                {
                    //mark as EOS
                    i = i + nextNonWhitespaceCharDist;
                    result.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                else
                {
                    if ((ch == '.'||ch==':'||ch=='?'||ch=='!') && i < input.Length - 1)
                    {
                        StringBuilder sbWord = new StringBuilder();
                        for (int k = lastSpaceIndex + 1; k <= i; k++)
                        {
                            sbWord.Append(input[k]);
                        }
                        string currWord = sbWord.ToString();
                        if (knownAbbreviations.Any(x => sb.ToString().EndsWith(x)))
                        {
                            //just an abbreviation, move to next
                        }
                        else if (currWord.Length <= 4)
                        {
                            int n;
                            bool isNumeric = int.TryParse(currWord, out n);
                            if (!isNumeric)
                            {
                                //word is small, probably an abbreviation
                                if (currWord.All(c => char.IsUpper(c)))
                                {
                                    //all uppercase, very likely an abbreviation
                                }
                                else
                                {
                                    //possible eos
                                    result.Add(sb.ToString());
                                    sb = new StringBuilder();
                                }
                            }
                        }
                        else
                        {
                            char nextChar = input[i + 1];
                            char nextNonWhitespaceChar = input[i + nextNonWhitespaceCharDist+1];
                            if (char.IsUpper(nextNonWhitespaceChar) || nextChar == '\r' || nextChar == '\n')
                            {
                                //next character is uppercase
                                //likely eos
                                result.Add(sb.ToString());
                                sb = new StringBuilder();
                            }
                        }
                    }

                }
            }
            result.Add(sb.ToString());
            return result;
        } 
    }
}
