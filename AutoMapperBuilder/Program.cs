using AutoMapperBuilder.Enums;
using AutoMapperBuilder.ExpressionCatagories;
using AutoMapperBuilder.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AutoMapperBuilder.Mapping.Mapper;

namespace AutoMapperBuilder
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<Source> sourceDataList = new List<Source>
            {
              new Source
              {
                registerName = "john",
                Number = 1,
                Grades = new List<Grade>
                {
                  new Grade { Subject = "Math", score = 100 }
                 }
              },
              new Source
              {
                registerName = "cindy",
                Number = 2,
                Grades = new List<Grade>
                {
                  new Grade { Subject = "Math", score = 94 }
                }
              },
              new Source
              {
               registerName = "teddy",
               Number = 3,
               Grades = new List<Grade>
               {
                new Grade { Subject = "Math", score = 55 }
               }
              }
            };

            // MethodCall 測試：小寫轉大寫
            Console.WriteLine("===== MethodCall測試 =====  ");
            var config = new PropertyMap<Source, Destination>()
            .ForMember(dest => dest.studentName, src => src.registerName.ToUpper())
            .ForMember(dest => dest.idNumber, src => src.Number)
            .ForMember(dest => dest.Grades, src => src.Grades);

            var mappedList = Mapper.Map<Destination, Source>(sourceDataList, config);

            foreach (var destination in mappedList)
            {
                Console.WriteLine(destination.idNumber);
                Console.WriteLine(destination.studentName);
                foreach (var grade in destination.Grades)
                {
                    Console.WriteLine($"  Subject: {grade.Subject}, Score: {grade.score}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("===== MethodCall測試end =====  ");
            Console.WriteLine(); // 空一行

            // Constant測試：測試新增一筆固定變數的資料
            Console.WriteLine("===== Constant測試 =====  ");
            var config2 = new PropertyMap<Source, Destination>()
            .ForMember(dest => dest.studentName, src => "Lisa")  
            .ForMember(dest => dest.idNumber, src => 4)          
            .ForMember(dest => dest.Grades, src => new List<Grade>{new Grade { Subject = "Math", score = 88 }});

            var mappedList2 = Mapper.Map<Destination, Source>(sourceDataList.Take(1), config2);

            foreach (var destination in mappedList2)
            {
                Console.WriteLine(destination.idNumber);
                Console.WriteLine(destination.studentName);
                foreach (var grade in destination.Grades)
                {
                    Console.WriteLine($"  Subject: {grade.Subject}, Score: {grade.score}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("===== Constant測試end =====  ");
            Console.WriteLine(); 

            // Binary測試:二元運算測試
            Console.WriteLine("===== Binary測試 =====  ");
            var config3 = new PropertyMap<Source, Destination>()
            .ForMember(dest => dest.studentName, src => src.registerName)
            .ForMember(dest => dest.idNumber, src => src.Number)
            .ForMember(dest => dest.Grades, src => src.Grades) 
            .ForMember(dest => dest.isQualified, src => src.Grades[0].score == 100); 
            var mappedList3 = Mapper.Map<Destination, Source>(sourceDataList, config3);
            foreach (var destination in mappedList3)
            {
                Console.WriteLine(destination.idNumber);
                Console.WriteLine(destination.studentName);
                Console.WriteLine($"符合條件: {destination.isQualified}");

                foreach (var grade in destination.Grades)
                {
                    Console.WriteLine($"  Subject: {grade.Subject}, Score: {grade.score}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("===== Binary測試end =====  ");            
            Console.WriteLine();
            
            // Lambda測試
            Console.WriteLine("===== Lambda 測試 =====  ");
            Expression<Func<Source, string>> expr = s => s.registerName;
            var func = expr.Compile();
            foreach (var s in sourceDataList)
            {
                var value = func(s);
                Console.WriteLine(value); 
            }
            Console.WriteLine("===== Lambda 測試end =====  ");
            Console.WriteLine();

            // parameter測試：傳入一個 Lambda 本身當成 Expression 的值








            Console.WriteLine("===== Member不額外測試，上方有使用到formember本身就已經做到測試 =====  ");
            Console.WriteLine();
            
            Console.WriteLine("===== Unary測試 =====  "); //類型轉換(object)x,取反運算!x、-x, Boxing / ValueType boxing
            Expression<Func<Source, bool>> expr2 = s => !s.isQualified;
            var func2 = expr2.Compile(); // 將 Expression 編譯成可執行函數

            var filtered = sourceDataList.Where(func2).ToList();

            foreach (var s in filtered)
            {
                Console.WriteLine($"Name: {s.registerName}, Qualified: {s.isQualified}");
            }
            Console.WriteLine("===== Unary測試end =====  ");
            //var highScorers = mappedList
            // .Where(dest => dest.Grades.Any(g => g.score == 100))
            //.ToList();

            //foreach (var item in highScorers)
            //{
            //    Console.WriteLine($"學生: {item.studentName}, 編號: {item.idNumber}");
            //    foreach (var grade in item.Grades)
            //    {
            //        Console.WriteLine($"  科目: {grade.Subject}, 分數: {grade.score}");
            //    }
            //}





            //var mappedList = Mapper.Map<Destination, Source>(sourceDataList);

            //IEnumerable<Destination> DestinationList = Mapper.Map<Destination, Source>(sourceDataList);

            //foreach (var destination in mappedList)
            //{
            //    Console.WriteLine($"Name: {destination.studentName}, idNumber: {destination.idNumber}");

            //    foreach (var grade in destination.Grades)
            //    {
            //        Console.WriteLine($"  Subject: {grade.Subject}, Score: {grade.score}");
            //    }
            //    Console.WriteLine(); // 空行分隔每個人
            //}

            //Console.WriteLine(mappedList);


            //Student student = new Student()
            //{
            //    Name = "Leo123",
            //    SignIn = 6,
            //    Chinese = 80,
            //    English = 60,
            //    Math = 20
            //};

            //Student student2 = new Student()
            //{
            //    Name = "Kelly",
            //    SignIn = 9,
            //    Chinese = 0,
            //    English = 10,
            //    Math = 95
            //};

            //Student student3 = new Student()
            //{
            //    Name = "Bob",
            //    SignIn = 2,
            //    Chinese = 20,
            //    English = 100,
            //    Math = 70
            //};

            ////委派→一種策略模式
            //    ShowFinalScore(student, x => x.SignIn, x => x.Chinese);
            //    ShowFinalScore(student2, x => x.SignIn, x => x.Math);
            //    ShowFinalScore(student3, x => x.SignIn, x => x.English);
            //}
            //static double AdjustScore(int condition, double score)
            //{
            //    if (condition > 8)
            //    {
            //        return score *= 1.1;
            //    }
            //    else if (condition > 5)
            //    {
            //        return score *= 1.05;
            //    }
            //    else
            //    {
            //        return score;
            //    }
            //}

            //// expression在這邊可解決欄位不對稱的問題，是Expression Tree 的語法，表示一個從 Student 到 double 的函式（Function），但是這個函式是可被分析的表示式樹，不是可直接執行的函式
            //static void ShowFinalScore(Student student, Expression<Func<Student, int>> condition, Expression<Func<Student, double>> score)
            //{
            //    MemberExpression memberExpression = condition.Body as MemberExpression;//嘗試把 Lambda 的主體 s.SignIn 轉成一個 MemberExpression 來進一步分析
            //    if (memberExpression.Member is PropertyInfo property) // 轉成property類型再去拿
            //    {
            //        int value = (int)property.GetValue(student);
            //    }
            //    else if (memberExpression.Member is FieldInfo fieldInfo)
            //    {

            //    }
            //    int a = condition.Compile().Invoke(student);
            //    double b = score.Compile().Invoke(student);
            //    double finalScore = AdjustScore(a, b);
            //    Console.WriteLine($"姓名:{student.Name} 期末成績:{finalScore}");
            //}
        }
    }
}


