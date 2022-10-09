using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace OuraRingDataIngest.Migrations
{
    public class Migration4 : MigrationBase
    {
        public override void Up()
        {
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM sys.tables t where t.name = 'HeartRate')
BEGIN
	CREATE TABLE [oura].[HeartRate](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Source] VARCHAR(50) NOT NULL,
        [Bpm] INT NOT NULL,
        [Timestamp] DATETIME NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
            ");
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM oura.HeartRate WHERE Id = 1)
BEGIN
    INSERT INTO [oura].[HeartRate] ([Source], [Bpm], [Timestamp])
    VALUES (N'awake', 60, '2021-01-01 00:00:00')
END
            ");
        }
    }
}
