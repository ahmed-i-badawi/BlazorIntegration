using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto;
public class ResultDto<T>
{
    public List<T> Result { get; set; }
    public int Count { get; set; }

    public ResultDto(List<T> result, int count)
    {
        Result = result;
        Count = count;
    }


}
