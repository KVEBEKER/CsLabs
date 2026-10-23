using System.Xml.Linq;

namespace sem_1_lab_07_directory_tree
{
    public class TreeBuilder
    {
        // Индекс текущей строки в файле
        private static int _lineIndex;

        // Текущий уровень вложенности.
        private static int _depth;

        // Строки исходного файла.
        private static string[] _lines = null!;

        /// <summary>
        /// Читает строки файла
        /// </summary>
        /// <param name="txtDir">Путь к файлу</param>
        /// <exception cref="ArgumentException">Если путь не верен</exception>
        public static void ReadTreeText(string txtDir)
        {
            if (File.Exists(txtDir))
            {
                _lines = File.ReadAllLines(txtDir);
                return;
            }
            throw new ArgumentException();
        }

        private static void Main()
        {
            ReadTreeText("tree.txt");

            PrintTree();

            Console.ReadLine();
        }

        /// <summary>
        /// Первая исправленная версия. Работала через i, к которому прибавляли значений пройденных линий из рекурсивных вызовов функции 
        /// </summary>
        /// <param name="startIndex">С какого индекса начать работу(рекурсию)</param>
        /// <returns>Количество пройденных функцией строк</returns>
        public static int PrintTree(int startIndex)
        {
            int passedLines = 0;
            for (int i = startIndex; i < _lines.Length; i++)
            {
                //Чтение строки и прибавление как прочитанное
                var line = _lines[i].Trim();
                passedLines++;
                //Если конец - уменьшает вложенность и возвращает пройденные строки
                if (line == "END")
                {
                    _depth--;
                    return passedLines;
                }
                //Если начало
                if (line.StartsWith("BEGIN "))
                {
                    //Получаем имя элемента
                    var name = line.Substring(5);
                    //Вводим глубину
                    Console.Write(GetIndent());
                    //Определяет конец ветки или нет
                    //Если конец всего дерева или нет
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
                                if (yourEnd == 0)
                                {
                                    Console.WriteLine("└─" + name);
                                    break;
                                }
                                yourEnd--;
                            }
                            if (futureLine.StartsWith("BEGIN "))
                            {
                                if (yourEnd == 0)
                                {
                                    Console.WriteLine("├─" + name);
                                    break;
                                }
                                yourEnd++;
                            }
                        }
                    }
                    //Повышаем вложенность
                    _depth++;
                    //Вызываем рекурсию и получаем количество пройденной рекурсией строк 
                    int pl = PrintTree(i + 1);
                    //И прибавляем к локальному индексу и пройденных этой функцией строк.
                    i += pl;
                    passedLines += pl;
                    continue;
                }
                //Если это просто название древа (первая строка А)
                if (line != "")
                {
                    Console.WriteLine(line);
                }

            }
            //Возвращает количество пройденных строк если дошел до конца
            return passedLines;
        }
        private static void PrintTree()
        {
            while (_lineIndex < _lines.Length)
            {
                //Чтение строки
                var line = _lines[_lineIndex].Trim();
                //Если конец - уменьшает вложенность и проходит дальше(чтобы остальные вызовы выше не упали в этот end)
                if (line == "END")
                {
                    _depth--;
                    _lineIndex++;
                    return;
                }
                //Если начало
                if (line.StartsWith("BEGIN "))
                {
                    //Получаем имя элемента
                    var name = line.Substring(5);
                    //Вводим глубину
                    Console.Write(GetIndent());
                    //Теперь это отдельная функция как и печать вложенности
                    Console.WriteLine(GetTreeBranch()+name);
                    //Повышаем вложенность и индекс(чтобы не зарекурсить на одной и то же строке)
                    //И вызываем саму рекурсию
                    _depth++;
                    _lineIndex++;
                    PrintTree();
                    continue;
                }
                //Если это просто название древа (первая строка А)
                if (line != "")
                {
                    Console.WriteLine(line);
                }
                //Если не перехвачен Begin или End, то к следующей строке
                _lineIndex++;
            }
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

        private static string GetTreeBranch()
        {
            int yourEnd = 1;
            //Начинаем цикл чтения строк
            for (int y = _lineIndex + 1; y < _lines.Length; y++)
            {
                //Чтение строки
                var futureLine = _lines[y].Trim();
                //Если конец
                if (futureLine == "END")
                {
                    //Если конец элемента с которого начали уже нашли(yourEnd == 0), то это конец ветки
                    if (yourEnd == 0)
                    {
                        return "└─";
                    }
                    //Иначе это конец это/вложенного элемента
                    yourEnd--;
                }
                if (futureLine.StartsWith("BEGIN "))
                {
                    //Если конец Элемента с которого начали уже нашли(yourEnd == 0), то это не последний элемент ветки
                    if (yourEnd == 0)
                    {
                        return "├─";
                    }
                    //Иначе это вложенный элемент и надо найти и вычитает его конец прежде чем продолжить
                    yourEnd++;
                }
            }
            //Если дерево вообще закончилось
            return "└─";
        }
    }
}
