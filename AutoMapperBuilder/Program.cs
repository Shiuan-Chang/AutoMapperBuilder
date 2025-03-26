using AutoMapperBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoMapperBuilder.Mapper;

namespace AutoMapperBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Source> sourceDataList = new List<Source> 
            {
                new Source {Name = "John", idNumber = 01 , score = 100},
                new Source {Name = "Cindy", idNumber = 02 , score = 96},
                new Source {Name = "Teddy", idNumber = 03 , score = 99}
            };

            var mappedList = (List<Destination>)Map<Destination, Source>(sourceDataList);

            //IEnumerable<Destination> DestinationList = Mapper.Map<Destination, Source>(sourceDataList);

            foreach (var destination in mappedList)
            {
                Console.WriteLine($"Name: {destination.Name}, idNumber: {destination.idNumber}, score:{destination.score}");
            }

            Console.WriteLine(mappedList);
        }
    }
}
