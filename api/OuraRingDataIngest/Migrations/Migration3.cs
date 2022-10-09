using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace OuraRingDataIngest.Migrations
{
    public class Migration3 : MigrationBase
    {
        public override void Up()
        {
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM sys.tables t where t.name = 'Test')
BEGIN
	CREATE TABLE [oura].[Test](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Test] VARCHAR(50) NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
            ");
            Db.ExecuteSql(@"
IF NOT EXISTS(SELECT 1 FROM oura.Test WHERE Id = 1)
BEGIN
    INSERT INTO [oura].[Post] ([Test])
    VALUES (N'This is a test')
END
            ");
        }
    }
}
