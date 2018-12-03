using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems)
                {
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ", name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges)
                {
                    Console.Write("{0} ", e.To().Name);
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Shows all orphans
        private static void Orphans()
        {
            Console.Write("Orphans: ");
            int orphanCount = 0;
            List<GraphNode> nodeList = rg.nodes.GetRange(0, rg.nodes.Count - 1);
            foreach (GraphNode n in nodeList)
            {
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                if (parentEdges.Count == 0)
                {
                    Console.Write("{0} ", n.Name);
                    orphanCount += 1;
                }
            }
            if (orphanCount == 0)
                Console.WriteLine("No orphans found");
            else
            {
                Console.WriteLine();
                Console.WriteLine("{0} orphans found", orphanCount);
            }
        }

        // Prints the shortest connection between two people
        /**
         * Algorithm:
         * have a hash table of visited edges
         * have a queue for nodes to be visited
         * add the start node to the queue
         * while the queue is not empty and (&&) the top node is not the ending node:
         *      dequeue the top node and get the list of its edges
         *      for each of the top node's edges:
         *          if the child for the edge is not in the hash table of visited edges, enqueue the edge
         *          add the edge from the child node to the parent node to the hash table of visited edges
         * if the top node is the ending node
         *      current name keeps track of what node we're on, starting with the name of the ending node
         *      make relationship stack
         *      while current name != start
         *          search the hash table of edges for current name and get the value (name of parent node)
         *          search relationship graph to find relationship between current node and parent node, add it to relationship stack
         *          change current name to name of parent node
         *      while relationship stack is not empty
         *          pop and print top relationship
         * else
         *      print no relationship found
         */
        private static void Bingo(string startString, string endString)
        {
            GraphNode startNode = rg.GetNode(startString);
            GraphNode endNode = rg.GetNode(endString);
            if (startNode != null && endNode != null)
            {
                Hashtable visitedEdges = new Hashtable();
                Queue<GraphNode> nodeBFS= new Queue<GraphNode>();
                nodeBFS.Enqueue(startNode);
                while (nodeBFS.Count != 0 && nodeBFS.Peek() != endNode)
                {
                    GraphNode tempNode = nodeBFS.Dequeue();
                    List<GraphEdge> childEdges = tempNode.GetEdges();
                    foreach (GraphEdge edge in childEdges)
                    {
                        if (!visitedEdges.Contains(edge.To()))
                        {
                            visitedEdges.Add(edge.To(), tempNode);      // keeps track of the spanning tree via edges
                            nodeBFS.Enqueue(edge.To());
                        }
                    }
                }
                if (nodeBFS.Count >= 1 && nodeBFS.Peek() == endNode)
                {
                    GraphNode currentNode = endNode;
                    GraphNode parentNode = endNode;
                    Stack<string> relationshipStack = new Stack<string>();
                    while (currentNode != startNode)
                    {
                        parentNode = (GraphNode) visitedEdges[currentNode];
                        List<GraphEdge> parentEdges = parentNode.GetEdges();
                        foreach (GraphEdge graphEdge in parentEdges)
                        {
                            if (graphEdge.To() == currentNode)
                            {
                                relationshipStack.Push(graphEdge.ToString());
                                break;
                            }
                        }
                        currentNode = parentNode;
                    }
                    while (relationshipStack.Count != 0)
                        Console.Write(relationshipStack.Pop() + "\n");
                }
                else
                {
                    Console.WriteLine("No relationship found between {0} and {1}", startString, endString);
                }
            }
            else
                Console.WriteLine("{0} and/or {1} were not found", startString, endString);
        }

        // Prints the list of all descendants of one person
        /**
         * Algorithm:
         * make a queue to hold all descendants
         * make a hash table to hold the descendants and what level they are
         * enqueue ancestor
         * add the ancestor to the hash table with value 0
         * while descendants queue is not empty:
         *      dequeue the top descendant
         *      get a list of all their edges
         *      for each edge:
         *          if the label is "hasChild":
         *              enqueue the child node
         *              add the child node to the hash table with a value 1 greater than their parent's value in the hash table
         * keep a count
         * figure out how to print all descendants nicely //TODO
         */
        private static void Descendants(string ancestorString)
        {
            GraphNode ancestorNode = rg.GetNode(ancestorString);
            if (ancestorNode != null)
            {
                Queue<GraphNode> descendants = new Queue<GraphNode>();
                Hashtable allDescendants = new Hashtable();
                descendants.Enqueue(ancestorNode);
                allDescendants.Add(ancestorNode, 0);
                while (descendants.Count != 0)
                {
                    GraphNode tempNode = descendants.Dequeue();
                    List<GraphEdge> descentantEdges = tempNode.GetEdges("hasChild");
                    foreach (GraphEdge child in descentantEdges)
                    {
                        if (!allDescendants.Contains(child.To()))
                        {
                            descendants.Enqueue(child.To());
                            allDescendants.Add(child.To(), (int) allDescendants[tempNode] + 1);
                        }

                    }
                }
                //Printing nicely
                int count = 0;
                ICollection descendantKeys = allDescendants.Keys;
                Console.WriteLine("{0} has {1} descendants", ancestorString, allDescendants.Count - 1);
                ICollection Generations = allDescendants.Values;
                int numGenerations = 0;
                foreach (int i in Generations)
                {
                    if (i > numGenerations)
                        numGenerations = i;
                }
                while (true)
                {
                    if (count >= numGenerations)
                        break;
                    foreach (GraphNode descendant in descendantKeys)
                    {
                        if ((int) allDescendants[descendant] == count)
                        {
                            if (count == 0)
                                Console.WriteLine("{0}'s descendants are:", descendant.Name);
                            else if (count == 1)
                                Console.WriteLine("Child: {0}", descendant.Name);
                            else if (count == 2)
                                Console.WriteLine("Grandchild: {0}", descendant.Name);
                            else
                            {
                                string title = "Grandchild";
                                for (int i = 0; i < count; i++)
                                    title = "Great-" + title;
                                Console.WriteLine(title + ": {0}", descendant.Name);
                            }
                        }
                    }
                    count++;
                }
                //Console.WriteLine("{0} descendants found for {1}", numDescendants, ancestorString);
            }
            else
            {
                Console.WriteLine("{0} not found", ancestorString);
            }
        }

        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // orphans comand prints out all orphans
                else if (command == "orphans")
                    Orphans();

                // bingo command prints shortest connection between two people if there is one
                else if (command == "bingo")
                    Bingo(commandWords[1], commandWords[2]);

                // descendants command prints the list of descendants of the given person if there are any
                else if (command == "descendants")
                    Descendants(commandWords[1]);

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname],\n  friends [personname], orphans, bingo [personname1] [personname2], descendants [personname], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
