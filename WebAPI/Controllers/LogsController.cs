using System;
using System.Web.Http;
using WebAPI.Database.Interaction;
using WebAPI.Database.Generation;
using WebAPI.Models.DatabaseTableClasses;
using WebAPI.Database.Parser;
using WebAPI.Database.Validation;

namespace WebAPI.Controllers
{
    public class LogsCreateController : ApiController
    {
        [Route("api/logs/create")]
        public IHttpActionResult Get(string token, string data)
        {
            if (!ValidationData.Valid(token) || !ValidationData.ValidationLog(data))
                return InternalServerError(new Exception("Token or log failed verification.")); 
            if (InteractionDatabase.Uniqueness("logs", "token", token))
            {
                if (InteractionDatabase.Add(
                    table: "logs",
                    columns: "token, data",
                    data: $"'{token}', '{data}'"
                )) return Ok("Log entry created.");
                else return InternalServerError(new Exception("Internal server error."));
            }
            else return InternalServerError(new Exception("This token already exists."));
        }

        [Route("api/logs/create")]
        public IHttpActionResult Get(string data)
        {
            if (!ValidationData.ValidationLog(data)) return InternalServerError(new Exception("Log failed verification."));
            try
            {
                int count = 0;
                string token = null;
                do 
                {
                    token = GenerationToken.Get(15);
                    if (++count == 20) return InternalServerError(new Exception("Failed to generate token, try again."));
                } while (new PetaPoco.Database("DatabaseConnect").ExecuteScalar<bool>($"select count(*) from logs where token=@token", new { token }));
                if (InteractionDatabase.Add(
                    table: "logs",
                    columns: "token, data",
                    data: $"'{token}', '{data}'"
                  )) return Ok("Log entry created.");
                else return InternalServerError(new Exception("Internal server error."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }

    public class LogsGetController : ApiController
    {
        private string[] ParseDataToSplit(string data, char symbol)
        {
            int index = 0;
            string[] NumberData = new string[3];
            for (int j = 0; j < data.Length; j++)
            {
                if (data.Substring(j, 1) == symbol.ToString()) { index++; continue; }
                NumberData[index] += data.Substring(j, 1);
            }
            return NumberData;
        }

        [Route("api/logs/get")]
        public IHttpActionResult Get(string token, string dataFrom, string dataTo)
        {
            if (!ValidationData.Valid(token) || !ValidationData.Valid(dataFrom) || !ValidationData.Valid(dataTo))
                return InternalServerError(new Exception("Token, dataFrom or dataTo failed verification."));
            try
            {
                var database = new PetaPoco.Database("DatabaseConnect");
                return Ok(ParserDataLogs.ParseData(
                    logs: database.Fetch<Logs>("select * from logs"),
                    DataFrom: ParseDataToSplit(dataFrom, '-'),
                    DataTo: ParseDataToSplit(dataTo, '-')
                ));
            }
            catch { return InternalServerError(new Exception("Internal server error.")); }
        }
       
    }
}
