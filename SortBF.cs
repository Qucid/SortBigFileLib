using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace SortBigFileLib
{
    public class SortBF
    {
        string sortFileFirst; // адрес Big файла
        string sortFileSecond;
        string sortFileNew;
        string status = "";
        long n; // maxline
        string status_iter = "";
        public string GetStatus()
        {
            return status;
        }
        public string GetStatus_Iter()
        {
            return status_iter;
        }
        public SortBF(string fileOne,string fileTwo,string fileNew, long maxLine) // Конструктор
        {
            sortFileFirst = fileOne;
            sortFileSecond = fileTwo;
            sortFileNew = fileNew;
            n = maxLine;
        }
        public void Sort() // Сортировка
        {
            StreamReader sReaderFirst = new StreamReader(sortFileFirst);
            StreamReader sReaderSecond = new StreamReader(sortFileSecond);
            StreamWriter sWriterNew;
            List<Int64> listForRead = new List<Int64>();
            Int64 currentLine = 0;
            int numberSubFile = 0, prevNumberSubFile = 0;
            if (!Directory.Exists("temp/"))
                Directory.CreateDirectory("temp/");
            while (!sReaderFirst.EndOfStream)// Пока не конец файла
            {
                currentLine++;
                listForRead.Add(Int64.Parse(sReaderFirst.ReadLine()));
                if (n == currentLine)
                {
                    status = "Разделение первого файла на блоки.";
                    currentLine = 0; // Сбрасываем счетчик для перехода к следующему файлу
                    StreamWriter sWriter = new StreamWriter("temp/_" + numberSubFile);
                    numberSubFile++; // Прибавление счетчика файлов
                    listForRead.Sort();
                    int Count = listForRead.Count;

                    for (int i = 0; i < Count; i++)
                    {
                        status_iter = (i + 1) + " / " + Count;
                        sWriter.WriteLine(listForRead[i]); // Вывод в файл _numberSubFile
                    }

                    listForRead.Clear(); // Очищение списка, для последующего блока
                    sWriter.Close();
                }
            }
            
            while (!sReaderSecond.EndOfStream)// Пока не конец файла
            {
                status = "Разделение второго файла на блоки.";
                currentLine++;
                listForRead.Add(Int64.Parse(sReaderSecond.ReadLine()));
                if (n == currentLine)
                {
                    currentLine = 0; // Сбрасываем счетчик для перехода к следующему файлу
                    StreamWriter sWriter = new StreamWriter("temp/_" + numberSubFile);
                    numberSubFile++; // Прибавление счетчика файлов
                    listForRead.Sort();
                    int Count = listForRead.Count;

                    for (int i = 0; i < Count; i++)
                    {
                        status_iter = (i + 1) + " / " + Count;
                        sWriter.WriteLine(listForRead[i]); // Вывод в файл _numberSubFile
                    }

                    listForRead.Clear(); // Очищение списка, для последующего блока
                    sWriter.Close();
                }
            }

            sReaderSecond.Close();
            sReaderFirst.Close();
            prevNumberSubFile = numberSubFile;
            numberSubFile = 0;
            Int64 currentIndex = 0;
            while (prevNumberSubFile > 1) // Пока временных файлов больше, чем один
            {
                status = "Сортировка слиянием";
                Int64 FirstElem, SecondElem;
                bool isFirstArray;
                sReaderFirst = new StreamReader("temp/_" + currentIndex++);
                sReaderSecond = new StreamReader("temp/_" + currentIndex++);
                sWriterNew = new StreamWriter("temp/_temp");
                FirstElem = Int64.Parse(sReaderFirst.ReadLine());
                SecondElem = Int64.Parse(sReaderSecond.ReadLine());
                if (FirstElem < SecondElem)
                {
                    isFirstArray = true;
                    sWriterNew.WriteLine(FirstElem);
                }
                else
                {
                    isFirstArray = false;
                    sWriterNew.WriteLine(SecondElem);
                }
                while (!sReaderFirst.EndOfStream && !sReaderSecond.EndOfStream)
                {
                    if (isFirstArray == true)
                    {
                        FirstElem = Int64.Parse(sReaderFirst.ReadLine());
                        if (FirstElem < SecondElem)
                        {
                            
                            isFirstArray = true;
                            sWriterNew.Write(FirstElem);
                        }
                        else
                        {
                            isFirstArray = false;
                            sWriterNew.Write(SecondElem);
                        }
                    }
                    else
                    {
                        SecondElem = Int64.Parse(sReaderSecond.ReadLine());
                        if (FirstElem < SecondElem)
                        {
                            isFirstArray = true;
                            sWriterNew.Write(FirstElem);
                        }
                        else
                        {
                            isFirstArray = false;
                            sWriterNew.Write(SecondElem);
                        }
                    }
                    sWriterNew.WriteLine();
                }
                
                if (isFirstArray) sWriterNew.WriteLine(SecondElem);
                else sWriterNew.WriteLine(FirstElem);
                while (!sReaderFirst.EndOfStream)
                {
                    sWriterNew.WriteLine(sReaderFirst.ReadLine());
                } 
                while (!sReaderSecond.EndOfStream)
                {
                    sWriterNew.WriteLine(sReaderSecond.ReadLine());
                }
                status_iter = currentIndex + " / " + prevNumberSubFile;
                sReaderFirst.Close();
                sReaderSecond.Close();
                sWriterNew.Close();
                File.Delete("temp/_" + (currentIndex - 2)); // Удаление более не нужных временных файлов
                if(currentIndex-1<prevNumberSubFile)
                File.Delete("temp/_" + (currentIndex - 1));
                File.Move("temp/_temp", "temp/_" + numberSubFile); // Переименование текущего временного файла
                numberSubFile++;
                if (prevNumberSubFile % 2 == 1 && currentIndex == prevNumberSubFile-1) // Нечетное количество временных файлов 
                {
                    File.Move("temp/_" + currentIndex, "temp/_" + numberSubFile++);
                    currentIndex++;
                }
                if (currentIndex == prevNumberSubFile) // Последний файл в текущем блоке итераций
                {
                    prevNumberSubFile = numberSubFile;
                    numberSubFile = 0;
                    currentIndex = 0;
                }
            }
            if (File.Exists(sortFileNew))
                File.Delete(sortFileNew);
            File.Move("temp/_0", sortFileNew);
            status = "Завершено.";
            status_iter = "";
            Directory.Delete("temp/");
        }

    }
}
