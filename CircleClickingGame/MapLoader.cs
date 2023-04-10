using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircleClickingGame
{
    public static class MapLoader
    {
        static void LoadAudioFile()
        {
            using (StreamReader sr = new StreamReader(Engine.MapPath))
            {
                string line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();   
                    if(line == null && line == string.Empty)
                    {
                        continue;
                    }
                    if (line.Contains("AudioFilename:"))
                    {
                        string name = line.Split(':')[1].Trim();
                        Engine.MapAudio = Engine.MapPath.Replace(Engine.MapPath.Split(@"\").Last(), name);
                        if (File.Exists(Engine.MapAudio))
                        {
                            Engine.MediaPlayer.Open(new Uri(Engine.MapAudio));
                            //MessageBox.Show("Map audio loaded!");
                        }
                        break;
                    }
                }
            }               
        }
        static void LoadHitObjects()
        {
            using (StreamReader sr = new StreamReader(Engine.MapPath))
            {
                string line = sr.ReadLine();
                while (!(line == "[HitObjects]"))
                {
                    line = sr.ReadLine();
                }
                line = sr.ReadLine();
                while (line != null && line != string.Empty) 
                {
                    string[] properties = line.Split(',');
                    int x = int.Parse(properties[0]);
                    int y = int.Parse(properties[1]);
                    int time = int.Parse(properties[2]);
                    int type = int.Parse(properties[3]);
                    string[] pars = new string[3];
                    if ((type & 2) > 0)
                    {
                        pars[0] = properties[5]; // curve pts
                        pars[1] = properties[6]; // slides 
                        pars[2] = properties[7]; // length
                    }
                    Engine.HitObjects.Add(new HitObjectEvent(x, y, time, type, pars));

                    line = sr.ReadLine();
                }

            }
        }
        static void LoadTimingPoints()
        {
            using (StreamReader sr = new StreamReader(Engine.MapPath))
            {
                string line = sr.ReadLine();
                while (!(line == "[TimingPoints]"))
                {
                    line = sr.ReadLine();
                }
                line = sr.ReadLine();
                while (line != null && line != string.Empty)
                {              
                    
                    string[] properties = line.Split(',');
                    int time = (int)(double.Parse(properties[0], CultureInfo.InvariantCulture));
                    double beatlen = double.Parse(properties[1], CultureInfo.InvariantCulture);
                    int inherit = int.Parse(properties[6]);
                    Engine.TimingPoints.Add(new TimingPoint(time, inherit, beatlen));

                    if (Engine.BPM == 0)
                    {
                        Engine.BPM = beatlen;
                    }

                    line = sr.ReadLine();
                } 

            }
        }
        public static void Load()
        {
            LoadAudioFile();
            LoadDifficulty();
            LoadHitObjects();
            LoadTimingPoints();
            LoadBreakEvents();
        }

        static void LoadBreakEvents()
        {
            using (StreamReader sr = new StreamReader(Engine.MapPath))
            {
                string line = sr.ReadLine();
                while (!(line == "[Events]"))
                {
                    line = sr.ReadLine();
                }
                while (!line.Contains("Break"))
                {
                    line = sr.ReadLine();
                }
                line = sr.ReadLine();
                while (!line.Contains(@"//") && line != null && line != string.Empty)
                {
                    int st = int.Parse(line.Split(',')[1]);
                    int fin = int.Parse(line.Split(',')[2]);

                    Engine.BreakEvents.Add(new BreakEvent(st, fin));

                    line = sr.ReadLine();
                }
            }
        }

        static void LoadDifficulty()
        {
            using (StreamReader sr = new StreamReader(Engine.MapPath))
            {
                string line = sr.ReadLine();
                while (!(line == "[Difficulty]"))
                {
                    line = sr.ReadLine();
                }
                line = sr.ReadLine();
                while (line != null && line != string.Empty) 
                {                 
                    if (line.Contains("HPDrainRate"))
                    {
                        Engine.HP = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if (line.Contains("OverallDifficulty"))
                    {
                        Engine.OD = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if (line.Contains("CircleSize"))
                    {
                        Engine.CircSize = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if (line.Contains("ApproachRate"))
                    {
                        Engine.AR = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if (line.Contains("SliderMultiplier"))
                    {
                        Engine.SliderMultiplier = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if (line.Contains("SliderTickRate"))
                    {
                        Engine.SliderTickrate = double.Parse(line.Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    line = sr.ReadLine();
                } 
            }
        }
    }
}
