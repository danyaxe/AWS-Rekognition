using System;
using System.Threading.Tasks;

public class RekognitionRegisterUserParameters
{
    public string FilePath { get; set; }

    public string FileName { get; set; }
}

public class RekognitionRegisterUserResponse
{
    public Task<string> ResponseValue { get; set; }

    public int ResponseCode { get; set; }

    public string ResponseDescription { get; set; }
}

public class RekognitionIsValidUserParameters
{
    public string FilePath { get; set; }

    public string FileName { get; set; }
}

public class RekognitionIsValidUserResponse
{
    public Task<Boolean> ResponseValue { get; set; }

    public int ResponseCode { get; set; }

    public string ResponseDescription { get; set; }
}

public class RekognitionUploadToS3Parameters
{
    public string FilePath { get; set; }

    public string FileName { get; set; }
}

public class RekognitionUploadToS3Response
{
    public Task<Boolean> ResponseValue { get; set; }

    public int ResponseCode { get; set; }

    public string ResponseDescription { get; set; }
}