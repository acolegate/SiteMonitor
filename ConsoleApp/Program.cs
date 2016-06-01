using System;
using System.Threading;

using HttpAgentManager;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Config config = new Config();
            config.Sites.Add(new Config.Site
                                 {
                                     Url = "http://www.camerapricebuster.co.uk/Canon/Canon-EF-lenses",
                                     CheckFrequency = new TimeSpan(0, 0, 1),
                                     RegexPattern = @"<tr\s[^>]+?><td><a\s[^>]+?>(?<product>[^<]+?)\s?</a></td><td\s[^>]+?><a\s[^>]+?>&pound;(?<price>[^<]+?)\s?</a>",
                                     CaptureGroupToRecord = "price",
                                     Timeout = new TimeSpan(0, 0, 10),
                                     Test = new Config.RegexTest("product", Config.TestType.Equals, "EF 70-200mm f2.8L IS USM II Lens"),
                                     ActionOnTestValueChange = new Config.Action(Config.ActionType.WriteLine, "Matched")
                                 });

            using (Manager manager = new Manager(config))
            {
                manager.WriteLineEvent += Manager_WriteLineEvent;
                manager.RunProgramEvent += Manager_RunProgramEvent;
                manager.ShowDialogEvent += Manager_ShowDialogEvent;

                manager.Start();

                Thread.Sleep(30000);
            }
        }

        private static void Manager_ShowDialogEvent(object sender, Manager.ShowDialogEventArgs e)
        {
            Console.WriteLine("SHOW DIALOG: " + e.Message);
        }

        private static void Manager_RunProgramEvent(object sender, Manager.RunProgramEventArgs e)
        {
            Console.WriteLine("RUN PROGRAM: " + e.Message);
        }

        private static void Manager_WriteLineEvent(object sender, Manager.WriteLineEventArgs e)
        {
            Console.WriteLine("WRITE LINE : " + e.Message);
        }
    }
}