namespace ThreeOldFloor.Core
{
    public class ThreeOldFloorHttpRequestException : ThreeOldFloorException
    {
        public ThreeOldFloorHttpRequestException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        public int Code { get; set; }
    }
}