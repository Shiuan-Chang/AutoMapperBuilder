using AutoMapperBuilder.Enums;
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
            List<Source> sourceDataList = new List<Source>
            {
              new Source
              {
                registerName = "John",
                Number = 1,
                Grades = new List<Grade>
                {
                  new Grade { Subject = "Math", score = 100 }
                 }
              },
              new Source
              {
                registerName = "Cindy",
                Number = 2,
                Grades = new List<Grade>
                {
                  new Grade { Subject = "Math", score = 96 }
                }
              },
              new Source
              {
               registerName = "Teddy",
               Number = 3,
               Grades = new List<Grade>
               {
                new Grade { Subject = "Math", score = 99 }
               }
              }
            };

            var config = new PropertyMap<Source, Destination>()
            .ForMember(dest => dest.studentName, src => src.registerName)
    .           ForMember(dest => dest.idNumber, src => src.Number);

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
            //ShowFinalScore(student, x => x.SignIn, x => x.Chinese);
            //ShowFinalScore(student2, x => x.SignIn, x => x.Math);
            //ShowFinalScore(student3, x => x.SignIn, x => x.English);
        }
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

// 這段是用來測試在Mapper中convert的類型
//Console.OutputEncoding = Encoding.UTF8;

//Type stringType = typeof(string);
//Type enumerableType = typeof(MappingTag[]);
//Type classTYpe = typeof(Program);

//Console.WriteLine(stringType.IsClass);

//MappingTag status1 = ConvertType(stringType); //(MappingTag)Enum.Parse(typeof(MappingTag), stringType.Name);
//MappingTag status2 = ConvertType(enumerableType);
//MappingTag status3 = ConvertType(classTYpe);
