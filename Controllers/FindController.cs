using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiFindJson.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiFindJson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindController : ControllerBase
    {
        // GET api/Find
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Masha", "Sasha" };
        }

        // GET api/Find/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/Find
        [HttpPost]
        public JsonResult Post([FromBody] Search root)
        {
            // Data structure to hold names of subfolders to be 
            // examined for files. 
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(root.Root))
            {
                throw new ArgumentException();
            }

            Result_Data result_Data;
            Result_List result_List = new Result_List();
            List<Result_Data> results = new List<Result_Data>();
            int amount = 0;
            dirs.Push(root.Root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have 
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by 
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run. 
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                // Perform the required action on each file here. 
                // Modify this block to perform your required task. 
                foreach (string file in files)
                                {
                    try
                    {
                        // Perform whatever action is required in your scenario. 
                        System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        if (fi.Extension == ".json")
                        {
                            Console.WriteLine("****************{0}: {1}, {2}", fi.Name, fi.Length, fi.CreationTime);
                            FileStream fs = new FileStream(file, FileMode.Open);                            
                            StreamReader sr = new StreamReader(fs);
                            JObject jo = (JObject)JToken.ReadFrom(new JsonTextReader(@sr));
                            Console.WriteLine(jo);
                            foreach (var item in root.Contents)
                            {
                                var value = jo.SelectToken(item.Name);
                                Console.WriteLine("item.Name {0}",item.Name);
                                Console.WriteLine("value  {0}", value);
                                string str = ".." + item.Name;
                                var classNameTokens = jo.SelectTokens(str);
                                var values = classNameTokens.Select(x => (x as JValue).Value);
                                foreach (var i in values)
                                {
                                    Console.WriteLine("name {0} value {1}",str, i);
                                    if (item.Content == i.ToString())
                                    {
                                        result_Data = new Result_Data() { Path = file, Field = str, Value = i.ToString() };
                                        results.Add(result_Data);
                                        amount++;
                                    }
                                }
                            }
                            fs.Close();
                        }
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // If file was deleted by a separate application 
                        // or thread since the call to TraverseTree() 
                        // then just continue. 
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal. 
                // This could also be done before handing the files. 
                foreach (string str in subDirs)
                    dirs.Push(str);
                result_List = new Result_List() { Amount = amount, Results = results };
            }
            return new JsonResult(result_List);
        }

        // PUT api/Find/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/Find/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
