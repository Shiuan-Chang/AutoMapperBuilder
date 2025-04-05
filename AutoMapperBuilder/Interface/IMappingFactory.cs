using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapperBuilder.Factories;
using AutoMapperBuilder.Enums;

namespace AutoMapperBuilder.Interface
{
    public interface IMappingFactory
    {
        MappingBase CreateMapping(MappingTag mappingTag);
    }
}
