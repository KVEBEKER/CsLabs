namespace sem_1_lab_07_directory_tree
{
    public class TreeBuilder
    {
        // Current line in file
        private static int _lineIndex;

        // Current nesting level
        private static int _depth;

        // File content
        private static string[] _lines = null!;

        private static void Main()
        {
            _lines = File.ReadAllLines("tree.txt");

            PrintTree(0);

            Console.ReadLine();
        }

        private static int PrintTree(int startIndex)
        {
            int passedLines = 0;
            for (int i = startIndex; i < _lines.Length; i++)
            {
                var line = _lines[i].Trim();
                passedLines++;

                if (line == "END")
                {
                    _depth--;
                    return passedLines;
                }

                if (line.StartsWith("BEGIN "))
                {
                    var name = line.Substring(5);

                    Console.Write(GetIndent());
                    if (i + 2 >= _lines.Length)
                    {
                        Console.WriteLine("└─" + name);
                    }
                    else
                    {
                        int yourEnd = 1;
                        for (int y = i + 1; y < _lines.Length; y++)
                        {
                            var futureLine = _lines[y].Trim();
                            if (futureLine == "END")
                            {
                                if(yourEnd == 0)
                                {
                                    Console.WriteLine("└─" + name);
                                    break;
                                }
                                yourEnd--;
                            }
                            if (futureLine.StartsWith("BEGIN "))
                            {
                                Console.WriteLine("├─" + name);
                                break;
                            }
                        }
                    }
                    _depth++;
                    int pl = PrintTree(i + 1);
                    i += pl;
                    passedLines += pl;
                    continue;
                } 
                if (line != "")
                {
                    Console.WriteLine(line);
                }

            }
            return passedLines;
        }

        private static string GetIndent()
        {
            var result = "";

            for (var i = 0; i < _depth; i++)
            {
                result += "│ ";
            }

            return result;
        }
    }
}
