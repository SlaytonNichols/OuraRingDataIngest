using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace OuraRingDataIngest.ServiceModel
{
    [Schema("oura")]
    public class Executions
    {
        [AutoIncrement]
        public int Id { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public DateTimeOffset StartQueryDateTime { get; set; }
        public DateTimeOffset EndQueryDateTime { get; set; }
        public int RecordsInserted { get; set; }
    }

    [Tag("executions"), Description("Find posts")]
    [Route("/executions", "GET")]
    [ValidateHasRole("Admin")]
    public class QueryExecutions : IQueryDb<Executions>, IReturn<QueryResponse<Executions>>
    {
        int? IQuery.Skip { get; set; }
        int? IQuery.Take { get; set; }
        string IQuery.OrderBy { get; set; }
        string IQuery.OrderByDesc { get; set; }
        string IQuery.Include { get; set; }
        string IQuery.Fields { get; set; }
        Dictionary<string, string> IMeta.Meta { get; set; }
    }

    [Tag("executions"), Description("Create a new record in the heart rate table")]
    [Route("/executions", "POST")]
    [ValidateHasRole("Admin")]
    public class CreateExecution : ICreateDb<Executions>, IReturn<CreateResponse>
    {
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset StartQueryDateTime { get; set; }
        public DateTimeOffset EndQueryDateTime { get; set; }

    }

    public class CreateResponse
    {
        public int Id { get; set; }
    }

    [Tag("executions"), Description("Update an existing post")]
    [Route("/executions/{Id}", "PATCH")]
    [ValidateHasRole("Admin")]
    public class UpdateExecution : IUpdateDb<Executions>, IReturn<UpdateResponse>
    {
        public int Id { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public int RecordsInserted { get; set; }
    }

    public class UpdateResponse
    {
        public int Id { get; set; }
    }
}
