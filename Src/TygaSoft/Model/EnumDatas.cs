namespace TygaSoft.Model
{
    public enum ResCodeOptions
    {
        Success = 1000,
        Error = 1001
    }

    public enum PayOptions
    {

    }

    public enum PurposeOptions
    {
        ADULT
    }

    public enum ParameterOptions
    {
        Cookie,
        GetOrPost,
        UrlSegment,
        HttpHeader,
        HttpContentHeader,
        RequestBody,
        QueryString
    }

    public enum HttpMethodOptions
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Options,
        Patch,
        Merge,
        Copy
    }
}