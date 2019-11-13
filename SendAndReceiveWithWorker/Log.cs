namespace SendAndReceiveWithWorker
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Log")]
    public class Log
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Message { get; set; }
    }
}
