using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Arguments;

public class SubmitClickedArg
{
    public SubmitClickedArg(bool isNewRecord, bool isSuccess, string errorMessage = "")
    {
        IsNewRecord = isNewRecord;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
    public bool IsNewRecord { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}
