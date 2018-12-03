using System;
using System.Collections;
using System.Collections.Generic;

class HashExample
{
    public static Dictionary<string,ArrayList> makeHashtable(String[] words)
    {
        
        Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();

        string firstWord = words[0];
        string secondWord;

        //  Go through each word in the array words and add each word as a key with the following word as an entry (based on code from Plantinga)
        for (int i = 1; i < words.Length; i++)
        {
                secondWord = words[i];
                if (!hashTable.ContainsKey(firstWord))
                    hashTable.Add(firstWord, new ArrayList());
                hashTable[firstWord].Add(secondWord);
                firstWord = secondWord;
        }

        //  Add last word as a key (if necessary), add the first word as an entry for that key
        if (!hashTable.ContainsKey(words[words.Length - 1]))
            hashTable.Add(words[words.Length - 1], new ArrayList());
        hashTable[words[words.Length - 1]].Add(words[0]);

        return hashTable;
    }

    public static void dump(Dictionary<string,ArrayList> hashTable)
    {
        //  Magically writes each key and entry for each key in a nice format (from Plantinga)
        foreach (KeyValuePair<string, ArrayList> entry in hashTable) {
            Console.Write("{0} -> ", entry.Key);
            foreach (string name in entry.Value)
                Console.Write("{0} ", name);
            Console.WriteLine();
        }
    }

    static void HashMain(String[] args)
    {
        Dictionary<string,ArrayList> hashTable = makeHashtable(args);
        dump(hashTable);
        Console.Write("\nPress enter to exit: ");
        Console.ReadLine();
    }
}

 