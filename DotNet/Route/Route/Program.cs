using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Route
{
    public class Program
    {
        class ShowPlace
        {
            private string name;
            private float time;
            private float importance;

            public float Time { get => time;  }
            public string Name { get => name;  }
            public float Importance { get => importance; }

            public ShowPlace(string Line)
            {
                string[] Data = Line.Split(';');
                name = Data[0];
                importance = float.Parse(Data[2]);
                time = float.Parse(Data[1].Replace('ч', ' '));
                
            }
            public override string ToString()
            {
                return Name;
            }
        }
        public static void Main()
        {
            string? filePath ;
            Console.WriteLine("Пусть к csv:");

            while (true)
            {
                filePath = Console.ReadLine();
                if(!File.Exists(filePath)) {
                    Console.WriteLine("Файл не найден.");
                }
                else
                {
                    break;
                }
            }

            const int SleepTime = 2 * 8;
            const int MaxTime = 48;
            string[] Text = File.ReadAllLines(filePath);
            //Массив достопримечательностей
            List<ShowPlace> ShowPlaces=new List<ShowPlace>();

            
            const int TimeForTravel = MaxTime - SleepTime;

            //Парсим csv
            for (int i = 1;i< Text.Length; i++) 
            {
                string Line = Text[i];
                //Логично, что строка должна быть не пустой
                if (Line.Length > 0)
                {
                    ShowPlaces.Add(new ShowPlace(Line));
                }
            }

            //Луший маршрут и его параметры
            List<ShowPlace> BestRoute = new List<ShowPlace>(100);
            float BestImportance = 0;
            float BestTime = 0;
            object locker=new object();
            //Кэшируем Count
            int ShowPlacesLength = ShowPlaces.Count;

            string routeString;

            //for (int i = 1; i <= ShowPlacesLength; i++)
            //Параллельно обходим комбинации
            Parallel.For (1, ShowPlacesLength+1,i=>
            {

                //Обходим всевозмоные комбинации в параллелизме
                //foreach (IEnumerable<ShowPlace> combo in GetCombinations(ShowPlaces, i))
                Parallel.ForEach(GetCombinations(ShowPlaces, i), combo =>
                {
                    float Importance = 0;
                    float Time = 0;

                    //Обходим набор и считаем его параметры
                    foreach (ShowPlace item in combo)
                    {
                        Importance += item.Importance;
                        Time += item.Time;


                    }
                    //Если набор лучше, то сохраняем
                    lock (locker)
                    {
                        if (Time <= TimeForTravel && Importance > BestImportance)
                        {
                            //Обновляем лушее
                            BestTime = Time;
                            BestImportance = Importance;
                            BestRoute = combo.ToList<ShowPlace>();

                            routeString = string.Empty;

                            foreach (var item in BestRoute)
                            {
                                routeString += " " + item.Name;
                            }
                            Console.WriteLine("Currect Best Route:" + routeString + " Importance: " + BestImportance + " Time: " + BestTime);
                        }
                    }
                });
            });

            routeString = string.Empty;
            foreach (var item in BestRoute)
            {
                routeString += " " + item.Name;
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("The Best Route:" + routeString + " Importance: " + BestImportance + " Time: " + BestTime);

        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 0)
            {
                yield return new T[0];
            }
            else
            {
                foreach (var i in Enumerable.Range(0, list.Count()))
                {
                    foreach (var tail in GetCombinations(list.Skip(i + 1), length - 1))
                    {
                        yield return list.Skip(i).Take(1).Concat(tail);
                    }
                }
            }
        }
    }
}
