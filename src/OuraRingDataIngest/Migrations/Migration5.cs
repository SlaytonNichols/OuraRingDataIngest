using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace OuraRingDataIngest.Migrations
{
    public class Migration5 : MigrationBase
    {
        public override void Up()
        {
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM sys.tables t where t.name = 'Executions')
BEGIN
	CREATE TABLE [oura].[Executions](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[StartDateTime] DateTimeOffset NULL,
        [EndDateTime] DateTimeOffset NULL,
        [StartQueryDateTime] DateTimeOffset NULL,
        [EndQueryDateTime] DateTimeOffset NULL,
        [RecordsInserted] INT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
            ");
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM oura.Executions WHERE Id = 1)
BEGIN
    INSERT INTO [oura].[Executions] ([StartDateTime], [EndDateTime], [StartQueryDateTime], [EndQueryDateTime], [RecordsInserted]) 
    VALUES ('2021-01-01 00:00:00', '2021-01-01 00:00:00', '2021-01-01 00:00:00', '2021-01-01 00:00:00', 0)
END
            ");
        }
    }
}
