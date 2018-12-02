using System;
using System.Collections.Generic;
using System.IO;
using ApiFindJson.Model;
using Microsoft.AspNetCore.Mvc;
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
            return new string[] { "Hello comrades", "!!!!" };
        }

        // GET api/Find/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/Find
        [HttpPost]
        public JsonResult Post([FromBody] Search search)
        {
            // Data structure to hold names of subfolders to be 
            // examined for files. 
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(search.Root))
            {
                throw new ArgumentException();
            }

            Result_Data result_Data;
            Result_List result_List = new Result_List();
            List<Result_Data> results = new List<Result_Data>();
            List<string> listFile = new List<string>();

            int amount = 0;
            dirs.Push(search.Root);

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

                    //Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    //Console.WriteLine(e.Message);
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

                            //Console.WriteLine("****************{0}: {1}, {2} файл № {3}", fi.Name, fi.Length, fi.CreationTimeUtc, amount);
                            FileStream fs = new FileStream(file, FileMode.Open);
                            StreamReader sr = new StreamReader(fs);
                            //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse();
                            
                            string json = sr.ReadToEnd();
                            ////Console.WriteLine("***********json************");
                            ////Console.WriteLine(json);
                            //foreach (var searchContentInFile in search.Contents)
                            //{
                            //    if (json.Contains(searchContentInFile.ContentField))
                            //    {
                            //        Console.WriteLine(fi.FullName);
                            //        listFile.Add(file);
                            //    }
                            //}
                            
                            try
                            {
                                JObject jo = JObject.Parse(json);
                                foreach (var searchContent in search.Contents)
                                {
                                    string s = "$.." + searchContent.NameField;
                                    IEnumerable<JToken> findFilds = jo.SelectTokens(s);

                                    foreach (var contentField in findFilds)
                                    {
                                        if (searchContent.ContentField == contentField.ToString())
                                        {
                                            result_Data = new Result_Data() { Path = file, Field = searchContent.NameField, Value = contentField.ToString() };
                                            results.Add(result_Data);
                                            amount++;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                               // Console.WriteLine(e.Message);
                                continue;
                            }

                            try
                            {
                                JArray jj = JArray.Parse(json);
                                foreach (var searchContent in search.Contents)
                                {
                                    string s = "$.." + searchContent.NameField;
                                    IEnumerable<JToken> findFilds = jj.SelectTokens(s);

                                    foreach (var contentField in findFilds)
                                    {
                                        if (searchContent.ContentField == contentField.ToString())
                                        {
                                            result_Data = new Result_Data() { Path = file, Field = searchContent.NameField, Value = contentField.ToString() };
                                            results.Add(result_Data);
                                            amount++;
                                        }
                                    }
                                }


                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine(e.Message);
                                continue;
                            }





                            //JsonTextReader jsonTextReader = new JsonTextReader(@sr)
                            //{
                            //    MaxDepth = 3
                            //};

                            //while (jsonTextReader.Read())
                            //{
                            //    if (jsonTextReader.Value != null)
                            //    {
                            //       // Console.WriteLine("Token: {0}, Value: {1} Depth {2}", jsonTextReader.TokenType, jsonTextReader.Value, jsonTextReader.Depth);
                            //        foreach (var searchContent in search.Contents)
                            //        {
                            //            if (jsonTextReader.TokenType.ToString() == "PropertyName")
                            //            {
                            //                if (jsonTextReader.Value.ToString() == searchContent.NameField)
                            //                {
                            //                   // Console.Write("Field =  {0} ======>>>>>>> ", jsonTextReader.Value);
                            //                    jsonTextReader.ReadAsString();
                            //                    if (jsonTextReader.Value.ToString() == searchContent.ContentField)
                            //                    {
                            //                       // Console.WriteLine("Field Value =  {0}", jsonTextReader.Value);
                            //                        // Console.WriteLine("Token: {0}, Value: {1} Depth {2}" , jsonTextReader.TokenType, jsonTextReader.Value, jsonTextReader.Depth);
                            //                        result_Data = new Result_Data() { Path = file, Field = searchContent.NameField+ " Depth " + jsonTextReader.Depth, Value = jsonTextReader.Value.ToString() };
                            //                        results.Add(result_Data);
                            //                        amount++;
                            //                    }
                            //                }
                            //            }
                            //        }

                            //    }
                            //    else
                            //    {
                            //       // Console.WriteLine("Token: {0}", jsonTextReader.TokenType);
                            //    }
                            //}







                            /* JObject jo = (JObject)JToken.ReadFrom(jsonTextReader);
                            Console.WriteLine(jo);
                            foreach (var searchContent in search.Contents)
                            {
                                Console.WriteLine("searchContent.NameField {0}",searchContent.NameField);
                                string nameField = ".." + searchContent.NameField;
                                var namesField = jo.SelectTokens(nameField);
                                var contentFields = namesField.Select(x => (x as JValue).Value);
                                foreach (var contentField in contentFields)
                                {
                                    Console.WriteLine("nameField {0} contentField {1}",nameField, contentField);
                                    if (searchContent.ContentField == contentField.ToString())
                                    {
                                        result_Data = new Result_Data() { Path = file, Field = nameField, Value = contentField.ToString() };
                                        results.Add(result_Data);
                                        amount++;
                                    }
                                }
                            } */
                            fs.Close();
                           // Console.ReadKey();
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
