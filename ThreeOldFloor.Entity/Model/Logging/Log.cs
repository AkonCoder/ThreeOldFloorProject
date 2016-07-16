using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThreeOldFloor.Data.MicroOrm.Attributes;
using ThreeOldFloor.Entity.Enum;

namespace ThreeOldFloor.Entity.Model.Logging
{
    [Table("Logs")]
    public class Log 
    {
        [Identity]
        [Key]
        public int LogId { get; set; }

        public string ShortMessage { get; set; }

        public string FullMessage { get; set; }

    
        public string IpAddress { get; set; }


        public string PageUrl { get; set; }

   
        public string ReferrerUrl { get; set; }


        public DateTime CreatedOnUtc { get; set; }


        public string RequestLogId { get; set; }

        public LogLevel LogLevel { get; set; }

    
    }
}
