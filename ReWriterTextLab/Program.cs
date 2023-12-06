using System;
using System.Collections.Generic;
using System.IO; 
using System.Text;  
using System.Text.RegularExpressions;  
using System.Linq; 

// Создаем базовый класс, представляющий символ в тексте
class Symbol
{
    public char Value { get; }  // Значение символа

    // Конструктор класса, принимающий значение символа
    protected Symbol(char value)
    {
        Value = value;
    }
}

// Создаем класс Punctuation, который представляет знаки препинания и наследуется от Symbol
class Punctuation : Symbol
{
    // Конструктор класса, который принимает значение знака препинания и передает его базовому классу
    public Punctuation(char value) : base(value)
    {
    }
}

// Создаем класс Word, который представляет слова и наследуется от Symbol
class Word : Symbol
{
    public string WordValue { get; }  // Значение слова

    // Конструктор класса, который принимает значение слова
    public Word(string value) : base(value.Length > 0 ? value[0] : ' ')
    {
        WordValue = value;
    }
}

// Создаем класс Sentence, который представляет предложение
class Sentence
{
    public List<Symbol> Elements { get; set; } = new List<Symbol>();  // Список элементов предложения

    // Метод для добавления элемента к предложению
    public void AddElement(Symbol element)
    {
        Elements.Add(element);
    }

    // Метод для подсчета количества слов в предложении
    public int WordCount()
    {
        return Elements.Count(element => element is Word);
    }

    // Переопределенный метод для преобразования предложения в строку
    public override string ToString()
    {
        StringBuilder sentenceText = new StringBuilder();  // Создаем объект StringBuilder для построения текста предложения
        // Перебираем все элементы (символы) в предложении
        foreach (var element in Elements)
        {
            sentenceText.Append(element.Value);  // Добавляем значение (символ) элемента к тексту предложения
        }
        return sentenceText.ToString();  // Возвращаем полученное предложение в виде строки
    }
}

// Создаем класс Text
class Text
{
    public List<Sentence> Sentences { get; set; } = new List<Sentence>();  // Список предложений в тексте
    // Метод для добавления предложения к тексту
    public void AddSentence(Sentence sentence)
    {
        Sentences.Add(sentence);
    }
    // Метод для сортировки предложений по количеству слов
    public void SortSentencesByWordCount()
    {
        Sentences.Sort((s1, s2) =>
        {
            var wordCount1 = GetWordCount(s1);  // Вычисляем количество слов в первом предложении
            var wordCount2 = GetWordCount(s2);  // Вычисляем количество слов во втором предложении
            return wordCount1.CompareTo(wordCount2);  // Сравниваем количество слов в двух предложениях и сортируем предложения
        });
    }

    // Метод для подсчета количества слов в предложении
    private int GetWordCount(Sentence sentence)
    {
        int wordCount = 0;  // Переменная для подсчета количества слов в предложении
        bool inWord = false;  // Флаг, указывающий, находится ли текущий символ внутри слова

        // Перебираем все элементы (символы) предложения
        foreach (var element in sentence.Elements)
        {
            if (element is Word word)
            {
                if (!inWord)
                {
                    // Если текущий элемент - слово и мы не находимся внутри слова, то увеличиваем счетчик слов
                    wordCount++;
                    inWord = true;  // Устанавливаем флаг, что мы находимся внутри слова
                }
            }
            else if (element is Punctuation punctuation && punctuation.Value == '-')
            {
                inWord = true;  // Если текущий элемент - дефис, считаем его частью слова
            }
            else
            {
                inWord = false;  // Если текущий элемент не является буквой или дефисом, то мы не находимся внутри слова
            }
        }
        return wordCount;  // Возвращаем общее количество слов в предложении
    }


    // Переопределенный метод для преобразования текста в строку
    public override string ToString()
    {
        StringBuilder text = new StringBuilder();
        foreach (var sentence in Sentences)
        {
            text.Append(sentence);
            text.Append(" ");  // Добавляем пробел после каждого предложения
        }
        return text.ToString().TrimEnd();  // Убираем лишний пробел в конце
    }

    // Метод для разделения текста на предложения
    public static List<Sentence> SplitTextIntoSentences(string text)
    {
        // Заменяем все табуляции на пробелы
        text = Regex.Replace(text, @"\t", " ");

        // Заменяем все последовательности пробелов на один пробел
        text = Regex.Replace(text, @"\s+", " ");

        // Создаем список, в который будем добавлять предложения
        List<Sentence> sentences = new List<Sentence>();

        // Разбиваем текст на предложения с помощью регулярного выражения
        string[] sentenceStrings = Regex.Split(text, @"(?<=[.!?])\s");

        // Обходим каждое предложение
        foreach (string sentenceString in sentenceStrings)
        {
            // Создаем новый объект Sentence для текущего предложения
            Sentence sentence = new Sentence();

            // Обходим каждый символ в предложении
            foreach (char c in sentenceString)
            {
                // Если символ является буквой или цифрой, создаем новый объект Word
                if (char.IsLetterOrDigit(c))
                {
                    sentence.AddElement(new Word(c.ToString()));
                }
                else
                {
                    // Если символ не является буквой или цифрой, создаем объект Punctuation
                    sentence.AddElement(new Punctuation(c));
                }
            }

            // Добавляем текущее предложение в список предложений
            sentences.Add(sentence);
        }

        // Возвращаем список предложений
        return sentences;
    }


    //Метод для поиска и вывода уникальных слов заданной длины в вопросительных предложениях
    public void FindAndPrintWordsWithLength(int length)
    {
        // Обходим каждое предложение в списке Sentences
        foreach (var sentence in Sentences)
        {
            // Проверяем, есть ли в данном предложении вопросительный знак ('?')
            if (sentence.Elements.Any(e => e is Punctuation punctuation && punctuation.Value == '?'))
            {
                // Создаем множество (HashSet) для уникальных слов заданной длины
                var uniqueWords = new HashSet<string>();

                // Создаем строку для текущего слова
                var currentWord = new StringBuilder();

                // Переменная, которая указывает, что мы находимся внутри слова
                var inWord = false;

                // Обходим каждый символ в предложении
                foreach (var element in sentence.Elements)
                {
                    // Если текущий элемент является объектом Word, это часть слова
                    if (element is Word word)
                    {
                        currentWord.Append(word.WordValue); // Добавляем символ в текущее слово
                        inWord = true; // Указываем, что мы находимся внутри слова
                    }
                    // Если текущий элемент является объектом Punctuation и его значение - дефис, он также считается частью слова
                    else if (element is Punctuation punctuation && punctuation.Value == '-')
                    {
                        currentWord.Append(punctuation.Value); // Добавляем дефис к текущему слову
                    }
                    else
                    {
                        // Если не находимся внутри слова и длина текущего слова равна заданной длине,
                        // добавляем его в множество уникальных слов
                        if (inWord && currentWord.Length == length)
                        {
                            uniqueWords.Add(currentWord.ToString());
                        }

                        // Очищаем текущее слово и указываем, что мы не находимся внутри слова
                        currentWord.Clear();
                        inWord = false;
                    }
                }

                // После обхода символов в предложении проверяем, есть ли уникальные слова заданной длины
                if (inWord && currentWord.Length == length)
                {
                    uniqueWords.Add(currentWord.ToString());
                }

                // Если уникальные слова найдены в данном предложении, выводим их на консоль
                if (uniqueWords.Count > 0)
                {
                    Console.WriteLine($"In question sentence: {sentence}, unique words of length {length}: {string.Join(", ", uniqueWords)}");
                }
            }
        }
    }


}

class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = @"C:\Users\gursk\Desktop\input.txt";  // Путь к файлу с входным текстом
        string outputFilePath = @"C:\Users\gursk\Desktop\Output.txt";  // Путь к файлу для сохранения результата

        if (File.Exists(inputFilePath))
        {
            string inputText = File.ReadAllText(inputFilePath);  // Считываем текст из файла

            Console.WriteLine("Input Text:");
            Console.WriteLine(inputText);  // Выводим входной текст

            List<Sentence> sentences = Text.SplitTextIntoSentences(inputText);  // Разделяем текст на предложения
            Text text = new Text();

            foreach (var sentence in sentences)
            {
                text.AddSentence(sentence);
            }
            
            text.SortSentencesByWordCount();  // Сортируем предложения по количеству слов

            int FindUnique = Convert.ToInt32(Console.ReadLine());  // Считываем длину слова для поиска уникальных слов
            text.FindAndPrintWordsWithLength(FindUnique);  // Находим и выводим уникальные слова
            Console.WriteLine($"Words of length {FindUnique} in question sentences have been printed.");  // Выводим сообщение о завершении




            string outputText = text.ToString();
            File.WriteAllText(outputFilePath, outputText);  // Записываем результат в файл

            
            Console.ReadLine();

        }
        else
        {
            Console.WriteLine("Input file not found.");  // Выводим сообщение об ошибке, если файл не найден
        }
    }
    
}




//char[] consonants = { 'б', 'в', 'г', 'д', 'ж', 'з', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ',
//'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ' };

//string inputFilePath = @"C:\Users\gursk\Desktop\input.txt";
//string outputFilePath = @"C:\Users\gursk\Desktop\output.txt";