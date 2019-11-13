namespace SendAndReceiveWithWorker
{
    using System.Data.Entity;

    public class RabbitMqModel : DbContext
    {
        public RabbitMqModel()
            : base("name=Rabbit")
        {
        }

        public virtual DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>()
                .Property(e => e.Message)
                .IsFixedLength();
        }
    }
}
