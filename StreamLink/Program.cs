using StreamLinkBussinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamLink
{
    internal class Program
    {
        public static void PrintSerieInfo(Serie serie)
        {
            Console.WriteLine($"Serie Name: {serie.Title}");
            Console.WriteLine($"Serie Total Eps: {serie.TotalEps}");
            Console.WriteLine($"Serie Uploaded Eps: {serie.UploadedEps}");
        }
        public static void TestGetSerie(int serieId)
        {
            Serie serie = Serie.Get(serieId, Serie.enEpsLoadType.All);

            if (serie != null)
            {
                Console.WriteLine("Serie retrieved successfully.");
                PrintSerieInfo(serie);
            }
            else
            {
                Console.WriteLine("Failed to retrieve the serie.");
            }
        }

        public static void TestAddSerie()
        {
            Serie serie = new Serie("Itaewon Class", 16, 16);

            if(serie.Save())
            {
                Console.WriteLine("Serie added successfully.");
                PrintSerieInfo(serie);
            }
            else
            {
                Console.WriteLine("Failed to add the serie.");
            }
        }

        public static void TestUpdateSerie(int serieId)
        {
            Serie serie = Serie.Get(serieId, Serie.enEpsLoadType.All);

            if(serie != null)
            {
                serie.Title = "Queen of Tears";

                if (serie.Save())
                {
                    Console.WriteLine("Serie updated successfully.");
                    PrintSerieInfo(serie);
                }
                else
                {
                    Console.WriteLine("Failed to update the serie.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve the serie with id {serieId}. (NOT FOUND)");
            }
        }

        public static void TestDeleteSerie(int serieId)
        {
            if (Serie.Delete(serieId))
            {
                Console.WriteLine("Serie deleted successfully.");
            }
            else
            {
                Console.WriteLine("Failed to delete the serie.");
            }
        }

        public static void PrintEpInfo(Ep ep)
        {
            Console.WriteLine($"Episode Number: {ep.EpNum}");
            Console.WriteLine($"Serie Id: {ep.SerieId}");
        }

        public static void TestGetEp(int epId)
        {
            Ep ep = Ep.Get(epId);

            if (ep != null)
            {
                Console.WriteLine("Episode retrieved successfully.");
                PrintEpInfo(ep);
            }
            else
            {
                Console.WriteLine("Failed to retrieve the episode.");
            }
        }

        public static void TestAddEp(int serieId)
        {
            Ep ep = new Ep(10, serieId);

            if (ep.Save())
            {
                Console.WriteLine("Episode added successfully.");
                PrintEpInfo(ep);
            }
            else
            {
                Console.WriteLine("Failed to add the episode.");
            }
        }

        public static void TestUpdateEp(int epId)
        {
            Ep ep = Ep.Get(epId);

            if (ep != null)
            {
                ep.EpNum = 11;

                if (ep.Save())
                {
                    Console.WriteLine("Episode updated successfully.");
                    PrintEpInfo(ep);
                }
                else
                {
                    Console.WriteLine("Failed to update the episode.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve the episode with id {epId}. (NOT FOUND)");
            }
        }

        public static void TestDeleteEp(int epId)
        {
            if (Ep.Delete(epId))
            {
                Console.WriteLine("Episode deleted successfully.");
            }
            else
            {
                Console.WriteLine("Failed to delete the episode.");
            }
        }

        static int Main(string[] args)
        {
            long intial = GC.GetTotalMemory(true);
            Serie serie = Serie.Get(1, Serie.enEpsLoadType.All);

            if (serie != null)
            {
                Console.WriteLine(serie.ToString() + '\n');
                foreach (Ep ep in serie.Eps)
                {
                    Console.WriteLine(ep.ToString());
                    foreach(Link link in  ep.Links)
                    {
                        Console.WriteLine(link.Url);

                        if(link.LinkSubtitles != null)
                        {
                            foreach (Subtitles subs in link.LinkSubtitles)
                            {
                                Console.WriteLine(subs.ToString());
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.Write("\n\n");
                }
            }

            Console.WriteLine((GC.GetTotalMemory(true) - intial) / Math.Pow(1024, 2));




            // TestGetSerie(2); //Worked
            // TestAddSerie(); // Worked
            // TestUpdateSerie(2); // Worked
            // TestDeleteSerie(2); // Worked

            return 0;
        }
    }
}
