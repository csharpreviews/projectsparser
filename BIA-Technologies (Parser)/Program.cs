using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Data.Entity;
using System.Data.Linq;
using System.Configuration;
using System.Data.SQLite.Linq;

namespace BIA_Technologies__Parser_
{

    class Program
    {
        static SQLiteConnection conn;

        static void Main(string[] args)
        {
            Thread bkThread = new Thread(() =>
            {
                //[FIXME] Паттерн "Где мой 1998" :) Перепиши на System.Threading.Timer.
                //Период обновления - в конфиг
                while (true)
                {
                    WorkFlowFun();
                    Thread.Sleep(60000);
                }
            }
            );
            bkThread.Start();

            //[FIXME] Зачем тебе петля? Испытываешь процессор на холостую нагрузку?
            while (true)
            {
                if (Console.ReadLine() == "PLS STOP")
                    Environment.Exit(0);
            }            
        }

        static void WorkFlowFun()
        {
            if (File.Exists(@"projects.json"))
            {
                //[FIXME] Уверен, что файл всегда будет десериализовываться?
                //И зачем тебе DataSet, если есть метадата всех моделей?
                DataSet data = Deserializer.Deserialize("projects.json");

                var tables = data.Tables;
                List<Project> projects = Deserializer.ExtractProjects(tables["Projects"]);
                List<ProjectOwner> owners = Deserializer.ExtractProjectOwners(tables["ProjectOwners"]);
                try
                {
                    conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                    conn.Open();

                    //Подготовка к обновлению, т.к. записей немного, то можно перезаписать все
                    using (SQLiteCommand cmd = new SQLiteCommand("Delete from Projects", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand("Delete from ProjectOwners", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    

                    //Запись
                    foreach (var project in projects)
                    {
                        //[FIXME] Хардкооодик. Вынести в ресурсы. Раз уж взялся руками всё делать
                        using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Projects (GUID, Name, OwnerGuid, StartDate, FinishDate) VALUES(?,?,?,?,?)", conn))
                        {
                            cmd.Parameters.AddWithValue("@GUID", project.GUID.ToString());
                            cmd.Parameters.AddWithValue("@Name", project.Name);
                            cmd.Parameters.AddWithValue("@OwnerGuid", project.OwnerGUID.ToString());
                            cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                            cmd.Parameters.AddWithValue("@FinishDate", project.FinishDate);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    foreach (var owner in owners)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO ProjectOwners (GUID, Name) VALUES(?,?)", conn))
                        {
                            cmd.Parameters.AddWithValue("@Guid", owner.GUID.ToString());
                            cmd.Parameters.AddWithValue("@Name", owner.Name);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    //[THINKABOUT] Прибираться надо за собой. Кстати, почему не взлетело?
                    /*
                    DataContext db = new DataContext(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                    Console.WriteLine("Подключение с БД установлено.");

                    db.GetTable<Project>().DeleteAllOnSubmit(db.GetTable<Project>());
                    db.GetTable<ProjectOwner>().DeleteAllOnSubmit(db.GetTable<ProjectOwner>());
                    db.SubmitChanges();
                    db.GetTable<Project>().InsertAllOnSubmit(projects);
                    db.GetTable<ProjectOwner>().InsertAllOnSubmit(owners);
                    db.SubmitChanges();
                    */
                    Console.WriteLine("Данные в БД были обновлены!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Необходимо поместить файл project.json в {0}", Directory.GetCurrentDirectory());
            }
            
            

        }
    }
}
