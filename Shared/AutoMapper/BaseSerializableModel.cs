using AutoMapper;
using CoreLibrary.Shared.Helpers;
using System.Text;

namespace CoreLibrary.Shared.AutoMapper
{
    public class BaseSerializableModel
    {
    }

    public class BaseMapper : Profile
    {
        public BaseMapper()
        {
            CreateMap<byte[], string>().ConvertUsing(x => Encoding.UTF8.GetString(x));
            CreateMap<string, byte[]>().ConvertUsing(x => Encoding.UTF8.GetBytes(x));

            CreateMap<BaseSerializableModel, string>()
                .ConstructUsing(x => x.Serialize())
                .IncludeAllDerived();
        }
    }
}
