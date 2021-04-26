using System.Collections.Generic;
using System.Linq;
using WebAPI.Models.DatabaseTableClasses;
using static System.Convert;

namespace WebAPI.Database.Parser
{
    public static class ParserDataLogs
    {
        private static string SearchInterval(string[] NumberData, string[] dataResult, string[] DataFrom, string[] DataTo, int i, int index=2)
        {
            if (ToInt32(NumberData[index]) > ToInt32(DataFrom[index]) && ToInt32(NumberData[index]) < ToInt32(DataTo[index]))
                return dataResult[i];
            else if (ToInt32(NumberData[index]) == ToInt32(DataFrom[index]) || ToInt32(NumberData[index]) == ToInt32(DataTo[index]))
                return (index == 0) ? dataResult[i] : SearchInterval(NumberData, dataResult, DataFrom, DataTo, i, index - 1);
            else return null;
        }

        public static string[] ParseData(IEnumerable<Logs> logs, string[] DataFrom, string[] DataTo)
        {
            var dataResult = new string[logs.ToList().Count];

            for (int i = 0; i < logs.ToList().Count; i++)
                for (int j = 0; j < logs.ToList()[i].data.Length - "date=".Length; j++)
                    if (logs.ToList()[i].data.Substring(j, "date=".Length) == "date=")
                    {
                        int count = 0;
                        for (int l = j; l < logs.ToList()[i].data.Length - 1; l++)
                        {
                            if (logs.ToList()[i].data.Substring(l, 1) == "\"") { count++; continue; }
                            if (count == 1) dataResult[i] += logs.ToList()[i].data.Substring(l, 1);
                        }
                    }

            for (int i = 0; i < dataResult.Length; i++)
            {
                int index = 0;
                string[] NumberData = new string[3];
                for (int j = 0; j < dataResult[i].Length; j++)
                {
                    if (dataResult[i].Substring(j, 1) == "-") { index++; continue; }
                    NumberData[index] += dataResult[i].Substring(j, 1);
                }
                dataResult[i] = SearchInterval(NumberData, dataResult, DataFrom, DataTo, i); 
            }

            int countNotNull = 0, indexDataFinalResult = 0;
            for (int i = 0; i < dataResult.Length; i++) if (dataResult[i] != null) countNotNull++;

            var dataFinalResult = new string[countNotNull];
            for (int i = 0; i < dataResult.Length; i++)
                if (dataResult[i] != null) dataFinalResult[indexDataFinalResult++] = dataResult[i];

            return dataFinalResult;
        }
    }
}