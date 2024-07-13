namespace LMS_BusinessLogic
{
    public enum ValidationResults // Although we have a StatusCodes enum, I think it is not a good idea to import a HTTP package into Services.
    {
        NotFound = 404,
        BadRequest = 400,
        OK = 200
    }
}
