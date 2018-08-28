using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace BIA_Technologies__Parser_
{
    static public class Deserializer
    {
        static public DataSet Deserialize(string filename)
        {
            StringBuilder temp = new StringBuilder();
            try
            {
                using (Stream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    try
                    {
                        int bytesToRead = (int)fs.Length;
                        int bytesRead = 0;
                        byte[] data = new byte[(int)fs.Length];

                        while (bytesToRead > 0)
                        {
                            int bytes = fs.Read(data, bytesRead, bytesToRead);

                            if (bytes == 0)
                                break;

                            bytesRead += bytes;
                            bytesToRead -= bytes;
                        }

                        temp.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            DataSet ds = JsonConvert.DeserializeObject<DataSet>(temp.ToString()) ?? new DataSet();

            return ds;
        }

        static public List<Project> ExtractProjects(DataTable dt)
        {
            List<Project> pjs = new List<Project>();

            foreach (DataRow row in dt.Rows)
            {
                Project temp = new Project()
                {
                    Name = row["Name"].ToString(),
                    FinishDate = DateTime.Parse(row["FinishDate"].ToString()),
                    StartDate = DateTime.Parse(row["StartDate"].ToString()),
                    GUID = Guid.Parse(row["GUID"].ToString()),
                    OwnerGUID = Guid.Parse(row["OwnerGUID"].ToString()),
                };

                pjs.Add(temp);
            }

            return pjs;
        }

        static public List<ProjectOwner> ExtractProjectOwners(DataTable dt)
        {
            List<ProjectOwner> pow = new List<ProjectOwner>();

            foreach (DataRow row in dt.Rows)
            {
                ProjectOwner temp = new ProjectOwner()
                {
                    Name = row["Name"].ToString(),
                    GUID = Guid.Parse(row["GUID"].ToString())
                };

                pow.Add(temp);
            }

            return pow;
        }
    }
}
